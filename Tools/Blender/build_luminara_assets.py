"""
Luminara (GoonZu-inspired) asset builder for Blender 4.2+/5.x.

Builds every 3D asset for the Unreal project procedurally, all original content:
  * stylized low-poly buildings (3 town styles), nature, props
  * characters / NPCs, monsters, items
  * the 400 x 400 m world terrain + water, with vertex-colored biomes
  * 2D UI icons and portraits rendered from those models (Cycles), converted to 8-bit pixel art
    (32x32 grid, fixed 32-colour palette, 1px outline; pass --smooth-icons to keep smooth renders)
  * layout.json: where Unreal should place everything (UE centimetres)

Outputs (relative to --out):
  Meshes/SM_GZ_*.fbx     -> imported by Unreal into /Game/GoonZu/Meshes
  Icons/T_*.png          -> imported by Unreal into /Game/GoonZu/Icons
  layout.json            -> read by Scripts/build_luminara_world.py inside Unreal
  luminara_assets.blend  -> editable source of every model

Run either inside Blender:
  blender --background --python build_luminara_assets.py -- --out <dir>
or with the `bpy` Python module:
  python build_luminara_assets.py --out <dir>

Vertex colours carry all colour information (alpha 0 = glowing/emissive).
Unreal uses one master material (M_GZ_VertexColor) for every mesh.
"""

import bpy
import bmesh
import json
import math
import os
import random
import sys
from mathutils import Matrix, Vector, noise

# ----------------------------------------------------------------------------- args

def _args():
    argv = sys.argv
    if "--" in argv:
        argv = argv[argv.index("--") + 1:]
    else:
        argv = argv[1:]
    out = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", "EtherealVeni", "Art")
    icons = True
    pixel = True
    for i, a in enumerate(argv):
        if a == "--smooth-icons":
            pixel = False
        if a == "--out" and i + 1 < len(argv):
            out = argv[i + 1]
        if a == "--no-icons":
            icons = False
    return os.path.abspath(out), icons, pixel


OUT, RENDER_ICONS, PIXEL_ART = _args()
MESH_DIR = os.path.join(OUT, "Meshes")
ICON_DIR = os.path.join(OUT, "Icons")
os.makedirs(MESH_DIR, exist_ok=True)
os.makedirs(ICON_DIR, exist_ok=True)
random.seed(1337)

# ----------------------------------------------------------------------------- palette (sRGB)

C = dict(
    cream=(0.93, 0.88, 0.76), plaster=(0.88, 0.85, 0.80), wood=(0.45, 0.29, 0.16), wood_dark=(0.30, 0.19, 0.11),
    wood_light=(0.66, 0.48, 0.30), stone=(0.55, 0.55, 0.53), stone_dark=(0.38, 0.38, 0.38), paper=(0.98, 0.95, 0.85),
    red=(0.72, 0.14, 0.12), red_dark=(0.50, 0.09, 0.08), gold=(0.95, 0.75, 0.25), slate=(0.24, 0.27, 0.33),
    slate_dark=(0.16, 0.18, 0.22), tile=(0.58, 0.24, 0.16), tile_dark=(0.42, 0.16, 0.11), thatch=(0.78, 0.63, 0.34),
    thatch_dark=(0.60, 0.46, 0.22), snow=(0.95, 0.97, 1.0), snow_shadow=(0.80, 0.86, 0.95), green=(0.30, 0.58, 0.24),
    leaf=(0.33, 0.62, 0.25), leaf_dark=(0.18, 0.42, 0.18), leaf_light=(0.48, 0.74, 0.30), pine=(0.13, 0.36, 0.22),
    blossom=(0.97, 0.70, 0.80), blossom2=(0.99, 0.82, 0.88), bark=(0.36, 0.25, 0.17), dead=(0.42, 0.38, 0.34),
    metal=(0.70, 0.72, 0.76), steel=(0.62, 0.70, 0.82), iron=(0.50, 0.50, 0.52), black=(0.06, 0.06, 0.07),
    white=(0.97, 0.97, 0.97), skin=(0.98, 0.82, 0.68), skin2=(0.88, 0.68, 0.52), blue=(0.20, 0.36, 0.78),
    navy=(0.12, 0.17, 0.38), purple=(0.45, 0.25, 0.62), teal=(0.15, 0.55, 0.55), orange=(0.95, 0.52, 0.12),
    yellow=(0.98, 0.85, 0.25), lava=(1.0, 0.45, 0.05), ice=(0.65, 0.88, 1.0), ice_dark=(0.40, 0.65, 0.90),
    water=(0.25, 0.55, 0.85), brown=(0.48, 0.33, 0.20), leather=(0.55, 0.36, 0.20), cloth_green=(0.25, 0.50, 0.30),
    pink=(0.95, 0.55, 0.65), slime=(0.35, 0.85, 0.40), goblin=(0.45, 0.65, 0.25), orc=(0.40, 0.52, 0.30),
    bone=(0.92, 0.90, 0.82), soil=(0.42, 0.29, 0.18), wheat=(0.92, 0.78, 0.35), hay=(0.85, 0.72, 0.36),
)


def col(name_or_tuple):
    return C[name_or_tuple] if isinstance(name_or_tuple, str) else name_or_tuple


# ----------------------------------------------------------------------------- model builder

def _trs(loc, rot, scale):
    return (Matrix.Translation(Vector(loc)) @ Matrix.Rotation(rot[2], 4, 'Z') @ Matrix.Rotation(rot[1], 4, 'Y')
            @ Matrix.Rotation(rot[0], 4, 'X') @ Matrix.Diagonal((scale[0], scale[1], scale[2], 1.0)))


class Model:
    """Accumulates coloured primitives into one bmesh."""

    def __init__(self, name):
        self.name = name
        self.bm = bmesh.new()
        self.cl = self.bm.loops.layers.color.new("Col")

    def _paint(self, verts, color, glow=False):
        c = col(color)
        rgba = (c[0], c[1], c[2], 0.0 if glow else 1.0)
        faces = {f for v in verts for f in v.link_faces}
        for f in faces:
            f.smooth = False
            for lp in f.loops:
                lp[self.cl] = rgba

    def box(self, color, loc, size, rot=(0, 0, 0), glow=False):
        r = bmesh.ops.create_cube(self.bm, size=1.0, matrix=_trs(loc, rot, size))
        self._paint(r["verts"], color, glow)
        return self

    def cyl(self, color, loc, r1, depth, r2=None, segs=8, rot=(0, 0, 0), scale=(1, 1, 1), glow=False):
        r = bmesh.ops.create_cone(self.bm, cap_ends=True, cap_tris=False, segments=segs, radius1=r1,
                                  radius2=r1 if r2 is None else r2, depth=depth,
                                  matrix=_trs(loc, rot, scale))
        self._paint(r["verts"], color, glow)
        return self

    def cone(self, color, loc, r, depth, segs=8, rot=(0, 0, 0), scale=(1, 1, 1), glow=False):
        return self.cyl(color, loc, r, depth, r2=0.0, segs=segs, rot=rot, scale=scale, glow=glow)

    def ball(self, color, loc, r, scale=(1, 1, 1), rot=(0, 0, 0), sub=2, glow=False, smooth=False):
        res = bmesh.ops.create_icosphere(self.bm, subdivisions=sub, radius=1.0,
                                         matrix=_trs(loc, rot, (r * scale[0], r * scale[1], r * scale[2])))
        self._paint(res["verts"], color, glow)
        return self

    def roof(self, color, base, w, d, h, top=0.3, rot_z=0.0):
        """Square-section frustum (hip roof). base = centre of the roof's bottom."""
        m = (Matrix.Translation(Vector((base[0], base[1], base[2] + h / 2))) @ Matrix.Rotation(rot_z, 4, 'Z')
             @ Matrix.Diagonal((w, d, h, 1.0)) @ Matrix.Rotation(math.pi / 4, 4, 'Z'))
        r = bmesh.ops.create_cone(self.bm, cap_ends=True, cap_tris=False, segments=4, radius1=math.sqrt(0.5),
                                  radius2=math.sqrt(0.5) * top, depth=1.0, matrix=m)
        self._paint(r["verts"], color)
        return self

    def rock(self, color, loc, r, scale=(1, 1, 0.7), seed=0, sub=1, amp=0.25):
        res = bmesh.ops.create_icosphere(self.bm, subdivisions=sub, radius=1.0, matrix=Matrix.Identity(4))
        rnd = random.Random(seed)
        for v in res["verts"]:
            v.co *= 1.0 + rnd.uniform(-amp, amp)
        bmesh.ops.transform(self.bm, matrix=_trs(loc, (0, 0, rnd.uniform(0, 6.28)),
                                                  (r * scale[0], r * scale[1], r * scale[2])), verts=res["verts"])
        self._paint(res["verts"], color)
        return self

    def build(self):
        me = bpy.data.meshes.new(self.name)
        self.bm.normal_update()
        self.bm.to_mesh(me)
        self.bm.free()
        try:
            me.color_attributes.active_color_name = "Col"
            me.color_attributes.default_color_name = "Col"
        except Exception:
            pass
        ob = bpy.data.objects.new(self.name, me)
        ob.data.materials.append(vc_material())
        return ob


_VC = None


