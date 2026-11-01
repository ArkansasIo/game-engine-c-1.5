"""
Builds the Luminara (GoonZu-inspired) world inside Unreal Editor 5.x from the Blender art pipeline.

Run from the editor's Output Log (Cmd):
    py "<Project>/Scripts/build_luminara_world.py"

Reads   <Project>/Art/Meshes/*.fbx, <Project>/Art/Icons/*.png, <Project>/Art/layout.json
Creates /Game/GoonZu/Meshes, /Game/GoonZu/Icons, /Game/GoonZu/Materials, /Game/GoonZu/Maps/L_Luminara
Options (set env var or edit below): SKIP_IMPORT=1 to only rebuild the level from already-imported assets.
"""

import json
import math
import os

import unreal

PROJECT = unreal.Paths.convert_relative_path_to_full(unreal.Paths.project_dir())
ART = os.path.join(PROJECT, "Art")
MESH_SRC = os.path.join(ART, "Meshes")
ICON_SRC = os.path.join(ART, "Icons")
LAYOUT_PATH = os.path.join(ART, "layout.json")

ROOT = "/Game/GoonZu"
MESH_DST = ROOT + "/Meshes"
ICON_DST = ROOT + "/Icons"
MAT_DST = ROOT + "/Materials"
MAP_PATH = ROOT + "/Maps/L_Luminara"

SKIP_IMPORT = os.environ.get("SKIP_IMPORT", "0") == "1"

asset_tools = unreal.AssetToolsHelpers.get_asset_tools()
eal = unreal.EditorAssetLibrary
mel = unreal.MaterialEditingLibrary
actors = unreal.get_editor_subsystem(unreal.EditorActorSubsystem)
level_sub = unreal.get_editor_subsystem(unreal.LevelEditorSubsystem)


def log(msg):
    unreal.log("[Luminara] " + msg)


# ----------------------------------------------------------------------------- import

def import_files(files, dest, is_mesh):
    tasks = []
    for f in files:
        name = os.path.splitext(os.path.basename(f))[0]
        t = unreal.AssetImportTask()
        t.set_editor_property("filename", f)
        t.set_editor_property("destination_path", dest)
        t.set_editor_property("destination_name", name)
        t.set_editor_property("replace_existing", True)
        t.set_editor_property("automated", True)
        t.set_editor_property("save", False)
        if is_mesh:
            ui = unreal.FbxImportUI()
            ui.set_editor_property("import_mesh", True)
            ui.set_editor_property("import_as_skeletal", False)
            ui.set_editor_property("import_materials", False)
            ui.set_editor_property("import_textures", False)
            ui.set_editor_property("import_animations", False)
            sm = ui.get_editor_property("static_mesh_import_data")
            sm.set_editor_property("combine_meshes", True)
            sm.set_editor_property("auto_generate_collision", False)
            sm.set_editor_property("vertex_color_import_option", unreal.VertexColorImportOption.REPLACE)
            try:
                sm.set_editor_property("generate_lightmap_u_vs", False)
            except Exception:
                pass
            t.set_editor_property("options", ui)
        tasks.append(t)
    asset_tools.import_asset_tasks(tasks)
    return tasks


def make_vertex_color_material():
    path = MAT_DST + "/M_GZ_VertexColor"
    if eal.does_asset_exist(path):
        return unreal.load_asset(path)
    mat = asset_tools.create_asset("M_GZ_VertexColor", MAT_DST, unreal.Material, unreal.MaterialFactoryNew())
    vc = mel.create_material_expression(mat, unreal.MaterialExpressionVertexColor, -700, 0)
    mel.connect_material_property(vc, "", unreal.MaterialProperty.MP_BASE_COLOR)
    inv = mel.create_material_expression(mat, unreal.MaterialExpressionOneMinus, -500, 200)
    mel.connect_material_expressions(vc, "A", inv, "")
    mul = mel.create_material_expression(mat, unreal.MaterialExpressionMultiply, -300, 150)
    mel.connect_material_expressions(vc, "", mul, "A")
    mel.connect_material_expressions(inv, "", mul, "B")
    glow = mel.create_material_expression(mat, unreal.MaterialExpressionMultiply, -150, 150)
    mel.connect_material_expressions(mul, "", glow, "A")
    glow.set_editor_property("const_b", 8.0)
    mel.connect_material_property(glow, "", unreal.MaterialProperty.MP_EMISSIVE_COLOR)
    rough = mel.create_material_expression(mat, unreal.MaterialExpressionConstant, -300, 350)
    rough.set_editor_property("r", 0.8)
    mel.connect_material_property(rough, "", unreal.MaterialProperty.MP_ROUGHNESS)
    mel.recompile_material(mat)
    eal.save_asset(path)
    return mat


def make_water_material():
    path = MAT_DST + "/M_GZ_Water"
    if eal.does_asset_exist(path):
        return unreal.load_asset(path)
    mat = asset_tools.create_asset("M_GZ_Water", MAT_DST, unreal.Material, unreal.MaterialFactoryNew())
    mat.set_editor_property("blend_mode", unreal.BlendMode.BLEND_TRANSLUCENT)
    try:
        mat.set_editor_property("translucency_lighting_mode", unreal.TranslucencyLightingMode.TLM_SURFACE)
    except Exception:
        pass
    col = mel.create_material_expression(mat, unreal.MaterialExpressionConstant3Vector, -400, 0)
    col.set_editor_property("constant", unreal.LinearColor(0.05, 0.28, 0.62, 1.0))
    mel.connect_material_property(col, "", unreal.MaterialProperty.MP_BASE_COLOR)
    op = mel.create_material_expression(mat, unreal.MaterialExpressionConstant, -400, 200)
    op.set_editor_property("r", 0.78)
    mel.connect_material_property(op, "", unreal.MaterialProperty.MP_OPACITY)
    r = mel.create_material_expression(mat, unreal.MaterialExpressionConstant, -400, 300)
    r.set_editor_property("r", 0.05)
    mel.connect_material_property(r, "", unreal.MaterialProperty.MP_ROUGHNESS)
    mel.recompile_material(mat)
    eal.save_asset(path)
    return mat


def setup_meshes(vc_mat, water_mat):
    sme = unreal.get_editor_subsystem(unreal.StaticMeshEditorSubsystem)
    names = [os.path.splitext(f)[0] for f in os.listdir(MESH_SRC) if f.lower().endswith(".fbx")]
    no_collision = ("SM_GZ_Water", "SM_GZ_AxisProbe")
    with unreal.ScopedSlowTask(len(names), "Luminara: configuring meshes") as task:
        task.make_dialog(True)
        for n in names:
            task.enter_progress_frame(1, n)
            sm = unreal.load_asset(MESH_DST + "/" + n)
            if not sm:
                unreal.log_warning("[Luminara] missing mesh " + n)
                continue
            try:
                for i in range(sm.get_num_sections(0)):
                    sm.set_material(i, water_mat if n == "SM_GZ_Water" else vc_mat)
            except Exception:
                sm.set_material(0, water_mat if n == "SM_GZ_Water" else vc_mat)
            try:
                sme.remove_collisions(sm)
            except Exception:
                pass
            if n == "SM_GZ_Terrain":
                bs = sm.get_editor_property("body_setup")
                if bs:
                    bs.set_editor_property("collision_trace_flag", unreal.CollisionTraceFlag.CTF_USE_COMPLEX_AS_SIMPLE)
            elif n not in no_collision:
                try:
                    sme.add_simple_collisions(sm, unreal.ScriptCollisionShapeType.BOX)
                except Exception as e:
                    unreal.log_warning("[Luminara] collision %s: %s" % (n, e))
            eal.save_asset(MESH_DST + "/" + n, only_if_is_dirty=False)


def setup_icons():
    for f in os.listdir(ICON_SRC):
        if not f.lower().endswith(".png"):
            continue
        n = os.path.splitext(f)[0]
        tex = unreal.load_asset(ICON_DST + "/" + n)
        if not tex:
            continue
        # crisp 8-bit pixels in the UI
        tex.set_editor_property("filter", unreal.TextureFilter.TF_NEAREST)
        tex.set_editor_property("mip_gen_settings", unreal.TextureMipGenSettings.TMGS_NO_MIPMAPS)
        tex.set_editor_property("lod_group", unreal.TextureGroup.TEXTUREGROUP_UI)
        tex.set_editor_property("compression_settings", unreal.TextureCompressionSettings.TC_EDITOR_ICON)
        tex.set_editor_property("srgb", True)
        eal.save_asset(ICON_DST + "/" + n, only_if_is_dirty=False)