def vc_material():
    global _VC
    if _VC:
        return _VC
    m = bpy.data.materials.new("M_GZ_VertexColor")
    m.use_nodes = True
    nt = m.node_tree
    bsdf = nt.nodes.get("Principled BSDF")
    attr = nt.nodes.new("ShaderNodeAttribute")
    attr.attribute_name = "Col"
    nt.links.new(attr.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = 0.65
    # alpha 0 -> glow
    inv = nt.nodes.new("ShaderNodeMath")
    inv.operation = 'SUBTRACT'
    inv.inputs[0].default_value = 1.0
    nt.links.new(attr.outputs["Alpha"], inv.inputs[1])
    mul = nt.nodes.new("ShaderNodeMath")
    mul.operation = 'MULTIPLY'
    mul.inputs[1].default_value = 6.0
    nt.links.new(inv.outputs[0], mul.inputs[0])
    try:
        nt.links.new(attr.outputs["Color"], bsdf.inputs["Emission Color"])
        nt.links.new(mul.outputs[0], bsdf.inputs["Emission Strength"])
    except Exception:
        pass
    _VC = m
    return m


# ----------------------------------------------------------------------------- town styles

STYLES = {
    # Emberfall: warm red-brown tiled roofs, cream walls
    "ember": dict(roof="tile", roof_dark="tile_dark", ridge="tile_dark", wall="cream", trim="wood", accent="red"),
    # Frostmere: snow-laden slate roofs, stone walls
    "frost": dict(roof="snow", roof_dark="slate", ridge="snow_shadow", wall="plaster", trim="wood_dark", accent="blue"),
    # Greenhollow: thatch roofs, timber walls
    "green": dict(roof="thatch", roof_dark="thatch_dark", ridge="thatch_dark", wall="wood_light", trim="wood_dark", accent="cloth_green"),
}


def hanok_roof(m, st, w, d, top, tiers=1, height=2.2):
    """Original stylised East-Asian inspired roof with upturned eave tips."""
    for t in range(tiers):
        s = 1.0 - t * 0.28
        z = top + t * height * 0.75
        m.roof(st["roof_dark"], (0, 0, z), (w + 2.4) * s, (d + 2.0) * s, 0.35, top=0.9)
        m.roof(st["roof"], (0, 0, z + 0.3), (w + 1.8) * s, (d + 1.4) * s, height * s, top=0.32)
        rl = (w * 0.55 + 0.3) * s
        m.box(st["ridge"], (0, 0, z + 0.3 + height * s), (rl, 0.42, 0.34))
        for sx in (-1, 1):
            m.box(st["ridge"], (sx * (rl / 2 + 0.15), 0, z + 0.48 + height * s), (0.45, 0.45, 0.45), rot=(0, sx * 0.6, 0))
            for sy in (-1, 1):
                m.cone(st["roof_dark"], (sx * ((w + 2.2) * s / 2), sy * ((d + 1.8) * s / 2), z + 0.45), 0.28, 0.9,
                       segs=5, rot=(sy * -0.9, sx * 0.9, 0))


def house_body(m, st, w, d, h, z0=0.5, door=True, windows=True):
    m.box("stone", (0, 0, z0 / 2), (w + 0.7, d + 0.7, z0))
    m.box(st["wall"], (0, 0, z0 + h / 2), (w, d, h))
    # timber posts
    for x in (-w / 2, w / 2):
        for y in (-d / 2, d / 2):
            m.cyl(st["trim"], (x, y, z0 + h / 2), 0.17, h, segs=6)
    n = max(1, int(w // 2.6))
    for i in range(1, n):
        x = -w / 2 + i * w / n
        m.box(st["trim"], (x, -d / 2 - 0.03, z0 + h / 2), (0.22, 0.1, h))
    m.box(st["trim"], (0, 0, z0 + h - 0.15), (w + 0.12, d + 0.12, 0.3))
    if door:
        m.box("wood_dark", (0, -d / 2 - 0.06, z0 + 1.15), (1.5, 0.12, 2.3))
        m.box("gold", (0.45, -d / 2 - 0.14, z0 + 1.1), (0.12, 0.05, 0.12))
    if windows:
        for x in (-w / 2 + 1.3, w / 2 - 1.3):
            m.box("wood_dark", (x, -d / 2 - 0.05, z0 + h * 0.58), (1.25, 0.08, 1.05))
            m.box("paper", (x, -d / 2 - 0.09, z0 + h * 0.58), (1.05, 0.06, 0.85))
            m.box("wood_dark", (x, -d / 2 - 0.12, z0 + h * 0.58), (0.08, 0.04, 0.85))
            m.box("wood_dark", (x, -d / 2 - 0.12, z0 + h * 0.58), (1.05, 0.04, 0.08))
    return z0 + h


def make_house(name, style, w=7.0, d=5.5, stories=1, extra=None):
    st = STYLES[style]
    m = Model(name)
    top = house_body(m, st, w, d, 3.0)
    if stories == 2:
        hanok_roof(m, st, w, d, top, height=1.0)
        m.box(st["wall"], (0, 0, top + 1.6), (w * 0.8, d * 0.8, 2.6))
        for x in (-w * 0.4, w * 0.4):
            m.cyl(st["trim"], (x, -d * 0.4, top + 1.6), 0.15, 2.6, segs=6)
        m.box("paper", (0, -d * 0.4 - 0.06, top + 1.7), (w * 0.5, 0.06, 1.0))
        top = top + 2.9
    hanok_roof(m, st, w if stories == 1 else w * 0.8, d if stories == 1 else d * 0.8, top)
    if extra:
        extra(m, st, w, d, top)
    return m.build()


def ex_blacksmith(m, st, w, d, top):
    m.box("stone_dark", (w / 2 - 1.0, d / 2 - 1.0, top + 1.0), (1.2, 1.2, 4.0))
    m.box("black", (w / 2 - 1.0, d / 2 - 1.0, top + 3.05), (1.0, 1.0, 0.1))
    m.box("stone_dark", (-w / 2 - 1.6, -d / 2 - 1.0, 0.6), (1.8, 1.4, 1.2))
    m.box("lava", (-w / 2 - 1.6, -d / 2 - 1.0, 1.22), (1.3, 0.9, 0.1), glow=True)
    m.box("iron", (1.8, -d / 2 - 2.0, 0.55), (0.5, 0.35, 0.5))
    m.box("iron", (1.8, -d / 2 - 2.0, 0.9), (1.0, 0.45, 0.25))
    m.box("wood", (1.8, -d / 2 - 2.0, 0.2), (0.7, 0.6, 0.4))
    m.box(st["accent"], (0, -d / 2 - 0.2, 3.6), (2.4, 0.1, 0.7))


def ex_alchemy(m, st, w, d, top):
    for i, c in enumerate(("teal", "purple", "red", "yellow")):
        m.cyl("stone_dark", (-w / 2 + 0.8 + i * 0.9, -d / 2 - 1.2, 0.35), 0.35, 0.7, r2=0.25, segs=8)
        m.ball(c, (-w / 2 + 0.8 + i * 0.9, -d / 2 - 1.2, 0.8), 0.18, glow=(i == 1))
    m.box("teal", (0, -d / 2 - 0.2, 3.6), (2.4, 0.1, 0.7))


def ex_inn(m, st, w, d, top):
    for x in (-w / 2 + 0.6, w / 2 - 0.6):
        m.ball("red", (x, -d / 2 - 0.6, 3.0), 0.32, scale=(1, 1, 1.3), glow=True)
        m.cyl("wood_dark", (x, -d / 2 - 0.6, 3.5), 0.03, 0.5)
    m.box("wood", (0, -d / 2 - 1.8, 0.45), (2.4, 1.0, 0.1))
    for x in (-1.6, 1.6):
        m.box("wood", (x, -d / 2 - 1.8, 0.25), (0.7, 0.4, 0.5))


def ex_warehouse(m, st, w, d, top):
    for i in range(3):
        m.box("wood_light", (-w / 2 - 1.0, -d / 2 + 1.0 + i * 1.3, 0.5), (1.0, 1.0, 1.0))
    m.cyl("wood", (w / 2 + 0.9, -d / 2 + 1, 0.55), 0.45, 1.1)
    m.cyl("wood", (w / 2 + 0.9, -d / 2 + 2.1, 0.55), 0.45, 1.1)


def ex_guild(m, st, w, d, top):
    for x in (-w / 2 - 0.6, w / 2 + 0.6):
        m.cyl("wood_dark", (x, -d / 2 - 0.8, 3.2), 0.1, 6.4)
        m.box("navy", (x + 0.0, -d / 2 - 0.8, 4.6), (0.08, 1.4, 2.6))
        m.box("gold", (x + 0.05, -d / 2 - 0.8, 4.9), (0.06, 0.6, 0.6))


def make_town_hall(style):
    st = STYLES[style]
    m = Model("SM_GZ_town_hall_" + style)
    w, d = 13.0, 8.0
    m.box("stone", (0, 0, 0.6), (w + 3, d + 3, 1.2))
    for i in range(3):
        m.box("stone", (0, -d / 2 - 1.7 - i * 0.6, 0.2 + i * 0.0), (5.0, 0.6, 1.2 - i * 0.4))
    m.box(st["wall"], (0, 0.5, 1.2 + 2.0), (w - 1.5, d - 1.5, 4.0))
    for i in range(7):
        x = -w / 2 + i * w / 6
        m.cyl("red", (x, -d / 2, 1.2 + 2.0), 0.28, 4.0, segs=8)
    m.box("red_dark", (0, -d / 2, 5.3), (w + 0.4, 0.5, 0.5))
    m.box("wood_dark", (0, -d / 2 + 0.75, 2.6), (2.6, 0.1, 2.8))
    m.box("gold", (0, -d / 2 - 0.35, 5.95), (3.2, 0.12, 0.9))
    m.box("navy", (0, -d / 2 - 0.42, 5.95), (2.8, 0.06, 0.6))
    hanok_roof(m, st, w, d, 5.5, tiers=2, height=2.4)
    for x in (-w / 2 - 2.2, w / 2 + 2.2):
        m.cyl("wood_dark", (x, -d / 2 - 2.5, 4.0), 0.12, 8.0)
        m.box(st["accent"], (x, -d / 2 - 2.5, 6.6), (0.06, 1.6, 2.4))
    return m.build()


def make_temple(style):
    st = STYLES[style]
    m = Model("SM_GZ_temple_" + style)
    m.box("stone", (0, 0, 0.5), (10, 10, 1.0))
    m.box("stone", (0, 0, 1.25), (8, 8, 0.5))
    m.box("red", (0, 0, 2.9), (5.5, 5.5, 3.2))
    for x in (-2.75, 2.75):
        for y in (-2.75, 2.75):
            m.cyl("red_dark", (x, y, 2.9), 0.25, 3.2)
    hanok_roof(m, st, 5.5, 5.5, 4.5, tiers=3, height=1.8)
    m.cyl("gold", (0, 0, 9.6), 0.12, 2.0)
    m.ball("gold", (0, 0, 10.7), 0.3)
    m.cyl("stone_dark", (0, -4.6, 1.1), 0.5, 1.2, r2=0.35)
    m.ball("orange", (0, -4.6, 1.85), 0.25, glow=True)
    return m.build()


def make_watchtower(style):
    st = STYLES[style]
    m = Model("SM_GZ_watchtower_" + style)
    for x in (-1.5, 1.5):
        for y in (-1.5, 1.5):
            m.cyl("wood_dark", (x * 0.9, y * 0.9, 4.5), 0.2, 9.0, r2=0.16, rot=(-y * 0.04, x * 0.04, 0))
    for z in (2.5, 5.5):
        m.box("wood", (0, -1.4, z), (3.0, 0.15, 0.2), rot=(0, 0.5, 0))
        m.box("wood", (1.4, 0, z), (0.15, 3.0, 0.2), rot=(0.5, 0, 0))
    m.box("wood", (0, 0, 8.8), (4.0, 4.0, 0.3))
    for s in (-1, 1):
        m.box("wood_light", (0, s * 1.9, 9.4), (4.0, 0.15, 1.0))
        m.box("wood_light", (s * 1.9, 0, 9.4), (0.15, 4.0, 1.0))
    m.roof(st["roof"], (0, 0, 11.0), 5.2, 5.2, 2.0, top=0.05)
    for s in (-1, 1):
        for t in (-1, 1):
            m.cyl("wood_dark", (s * 1.8, t * 1.8, 10.4), 0.1, 1.4)
    m.ball("orange", (0, -2.1, 9.6), 0.25, glow=True)
    return m.build()


def make_gate(style):
    st = STYLES[style]
    m = Model("SM_GZ_gate_" + style)
    for x in (-4.0, 4.0):
        m.box("stone", (x, 0, 0.6), (2.0, 2.0, 1.2))
        m.cyl("red", (x, 0, 3.6), 0.45, 5.0)
    m.box("red_dark", (0, 0, 6.0), (10.0, 0.7, 0.6))
    m.box("red_dark", (0, 0, 5.1), (8.6, 0.4, 0.35))
    m.box("navy", (0, -0.4, 5.6), (2.6, 0.08, 0.7))
    m.box("gold", (0, -0.45, 5.6), (2.2, 0.05, 0.4))
    m.roof(st["roof_dark"], (0, 0, 6.3), 11.5, 2.6, 0.3, top=0.9)
    m.roof(st["roof"], (0, 0, 6.55), 11.0, 2.2, 1.3, top=0.25)
    for s in (-1, 1):
        m.cone(st["roof_dark"], (s * 5.7, -1.2, 6.5), 0.25, 0.8, rot=(0.9, s * 0.9, 0))
        m.cone(st["roof_dark"], (s * 5.7, 1.2, 6.5), 0.25, 0.8, rot=(-0.9, s * 0.9, 0))
    return m.build()


def make_stall(name, canopy, goods):
    m = Model(name)
    for x in (-1.4, 1.4):
        for y in (-1.0, 1.0):
            m.cyl("wood", (x, y, 1.25), 0.07, 2.5)
    m.box("wood_light", (0, -0.6, 0.85), (3.0, 0.9, 0.12))
    m.box("wood", (0, -0.6, 0.42), (2.9, 0.8, 0.8))
    for i in range(6):
        m.box(canopy if i % 2 == 0 else "paper", (-1.5 + 0.25 + i * 0.5, 0, 2.65), (0.5, 2.6, 0.12), rot=(0.18, 0, 0))
    rnd = random.Random(hash(name) & 0xffff)
    for i in range(7):
        c = goods[i % len(goods)]
        if i % 2:
            m.ball(c, (-1.2 + i * 0.4, -0.6 + rnd.uniform(-0.2, 0.2), 1.05), 0.16)
        else:
            m.box(c, (-1.2 + i * 0.4, -0.6 + rnd.uniform(-0.2, 0.2), 1.02), (0.26, 0.26, 0.26))
    return m.build()


def make_stable(style):
    st = STYLES[style]
    m = Model("SM_GZ_stable_" + style)
    for x in (-4, 0, 4):
        for y in (-2.5, 2.5):
            m.cyl("wood_dark", (x, y, 1.6), 0.15, 3.2)
    m.box("wood", (0, 2.6, 1.4), (8.4, 0.2, 2.8))
    m.roof(st["roof"], (0, 0, 3.2), 9.6, 6.6, 1.6, top=0.2)
    for i in range(3):
        m.box("hay", (-3 + i * 1.3, 1.6, 0.45), (1.2, 0.9, 0.9))
    for x in (-5.5, -2, 2, 5.5):
        m.cyl("wood", (x, -4.5, 0.6), 0.08, 1.2)
    m.box("wood_light", (0, -4.5, 0.9), (11.2, 0.1, 0.15))
    m.box("wood_light", (0, -4.5, 0.5), (11.2, 0.1, 0.15))
    m.box("wood", (2.5, -1.0, 0.35), (2.0, 0.6, 0.5))
    m.box("water", (2.5, -1.0, 0.58), (1.8, 0.45, 0.05))
    return m.build()


def make_well():
    m = Model("SM_GZ_well")
    m.cyl("stone", (0, 0, 0.5), 1.2, 1.0, segs=10)
    m.cyl("water", (0, 0, 0.98), 0.95, 0.06, segs=10)
    for x in (-1.0, 1.0):
        m.cyl("wood_dark", (x, 0, 1.6), 0.1, 2.2)
    m.roof("tile", (0, 0, 2.6), 2.8, 1.8, 0.9, top=0.1)
    m.cyl("wood", (0, 0, 2.0), 0.08, 2.0, rot=(0, math.pi / 2, 0))
    return m.build()


def make_lantern():
    m = Model("SM_GZ_lantern")
    m.box("stone", (0, 0, 0.3), (0.6, 0.6, 0.6))
    m.cyl("stone", (0, 0, 1.0), 0.15, 0.9)
    m.box("stone", (0, 0, 1.55), (0.7, 0.7, 0.2))
    m.box("orange", (0, 0, 1.85), (0.45, 0.45, 0.4), glow=True)
    m.roof("stone_dark", (0, 0, 2.05), 0.9, 0.9, 0.45, top=0.1)
    return m.build()


def make_signpost():
    m = Model("SM_GZ_signpost")
    m.cyl("wood_dark", (0, 0, 1.3), 0.08, 2.6)
    m.box("wood_light", (0.5, 0, 2.1), (1.2, 0.08, 0.35))
    m.box("wood_light", (-0.4, 0, 1.6), (1.0, 0.08, 0.3), rot=(0, 0, 0.6))
    return m.build()


def make_fence():
    m = Model("SM_GZ_fence")
    for x in (-2.0, 0.0, 2.0):
        m.cyl("wood_dark", (x, 0, 0.7), 0.1, 1.4, segs=6)
    for z in (0.5, 1.0):
        m.box("wood", (0, 0, z), (4.2, 0.1, 0.14))
    return m.build()


def make_wall(style):
    st = STYLES[style]
    m = Model("SM_GZ_wall_" + style)
    m.box("stone", (0, 0, 0.9), (8.0, 1.0, 1.8))
    m.box("stone_dark", (0, 0, 0.15), (8.2, 1.2, 0.3))
    m.roof(st["roof"], (0, 0, 1.8), 8.6, 1.6, 0.5, top=0.3)
    return m.build()


def make_bridge():
    m = Model("SM_GZ_bridge")
    L, W = 28.0, 4.6
    for i in range(14):
        x = -L / 2 + (i + 0.5) * L / 14
        m.box("wood_light" if i % 2 else "wood", (x, 0, 0.5), (L / 14 + 0.02, W, 0.25))
    for x in (-L / 2 + 4, -4, 4, L / 2 - 4):
        for y in (-W / 2, W / 2):
            m.cyl("wood_dark", (x, y, -0.6), 0.22, 2.8)
    for y in (-W / 2, W / 2):
        m.box("red", (0, y, 1.5), (L, 0.18, 0.18))
        for i in range(8):
            m.cyl("red_dark", (-L / 2 + i * L / 7, y, 1.05), 0.1, 1.1)
    return m.build()


def make_barrel():
    m = Model("SM_GZ_barrel")
    m.cyl("wood", (0, 0, 0.55), 0.42, 1.1, segs=10)
    for z in (0.2, 0.9):
        m.cyl("iron", (0, 0, z), 0.44, 0.08, segs=10)
    return m.build()


def make_crate():
    m = Model("SM_GZ_crate")
    m.box("wood_light", (0, 0, 0.45), (0.9, 0.9, 0.9))
    m.box("wood", (0, -0.46, 0.45), (0.12, 0.04, 0.9), rot=(0, 0.78, 0))
    return m.build()


# ----------------------------------------------------------------------------- nature

def make_tree_oak(name, leaf=("leaf", "leaf_dark", "leaf_light"), seed=1, trunk="bark"):
    rnd = random.Random(seed)
    m = Model(name)
    m.cyl(trunk, (0, 0, 1.6), 0.38, 3.2, r2=0.24, segs=7)
    m.cyl(trunk, (0.5, 0, 2.8), 0.12, 1.6, rot=(0, 0.7, 0), segs=5)
    m.cyl(trunk, (-0.4, 0.3, 3.0), 0.1, 1.4, rot=(0.5, -0.6, 0), segs=5)
    for i in range(5):
        a = i * 1.26 + rnd.uniform(-0.3, 0.3)
        r = 0.0 if i == 0 else rnd.uniform(1.0, 1.5)
        m.ball(leaf[i % len(leaf)], (math.cos(a) * r, math.sin(a) * r, 4.4 + rnd.uniform(-0.3, 0.6)),
               rnd.uniform(1.5, 2.0) if i == 0 else rnd.uniform(1.0, 1.4), sub=1)
    return m.build()


def make_tree_pine(name, snowy=False, seed=2):
    rnd = random.Random(seed)
    m = Model(name)
    m.cyl("bark", (0, 0, 0.9), 0.3, 1.8, r2=0.22, segs=6)
    for i, (z, r) in enumerate(((2.2, 2.2), (3.6, 1.75), (4.9, 1.3), (6.0, 0.85))):
        m.cone("pine", (0, 0, z + 0.9), r, 2.2, segs=7, rot=(0, 0, rnd.uniform(0, 1)))
        if snowy:
            m.cone("snow", (0, 0, z + 1.45), r * 0.55, 1.1, segs=7)
    return m.build()


def make_tree_cherry(seed=3):
    return make_tree_oak("SM_GZ_tree_cherry", leaf=("blossom", "blossom2", "pink"), seed=seed, trunk="wood_dark")


def make_tree_dead(name, burnt=False, seed=4):
    rnd = random.Random(seed)
    m = Model(name)
    c = "black" if burnt else "dead"
    m.cyl(c, (0, 0, 1.8), 0.32, 3.6, r2=0.14, segs=6)
    for i in range(4):
        a = i * 1.57 + rnd.uniform(-0.4, 0.4)
        m.cyl(c, (math.cos(a) * 0.5, math.sin(a) * 0.5, 2.6 + i * 0.3), 0.08, 1.6, r2=0.02,
              rot=(math.sin(a) * -0.9, math.cos(a) * 0.9, 0), segs=5)
    return m.build()


def make_bush(name="SM_GZ_bush", c1="leaf", c2="leaf_dark"):
    m = Model(name)
    for i, (x, y, r) in enumerate(((0, 0, 0.8), (0.7, 0.2, 0.6), (-0.6, -0.2, 0.65), (0.1, 0.6, 0.55))):
        m.ball(c1 if i % 2 == 0 else c2, (x, y, r * 0.7), r, sub=1)
    return m.build()


def make_rock(name, color="stone", seed=5, snow=False, lava=False):
    m = Model(name)
    m.rock(color, (0, 0, 0.5), 1.4, scale=(1.2, 1.0, 0.75), seed=seed)
    m.rock(color, (1.3, 0.6, 0.3), 0.7, seed=seed + 1)
    if snow:
        m.rock("snow", (0, 0, 1.15), 0.9, scale=(1.1, 0.9, 0.35), seed=seed + 2, amp=0.1)
    if lava:
        m.box("lava", (0.2, -0.9, 0.35), (1.2, 0.1, 0.08), rot=(0, 0, 0.4), glow=True)
        m.box("lava", (-0.5, -0.6, 0.6), (0.7, 0.1, 0.08), rot=(0, 0, -0.6), glow=True)
    return m.build()


def make_ore_vein():
    m = Model("SM_GZ_res_ore")
    m.rock("stone_dark", (0, 0, 0.7), 1.6, scale=(1.2, 1.0, 0.8), seed=11)
    rnd = random.Random(12)
    for i in range(7):
        a = rnd.uniform(0, 6.28)
        m.cone("orange" if i % 3 == 0 else "metal", (math.cos(a) * 1.0, math.sin(a) * 1.0, 1.0 + rnd.uniform(0, 0.5)),
               0.22, 0.9, segs=5, rot=(rnd.uniform(-0.6, 0.6), rnd.uniform(-0.6, 0.6), 0), glow=(i % 3 == 0))
    m.cyl("wood", (1.8, -0.4, 0.5), 0.05, 1.3, rot=(0.3, 0.6, 0))
    m.box("iron", (2.2, -0.4, 1.0), (0.6, 0.12, 0.15), rot=(0.3, 0.6, 0))
    return m.build()


def make_oak_grove():
    m = Model("SM_GZ_res_oak")
    m.cyl("bark", (0, 0, 0.35), 0.55, 0.7, segs=8)
    m.cyl("wood_light", (0, 0, 0.71), 0.48, 0.02, segs=8)
    for i in range(3):
        m.cyl("bark", (1.4, -0.6 + i * 0.6, 0.3 + (0.45 if i == 1 else 0)), 0.28, 2.2, rot=(0, math.pi / 2, 0.1 * i), segs=7)
    m.cyl("wood_dark", (-0.9, 0.2, 0.75), 0.04, 1.0, rot=(0, 0.8, 0))
    m.box("metal", (-1.25, 0.2, 1.1), (0.35, 0.05, 0.3), rot=(0, 0.8, 0))
    return m.build()


def make_herb_patch():
    m = Model("SM_GZ_res_herb")
    rnd = random.Random(21)
    for i in range(9):
        x, y = rnd.uniform(-1.2, 1.2), rnd.uniform(-1.2, 1.2)
        m.cone("leaf_dark", (x, y, 0.3), 0.35, 0.6, segs=5)
        m.ball("red", (x, y, 0.62), 0.14, sub=1, glow=(i % 4 == 0))
    return m.build()


def make_crop_field():
    m = Model("SM_GZ_res_field")
    m.box("soil", (0, 0, 0.12), (8.0, 8.0, 0.24))
    for r in range(6):
        y = -3.2 + r * 1.28
        m.box("brown", (0, y, 0.28), (7.6, 0.5, 0.12))
        for k in range(10):
            x = -3.4 + k * 0.76
            m.cone("wheat" if r % 2 else "green", (x, y, 0.75), 0.18, 0.9, segs=4)
    for x in (-4.2, 4.2):
        m.box("wood", (x, 0, 0.4), (0.12, 8.4, 0.12))
    m.cyl("wood", (4.5, 4.5, 1.0), 0.08, 2.0)
    m.ball("hay", (4.5, 4.5, 2.1), 0.3)
    m.box("red", (4.5, 4.5, 1.6), (0.9, 0.12, 0.12))
    return m.build()


def make_fish_spot():
    m = Model("SM_GZ_res_fish")
    for i in range(6):
        m.box("wood_light" if i % 2 else "wood", (0, -i * 0.7, 0.25), (2.2, 0.66, 0.12))
    for x in (-1.0, 1.0):
        for y in (0, -3.5):
            m.cyl("wood_dark", (x, y, -0.4), 0.12, 1.6)
    m.cyl("wood_dark", (0.8, -3.6, 1.1), 0.03, 2.4, rot=(0.6, 0, 0))
    m.box("wood", (-0.6, -0.5, 0.55), (0.6, 0.5, 0.5))
    m.ball("water", (0, -6.0, -0.9), 0.4, scale=(2.5, 2.5, 0.05))
    return m.build()


def make_mine_entrance():
    m = Model("SM_GZ_mine")
    m.rock("stone_dark", (0, 1.5, 2.2), 5.0, scale=(1.2, 0.9, 0.8), seed=31, amp=0.15)
    m.box("black", (0, -2.0, 1.6), (3.0, 1.2, 3.2))
    for x in (-1.6, 1.6):
        m.box("wood", (x, -2.6, 1.6), (0.35, 0.35, 3.2))
    m.box("wood", (0, -2.6, 3.3), (3.9, 0.4, 0.4))
    m.box("iron", (0, -4.2, 0.08), (0.9, 4.0, 0.08))
    m.box("wood_dark", (0, -4.8, 0.5), (1.2, 1.6, 0.7))
    m.ball("orange", (-1.6, -2.9, 2.6), 0.18, glow=True)
    return m.build()


def make_lava_pool():
    m = Model("SM_GZ_lava_pool")
    m.cyl("lava", (0, 0, 0.05), 4.0, 0.1, segs=12, scale=(1.2, 0.9, 1), glow=True)
    for i in range(10):
        a = i * 0.628
        m.rock("black", (math.cos(a) * 4.6, math.sin(a) * 3.6, 0.2), 0.7, seed=40 + i)
    return m.build()


def make_ruin():
    m = Model("SM_GZ_ruin")
    m.box("stone", (0, 0, 0.15), (8, 6, 0.3))
    for i, (x, y, h) in enumerate(((-3, -2, 4.0), (3, -2, 2.2), (-3, 2, 1.4), (3, 2, 3.5))):
        m.cyl("stone", (x, y, h / 2 + 0.3), 0.45, h, segs=8)
    m.box("stone", (-1.2, -2, 4.1), (4.4, 0.8, 0.5), rot=(0, 0.1, 0))
    m.box("leaf_dark", (2.4, 1.5, 0.5), (1.4, 1.2, 0.4))
    return m.build()


def make_crystal_cluster():
    m = Model("SM_GZ_crystals")
    rnd = random.Random(51)
    for i in range(8):
        a = rnd.uniform(0, 6.28)
        r = rnd.uniform(0.0, 1.2)
        m.cone("ice" if i % 2 else "ice_dark", (math.cos(a) * r, math.sin(a) * r, 0.9), 0.35, rnd.uniform(1.5, 2.8), segs=5,
               rot=(rnd.uniform(-0.4, 0.4), rnd.uniform(-0.4, 0.4), 0), glow=(i == 0))
    return m.build()


def make_statue():
    m = Model("SM_GZ_statue")
    m.cyl("stone", (0, 0, 0.4), 2.2, 0.8, segs=12)
    m.cyl("stone_dark", (0, 0, 1.1), 1.3, 0.6, segs=12)
    m.cyl("stone", (0, 0, 2.6), 0.5, 2.4, r2=0.35, segs=8)
    m.ball("stone", (0, 0, 4.15), 0.55)
    m.cyl("black", (0, 0, 4.6), 0.75, 0.05, segs=12)
    m.cyl("black", (0, 0, 4.75), 0.28, 0.3, segs=8)
    m.cyl("gold", (0.6, 0, 3.2), 0.05, 2.6, rot=(0, 0.2, 0))
    return m.build()


# ----------------------------------------------------------------------------- characters

def humanoid(name, skin="skin", top="blue", bottom="navy", hair="black", hat=None, tool=None, scale=1.0,
             robe=True, ears=False, tusks=False, eyes="black", belt="wood_dark", mask=None, bulk=1.0):
    m = Model(name)
    s = scale
    for x in (-0.13, 0.13):
        m.cyl(bottom, (x * s * bulk, 0, 0.32 * s), 0.11 * s * bulk, 0.64 * s, segs=6)
        m.box("wood_dark", (x * s * bulk, -0.05 * s, 0.05 * s), (0.18 * s, 0.3 * s, 0.1 * s))
    if robe:
        m.cyl(top, (0, 0, 0.75 * s), 0.36 * s * bulk, 0.7 * s, r2=0.25 * s * bulk, segs=8)
    m.cyl(top, (0, 0, 1.18 * s), 0.25 * s * bulk, 0.5 * s, r2=0.22 * s * bulk, segs=8)
    m.cyl(belt, (0, 0, 0.98 * s), 0.27 * s * bulk, 0.08 * s, segs=8)
    m.box("white" if top != "white" else "navy", (0, -0.2 * s * bulk, 1.3 * s), (0.18 * s, 0.05 * s, 0.25 * s), rot=(0, 0, 0))
    for sx in (-1, 1):
        m.cyl(top, (sx * 0.33 * s * bulk, 0, 1.12 * s), 0.09 * s * bulk, 0.55 * s, rot=(0, sx * 0.25, 0), segs=6)
        m.ball(skin, (sx * 0.38 * s * bulk, -0.02 * s, 0.82 * s), 0.09 * s * bulk, sub=1)
    m.ball(skin, (0, 0, 1.68 * s), 0.3 * s, sub=2)
    for sx in (-1, 1):
        m.ball(eyes, (sx * 0.1 * s, -0.27 * s, 1.7 * s), 0.045 * s, sub=1, glow=(eyes in ("red", "ice", "orange")))
    if ears:
        for sx in (-1, 1):
            m.cone(skin, (sx * 0.38 * s, 0, 1.72 * s), 0.08 * s, 0.35 * s, rot=(0, sx * 1.4, 0), segs=4)
    if tusks:
        for sx in (-1, 1):
            m.cone("bone", (sx * 0.1 * s, -0.27 * s, 1.55 * s), 0.035 * s, 0.16 * s, segs=4)
    if mask:
        m.box(mask, (0, -0.24 * s, 1.6 * s), (0.5 * s, 0.1 * s, 0.14 * s))
    if hair:
        m.ball(hair, (0, 0.06 * s, 1.8 * s), 0.31 * s, scale=(1.0, 1.0, 0.75), sub=2)
    if hat == "gat":
        m.cyl("black", (0, 0, 1.98 * s), 0.55 * s, 0.03 * s, segs=14)
        m.cyl("black", (0, 0, 2.1 * s), 0.18 * s, 0.25 * s, r2=0.16 * s, segs=10)
    elif hat == "helmet":
        m.ball("steel", (0, 0, 1.86 * s), 0.33 * s, scale=(1, 1, 0.7))
        m.cone("red", (0, 0, 2.15 * s), 0.06 * s, 0.3 * s)
    elif hat == "straw":
        m.cone("thatch", (0, 0, 2.02 * s), 0.6 * s, 0.35 * s, segs=12)
    elif hat == "crown":
        m.cyl("gold", (0, 0, 2.0 * s), 0.22 * s, 0.14 * s, segs=10)
        for i in range(5):
            a = i * 1.256
            m.cone("gold", (math.cos(a) * 0.19 * s, math.sin(a) * 0.19 * s, 2.13 * s), 0.04 * s, 0.14 * s, segs=4)
    elif hat == "hood":
        m.ball(top, (0, 0.05 * s, 1.78 * s), 0.36 * s, scale=(1.0, 1.0, 1.0))
        m.box(skin, (0, -0.26 * s, 1.68 * s), (0.4 * s, 0.1 * s, 0.35 * s))
        for sx in (-1, 1):
            m.ball(eyes, (sx * 0.1 * s, -0.31 * s, 1.7 * s), 0.045 * s, sub=1)
    elif hat == "bandana":
        m.cyl("red", (0, 0, 1.88 * s), 0.31 * s, 0.1 * s, segs=10)
    elif hat == "cap":
        m.ball(top, (0, 0.02 * s, 1.88 * s), 0.3 * s, scale=(1, 1, 0.5))
    elif hat == "scholar":
        m.box("black", (0, 0, 2.02 * s), (0.5 * s, 0.5 * s, 0.06 * s))
        m.cyl("black", (0, 0, 1.92 * s), 0.25 * s, 0.15 * s, segs=8)
    hx, hy, hz = 0.42 * s * bulk, -0.1 * s, 0.85 * s
    if tool == "sword":
        m.box("steel", (hx, hy - 0.45 * s, hz + 0.05 * s), (0.06 * s, 0.9 * s, 0.12 * s))
        m.box("gold", (hx, hy, hz), (0.08 * s, 0.08 * s, 0.25 * s))
    elif tool == "hammer":
        m.cyl("wood", (hx, hy, hz + 0.2 * s), 0.04 * s, 0.7 * s)
        m.box("iron", (hx, hy, hz + 0.55 * s), (0.18 * s, 0.32 * s, 0.18 * s))
    elif tool == "staff":
        m.cyl("wood_dark", (hx, hy, hz + 0.3 * s), 0.04 * s, 1.8 * s)
        m.ball("teal", (hx, hy, hz + 1.25 * s), 0.12 * s, glow=True)
    elif tool == "spear":
        m.cyl("wood", (hx, hy, hz + 0.4 * s), 0.035 * s, 2.0 * s)
        m.cone("steel", (hx, hy, hz + 1.5 * s), 0.07 * s, 0.3 * s)
    elif tool == "pick":
        m.cyl("wood", (hx, hy, hz + 0.25 * s), 0.035 * s, 0.9 * s)
        m.box("iron", (hx, hy, hz + 0.68 * s), (0.08 * s, 0.6 * s, 0.08 * s), rot=(0.3, 0, 0))
    elif tool == "rod":
        m.cyl("wood_light", (hx, hy - 0.4 * s, hz + 0.6 * s), 0.025 * s, 1.8 * s, rot=(0.6, 0, 0))
    elif tool == "basket":
        m.cyl("thatch", (hx, hy, hz - 0.05 * s), 0.18 * s, 0.25 * s, r2=0.22 * s, segs=8)
        m.ball("red", (hx, hy, hz + 0.1 * s), 0.08 * s)
    elif tool == "book":
        m.box("red_dark", (hx, hy, hz + 0.05 * s), (0.08 * s, 0.25 * s, 0.32 * s))
    elif tool == "bag":
        m.ball("leather", (hx, hy, hz - 0.05 * s), 0.17 * s, scale=(1, 1, 1.2))
        m.ball("gold", (hx, hy - 0.15 * s, hz), 0.05 * s)
    elif tool == "flask":
        m.ball("purple", (hx, hy, hz), 0.12 * s, glow=True)
        m.cyl("paper", (hx, hy, hz + 0.14 * s), 0.03 * s, 0.12 * s)
    elif tool == "club":
        m.cyl("wood", (hx, hy, hz + 0.3 * s), 0.06 * s, 0.8 * s, r2=0.12 * s)
    elif tool == "axe":
        m.cyl("wood", (hx, hy, hz + 0.3 * s), 0.05 * s, 1.2 * s)
        m.box("iron", (hx, hy - 0.15 * s, hz + 0.8 * s), (0.06 * s, 0.4 * s, 0.35 * s))
    elif tool == "dagger":
        m.box("steel", (hx, hy - 0.2 * s, hz), (0.04 * s, 0.4 * s, 0.08 * s))
    elif tool == "whip":
        m.cyl("leather", (hx, hy, hz), 0.03 * s, 0.6 * s, rot=(0.8, 0, 0))
    elif tool == "scroll":
        m.cyl("paper", (hx, hy, hz), 0.06 * s, 0.4 * s, rot=(0, math.pi / 2, 0))
    return m.build()


NPC_LOOKS = {
    "player": dict(top="blue", bottom="navy", hair="black", hat="gat", tool="sword"),
    "merchant": dict(top="yellow", bottom="brown", hair="black", hat="cap", tool="bag"),
    "banker": dict(top="purple", bottom="navy", hair="black", hat="scholar", tool="book"),
    "blacksmith": dict(top="leather", bottom="brown", hair="brown", hat="bandana", tool="hammer", bulk=1.2),
    "alchemist": dict(top="teal", bottom="navy", hair="white", hat="hood", tool="flask"),
    "mayor": dict(top="red", bottom="navy", hair="black", hat="gat", tool="scroll"),
    "quest_giver": dict(top="white", bottom="slate", hair="white", hat="straw", tool="staff"),
    "guild_master": dict(top="navy", bottom="black", hair="black", hat="helmet", tool="spear"),
    "guard": dict(top="red_dark", bottom="black", hair="black", hat="helmet", tool="spear"),
    "stable_master": dict(top="brown", bottom="leather", hair="brown", hat="straw", tool="whip"),
    "pet_trainer": dict(top="pink", bottom="cloth_green", hair="brown", hat="cap", tool="basket"),
    "farmer": dict(top="cloth_green", bottom="brown", hair="brown", hat="straw", tool="basket"),
    "miner": dict(top="slate", bottom="brown", hair="black", hat="helmet", tool="pick"),
    "fisher": dict(top="water", bottom="navy", hair="brown", hat="straw", tool="rod"),
    "candidate_0": dict(top="yellow", bottom="navy", hair="black", hat="gat", tool="bag"),
    "candidate_1": dict(top="leather", bottom="brown", hair="brown", hat="bandana", tool="hammer"),
    "candidate_2": dict(top="teal", bottom="navy", hair="white", hat="scholar", tool="book"),
}


def make_npc(role):
    look = dict(NPC_LOOKS[role])
    return humanoid("SM_GZ_npc_" + role if role != "player" else "SM_GZ_player", **look)


# ----------------------------------------------------------------------------- monsters

def make_slime():
    m = Model("SM_GZ_mon_slime")
    m.ball("slime", (0, 0, 0.55), 0.75, scale=(1, 1, 0.75), sub=3)
    m.ball("leaf_light", (0.2, -0.3, 0.95), 0.18, sub=1)
    for sx in (-1, 1):
        m.ball("white", (sx * 0.22, -0.62, 0.7), 0.13, sub=1)
        m.ball("black", (sx * 0.22, -0.72, 0.7), 0.07, sub=1)
    return m.build()


def quadruped(name, body, head_c, size=1.0, snout="black", tusks=False, ears=True, tail=True, stripes=None, eyes="black"):
    m = Model(name)
    s = size
    m.ball(body, (0, 0, 0.9 * s), 0.6 * s, scale=(0.85, 1.5, 0.8), sub=2)
    for x in (-0.3, 0.3):
        for y in (-0.55, 0.55):
            m.cyl(body, (x * s, y * s, 0.35 * s), 0.13 * s, 0.7 * s, segs=6)
    m.ball(head_c, (0, -1.05 * s, 1.15 * s), 0.38 * s, sub=2)
    m.box(head_c, (0, -1.4 * s, 1.05 * s), (0.32 * s, 0.4 * s, 0.26 * s))
    m.box(snout, (0, -1.62 * s, 1.1 * s), (0.16 * s, 0.06 * s, 0.1 * s))
    for sx in (-1, 1):
        m.ball(eyes, (sx * 0.17 * s, -1.33 * s, 1.3 * s), 0.05 * s, sub=1, glow=(eyes != "black"))
        if ears:
            m.cone(head_c, (sx * 0.2 * s, -0.95 * s, 1.5 * s), 0.1 * s, 0.3 * s, segs=4)
        if tusks:
            m.cone("bone", (sx * 0.14 * s, -1.55 * s, 1.02 * s), 0.05 * s, 0.25 * s, rot=(0.6, 0, 0), segs=4)
    if tail:
        m.cone(body, (0, 1.0 * s, 1.05 * s), 0.12 * s, 0.7 * s, rot=(-1.0, 0, 0), segs=5)
    if stripes:
        for i in range(3):
            m.box(stripes, (0, -0.3 * s + i * 0.35 * s, 1.35 * s), (0.5 * s, 0.1 * s, 0.1 * s))
    return m.build()


def make_spider():
    m = Model("SM_GZ_mon_giant_spider")
    m.ball("purple", (0, 0.5, 0.9), 0.75, scale=(1, 1.2, 0.85))
    m.ball("black", (0, -0.4, 0.75), 0.45)
    for sx in (-1, 1):
        for i in range(4):
            y = -0.6 + i * 0.35
            m.cyl("black", (sx * 0.75, y, 1.0), 0.06, 1.0, rot=(0, sx * 1.1, 0), segs=5)
            m.cyl("black", (sx * 1.35, y, 0.5), 0.05, 1.0, rot=(0, sx * -0.4, 0), segs=5)
        m.ball("red", (sx * 0.12, -0.8, 0.85), 0.07, sub=1, glow=True)
        m.ball("red", (sx * 0.22, -0.75, 0.95), 0.05, sub=1, glow=True)
    m.box("red", (0, 0.5, 1.6), (0.25, 0.6, 0.05))
    return m.build()


def make_ice_elemental():
    m = Model("SM_GZ_mon_ice_elemental")
    m.ball("ice_dark", (0, 0, 1.2), 0.65, sub=1)
    rnd = random.Random(61)
    for i in range(9):
        a = i * 0.7
        m.cone("ice", (math.cos(a) * 0.5, math.sin(a) * 0.5, 1.3 + rnd.uniform(-0.4, 0.5)), 0.22, 1.2, segs=4,
               rot=(math.sin(a) * -1.0, math.cos(a) * 1.0, 0))
    for sx in (-1, 1):
        m.ball("white", (sx * 0.2, -0.6, 1.35), 0.1, sub=1, glow=True)
        m.cone("ice", (sx * 0.9, 0, 1.2), 0.2, 0.9, rot=(0, sx * 1.9, 0), segs=4)
    m.cone("ice_dark", (0, 0, 0.35), 0.5, 0.7, rot=(math.pi, 0, 0), segs=5)
    return m.build()


def make_fire_elemental():
    m = Model("SM_GZ_mon_fire_elemental")
    m.rock("black", (0, 0, 0.35), 0.7, seed=71)
    m.cone("lava", (0, 0, 1.3), 0.7, 1.8, segs=6, glow=True)
    m.cone("orange", (0.25, 0.1, 1.9), 0.4, 1.4, segs=5, glow=True)
    m.cone("yellow", (-0.2, -0.1, 2.0), 0.3, 1.2, segs=5, glow=True)
    for sx in (-1, 1):
        m.ball("white", (sx * 0.2, -0.55, 1.35), 0.1, sub=1, glow=True)
        m.cone("orange", (sx * 0.8, 0, 1.3), 0.18, 0.8, rot=(0, sx * 1.9, 0), segs=5, glow=True)
    return m.build()


def make_guardian():
    m = Model("SM_GZ_mon_forest_guardian")
    m.cyl("bark", (0, 0, 1.4), 0.6, 2.2, r2=0.45, segs=7)
    for x in (-0.35, 0.35):
        m.cyl("bark", (x, 0, 0.4), 0.22, 0.8, segs=6)
    for sx in (-1, 1):
        m.cyl("bark", (sx * 0.9, 0, 1.8), 0.15, 1.4, rot=(0, sx * 1.0, 0), segs=6)
        m.ball("leaf_dark", (sx * 1.4, 0, 1.3), 0.3, sub=1)
        m.ball("yellow", (sx * 0.2, -0.5, 2.2), 0.09, sub=1, glow=True)
    m.ball("leaf", (0, 0, 2.9), 0.9, sub=1)
    m.ball("leaf_dark", (0.5, 0.3, 3.1), 0.55, sub=1)
    m.ball("blossom", (-0.4, -0.4, 3.3), 0.25, sub=1)
    return m.build()


def make_drake():
    m = Model("SM_GZ_mon_ember_drake")
    m.ball("red", (0, 0, 2.2), 1.5, scale=(1.0, 1.8, 0.9), sub=2)
    m.ball("orange", (0, -0.5, 1.7), 1.1, scale=(0.8, 1.4, 0.6), sub=2)
    for x in (-0.9, 0.9):
        for y in (-1.3, 1.3):
            m.cyl("red_dark", (x, y, 0.8), 0.35, 1.6, segs=7)
            m.cone("bone", (x, y - 0.35, 0.1), 0.1, 0.35, rot=(1.57, 0, 0), segs=4)
    for i in range(4):
        m.cyl("red", (0, -2.3 - i * 0.45, 2.7 + i * 0.45), 0.5 - i * 0.06, 0.8, rot=(0.7, 0, 0), segs=8)
    m.ball("red", (0, -4.3, 4.6), 0.75, scale=(0.9, 1.3, 0.8), sub=2)
    m.box("red_dark", (0, -5.1, 4.4), (0.7, 0.9, 0.5))
    for sx in (-1, 1):
        m.cone("bone", (sx * 0.35, -3.9, 5.3), 0.12, 1.0, rot=(-0.6, sx * 0.4, 0), segs=5)
        m.ball("yellow", (sx * 0.38, -4.8, 4.85), 0.12, sub=1, glow=True)
        # wings
        m.cone("red_dark", (sx * 2.6, 0.0, 3.6), 1.8, 0.12, segs=3, rot=(0, sx * 0.35, 0), scale=(1.6, 1.0, 1.0))
        m.cyl("red", (sx * 1.6, -0.4, 3.4), 0.12, 2.6, rot=(0, sx * 1.2, 0), segs=5)
    for i in range(5):
        m.cone("red", (0, 2.6 + i * 0.7, 2.0 - i * 0.25), 0.5 - i * 0.08, 0.9, rot=(-1.4, 0, 0), segs=6)
    for i in range(6):
        m.cone("gold", (0, -1.5 + i * 0.6, 3.25 - abs(i - 2) * 0.1), 0.15, 0.5, segs=4)
    m.cone("lava", (0, -5.7, 4.3), 0.3, 0.8, rot=(1.57, 0, 0), segs=5, glow=True)
    return m.build()


MONSTERS = {
    "slime": make_slime,
    "goblin": lambda: humanoid("SM_GZ_mon_goblin", skin="goblin", top="leather", bottom="brown", hair=None, tool="club", scale=0.75, ears=True, robe=False, eyes="red"),
    "wolf": lambda: quadruped("SM_GZ_mon_dire_wolf", "stone_dark", "stone", eyes="yellow"),
    "boar": lambda: quadruped("SM_GZ_mon_wild_boar", "brown", "wood_dark", tusks=True, size=0.85, stripes="wood_dark"),
    "skeleton": lambda: humanoid("SM_GZ_mon_skeleton_warrior", skin="bone", top="bone", bottom="bone", hair=None, tool="sword", robe=False, eyes="red", belt="stone_dark", bulk=0.75),
    "spider": make_spider,
    "bandit": lambda: humanoid("SM_GZ_mon_bandit", top="black", bottom="slate", hair="black", hat="bandana", tool="dagger", mask="black"),
    "orc": lambda: humanoid("SM_GZ_mon_orc_brute", skin="orc", top="iron", bottom="brown", hair="black", tool="axe", scale=1.35, tusks=True, robe=False, eyes="red", bulk=1.3),
    "ice_elemental": make_ice_elemental,
    "fire_elemental": make_fire_elemental,
    "forest_guardian": make_guardian,
    "drake": make_drake,
}

# Display name -> mesh name (must match the HUD's "mon_<lower_name>" icon id).
MONSTER_NAMES = {
    "Slime": "SM_GZ_mon_slime", "Goblin": "SM_GZ_mon_goblin", "Dire Wolf": "SM_GZ_mon_dire_wolf",
    "Wild Boar": "SM_GZ_mon_wild_boar", "Skeleton Warrior": "SM_GZ_mon_skeleton_warrior",
    "Giant Spider": "SM_GZ_mon_giant_spider", "Bandit": "SM_GZ_mon_bandit", "Orc Brute": "SM_GZ_mon_orc_brute",
    "Ice Elemental": "SM_GZ_mon_ice_elemental", "Fire Elemental": "SM_GZ_mon_fire_elemental",
    "Forest Guardian": "SM_GZ_mon_forest_guardian", "Ember Drake": "SM_GZ_mon_ember_drake",
}

# ----------------------------------------------------------------------------- items (icons)


def bottle(name, liquid, round_=True):
    m = Model(name)
    if round_:
        m.ball(liquid, (0, 0, 0.35), 0.32, sub=2, glow=True)
    else:
        m.cyl(liquid, (0, 0, 0.35), 0.25, 0.6, segs=10, glow=True)
    m.cyl("ice", (0, 0, 0.75), 0.1, 0.3, segs=8)
    m.cyl("wood_light", (0, 0, 0.95), 0.11, 0.12, segs=8)
    return m.build()


def sword(name, blade="metal"):
    m = Model(name)
    m.box(blade, (0, 0, 0.75), (0.16, 0.04, 1.2))
    m.cone(blade, (0, 0, 1.45), 0.11, 0.2, segs=4, scale=(1, 0.3, 1))
    m.box("gold", (0, 0, 0.13), (0.5, 0.1, 0.08))
    m.cyl("leather", (0, 0, -0.08), 0.05, 0.35, segs=6)
    m.ball("gold", (0, 0, -0.27), 0.07, sub=1)
    return m.build()


def ingot(name, c):
    m = Model(name)
    for i, (x, y, z) in enumerate(((-0.3, 0, 0.12), (0.3, 0, 0.12), (0, 0, 0.36))):
        m.cyl(c, (x, y, z), 0.42, 0.24, r2=0.32, segs=4, rot=(0, 0, math.pi / 4), scale=(1.5, 0.7, 1))
    return m.build()


def make_items():
    out = []
    out.append(sword("SM_GZ_item_iron_sword"))
    out.append(sword("SM_GZ_item_steel_sword", "steel"))
    out.append(ingot("SM_GZ_item_iron_ingot", "iron"))
    out.append(ingot("SM_GZ_item_steel_ingot", "steel"))
    m = Model("SM_GZ_item_iron_ore"); m.rock("stone_dark", (0, 0, 0.4), 0.5, seed=81)
    for i in range(5):
        m.ball("orange", (math.cos(i) * 0.35, math.sin(i) * 0.35, 0.55), 0.09, sub=1, glow=True)
    out.append(m.build())
    m = Model("SM_GZ_item_oak_log"); m.cyl("bark", (0, 0, 0.3), 0.3, 1.2, rot=(0, math.pi / 2, 0.3), segs=10)
    m.cyl("wood_light", (0.6 * math.cos(0.3), 0.6 * math.sin(0.3), 0.3), 0.27, 0.02, rot=(0, math.pi / 2, 0.3), segs=10); out.append(m.build())
    for n, c in (("wood_handle", "wood_light"), ("oak_handle", "wood_dark")):
        m = Model("SM_GZ_item_" + n); m.cyl(c, (0, 0, 0.5), 0.07, 1.0, segs=8)
        m.cyl("leather", (0, 0, 0.35), 0.085, 0.4, segs=8); m.ball("gold", (0, 0, 0.0), 0.09, sub=1); out.append(m.build())
    m = Model("SM_GZ_item_leather_strip")
    for i in range(3):
        m.cyl("leather", (0, 0, 0.1 + i * 0.06), 0.4 - i * 0.08, 0.06, r2=0.4 - i * 0.08, segs=12)
    m.box("leather", (0.5, 0, 0.05), (0.6, 0.2, 0.04)); out.append(m.build())
    m = Model("SM_GZ_item_red_herb")
    for i in range(5):
        m.cone("leaf_dark", (math.cos(i * 1.2) * 0.15, math.sin(i * 1.2) * 0.15, 0.3), 0.12, 0.6, rot=(math.sin(i) * 0.4, math.cos(i) * 0.4, 0), segs=4)
    for i in range(3):
        m.ball("red", (math.cos(i * 2) * 0.12, math.sin(i * 2) * 0.12, 0.65), 0.1, sub=1)
    out.append(m.build())
    out.append(bottle("SM_GZ_item_herbal_extract", "leaf_light", round_=False))
    out.append(bottle("SM_GZ_item_clear_water", "water", round_=False))
    out.append(bottle("SM_GZ_item_healing_tonic", "red"))
    out.append(bottle("SM_GZ_item_mana_draught", "blue"))
    m = Model("SM_GZ_item_grain"); m.ball("thatch", (0, 0, 0.4), 0.42, scale=(1, 1, 1.1)); m.cyl("wood_dark", (0, 0, 0.82), 0.12, 0.08)
    for i in range(4):
        m.cone("wheat", (math.cos(i) * 0.1, math.sin(i) * 0.1, 1.0), 0.06, 0.4, segs=4)
    out.append(m.build())
    m = Model("SM_GZ_item_salt"); m.cyl("wood", (0, 0, 0.12), 0.45, 0.24, r2=0.38, segs=12); m.ball("white", (0, 0, 0.28), 0.36, scale=(1, 1, 0.45)); out.append(m.build())
    m = Model("SM_GZ_item_fish"); m.ball("water", (0, 0, 0.3), 0.3, scale=(1.8, 0.6, 0.8)); m.cone("water", (0.75, 0, 0.3), 0.25, 0.4, rot=(0, -math.pi / 2, 0), segs=3, scale=(1, 0.3, 1))
    m.ball("black", (-0.38, -0.15, 0.36), 0.05, sub=1); out.append(m.build())
    m = Model("SM_GZ_item_grilled_fish"); m.ball("orange", (0, 0, 0.3), 0.3, scale=(1.8, 0.6, 0.8)); m.cone("brown", (0.75, 0, 0.3), 0.25, 0.4, rot=(0, -math.pi / 2, 0), segs=3, scale=(1, 0.3, 1))
    m.cyl("wood_light", (0, 0, 0.3), 0.03, 1.6, rot=(0, math.pi / 2, 0)); out.append(m.build())
    m = Model("SM_GZ_item_crystal_gem"); m.cone("ice_dark", (0, 0, 0.55), 0.35, 0.4, segs=6, glow=True); m.cone("ice", (0, 0, 0.15), 0.35, 0.4, segs=6, rot=(math.pi, 0, 0)); out.append(m.build())
    m = Model("SM_GZ_item_traveler_bread"); m.ball("wheat", (0, 0, 0.25), 0.4, scale=(1.5, 0.8, 0.6)); m.ball("brown", (0, 0, 0.35), 0.3, scale=(1.5, 0.6, 0.4)); out.append(m.build())
    m = Model("SM_GZ_item_hunting_bow"); m.cyl("wood", (0, 0, 0.6), 0.05, 1.3, rot=(0.0, 0, 0)); m.cyl("wood", (0.15, 0, 1.25), 0.04, 0.5, rot=(0, -0.5, 0)); m.cyl("wood", (0.15, 0, -0.05), 0.04, 0.5, rot=(0, 0.5, 0))
    m.cyl("white", (0.27, 0, 0.6), 0.01, 1.5); m.cyl("leather", (0, 0, 0.6), 0.06, 0.25); out.append(m.build())
    for n, c, t in (("leather_armor", "leather", "wood_dark"), ("iron_armor", "steel", "gold")):
        m = Model("SM_GZ_item_" + n); m.cyl(c, (0, 0, 0.5), 0.45, 0.9, r2=0.38, segs=8, scale=(1, 0.6, 1))
        for sx in (-1, 1):
            m.ball(c, (sx * 0.5, 0, 0.85), 0.2, sub=1)
        m.box(t, (0, -0.28, 0.5), (0.08, 0.05, 0.8)); m.box(t, (0, 0, 0.12), (0.9, 0.55, 0.1)); out.append(m.build())
    m = Model("SM_GZ_item_bag"); m.ball("leather", (0, 0, 0.4), 0.42, scale=(1, 1, 1.05)); m.cyl("wood_dark", (0, 0, 0.82), 0.14, 0.12); m.ball("gold", (0, -0.4, 0.45), 0.08, sub=1); out.append(m.build())
    m = Model("SM_GZ_item_scroll"); m.box("paper", (0, 0, 0.3), (0.9, 0.05, 0.6)); m.cyl("wood", (0, 0, 0.62), 0.06, 1.0, rot=(0, math.pi / 2, 0)); m.cyl("wood", (0, 0, -0.02), 0.06, 1.0, rot=(0, math.pi / 2, 0))
    m.box("red", (0, -0.04, 0.3), (0.3, 0.02, 0.3)); out.append(m.build())
    # skill / tool icons
    m = Model("SM_GZ_tool_pickaxe"); m.cyl("wood", (0, 0, 0.5), 0.05, 1.2); m.cyl("iron", (0, 0, 1.08), 0.07, 1.0, r2=0.02, rot=(0, math.pi / 2, 0), segs=6); out.append(m.build())
    m = Model("SM_GZ_tool_rod"); m.cyl("wood_light", (0, 0, 0.7), 0.03, 1.6, rot=(0, 0.5, 0)); m.cyl("white", (0.75, 0, 0.6), 0.008, 1.0); m.ball("water", (0.75, 0, 0.1), 0.15, scale=(1.8, 0.6, 0.8)); out.append(m.build())
    m = Model("SM_GZ_tool_sheaf")
    for i in range(9):
        a = i * 0.7
        m.cyl("wheat", (math.cos(a) * 0.08, math.sin(a) * 0.08, 0.5), 0.03, 1.0, rot=(math.sin(a) * 0.15, math.cos(a) * 0.15, 0))
        m.cone("wheat", (math.cos(a) * 0.15, math.sin(a) * 0.15, 1.05), 0.06, 0.25, segs=4)
    m.cyl("red", (0, 0, 0.45), 0.13, 0.08); out.append(m.build())
    m = Model("SM_GZ_tool_anvil"); m.box("iron", (0, 0, 0.15), (0.5, 0.35, 0.3)); m.box("iron", (0, 0, 0.42), (0.3, 0.25, 0.25)); m.box("iron", (0, 0, 0.62), (0.9, 0.35, 0.18)); m.cone("iron", (0.6, 0, 0.62), 0.12, 0.35, rot=(0, math.pi / 2, 0), segs=4)
    m.cyl("wood", (0.2, -0.3, 0.95), 0.03, 0.6, rot=(0.9, 0, 0)); m.box("steel", (0.2, -0.05, 1.15), (0.12, 0.25, 0.12), rot=(0.9, 0, 0)); out.append(m.build())
    m = Model("SM_GZ_tool_cauldron"); m.ball("black", (0, 0, 0.45), 0.5, scale=(1, 1, 0.8)); m.cyl("slime", (0, 0, 0.82), 0.4, 0.05, segs=12, glow=True)
    for i in range(3):
        m.ball("leaf_light", (0.15 * i - 0.15, 0.1, 0.95 + 0.1 * i), 0.07, sub=1, glow=True)
    out.append(m.build())
    m = Model("SM_GZ_tool_pot"); m.cyl("iron", (0, 0, 0.3), 0.45, 0.5, r2=0.5, segs=12); m.cyl("orange", (0, 0, 0.56), 0.42, 0.04, segs=12)
    m.ball("leaf", (0.1, 0.1, 0.62), 0.1, sub=1); m.ball("wheat", (-0.15, 0, 0.62), 0.1, sub=1); m.cyl("wood", (0.5, 0, 0.8), 0.03, 0.9, rot=(0, 0.6, 0)); out.append(m.build())
    m = Model("SM_GZ_tool_coins")
    for i in range(6):
        m.cyl("gold", (0.05 * (i % 2), 0, 0.05 + i * 0.09), 0.35, 0.08, segs=12)
    m.cyl("gold", (0.55, 0.2, 0.04), 0.3, 0.08, segs=12, rot=(0.2, 0, 0)); out.append(m.build())
    m = Model("SM_GZ_tool_axe"); m.cyl("wood", (0, 0, 0.6), 0.05, 1.3); m.box("iron", (0.18, 0, 1.1), (0.4, 0.06, 0.35)); out.append(m.build())
    m = Model("SM_GZ_tool_bow"); m.cyl("wood", (0, 0, 0.6), 0.05, 1.3); m.cyl("wood", (0.15, 0, 1.25), 0.04, 0.5, rot=(0, -0.5, 0)); m.cyl("wood", (0.15, 0, -0.05), 0.04, 0.5, rot=(0, 0.5, 0))
    m.cyl("white", (0.27, 0, 0.6), 0.01, 1.5); m.cyl("wood_light", (0.1, 0, 0.6), 0.015, 1.3, rot=(0, math.pi / 2, 0)); m.cone("steel", (0.78, 0, 0.6), 0.04, 0.12, rot=(0, math.pi / 2, 0), segs=4); out.append(m.build())
    return out


# Icon id -> mesh used for it (items use the same id as the RPG item ids).
SKILL_ICONS = {
    "sword": "SM_GZ_item_iron_sword", "bow": "SM_GZ_tool_bow", "mining": "SM_GZ_tool_pickaxe", "fishing": "SM_GZ_tool_rod",
    "farming": "SM_GZ_tool_sheaf", "blacksmithing": "SM_GZ_tool_anvil", "alchemy": "SM_GZ_tool_cauldron",
    "cooking": "SM_GZ_tool_pot", "trading": "SM_GZ_tool_coins", "woodworking": "SM_GZ_tool_axe",
}

# ----------------------------------------------------------------------------- world definition (UE metres)

TOWNS = [
    dict(name="Frostmere", style="frost", c=(130.0, -70.0), r=46.0),
    dict(name="Emberfall", style="ember", c=(0.0, 0.0), r=46.0),
    dict(name="Greenhollow", style="green", c=(-110.0, 90.0), r=46.0),
]
# Small villages along the roads (GoonZu-style network of settlements between the main towns).
HAMLETS = [
    dict(name="Pinewatch", c=(72.0, -62.0), style="frost", elder="Elder Moon"),
    dict(name="Ashford", c=(96.0, 92.0), style="ember", elder="Elder Cheon"),
    dict(name="Mossgate", c=(-58.0, -66.0), style="green", elder="Elder Gu"),
    dict(name="Saltmarsh", c=(-165.0, -28.0), style="green", elder="Elder Pyeon"),
    dict(name="Snowcrest", c=(98.0, -128.0), style="frost", elder="Elder Seo"),
    dict(name="Willowford", c=(-40.0, 112.0), style="ember", elder="Elder Ha"),
]
HAMLET_R = 16.0

RIVER = [(212, 62), (150, 58), (100, 48), (60, 52), (25, 60), (-20, 54), (-60, 36), (-100, 22), (-150, 8), (-212, -2)]
LAKE = dict(c=(-138.0, 128.0), r=17.0)
VOLCANO = (152.0, 152.0)


def _curve(a, b, bend):
    mx, my = (a[0] + b[0]) / 2, (a[1] + b[1]) / 2
    dx, dy = b[0] - a[0], b[1] - a[1]
    L = math.hypot(dx, dy) or 1
    mid = (mx - dy / L * bend, my + dx / L * bend)
    pts = []
    for i in range(9):
        t = i / 8
        x = (1 - t) ** 2 * a[0] + 2 * (1 - t) * t * mid[0] + t * t * b[0]
        y = (1 - t) ** 2 * a[1] + 2 * (1 - t) * t * mid[1] + t * t * b[1]
        pts.append((x, y))
    return pts


ROADS = [
    dict(name="North Road", pts=_curve((0, 0), (130, -70), 14), to="Frostmere"),
    dict(name="West Road", pts=_curve((0, 0), (-110, 90), -12), to="Greenhollow"),
    dict(name="Lair Road", pts=_curve((0, 0), (122, 116), 10), to="Dragon Lair"),
    dict(name="Forest Road", pts=_curve((0, 0), (-92, -92), 10), to="Forgotten Forest"),
    dict(name="Mine Road", pts=_curve((130, -70), (168, -36), 5), to="Iron Mine"),
    dict(name="Lake Path", pts=_curve((-110, 90), (-128, 118), 3), to="Mirror Lake"),
    dict(name="Goblin Trail", pts=_curve((60, 60), (52, 118), 6), to="Goblin Hills"),
    dict(name="Marsh Road", pts=_curve((-110, 90), (-165, -28), -14), to="Saltmarsh"),
    dict(name="Snow Trail", pts=_curve((130, -70), (98, -128), 6), to="Snowcrest"),
    dict(name="Willow Lane", pts=_curve((-110, 90), (-40, 112), 8), to="Willowford"),
    dict(name="Pine Lane", pts=_curve((60, -22), (72, -62), 4), to="Pinewatch"),
    dict(name="Moss Lane", pts=_curve((-45, -44), (-58, -66), 3), to="Mossgate"),
]

ZONES = [
    dict(name="Emberfall Meadows", c=(25, -70), r=38, mons=[("Slime", 1, 3, 10), ("Wild Boar", 3, 4, 5)]),
    dict(name="Emberfall Farmlands", c=(-62, -18), r=18, mons=[]),
    dict(name="Goblin Hills", c=(50, 125), r=34, mons=[("Goblin", 4, 6, 9)]),
    dict(name="Iron Hills", c=(172, -32), r=26, mons=[("Dire Wolf", 5, 6, 3)]),
    dict(name="Frost Peaks", c=(150, -150), r=40, mons=[("Dire Wolf", 6, 8, 5), ("Ice Elemental", 8, 10, 6)]),
    dict(name="Greenwood", c=(-152, 152), r=40, mons=[("Dire Wolf", 5, 6, 4), ("Forest Guardian", 9, 11, 3)]),
    dict(name="Mirror Lake", c=(-138, 128), r=22, mons=[]),
    dict(name="Forgotten Forest", c=(-112, -112), r=50, mons=[("Giant Spider", 5, 7, 6), ("Skeleton Warrior", 7, 9, 6), ("Bandit", 6, 8, 4)]),
    dict(name="Dragon Lair", c=(132, 130), r=46, mons=[("Fire Elemental", 12, 14, 6), ("Orc Brute", 11, 13, 6)]),
]


def smooth(e0, e1, x):
    t = max(0.0, min(1.0, (x - e0) / (e1 - e0)))
    return t * t * (3 - 2 * t)


def seg_dist(p, a, b):
    ax, ay = a
    bx, by = b
    dx, dy = bx - ax, by - ay
    L2 = dx * dx + dy * dy or 1e-9
    t = max(0.0, min(1.0, ((p[0] - ax) * dx + (p[1] - ay) * dy) / L2))
    return math.hypot(p[0] - (ax + t * dx), p[1] - (ay + t * dy))


def poly_dist(p, pts):
    return min(seg_dist(p, pts[i], pts[i + 1]) for i in range(len(pts) - 1))


def road_dist(p):
    return min(poly_dist(p, r["pts"]) for r in ROADS)


def river_dist(p):
    return poly_dist(p, RIVER)


def n2(x, y, s, z=0.0):
    return noise.noise(Vector((x / s, y / s, z)))


def base_height(x, y):
    h = 2.0 + n2(x, y, 55, 0.3) * 3.2 + n2(x, y, 17, 1.7) * 0.9
    d = max(abs(x), abs(y))
    if d > 160:
        h += ((d - 160) / 40.0) ** 1.6 * 32.0
    dv = math.hypot(x - VOLCANO[0], y - VOLCANO[1])
    if dv < 60:
        h += (60 - dv) * 0.6
        if dv < 10:
            h -= (10 - dv) * 2.4
    dp = math.hypot(x - 165, y - 165 * -1)
    if dp < 55:
        h += (55 - dp) * 0.4 * (1 + 0.3 * n2(x, y, 9, 4.0))
    return h


def flat_mask(x, y):
    m = 1.0
    for t in TOWNS:
        d = math.hypot(x - t["c"][0], y - t["c"][1])
        m = min(m, smooth(t["r"], t["r"] + 22, d))
    m = min(m, smooth(5, 14, road_dist((x, y))))
    for hm in HAMLETS:
        m = min(m, smooth(HAMLET_R, HAMLET_R + 14, math.hypot(x - hm["c"][0], y - hm["c"][1])))
    for z in ZONES:
        if z["name"] in ("Emberfall Farmlands", "Mirror Lake"):
            m = min(m, smooth(z["r"] * 0.8, z["r"] + 12, math.hypot(x - z["c"][0], y - z["c"][1])))
    return m


WATER_Z = -0.9


def height(x, y):
    m = flat_mask(x, y)
    h = base_height(x, y) * m + 0.12 * (1 - m)
    dr = river_dist((x, y))
    carve = smooth(15, 6, dr)
    dl = math.hypot(x - LAKE["c"][0], y - LAKE["c"][1])
    carve = max(carve, smooth(LAKE["r"] + 8, LAKE["r"] - 2, dl))
    return h * (1 - carve) + (-2.3) * carve


BIOME_SEEDS = [
    ("temperate", (0, 0)), ("temperate", (40, -70)), ("temperate", (-40, -20)), ("temperate", (60, 40)), ("temperate", (45, 120)),
    ("snow", (130, -70)), ("snow", (150, -150)), ("snow", (180, -20)), ("snow", (90, -140)),
    ("forest", (-110, 90)), ("forest", (-150, 150)), ("forest", (-60, 150)), ("forest", (-170, 60)),
    ("swamp", (-112, -112)), ("swamp", (-170, -60)), ("swamp", (-60, -165)),
    ("volcanic", (140, 135)), ("volcanic", (180, 90)), ("volcanic", (95, 180)),
]

BIOME_COL = dict(temperate=(0.40, 0.66, 0.27), snow=(0.90, 0.93, 0.98), forest=(0.24, 0.52, 0.22),
                 swamp=(0.30, 0.36, 0.22), volcanic=(0.33, 0.25, 0.22))


def biome_at(x, y):
    best, bd = "temperate", 1e9
    jx = n2(x, y, 30, 7.0) * 18
    jy = n2(x, y, 30, 9.0) * 18
    for b, (sx, sy) in BIOME_SEEDS:
        d = math.hypot(x + jx - sx, y + jy - sy)
        if d < bd:
            bd, best = d, b
    return best


def lerp3(a, b, t):
    return tuple(a[i] + (b[i] - a[i]) * t for i in range(3))


def ground_color(x, y, h):
    b = biome_at(x, y)
    c = BIOME_COL[b]
    v = n2(x, y, 6, 3.3) * 0.06
    c = tuple(max(0, min(1, k + v)) for k in c)
    if b == "temperate" and n2(x, y, 25, 5.0) > 0.25:
        c = lerp3(c, (0.52, 0.70, 0.30), 0.5)
    rd = road_dist((x, y))
    c = lerp3(c, (0.66, 0.54, 0.38) if b != "snow" else (0.75, 0.75, 0.78), smooth(6.5, 3.5, rd))
    for t in TOWNS:
        d = math.hypot(x - t["c"][0], y - t["c"][1])
        if d < 16:
            c = lerp3(c, (0.72, 0.66, 0.56) if t["style"] != "frost" else (0.8, 0.82, 0.86), smooth(16, 12, d))
    wd = min(river_dist((x, y)), math.hypot(x - LAKE["c"][0], y - LAKE["c"][1]) - LAKE["r"] + 6)
    c = lerp3(c, (0.80, 0.74, 0.52), smooth(12, 7, wd))
    if h < -1.0:
        c = lerp3(c, (0.42, 0.40, 0.33), 0.8)
    if h > 16:
        c = lerp3(c, (0.50, 0.48, 0.46) if b != "volcanic" else (0.22, 0.18, 0.17), smooth(16, 24, h))
    if h > 30 and b != "volcanic":
        c = lerp3(c, (0.95, 0.96, 1.0), smooth(30, 36, h))
    dv = math.hypot(x - VOLCANO[0], y - VOLCANO[1])
    if dv < 12:
        c = (1.0, 0.45, 0.08) if dv < 7 else lerp3(c, (0.15, 0.1, 0.1), 0.7)
    return c


def make_terrain(step=2.0, half=200.0):
    """UE coordinates (x, y) are written to Blender as (x, -y) so the FBX import lands them in place."""
    m = Model("SM_GZ_Terrain")
    bm = m.bm
    n = int(half * 2 / step) + 1
    verts = []
    hs = []
    for j in range(n):
        y = -half + j * step
        for i in range(n):
            x = -half + i * step
            h = height(x, y)
            hs.append(h)
            verts.append(bm.verts.new((x, -y, h)))
    bm.verts.ensure_lookup_table()
    bm.verts.index_update()
    vcol = [ground_color(-half + (k % n) * step, -half + (k // n) * step, hs[k]) for k in range(len(verts))]
    for j in range(n - 1):
        for i in range(n - 1):
            a = j * n + i
            f = bm.faces.new((verts[a], verts[a + n], verts[a + n + 1], verts[a + 1]))
            f.smooth = True
            for lp in f.loops:
                k = lp.vert.index
                cc = vcol[k]
                glow = cc[0] > 0.95 and cc[1] < 0.5
                lp[m.cl] = (cc[0], cc[1], cc[2], 0.0 if glow else 1.0)
    bm.normal_update()
    return m.build()


def make_water():
    m = Model("SM_GZ_Water")
    bm = m.bm
    # river ribbon
    pts = []
    for i in range(len(RIVER) - 1):
        a, b = RIVER[i], RIVER[i + 1]
        L = math.hypot(b[0] - a[0], b[1] - a[1])
        k = max(1, int(L / 4))
        for s in range(k):
            t = s / k
            pts.append((a[0] + (b[0] - a[0]) * t, a[1] + (b[1] - a[1]) * t))
    pts.append(RIVER[-1])
    prev = None
    for i, p in enumerate(pts):
        q = pts[min(i + 1, len(pts) - 1)]
        o = pts[max(i - 1, 0)]
        dx, dy = q[0] - o[0], q[1] - o[1]
        L = math.hypot(dx, dy) or 1
        nx, ny = -dy / L * 11, dx / L * 11
        v1 = bm.verts.new((p[0] + nx, -(p[1] + ny), WATER_Z))
        v2 = bm.verts.new((p[0] - nx, -(p[1] - ny), WATER_Z))
        if prev:
            f = bm.faces.new((prev[0], prev[1], v2, v1))
        prev = (v1, v2)
    # lake
    c = bm.verts.new((LAKE["c"][0], -LAKE["c"][1], WATER_Z))
    ring = [bm.verts.new((LAKE["c"][0] + math.cos(a * math.pi / 16) * (LAKE["r"] + 3), -(LAKE["c"][1] + math.sin(a * math.pi / 16) * (LAKE["r"] + 3)), WATER_Z)) for a in range(32)]
    for i in range(32):
        bm.faces.new((c, ring[(i + 1) % 32], ring[i]))
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces[:])
    for f in bm.faces:
        if f.normal.z < 0:
            f.normal_flip()
        for lp in f.loops:
            lp[m.cl] = (0.25, 0.55, 0.85, 1.0)
    return m.build()


def make_probe():
    """Asymmetric box (x 0..1, y 0..2, z 0..3 m) so Unreal can detect the import axis convention."""
    m = Model("SM_GZ_AxisProbe")
    m.box("red", (0.5, 1.0, 1.5), (1.0, 2.0, 3.0))
    return m.build()


# ----------------------------------------------------------------------------- layout

LAYOUT = dict(instances=[], labels=[], tagged=[], zones=[], player_start=None, roads=[])
CM = 100.0


def place(mesh, x, y, yaw=0.0, s=1.0, folder="Luminara/Props", col=True, z=None, label=None):
    zz = height(x, y) if z is None else z
    LAYOUT["instances"].append(dict(m=mesh, x=x * CM, y=y * CM, z=zz * CM, yaw=yaw, s=s, f=folder, col=col))
    if label:
        add_label(label, x, y, zz + 6.5 * s, folder)


def add_label(text, x, y, z, folder, size=90, color=(1.0, 0.95, 0.75)):
    LAYOUT["labels"].append(dict(text=text, x=x * CM, y=y * CM, z=z * CM, size=size, color=color, f=folder))


def face_yaw(px, py, tx, ty):
    """Yaw (deg, UE) for an asset whose front faces -Y in Blender to look from p toward t."""
    return math.degrees(math.atan2(ty - py, tx - px))


def angle_free(a, road_angles, gap=20):
    return all(abs((a - r + 180) % 360 - 180) > gap for r in road_angles)


NPC_NAMES = {
    "Emberfall": dict(mayor="Mayor Seol", merchant="Merchant Yuri", blacksmith="Smith Doyun", alchemist="Alchemist Mirae", banker="Banker Gyeom",
                      quest_giver="Elder Han", guild_master="Guildmaster Ara", guard="Guard Taeho", stable_master="Stablemaster Bom", pet_trainer="Trainer Nari"),
    "Frostmere": dict(mayor="Mayor Haneul", merchant="Merchant Sora", blacksmith="Smith Gang", alchemist="Alchemist Yena", banker="Banker Jiwoo",
                      quest_giver="Scout Rin", guild_master="Guildmaster Bora", guard="Guard Minsu", stable_master="Stablemaster Chul", pet_trainer="Trainer Dami"),
    "Greenhollow": dict(mayor="Mayor Dara", merchant="Merchant Hyejin", blacksmith="Smith Woo", alchemist="Alchemist Seori", banker="Banker Pyo",
                        quest_giver="Ranger Eun", guild_master="Guildmaster Sol", guard="Guard Junho", stable_master="Stablemaster Byeol", pet_trainer="Trainer Gaeul"),
}


def npc(role, name, town, x, y, yaw):
    mesh = "SM_GZ_npc_" + role
    z = height(x, y)
    LAYOUT["tagged"].append(dict(tags=["GZNPC", name, role, town], m=mesh, x=x * CM, y=y * CM, z=z * CM, yaw=yaw, s=1.0,
                                 f="Luminara/%s/NPCs" % town, label=name, label_z=2.6 * CM, label_color=(1.0, 0.9, 0.35)))


def build_town(t):
    cx, cy = t["c"]
    st = t["style"]
    folder = "Luminara/%s" % t["name"]
    names = NPC_NAMES[t["name"]]
    road_angles = []
    for r in ROADS:
        for end in (r["pts"][0], r["pts"][-1]):
            if math.hypot(end[0] - cx, end[1] - cy) < 1:
                other = r["pts"][2] if end == r["pts"][0] else r["pts"][-3]
                road_angles.append(math.degrees(math.atan2(other[1] - cy, other[0] - cx)))
    LAYOUT["zones"].append(dict(name=t["name"], x=cx * CM, y=cy * CM, z=1.0 * CM, r=t["r"] * CM, type="town"))
    add_label(t["name"], cx, cy, 9.5, folder, size=260, color=(1.0, 0.85, 0.4))

    # plaza
    place("SM_GZ_statue", cx, cy, 0, 1.0, folder + "/Plaza")
    for k in range(4):
        a = math.radians(45 + k * 90)
        place("SM_GZ_lantern", cx + math.cos(a) * 7, cy + math.sin(a) * 7, 0, 1.2, folder + "/Plaza")

    # ring 1 civic buildings
    civic = [("town_hall", "SM_GZ_town_hall_" + st, "Town Hall"), ("blacksmith", "SM_GZ_blacksmith_" + st, "Blacksmith"),
             ("inn", "SM_GZ_inn_" + st, "Inn"), ("alchemist", "SM_GZ_alchemy_" + st, "Alchemy Shop"),
             ("banker", "SM_GZ_warehouse_" + st, "Warehouse & Bank"), ("guild_master", "SM_GZ_guild_" + st, "Guild Hall"),
             ("stable_master", "SM_GZ_stable_" + st, "Stable"), ("temple", "SM_GZ_temple_" + st, "Temple")]
    slots = [a for a in range(0, 360, 30) if angle_free(a, road_angles, 22)]
    slots.sort(key=lambda a: abs((a - 0 + 180) % 360 - 180))
    hall = slots.pop(0)
    order = [hall] + sorted(slots, key=lambda a: (a - hall) % 360)
    for i, a in enumerate(order):
        rad = math.radians(a)
        r = 27 if i else 26
        x, y = cx + math.cos(rad) * r, cy + math.sin(rad) * r
        yaw = face_yaw(x, y, cx, cy)
        if i < len(civic):
            role, mesh, label = civic[i]
            place(mesh, x, y, yaw, 1.0, folder + "/Buildings", label=label)
            # NPC in front of the building
            nx, ny = cx + math.cos(rad) * (r - 9), cy + math.sin(rad) * (r - 9)
            if role in names:
                npc(role, names[role], t["name"], nx, ny, face_yaw(nx, ny, cx, cy))
            elif role == "temple":
                pass
            if role == "stable_master":
                px, py = cx + math.cos(rad + 0.12) * (r - 8), cy + math.sin(rad + 0.12) * (r - 8)
                npc("pet_trainer", names["pet_trainer"], t["name"], px, py, face_yaw(px, py, cx, cy))
        else:
            place("SM_GZ_house_%s_%d" % (st, i % 3), x, y, yaw, 1.0, folder + "/Buildings")
    # market stalls
    stall_i = 0
    for a in range(0, 360, 40):
        if not angle_free(a, road_angles, 16):
            continue
        rad = math.radians(a + 20)
        x, y = cx + math.cos(rad) * 13, cy + math.sin(rad) * 13
        place("SM_GZ_stall_%d" % (stall_i % 4), x, y, face_yaw(x, y, cx, cy), 1.0, folder + "/Market")
        if stall_i == 0:
            nx, ny = cx + math.cos(rad) * 10.5, cy + math.sin(rad) * 10.5
            npc("merchant", names["merchant"], t["name"], nx, ny, face_yaw(nx, ny, cx, cy))
        stall_i += 1
    add_label("Market", cx, cy - 13, 5.0, folder + "/Market", size=110)
    # quest giver by the statue
    npc("quest_giver", names["quest_giver"], t["name"], cx - 4.5, cy + 3.5, face_yaw(cx - 4.5, cy + 3.5, cx, cy - 20))
    place("SM_GZ_well", cx + 5, cy - 4.5, 0, 1.0, folder + "/Plaza")

    # ring 2 houses
    for k, a in enumerate(range(15, 360, 30)):
        if not angle_free(a, road_angles, 16):
            continue
        rad = math.radians(a)
        x, y = cx + math.cos(rad) * 38, cy + math.sin(rad) * 38
        place("SM_GZ_house_%s_%d" % (st, k % 3), x, y, face_yaw(x, y, cx, cy), 0.9, folder + "/Houses")
        if k % 3 == 0:
            place("SM_GZ_barrel", x + math.cos(rad + 1.6) * 5, y + math.sin(rad + 1.6) * 5, 0, 1.0, folder + "/Props")
        if k % 3 == 1:
            place("SM_GZ_crate", x + math.cos(rad - 1.6) * 5, y + math.sin(rad - 1.6) * 5, k * 20, 1.0, folder + "/Props")
    # walls + gates
    for a in range(0, 360, 10):
        if angle_free(a, road_angles, 9):
            rad = math.radians(a)
            x, y = cx + math.cos(rad) * 46, cy + math.sin(rad) * 46
            place("SM_GZ_wall_" + st, x, y, a + 90 - 90 + 0, 1.0, folder + "/Walls")
    for ra in road_angles:
        rad = math.radians(ra)
        x, y = cx + math.cos(rad) * 46, cy + math.sin(rad) * 46
        place("SM_GZ_gate_" + st, x, y, ra + 90 - 90, 1.0, folder + "/Walls")
        gx, gy = cx + math.cos(rad + 0.12) * 42, cy + math.sin(rad + 0.12) * 42
        npc("guard", names["guard"], t["name"], gx, gy, face_yaw(gx, gy, cx + math.cos(rad) * 80, cy + math.sin(rad) * 80))
        for s in (-1, 1):
            wx, wy = cx + math.cos(rad + s * 0.2) * 50, cy + math.sin(rad + s * 0.2) * 50
            place("SM_GZ_watchtower_" + st, wx, wy, ra, 1.0, folder + "/Walls")


def scatter_ok(x, y, keep=10.0):
    for t in TOWNS:
        if math.hypot(x - t["c"][0], y - t["c"][1]) < t["r"] + keep:
            return False
    for hm in HAMLETS:
        if math.hypot(x - hm["c"][0], y - hm["c"][1]) < HAMLET_R + max(keep, 2):
            return False
    if road_dist((x, y)) < 8:
        return False
    if river_dist((x, y)) < 15:
        return False
    if math.hypot(x - LAKE["c"][0], y - LAKE["c"][1]) < LAKE["r"] + 8:
        return False
    if math.hypot(x - VOLCANO[0], y - VOLCANO[1]) < 14:
        return False
    for z in ZONES:
        if z["name"] in ("Emberfall Farmlands",) and math.hypot(x - z["c"][0], y - z["c"][1]) < z["r"] + 6:
            return False
    return True


def build_nature():
    rnd = random.Random(99)
    step = 8.0
    y = -194.0
    while y <= 194:
        x = -194.0
        while x <= 194:
            px, py = x + rnd.uniform(-3.5, 3.5), y + rnd.uniform(-3.5, 3.5)
            if scatter_ok(px, py):
                b = biome_at(px, py)
                h = height(px, py)
                dens = dict(temperate=0.10, forest=0.62, swamp=0.45, snow=0.28, volcanic=0.12)[b]
                if h > 22:
                    dens *= 0.3
                r = rnd.random()
                folder = "Luminara/Nature/" + b.capitalize()
                if r < dens:
                    k = rnd.random()
                    if b == "temperate":
                        mesh = "SM_GZ_tree_oak_0" if k < 0.45 else ("SM_GZ_tree_cherry" if k < 0.75 else "SM_GZ_tree_pine")
                    elif b == "forest":
                        mesh = ("SM_GZ_tree_oak_%d" % (int(k * 10) % 2)) if k < 0.55 else ("SM_GZ_tree_pine" if k < 0.85 else "SM_GZ_bush")
                    elif b == "swamp":
                        mesh = "SM_GZ_tree_dead" if k < 0.4 else ("SM_GZ_tree_oak_dark" if k < 0.8 else "SM_GZ_bush_dark")
                    elif b == "snow":
                        mesh = "SM_GZ_tree_pine_snow" if k < 0.85 else "SM_GZ_rock_snow"
                    else:
                        mesh = "SM_GZ_tree_dead_burnt" if k < 0.45 else "SM_GZ_rock_lava"
                    place(mesh, px, py, rnd.uniform(0, 360), rnd.uniform(0.8, 1.3), folder, col=mesh.startswith("SM_GZ_rock"))
                elif r < dens + 0.035:
                    mesh = {"snow": "SM_GZ_rock_snow", "volcanic": "SM_GZ_rock_lava"}.get(b, "SM_GZ_rock")
                    place(mesh, px, py, rnd.uniform(0, 360), rnd.uniform(0.6, 1.4), folder, col=True)
                elif b in ("temperate", "forest") and r < dens + 0.08:
                    place("SM_GZ_bush", px, py, rnd.uniform(0, 360), rnd.uniform(0.6, 1.1), folder, col=False)
            x += step
        y += step


def spawn_points(z, n, rnd, min_sep=7.0):
    pts = []
    tries = 0
    while len(pts) < n and tries < n * 80:
        tries += 1
        a = rnd.uniform(0, 6.283)
        r = math.sqrt(rnd.random()) * z["r"]
        x, y = z["c"][0] + math.cos(a) * r, z["c"][1] + math.sin(a) * r
        if not scatter_ok(x, y, keep=4) and road_dist((x, y)) >= 8:
            continue
        if height(x, y) < -0.5 or abs(x) > 185 or abs(y) > 185:
            continue
        if any(math.hypot(x - p[0], y - p[1]) < min_sep for p in pts):
            continue
        pts.append((x, y))
    return pts


def build_hamlet(hm):
    cx, cy = hm["c"]
    st = hm["style"]
    folder = "Luminara/Villages/%s" % hm["name"]
    LAYOUT["zones"].append(dict(name=hm["name"], x=cx * CM, y=cy * CM, z=1.0 * CM, r=(HAMLET_R + 6) * CM, type="village"))
    add_label(hm["name"], cx, cy, 8.0, folder, size=170, color=(1.0, 0.9, 0.55))
    place("SM_GZ_well", cx, cy, 0, 1.0, folder)
    for k in range(5):
        a = math.radians(30 + k * 72)
        x, y = cx + math.cos(a) * 11, cy + math.sin(a) * 11
        if road_dist((x, y)) < 6:
            continue
        place("SM_GZ_house_%s_%d" % (st, (k + 2) % 3 if k % 2 else 2), x, y, face_yaw(x, y, cx, cy), 0.8, folder)
    for k in range(3):
        a = math.radians(66 + k * 120)
        place("SM_GZ_lantern", cx + math.cos(a) * 5, cy + math.sin(a) * 5, 0, 1.0, folder)
    place("SM_GZ_barrel", cx + 3, cy + 2.5, 0, 1.0, folder)
    place("SM_GZ_crate", cx + 3.8, cy + 1.2, 25, 1.0, folder)
    npc("quest_giver", hm["elder"], hm["name"], cx - 3, cy - 3, face_yaw(cx - 3, cy - 3, cx, cy))
    npc("merchant", "Peddler of " + hm["name"], hm["name"], cx + 4, cy - 3, face_yaw(cx + 4, cy - 3, cx, cy))


def build_zones_and_monsters():
    rnd = random.Random(7)
    for z in ZONES:
        LAYOUT["zones"].append(dict(name=z["name"], x=z["c"][0] * CM, y=z["c"][1] * CM, z=1.0 * CM, r=z["r"] * CM, type="wild"))
        if z["name"] not in ("Emberfall Farmlands", "Mirror Lake"):
            add_label(z["name"], z["c"][0], z["c"][1], height(*z["c"]) + 12, "Luminara/Zones", size=200, color=(0.85, 0.95, 0.85))
        for (mname, lo, hi, count) in z["mons"]:
            for (x, y) in spawn_points(z, count, rnd):
                lvl = rnd.randint(lo, hi)
                yaw = rnd.uniform(0, 360)
                LAYOUT["tagged"].append(dict(tags=["GZMonster", mname, str(lvl)], m=MONSTER_NAMES[mname], x=x * CM, y=y * CM,
                                             z=height(x, y) * CM, yaw=yaw, s=1.0, f="Luminara/Monsters/" + z["name"],
                                             label="%s Lv.%d" % (mname, lvl), label_z=(2.6 if mname not in ("Orc Brute",) else 3.3) * CM,
                                             label_color=(1.0, 0.55, 0.45)))
    # boss
    bx, by = 130.0, 136.0
    LAYOUT["tagged"].append(dict(tags=["GZMonster", "Ember Drake", "20"], m="SM_GZ_mon_ember_drake", x=bx * CM, y=by * CM, z=height(bx, by) * CM,
                                 yaw=-135, s=1.6, f="Luminara/Monsters/Dragon Lair", label="Ember Drake Lv.20 (Boss)", label_z=10.5 * CM,
                                 label_color=(1.0, 0.35, 0.2)))
    for k in range(5):
        a = k * 1.25
        place("SM_GZ_lava_pool", VOLCANO[0] - 22 + math.cos(a) * 9, VOLCANO[1] - 22 + math.sin(a) * 9, rnd.uniform(0, 360), 1.0, "Luminara/Dragon Lair", col=False)
    for k in range(6):
        place("SM_GZ_crystals", 150 + rnd.uniform(-25, 25), -150 + rnd.uniform(-25, 25), rnd.uniform(0, 360), rnd.uniform(0.8, 1.4), "Luminara/Frost Peaks")
    for k in range(4):
        x, y = -112 + rnd.uniform(-30, 30), -112 + rnd.uniform(-30, 30)
        place("SM_GZ_ruin", x, y, rnd.uniform(0, 360), 1.0, "Luminara/Forgotten Forest", label="Ancient Ruins" if k == 0 else None)


def resource(item, skill, label, mesh, x, y, yaw, folder):
    z = height(x, y)
    LAYOUT["tagged"].append(dict(tags=["GZResource", label, item, skill], m=mesh, x=x * CM, y=y * CM, z=z * CM, yaw=yaw, s=1.0,
                                 f=folder, label=label, label_z=2.4 * CM, label_color=(0.55, 1.0, 0.6)))


def build_resources():
    rnd = random.Random(17)
    # Iron Hills: mine + ore
    place("SM_GZ_mine", 176, -28, face_yaw(176, -28, 168, -36), 1.0, "Luminara/Iron Hills", label="Iron Mine")
    for (x, y) in spawn_points(dict(c=(166, -40), r=16), 7, rnd, 6):
        resource("iron_ore", "mining", "Iron Ore Vein", "SM_GZ_res_ore", x, y, rnd.uniform(0, 360), "Luminara/Resources/Iron Hills")
    npc("miner", "Miner Kwon", "Frostmere", 164, -44, face_yaw(164, -44, 130, -70))
    # Greenwood / Greenhollow oak groves
    for (x, y) in spawn_points(dict(c=(-150, 70), r=20), 3, rnd, 8) + spawn_points(dict(c=(-150, 150), r=25), 3, rnd, 8):
        resource("oak_log", "woodworking", "Oak Grove", "SM_GZ_res_oak", x, y, rnd.uniform(0, 360), "Luminara/Resources/Greenwood")
    # Forgotten Forest herbs
    for (x, y) in spawn_points(dict(c=(-95, -95), r=40), 8, rnd, 8):
        resource("red_herb", "farming", "Red Herb Patch", "SM_GZ_res_herb", x, y, rnd.uniform(0, 360), "Luminara/Resources/Forgotten Forest")
    # Emberfall farmlands
    fx, fy = -62, -18
    for i, (dx, dy) in enumerate(((-5, -5), (5, -5), (-5, 5), (5, 5))):
        resource("grain", "farming", "Wheat Field", "SM_GZ_res_field", fx + dx * 1.8, fy + dy * 1.8, 0, "Luminara/Resources/Farmlands")
    place("SM_GZ_house_ember_2", fx - 18, fy, face_yaw(fx - 18, fy, fx, fy), 0.9, "Luminara/Resources/Farmlands", label="Farmhouse")
    npc("farmer", "Farmer Areum", "Emberfall", fx + 12, fy + 2, face_yaw(fx + 12, fy + 2, fx, fy))
    # Mirror Lake + river fishing
    lx, ly = LAKE["c"]
    for k in range(3):
        a = math.radians(200 + k * 50)
        x, y = lx + math.cos(a) * (LAKE["r"] + 4), ly + math.sin(a) * (LAKE["r"] + 4)
        resource("fish", "fishing", "Fishing Dock", "SM_GZ_res_fish", x, y, face_yaw(x, y, lx, ly) - 90 + 90, "Luminara/Resources/Mirror Lake")
    a = math.radians(225)
    npc("fisher", "Fisher Ian", "Greenhollow", lx + math.cos(a) * (LAKE["r"] + 9), ly + math.sin(a) * (LAKE["r"] + 9),
        face_yaw(lx + math.cos(a) * (LAKE["r"] + 9), ly + math.sin(a) * (LAKE["r"] + 9), lx, ly))
    for (x, y) in ((-20, 70), (95, 66)):
        # river docks: snap to just outside the river bank
        resource("fish", "fishing", "River Fishing Spot", "SM_GZ_res_fish", x, y, -90, "Luminara/Resources/River")


def build_roads():
    rnd = random.Random(5)
    for r in ROADS:
        LAYOUT["roads"].append(dict(name=r["name"], pts=[(p[0] * CM, p[1] * CM) for p in r["pts"]]))
        pts = r["pts"]
        # signpost at the start of the road outside town
        sx, sy = pts[2]
        place("SM_GZ_signpost", sx + 5, sy + 5, face_yaw(sx, sy, pts[3][0], pts[3][1]), 1.0, "Luminara/Roads",
              label="-> " + r["to"])
        # lanterns + fences along the road
        for i in range(1, len(pts) - 1):
            a, b = pts[i], pts[i + 1]
            dx, dy = b[0] - a[0], b[1] - a[1]
            L = math.hypot(dx, dy) or 1
            nx, ny = -dy / L, dx / L
            for s in (-1, 1):
                x, y = a[0] + nx * 6.5 * s, a[1] + ny * 6.5 * s
                if scatter_ok(x, y, keep=-2) or True:
                    near_town = any(math.hypot(x - t["c"][0], y - t["c"][1]) < t["r"] + 4 for t in TOWNS)
                    if not near_town and river_dist((x, y)) > 14 and i % 2 == 0:
                        place("SM_GZ_lantern", x, y, 0, 1.0, "Luminara/Roads")
        # bridges where the road crosses the river
        best = None
        for i in range(len(pts) - 1):
            for k in range(11):
                t = k / 10
                p = (pts[i][0] + (pts[i + 1][0] - pts[i][0]) * t, pts[i][1] + (pts[i + 1][1] - pts[i][1]) * t)
                d = river_dist(p)
                if best is None or d < best[0]:
                    best = (d, p, i)
        if best and best[0] < 3.0:
            d, p, i = best
            yaw = math.degrees(math.atan2(pts[i + 1][1] - pts[i][1], pts[i + 1][0] - pts[i][0]))
            place("SM_GZ_bridge", p[0], p[1], yaw, 1.0, "Luminara/Roads", z=0.05, label="%s Bridge" % r["to"])


def build_world_bounds():
    for (x, y, sx, sy) in ((0, 188, 380, 2), (0, -188, 380, 2), (188, 0, 2, 380), (-188, 0, 2, 380)):
        LAYOUT.setdefault("blockers", []).append(dict(x=x * CM, y=y * CM, z=10 * CM, sx=sx, sy=sy, sz=40))


# ----------------------------------------------------------------------------- icon rendering


def setup_studio():
    sc = bpy.data.scenes.new("Studio")
    sc.render.engine = 'CYCLES'
    try:
        sc.cycles.device = 'CPU'
        sc.cycles.samples = 24
        sc.cycles.use_denoising = True
        sc.cycles.max_bounces = 3
    except Exception:
        pass
    sc.render.film_transparent = True
    sc.render.resolution_x = 128
    sc.render.resolution_y = 128
    sc.render.image_settings.file_format = 'PNG'
    sc.render.image_settings.color_mode = 'RGBA'
    try:
        sc.view_settings.view_transform = 'Standard'
    except Exception:
        pass
    world = bpy.data.worlds.new("StudioWorld")
    world.use_nodes = True
    bg = world.node_tree.nodes.get("Background")
    bg.inputs[0].default_value = (0.75, 0.78, 0.85, 1)
    bg.inputs[1].default_value = 0.9
    sc.world = world
    sun = bpy.data.lights.new("Sun", 'SUN')
    sun.energy = 3.5
    sun.angle = 0.3
    so = bpy.data.objects.new("Sun", sun)
    so.rotation_euler = (math.radians(50), math.radians(10), math.radians(-30))
    sc.collection.objects.link(so)
    cam = bpy.data.cameras.new("Cam")
    cam.type = 'ORTHO'
    co = bpy.data.objects.new("Cam", cam)
    sc.collection.objects.link(co)
    sc.camera = co
    return sc, co


def render_icon(sc, cam, ob, out_path, portrait=False, size=128):
    sc.collection.objects.link(ob)
    sc.render.resolution_x = size
    sc.render.resolution_y = size
    bpy.context.view_layer.update() if False else None
    corners = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    mn = Vector((min(c.x for c in corners), min(c.y for c in corners), min(c.z for c in corners)))
    mx = Vector((max(c.x for c in corners), max(c.y for c in corners), max(c.z for c in corners)))
    if portrait:
        # frame the upper part (head and shoulders), facing the camera (-Y front)
        h = mx.z - mn.z
        mn = Vector((mn.x, mn.y, mn.z + h * 0.5))
        rot = (math.radians(80), 0, math.radians(-10))
    else:
        rot = (math.radians(62), 0, math.radians(-38))
    center = (mn + mx) / 2
    cam.rotation_euler = rot
    rm = cam.rotation_euler.to_matrix()
    fwd = rm @ Vector((0, 0, -1))
    right = rm @ Vector((1, 0, 0))
    up = rm @ Vector((0, 1, 0))
    ext = 0.0
    for c in corners:
        if portrait and c.z < mn.z:
            continue
        d = c - center
        ext = max(ext, abs(d.dot(right)), abs(d.dot(up)))
    if portrait:
        ext = max(ext, (mx.x - mn.x) * 0.45)
    cam.data.ortho_scale = max(0.2, ext * 2.0 * (1.08 if portrait else 1.12))
    cam.location = center - fwd * 50.0
    cam.data.clip_end = 200
    sc.render.filepath = out_path
    bpy.context.window_manager  # noqa
    win_scene = bpy.context.scene if hasattr(bpy.context, "scene") else None
    bpy.ops.render.render(write_still=True, scene=sc.name)
    sc.collection.objects.unlink(ob)



# ----------------------------------------------------------------------------- 8-bit pixel art

# Original 32-colour retro palette (sRGB 0-255).
PALETTE_8BIT = [
    # NES-style colour set (sRGB 0-255): greys, blues, purples, reds, browns/oranges, greens, teals, skin
    (0, 0, 0), (84, 84, 84), (124, 124, 124), (188, 188, 188), (248, 248, 248),
    (0, 0, 188), (0, 88, 248), (60, 188, 252), (164, 228, 252),
    (104, 68, 252), (152, 120, 248), (216, 0, 204), (248, 120, 248),
    (136, 20, 0), (168, 16, 0), (228, 0, 88), (248, 56, 0), (248, 120, 88),
    (124, 8, 0), (172, 124, 0), (228, 92, 16), (252, 160, 68), (248, 184, 0), (252, 224, 168), (248, 216, 120),
    (0, 88, 0), (0, 120, 0), (0, 168, 0), (88, 216, 84), (184, 248, 24),
    (0, 136, 136), (0, 232, 216),
    # earth tones for roads, swamps and ash fields
    (80, 48, 0), (136, 112, 0), (176, 140, 88), (216, 184, 128), (64, 80, 40), (104, 112, 56), (56, 40, 40), (96, 64, 56),
]


def pixelate_png(path, grid=32, out_size=128, outline=True):
    """Turn a smooth render into 8-bit pixel art: downsample, palette-quantise, 1px outline, nearest upscale."""
    import numpy as np
    img = bpy.data.images.load(path, check_existing=False)
    w, h = img.size
    px = np.array(img.pixels[:], dtype=np.float32).reshape(h, w, 4)
    bpy.data.images.remove(img)
    f = w // grid
    px = px[:grid * f, :grid * f].reshape(grid, f, grid, f, 4)
    a = px[..., 3]
    asum = a.sum(axis=(1, 3))
    rgb = (px[..., :3] * a[..., None]).sum(axis=(1, 3)) / np.maximum(asum[..., None], 1e-6)
    alpha = asum / (f * f)
    # 8-bit PNGs come back from images.pixels already in sRGB
    srgb = np.clip(rgb, 0, 1)
    # a touch more saturation/contrast, retro style
    lum = srgb.mean(axis=2, keepdims=True)
    srgb = np.clip(lum + (srgb - lum) * 1.35, 0, 1)
    pal = np.array(PALETTE_8BIT, dtype=np.float32) / 255.0
    d = ((srgb[:, :, None, :] - pal[None, None, :, :]) ** 2).sum(axis=3)
    q = pal[d.argmin(axis=2)]
    solid = alpha > 0.45
    # 1px dark outline around the sprite
    pad = np.pad(solid, 1)
    neigh = pad[:-2, 1:-1] | pad[2:, 1:-1] | pad[1:-1, :-2] | pad[1:-1, 2:]
    outline = (neigh & ~solid) if outline else np.zeros_like(solid)
    outc = pal[0]
    res = np.zeros((grid, grid, 4), dtype=np.float32)
    res[solid, :3] = q[solid]
    res[solid, 3] = 1.0
    res[outline, :3] = outc
    res[outline, 3] = 1.0
    # nearest-neighbour upscale (byte image buffer stores sRGB as-is)
    k = out_size // grid
    big = res.repeat(k, axis=0).repeat(k, axis=1)
    out = bpy.data.images.new("pix", out_size, out_size, alpha=True)
    out.pixels[:] = big.ravel()
    out.filepath_raw = path
    out.file_format = 'PNG'
    out.save()
    bpy.data.images.remove(out)

def render_world_map(by_name, path, size=1024, grid=256):
    """Top-down render of the whole world (image up = UE +X, right = UE +Y), then NES pixel-art pass."""
    sc = bpy.data.scenes.new("WorldMap")
    sc.render.engine = 'CYCLES'
    try:
        sc.cycles.device = 'CPU'
        sc.cycles.samples = 12
        sc.cycles.use_denoising = True
    except Exception:
        pass
    sc.render.resolution_x = size
    sc.render.resolution_y = size
    sc.render.film_transparent = False
    try:
        sc.view_settings.view_transform = 'Standard'
    except Exception:
        pass
    w = bpy.data.worlds.new("MapWorld")
    w.use_nodes = True
    w.node_tree.nodes["Background"].inputs[0].default_value = (0.2, 0.35, 0.6, 1)
    sc.world = w
    sun = bpy.data.objects.new("MapSun", bpy.data.lights.new("MapSun", 'SUN'))
    sun.data.energy = 3.0
    sun.rotation_euler = (math.radians(35), 0, math.radians(-40))
    sc.collection.objects.link(sun)
    cam = bpy.data.objects.new("MapCam", bpy.data.cameras.new("MapCam"))
    cam.data.type = 'ORTHO'
    cam.data.ortho_scale = 400.0
    cam.data.clip_end = 1000
    cam.location = (0, 0, 300)
    cam.rotation_euler = (0, 0, -math.pi / 2)
    sc.collection.objects.link(cam)
    sc.camera = cam

    def inst(m, x, y, z, yaw, s_):
        if m not in by_name:
            return
        o = bpy.data.objects.new(m + "_map", by_name[m].data)
        o.location = (x / CM, -y / CM, z / CM)
        o.rotation_euler = (0, 0, -math.radians(yaw - 90))
        o.scale = (s_ * 1.6, s_ * 1.6, s_)   # slightly enlarge props so they read on the map
        sc.collection.objects.link(o)

    for i in LAYOUT["instances"]:
        inst(i["m"], i["x"], i["y"], i["z"], i["yaw"], i["s"])
    for t in LAYOUT["tagged"]:
        if t["tags"][0] != "GZMonster":
            inst(t["m"], t["x"], t["y"], t["z"], t["yaw"], t["s"])
    for m in ("SM_GZ_Terrain", "SM_GZ_Water"):
        o = bpy.data.objects.new(m + "_map", by_name[m].data)
        sc.collection.objects.link(o)
    sc.render.filepath = path
    bpy.ops.render.render(write_still=True, scene=sc.name)
    pixelate_png(path, grid=grid, out_size=size, outline=False)


# ----------------------------------------------------------------------------- main


def export_fbx(ob, path):
    for o in bpy.context.view_layer.objects:
        o.select_set(False)
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.export_scene.fbx(filepath=path, use_selection=True, object_types={'MESH'}, mesh_smooth_type='FACE',
                             colors_type='SRGB', apply_unit_scale=True, use_mesh_modifiers=False, bake_anim=False,
                             add_leaf_bones=False, path_mode='STRIP')


def main():
    # clean scene
    for o in list(bpy.data.objects):
        bpy.data.objects.remove(o, do_unlink=True)
    scene = bpy.context.scene
    coll = bpy.data.collections.new("Luminara")
    scene.collection.children.link(coll)

    models = []
    # buildings per style
    for st in STYLES:
        models.append(make_town_hall(st))
        models.append(make_house("SM_GZ_blacksmith_" + st, st, 8, 6, extra=ex_blacksmith))
        models.append(make_house("SM_GZ_inn_" + st, st, 9, 6.5, stories=2, extra=ex_inn))
        models.append(make_house("SM_GZ_alchemy_" + st, st, 7, 5.5, extra=ex_alchemy))
        models.append(make_house("SM_GZ_warehouse_" + st, st, 11, 6, extra=ex_warehouse))
        models.append(make_house("SM_GZ_guild_" + st, st, 9, 7, stories=2, extra=ex_guild))
        models.append(make_stable(st))
        models.append(make_temple(st))
        models.append(make_watchtower(st))
        models.append(make_gate(st))
        models.append(make_wall(st))
        models.append(make_house("SM_GZ_house_%s_0" % st, st, 6.5, 5.0))
        models.append(make_house("SM_GZ_house_%s_1" % st, st, 7.5, 5.5, stories=2))
        models.append(make_house("SM_GZ_house_%s_2" % st, st, 5.5, 4.5))
    for i, (canopy, goods) in enumerate((("red", ("orange", "yellow", "red")), ("blue", ("steel", "iron", "wood_light")),
                                         ("cloth_green", ("leaf", "red", "teal")), ("purple", ("gold", "ice", "pink")))):
        models.append(make_stall("SM_GZ_stall_%d" % i, canopy, goods))
    models += [make_well(), make_lantern(), make_signpost(), make_fence(), make_bridge(), make_barrel(), make_crate(), make_statue()]
    # nature
    models += [make_tree_oak("SM_GZ_tree_oak_0", seed=1), make_tree_oak("SM_GZ_tree_oak_1", seed=8),
               make_tree_oak("SM_GZ_tree_oak_dark", leaf=("leaf_dark", "pine", "dead"), seed=9, trunk="wood_dark"),
               make_tree_pine("SM_GZ_tree_pine"), make_tree_pine("SM_GZ_tree_pine_snow", snowy=True), make_tree_cherry(),
               make_tree_dead("SM_GZ_tree_dead"), make_tree_dead("SM_GZ_tree_dead_burnt", burnt=True), make_bush(),
               make_bush("SM_GZ_bush_dark", "leaf_dark", "dead"), make_rock("SM_GZ_rock"), make_rock("SM_GZ_rock_snow", snow=True, seed=6),
               make_rock("SM_GZ_rock_lava", color="black", lava=True, seed=7), make_ore_vein(), make_oak_grove(), make_herb_patch(),
               make_crop_field(), make_fish_spot(), make_mine_entrance(), make_lava_pool(), make_ruin(), make_crystal_cluster()]
    # characters / monsters
    for role in NPC_LOOKS:
        models.append(make_npc(role))
    for k, fn in MONSTERS.items():
        models.append(fn())
    items = make_items()
    models += items
    models += [make_terrain(), make_water(), make_probe()]

    for i, ob in enumerate(models):
        coll.objects.link(ob)
    bpy.context.view_layer.update()

    # exports
    names = []
    for ob in models:
        export_fbx(ob, os.path.join(MESH_DIR, ob.name + ".fbx"))
        names.append(ob.name)
    print("Exported %d meshes" % len(names))

    # layout
    for t in TOWNS:
        build_town(t)
    for hm in HAMLETS:
        build_hamlet(hm)
    build_roads()
    build_resources()
    build_zones_and_monsters()
    build_nature()
    build_world_bounds()
    ex, ey = TOWNS[1]["c"]
    LAYOUT["player_start"] = dict(x=(ex + 8.5) * CM, y=(ey - 1.5) * CM, z=(height(ex + 8.5, ey - 1.5) + 1.2) * CM)
    LAYOUT["water_z"] = WATER_Z * CM
    LAYOUT["meshes"] = names
    with open(os.path.join(OUT, "layout.json"), "w") as f:
        json.dump(LAYOUT, f, indent=1)
    print("Layout: %d instances, %d tagged, %d labels, %d zones" % (len(LAYOUT["instances"]), len(LAYOUT["tagged"]),
                                                                  len(LAYOUT["labels"]), len(LAYOUT["zones"])))

    # lay models out in a grid in the .blend for editing
    x = 0.0
    for ob in models:
        if ob.name in ("SM_GZ_Terrain", "SM_GZ_Water"):
            continue
        ob.location = (x, -260.0, 0)
        x += 14.0
    bpy.ops.wm.save_as_mainfile(filepath=os.path.join(OUT, "luminara_assets.blend"))
    for ob in models:
        ob.location = (0, 0, 0)

    if not RENDER_ICONS:
        return
    by_name = {o.name: o for o in models}
    render_world_map(by_name, os.path.join(ICON_DIR, "T_worldmap.png"))
    print("world map rendered")
    sc, cam = setup_studio()
    jobs = []
    for ob in items:
        jobs.append((ob.name.replace("SM_GZ_item_", "").replace("SM_GZ_tool_", "tool_"), ob, False))
    for sk, mesh in SKILL_ICONS.items():
        jobs.append(("skill_" + sk, by_name[mesh], False))
    for role in NPC_LOOKS:
        mesh = "SM_GZ_player" if role == "player" else "SM_GZ_npc_" + role
        jobs.append(("npc_" + role, by_name[mesh], True))
    jobs.append(("portrait_player", by_name["SM_GZ_player"], True))
    for disp, mesh in MONSTER_NAMES.items():
        jobs.append(("mon_" + disp.lower().replace(" ", "_"), by_name[mesh], False))
    jobs.append(("bld_town_hall", by_name["SM_GZ_town_hall_ember"], False))
    for st in STYLES:
        for b in ("town_hall", "blacksmith", "inn", "alchemy", "warehouse", "guild", "stable", "temple"):
            jobs.append(("bld_%s_%s" % (b, st), by_name["SM_GZ_%s_%s" % (b, st)], False))
    # warm-up render (the first Cycles render of a session can come out empty)
    warm = by_name["SM_GZ_item_bag"]
    coll.objects.unlink(warm)
    render_icon(sc, cam, warm, os.path.join(ICON_DIR, "_warmup.png"))
    coll.objects.link(warm)
    try:
        os.remove(os.path.join(ICON_DIR, "_warmup.png"))
    except OSError:
        pass
    for i, (icon, ob, portrait) in enumerate(jobs):
        path = os.path.join(ICON_DIR, "T_%s.png" % icon)
        for o in list(coll.objects):
            pass
        coll.objects.unlink(ob) if ob.name in coll.objects else None
        grid = 64 if icon.startswith("bld_") else 40
        render_icon(sc, cam, ob, path, portrait=portrait, size=grid * 8 if PIXEL_ART else 256)
        coll.objects.link(ob)
        if PIXEL_ART:
            pixelate_png(path, grid=grid, out_size=grid * 4)
        print("icon %d/%d %s" % (i + 1, len(jobs), icon))


if __name__ == "__main__":
    main()