# ----------------------------------------------------------------------------- axis detection

def detect_axes():
    """Returns (terrain_scale_y, facing_offset_deg) from the asymmetric probe mesh (x 0..1, y 0..2, z 0..3 m in Blender)."""
    probe = unreal.load_asset(MESH_DST + "/SM_GZ_AxisProbe")
    if not probe:
        return 1.0, -90.0
    box = probe.get_bounding_box()
    mn, mx = box.min, box.max
    log("probe bounds min=(%.0f,%.0f,%.0f) max=(%.0f,%.0f,%.0f)" % (mn.x, mn.y, mn.z, mx.x, mx.y, mx.z))
    ext_x, ext_y = mx.x - mn.x, mx.y - mn.y
    if abs(ext_y - 200) < 30:
        # Blender Y -> UE Y axis
        by_sign = -1.0 if (mn.y + mx.y) < 0 else 1.0
        # Terrain was authored as Blender (x, -y): correct when Blender Y maps to UE -Y.
        terrain_sy = 1.0 if by_sign < 0 else -1.0
        # Asset fronts face Blender -Y -> UE (0, -by_sign)
        facing = math.degrees(math.atan2(-by_sign, 0.0))
        return terrain_sy, facing
    log("unexpected axis mapping (ext_x=%.0f ext_y=%.0f); using defaults" % (ext_x, ext_y))
    return 1.0, -90.0


# ----------------------------------------------------------------------------- level

def vec(x, y, z):
    return unreal.Vector(float(x), float(y), float(z))


def clear_level():
    keep = (unreal.WorldSettings,)
    for a in actors.get_all_level_actors():
        if isinstance(a, keep):
            continue
        try:
            actors.destroy_actor(a)
        except Exception:
            pass


def spawn_mesh(mesh, loc, yaw, scale, folder, collide=True, tags=None):
    a = actors.spawn_actor_from_class(unreal.StaticMeshActor, loc, unreal.Rotator(0.0, 0.0, float(yaw)))
    comp = a.static_mesh_component
    comp.set_static_mesh(mesh)
    if not collide:
        comp.set_collision_enabled(unreal.CollisionEnabled.NO_COLLISION)
    a.set_actor_scale3d(vec(scale, scale, scale))
    a.set_folder_path(folder)
    if tags:
        a.set_editor_property("tags", [unreal.Name(t) for t in tags])
    return a


def spawn_label(text, loc, size, color, folder, parent=None):
    a = actors.spawn_actor_from_class(unreal.TextRenderActor, loc, unreal.Rotator(0.0, 40.0, 225.0))
    tr = a.text_render
    try:
        tr.set_text(unreal.Text(text))
    except Exception:
        tr.set_editor_property("text", unreal.Text(text))
    tr.set_editor_property("world_size", float(size))
    tr.set_editor_property("horizontal_alignment", unreal.HorizTextAligment.EHTA_CENTER)
    tr.set_editor_property("vertical_alignment", unreal.VerticalTextAligment.EVRTA_TEXT_CENTER)
    tr.set_editor_property("text_render_color", unreal.Color(r=int(color[0] * 255), g=int(color[1] * 255), b=int(color[2] * 255), a=255))
    a.set_folder_path(folder)
    a.set_actor_label("Label_" + text[:24])
    a.set_editor_property("tags", [unreal.Name("GZLabel")])
    if parent:
        a.attach_to_actor(parent, "", unreal.AttachmentRule.KEEP_WORLD, unreal.AttachmentRule.KEEP_WORLD,
                          unreal.AttachmentRule.KEEP_WORLD, False)
    return a


def setup_lighting():
    f = "Luminara/Lighting"
    sun = actors.spawn_actor_from_class(unreal.DirectionalLight, vec(0, 0, 5000), unreal.Rotator(0.0, -48.0, 35.0))
    sun.set_folder_path(f)
    lc = sun.get_component_by_class(unreal.DirectionalLightComponent)
    try:
        lc.set_editor_property("intensity", 3.2)
        lc.set_editor_property("atmosphere_sun_light", True)
        lc.set_editor_property("light_color", unreal.Color(235, 245, 255, 255))
    except Exception as e:
        unreal.log_warning(str(e))
    actors.spawn_actor_from_class(unreal.SkyAtmosphere, vec(0, 0, 0)).set_folder_path(f)
    sky = actors.spawn_actor_from_class(unreal.SkyLight, vec(0, 0, 6000))
    sky.set_folder_path(f)
    try:
        sc = sky.get_component_by_class(unreal.SkyLightComponent)
        sc.set_editor_property("real_time_capture", True)
        sc.set_editor_property("mobility", unreal.ComponentMobility.MOVABLE)
        sc.set_editor_property("intensity", 0.9)
    except Exception as e:
        unreal.log_warning(str(e))
    fog = actors.spawn_actor_from_class(unreal.ExponentialHeightFog, vec(0, 0, -500))
    fog.set_folder_path(f)
    try:
        fc = fog.get_component_by_class(unreal.ExponentialHeightFogComponent)
        fc.set_editor_property("fog_density", 0.004)
    except Exception:
        pass
    try:
        actors.spawn_actor_from_class(unreal.VolumetricCloud, vec(0, 0, 0)).set_folder_path(f)
    except Exception:
        pass
    ppv = actors.spawn_actor_from_class(unreal.PostProcessVolume, vec(0, 0, 0))
    ppv.set_folder_path(f)
    ppv.set_editor_property("unbound", True)
    try:
        s = ppv.get_editor_property("settings")
        s.set_editor_property("override_auto_exposure_min_brightness", True)
        s.set_editor_property("auto_exposure_min_brightness", 1.0)
        s.set_editor_property("override_auto_exposure_max_brightness", True)
        s.set_editor_property("auto_exposure_max_brightness", 1.0)
        s.set_editor_property("override_auto_exposure_bias", True)
        s.set_editor_property("auto_exposure_bias", -1.0)
        s.set_editor_property("override_color_saturation", True)
        s.set_editor_property("color_saturation", unreal.Vector4(1.15, 1.15, 1.15, 1.0))
        ppv.set_editor_property("settings", s)
    except Exception as e:
        unreal.log_warning("post process: %s" % e)


def build_level(layout, terrain_sy, facing):
    if eal.does_asset_exist(MAP_PATH):
        level_sub.load_level(MAP_PATH)
        clear_level()
    else:
        level_sub.new_level(MAP_PATH)

    cache = {}

    def mesh(n):
        if n not in cache:
            cache[n] = unreal.load_asset(MESH_DST + "/" + n)
            if not cache[n]:
                unreal.log_warning("[Luminara] missing mesh asset " + n)
        return cache[n]

    setup_lighting()
    t = spawn_mesh(mesh("SM_GZ_Terrain"), vec(0, 0, 0), 0, 1, "Luminara/Terrain")
    t.set_actor_scale3d(vec(1, terrain_sy, 1))
    t.set_actor_label("Luminara_Terrain")
    w = spawn_mesh(mesh("SM_GZ_Water"), vec(0, 0, 0), 0, 1, "Luminara/Terrain", collide=False)
    w.set_actor_scale3d(vec(1, terrain_sy, 1))
    w.set_actor_label("Luminara_Water")

    items = layout["instances"]
    tagged = layout["tagged"]
    total = len(items) + len(tagged) + len(layout["labels"]) + len(layout["zones"])
    with unreal.ScopedSlowTask(total, "Luminara: building world") as task:
        task.make_dialog(True)
        for i in items:
            task.enter_progress_frame(1)
            m = mesh(i["m"])
            if m:
                spawn_mesh(m, vec(i["x"], i["y"], i["z"]), i["yaw"] - facing, i["s"], i["f"], i["col"])
        for tg in tagged:
            task.enter_progress_frame(1)
            m = mesh(tg["m"])
            if not m:
                continue
            a = spawn_mesh(m, vec(tg["x"], tg["y"], tg["z"]), tg["yaw"] - facing, tg["s"], tg["f"], True, tg["tags"])
            a.set_actor_label(tg["tags"][1])
            if tg.get("label"):
                spawn_label(tg["label"], vec(tg["x"], tg["y"], tg["z"] + tg.get("label_z", 260) * tg["s"]), 42,
                            tg.get("label_color", (1, 1, 1)), tg["f"], parent=a)
        for lb in layout["labels"]:
            task.enter_progress_frame(1)
            spawn_label(lb["text"], vec(lb["x"], lb["y"], lb["z"]), lb["size"] * 0.6, lb["color"], lb["f"])
        for z in layout["zones"]:
            task.enter_progress_frame(1)
            tp = actors.spawn_actor_from_class(unreal.TargetPoint, vec(z["x"], z["y"], z["z"]))
            tp.set_folder_path("Luminara/Zones")
            tp.set_actor_label("Zone_" + z["name"])
            tp.set_editor_property("tags", [unreal.Name("GZZone"), unreal.Name(z["name"]), unreal.Name(str(int(z["r"]))),
                                            unreal.Name(z["type"])])

    cube = unreal.load_asset("/Engine/BasicShapes/Cube")
    for b in layout.get("blockers", []):
        a = spawn_mesh(cube, vec(b["x"], b["y"], b["z"]), 0, 1, "Luminara/Bounds")
        a.set_actor_scale3d(vec(b["sx"], b["sy"], b["sz"]))
        a.static_mesh_component.set_editor_property("visible", False)
        a.set_actor_hidden_in_game(True)

    # Spawn on open plaza ground east of Emberfall's statue (clear of stalls and lanterns).
    start = actors.spawn_actor_from_class(unreal.PlayerStart, vec(850, -150, 160), unreal.Rotator(0, 0, 45))
    start.set_folder_path("Luminara")

    # Game mode override for this map.
    gm = unreal.load_class(None, "/Script/EtherealVeni.GZGameMode")
    world = unreal.get_editor_subsystem(unreal.UnrealEditorSubsystem).get_editor_world()
    if gm:
        world.get_world_settings().set_editor_property("default_game_mode", gm)
        log("Game mode set to GZGameMode")
    else:
        unreal.log_warning("[Luminara] GZGameMode class not found - compile the C++ module first")

    level_sub.save_current_level()


def unpack_art():
    """If the Blender output was delivered as LuminaraArt.zip, extract it into <Project>/Art."""
    import zipfile
    for cand in (os.path.join(PROJECT, "LuminaraArt.zip"), os.path.join(ART, "LuminaraArt.zip")):
        if os.path.exists(cand):
            log("extracting " + cand)
            os.makedirs(ART, exist_ok=True)
            with zipfile.ZipFile(cand) as z:
                z.extractall(ART)
            os.remove(cand)
            return


def main():
    unpack_art()
    if not os.path.exists(LAYOUT_PATH):
        unreal.log_error("[Luminara] layout.json not found at " + LAYOUT_PATH + " - run Tools/Blender/build_luminara_assets.py first")
        return
    layout = json.load(open(LAYOUT_PATH))
    if not SKIP_IMPORT:
        meshes = [os.path.join(MESH_SRC, f) for f in sorted(os.listdir(MESH_SRC)) if f.lower().endswith(".fbx")]
        icons = [os.path.join(ICON_SRC, f) for f in sorted(os.listdir(ICON_SRC)) if f.lower().endswith(".png")]
        log("importing %d meshes, %d icons" % (len(meshes), len(icons)))
        import_files(meshes, MESH_DST, True)
        import_files(icons, ICON_DST, False)
        setup_icons()
    vc = make_vertex_color_material()
    water = make_water_material()
    if not SKIP_IMPORT:
        setup_meshes(vc, water)
    terrain_sy, facing = detect_axes()
    log("terrain Y scale %.0f, facing offset %.0f" % (terrain_sy, facing))
    build_level(layout, terrain_sy, facing)
    eal.save_directory(ROOT, only_if_is_dirty=True, recursive=True)
    log("DONE - L_Luminara built: %d props, %d NPCs/monsters/resources" % (len(layout["instances"]), len(layout["tagged"])))


main()
