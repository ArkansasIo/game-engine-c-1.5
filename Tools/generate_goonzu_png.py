#!/usr/bin/env python3
"""Generate deterministic PNG game assets from goonzu_prompts.json.

Every entry in the prompt catalog gets a real 512x512 RGBA PNG sprite with a
transparent background, drawn procedurally per category (characters, buildings,
weapons, armor, items, creatures, UI, tiles, effects, furniture). The output is
game-ready placeholder art: consistent palette, clean silhouettes, and it can
later be swapped for AI-generated production art without changing asset paths.

Usage (from repo root):
    python Tools/generate_goonzu_png.py
    python Tools/generate_goonzu_png.py --size 256 --force
"""
from __future__ import annotations

import argparse
import json
import math
import random
import re
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter

ROOT = Path(__file__).resolve().parents[1]
DEFAULT_PROMPTS = ROOT / "goonzu_prompts.json"
ASSETS_ROOT = ROOT / "Assets" / "GoonzuGame"

# Category -> (dark, mid, light, accent)
PALETTES = {
    "character": ((37, 53, 74), (121, 184, 255), (219, 234, 254), (255, 205, 148)),
    "npc":       ((59, 47, 37), (242, 192, 120), (255, 240, 208), (120, 200, 160)),
    "enemy":     ((74, 32, 40), (255, 107, 107), (255, 225, 225), (140, 40, 60)),
    "monster":   ((56, 41, 79), (192, 132, 252), (243, 232, 255), (90, 60, 140)),
    "environment": ((25, 54, 47), (99, 214, 162), (220, 255, 240), (60, 120, 100)),
    "terrain":   ((60, 50, 31), (217, 164, 65), (255, 244, 206), (150, 110, 50)),
    "weapon":    ((49, 52, 59), (199, 206, 219), (255, 255, 255), (150, 110, 60)),
    "armor":     ((41, 52, 58), (145, 164, 173), (230, 242, 245), (100, 120, 130)),
    "item":      ((43, 46, 66), (170, 180, 255), (236, 238, 255), (255, 190, 100)),
    "ui":        ((32, 36, 44), (110, 231, 249), (230, 251, 255), (70, 80, 100)),
    "icon":      ((32, 36, 44), (251, 191, 36), (255, 247, 214), (200, 150, 30)),
    "default":   ((36, 42, 51), (154, 167, 184), (237, 242, 247), (90, 100, 120)),
}

SKIN_TONES = [(255, 220, 184), (232, 190, 152), (205, 160, 120), (150, 105, 75), (110, 75, 55)]
CLOTH_COLORS = [(180, 60, 60), (60, 90, 170), (70, 140, 80), (150, 100, 180), (220, 170, 60),
                (90, 90, 100), (200, 200, 210), (120, 70, 50)]
HAIR_COLORS = [(40, 30, 25), (90, 60, 35), (170, 120, 60), (220, 200, 160), (200, 60, 40), (120, 120, 130)]


def classify(name: str, prompt: str) -> str:
    s = f"{Path(name).stem} {prompt}".lower()
    groups = [
        ("terrain", ["grass", "dirt", "stone_floor", "floor_tile", "road", "path", "tile_", "terrain", "sand", "snow", "water", "mountain"]),
        ("environment", ["tree", "pine", "oak", "bush", "rock", "castle", "tower", "house", "tavern", "temple", "shop", "forge", "mine", "portal", "building", "fence", "wall", "well", "sign", "statue", "bridge", "gate", "dungeon", "mountain_", "hill", "waterfall", "campfire", "torch"]),
        ("weapon", ["sword", "axe", "bow", "staff", "spear", "dagger", "hammer", "mace", "shield_", "weapon", "knife", "crossbow", "wand"]),
        ("armor", ["helmet", "armor", "armour", "chestplate", "gauntlet", "boots", "cape", "cloak", "hood", "glove", "shield"]),
        ("item", ["potion", "scroll", "ring", "amulet", "book", "gem", "crystal", "key", "coin", "ore", "ingot", "wood", "leather", "cloth", "herb", "seed", "meat", "bread", "food", "meal", "fish", "lockpick", "rope", "lantern", "flask", "vial", "bag", "pouch", "tool", "pickaxe", "shovel", "axe_", "hammer_", "item", "material", "resource"]),
        ("creature", ["dragon", "wolf", "bear", "spider", "snake", "rat", "goblin", "orc", "troll", "slime", "bat", "boar", "deer", "horse", "mount", "pet", "beast", "monster", "skeleton", "zombie", "ghost", "wraith", "demon", "imp", "elemental", "golem", "boss", "enemy", "bandit", "marauder", "cultist", "thug", "brigand", "creature", "mob", "crab", "scorpion", "lizard", "wisp", "shade", "phantom", "harpy", "griffin", "wyvern", "basilisk", "chimera", "hydra", "minotaur", "centaur", "giant", "ogre"]),
        ("character", ["knight", "warrior", "mage", "wizard", "ranger", "rogue", "cleric", "paladin", "bard", "druid", "monk", "necromancer", "warlock", "sorcerer", "hunter", "assassin", "archer", "character", "hero", "player", "human", "elf", "dwarf", "halfling", "tiefling", "gnome", "orc_male", "orc_female", "villager"]),
        ("npc", ["npc", "merchant", "blacksmith", "innkeeper", "trainer", "guard", "captain", "king", "queen", "priest", "farmer", "miner", "fisher", "hunter_npc", "shopkeeper", "noble", "stable", "sailor", "captain_", "npc_"]),
        ("furniture", ["chair", "table", "bed", "chest", "shelf", "bookshelf", "wardrobe", "throne", "anvil", "forge", "cauldron", "stool", "bench", "barrel", "crate", "rug", "carpet", "lamp", "candle", "mirror", "furniture", "desk", "cabinet", "pot_", "vase"]),
        ("ui", ["ui_", "panel", "window", "button", "bar", "hud", "menu", "frame", "border", "slot", "cursor", "icon_"]),
        ("icon", ["icon", "symbol", "badge", "emblem", "sigil", "crest", "banner"]),
        ("effect", ["effect", "particle", "aura", "spell", "fire", "ice", "lightning", "smoke", "glow", "magic_", "vfx", "flash", "spark"]),
    ]
    for cat, words in groups:
        if any(w in s for w in words):
            return cat
    return "default"


def _rr(draw: ImageDraw.ImageDraw, box, radius, fill):
    x0, y0, x1, y1 = box
    r = min(radius, (x1 - x0) // 2, (y1 - y0) // 2)
    draw.rounded_rectangle(box, radius=r, fill=fill)


def _draw_character(img, rng, pal, outline):
    """Humanoid figure: head, torso, arms, legs, weapon hint."""
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 300
    skin = rng.choice(SKIN_TONES)
    cloth = rng.choice(CLOTH_COLORS)
    hair = rng.choice(HAIR_COLORS)
    o = outline

    # legs
    draw.rounded_rectangle([cx - 46, cy + 40, cx - 12, cy + 190], 18, fill=(60, 55, 70), outline=o, width=3)
    draw.rounded_rectangle([cx + 12, cy + 40, cx + 46, cy + 190], 18, fill=(60, 55, 70), outline=o, width=3)
    # boots
    draw.rounded_rectangle([cx - 50, cy + 165, cx - 8, cy + 196], 12, fill=(70, 45, 35), outline=o, width=3)
    draw.rounded_rectangle([cx + 8, cy + 165, cx + 50, cy + 196], 12, fill=(70, 45, 35), outline=o, width=3)
    # arms
    draw.rounded_rectangle([cx - 82, cy - 30, cx - 52, cy + 60], 16, fill=cloth, outline=o, width=3)
    draw.rounded_rectangle([cx + 52, cy - 30, cx + 82, cy + 60], 16, fill=cloth, outline=o, width=3)
    # hands
    draw.ellipse([cx - 88, cy + 55, cx - 50, cy + 92], fill=skin, outline=o, width=3)
    draw.ellipse([cx + 50, cy + 55, cx + 88, cy + 92], fill=skin, outline=o, width=3)
    # torso
    draw.rounded_rectangle([cx - 58, cy - 40, cx + 58, cy + 60], 24, fill=cloth, outline=o, width=4)
    # chest detail
    draw.line([cx - 30, cy - 20, cx + 30, cy - 20], fill=(*pal[1], 255), width=4)
    draw.line([cx - 34, cy + 10, cx + 34, cy + 10], fill=(*pal[1], 255), width=3)
    # belt
    draw.rectangle([cx - 56, cy + 28, cx + 56, cy + 46], fill=(80, 60, 40), outline=o, width=3)
    draw.rectangle([cx - 12, cy + 26, cx + 12, cy + 48], fill=(220, 190, 80), outline=o, width=3)
    # head
    draw.ellipse([cx - 44, cy - 128, cx + 44, cy - 44], fill=skin, outline=o, width=4)
    # hair
    draw.pieslice([cx - 48, cy - 134, cx + 48, cy - 64], 180, 360, fill=hair)
    # eyes
    draw.ellipse([cx - 26, cy - 96, cx - 8, cy - 82], fill=(255, 255, 255), outline=o, width=2)
    draw.ellipse([cx + 8, cy - 96, cx + 26, cy - 82], fill=(255, 255, 255), outline=o, width=2)
    draw.ellipse([cx - 20, cy - 93, cx - 12, cy - 85], fill=(40, 40, 60))
    draw.ellipse([cx + 14, cy - 93, cx + 22, cy - 85], fill=(40, 40, 60))


def _draw_npc(img, rng, pal, outline):
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 300
    skin = rng.choice(SKIN_TONES)
    cloth = (rng.choice(CLOTH_COLORS))
    draw.rounded_rectangle([cx - 44, cy + 40, cx - 14, cy + 180], 16, fill=(90, 75, 60), outline=outline, width=3)
    draw.rounded_rectangle([cx + 14, cy + 40, cx + 44, cy + 180], 16, fill=(90, 75, 60), outline=outline, width=3)
    draw.polygon([(cx - 70, cy + 30), (cx + 70, cy + 30), (cx + 54, cy - 46), (cx - 54, cy - 46)], fill=cloth, outline=outline)
    draw.polygon([(cx - 54, cy - 46), (cx + 54, cy - 46), (cx, cy - 78)], fill=cloth, outline=outline)
    draw.ellipse([cx - 40, cy - 120, cx + 40, cy - 44], fill=skin, outline=outline, width=4)
    draw.pieslice([cx - 44, cy - 126, cx + 44, cy - 66], 180, 360, fill=rng.choice(HAIR_COLORS))
    draw.ellipse([cx - 22, cy - 92, cx - 8, cy - 80], fill=(40, 40, 60))
    draw.ellipse([cx + 8, cy - 92, cx + 22, cy - 80], fill=(40, 40, 60))


def _draw_weapon(img, rng, pal, outline):
    kind = rng.choice(["sword", "axe", "bow", "staff", "dagger"])
    draw = ImageDraw.Draw(img)
    steel = (200, 208, 220)
    if kind == "sword":
        draw.polygon([(256, 80), (276, 100), (256, 420), (236, 100)], fill=steel, outline=outline)
        draw.line([256, 100, 256, 420], fill=(150, 160, 175), width=4)
        draw.rectangle([230, 400, 282, 416], fill=(120, 85, 45), outline=outline)
        draw.rectangle([248, 416, 264, 448], fill=(90, 60, 35), outline=outline)
    elif kind == "axe":
        draw.rectangle([250, 70, 262, 440], fill=(120, 85, 45), outline=outline)
        draw.polygon([(256, 90), (350, 130), (350, 260), (256, 300)], fill=steel, outline=outline)
        draw.polygon([(256, 90), (162, 130), (162, 260), (256, 300)], fill=steel, outline=outline)
    elif kind == "bow":
        draw.arc([150, 90, 420, 410], 300, 60, fill=(120, 85, 45), width=14)
        draw.line([160, 130, 160, 380], fill=(230, 230, 235), width=5)
        draw.polygon([(256, 150), (268, 256), (256, 362), (244, 256)], fill=(220, 220, 225), outline=outline)
    elif kind == "staff":
        draw.line([256, 80, 256, 440], fill=(120, 85, 45), width=16)
        draw.ellipse([232, 40, 280, 90], fill=(*pal[1], 255), outline=outline)
        draw.ellipse([240, 48, 272, 80], fill=(255, 255, 255, 90))
    else:
        draw.polygon([(256, 80), (280, 100), (256, 340), (232, 100)], fill=steel, outline=outline)
        draw.rectangle([230, 320, 282, 340], fill=(120, 85, 45), outline=outline)
        draw.rectangle([248, 340, 264, 400], fill=(90, 60, 35), outline=outline)


def _draw_armor(img, rng, pal, outline):
    draw = ImageDraw.Draw(img)
    steel = (150, 165, 180)
    draw.polygon([(256, 90), (256, 90), (180, 130), (196, 330), (256, 360), (316, 330), (332, 130)], fill=steel, outline=outline)
    draw.line([256, 110, 256, 350], fill=(110, 125, 140), width=6)
    draw.rounded_rectangle([196, 150, 316, 190], 16, fill=(110, 125, 140), outline=outline, width=3)
    draw.ellipse([236, 200, 276, 240], fill=(*pal[1], 255), outline=outline, width=4)
    draw.rounded_rectangle([160, 130, 210, 300], 24, fill=(130, 145, 160), outline=outline, width=3)
    draw.rounded_rectangle([302, 130, 352, 300], 24, fill=(130, 145, 160), outline=outline, width=3)


def _draw_item(img, rng, pal, outline):
    kind = rng.choice(["potion", "gem", "scroll", "book", "coin", "ore"])
    draw = ImageDraw.Draw(img)
    if kind == "potion":
        draw.rounded_rectangle([238, 190, 274, 220], 6, fill=(120, 100, 70), outline=outline)
        draw.polygon([(238, 220), (274, 220), (300, 330), (256, 380), (212, 330)], fill=(*pal[1], 255), outline=outline, width=4)
        draw.ellipse([234, 262, 278, 306], fill=(255, 255, 255, 60))
    elif kind == "gem":
        draw.polygon([(256, 140), (330, 220), (256, 380), (182, 220)], fill=(*pal[1], 255), outline=outline, width=4)
        draw.polygon([(256, 190), (298, 232), (256, 320), (214, 232)], fill=(255, 255, 255, 120))
    elif kind == "scroll":
        draw.rounded_rectangle([180, 180, 332, 320], 16, fill=(240, 228, 200), outline=outline, width=4)
        draw.rectangle([196, 205, 316, 215], fill=(120, 110, 90))
        draw.rectangle([196, 230, 316, 240], fill=(120, 110, 90))
        draw.rectangle([196, 255, 316, 265], fill=(120, 110, 90))
        draw.rounded_rectangle([168, 170, 188, 330], 10, fill=(150, 60, 60), outline=outline)
        draw.rounded_rectangle([324, 170, 344, 330], 10, fill=(150, 60, 60), outline=outline)
    elif kind == "book":
        draw.rounded_rectangle([170, 170, 342, 340], 10, fill=(120, 60, 50), outline=outline, width=4)
        draw.rectangle([186, 186, 326, 324], fill=(230, 220, 195), outline=outline, width=3)
        draw.line([256, 186, 256, 324], fill=(150, 140, 120), width=4)
    elif kind == "coin":
        draw.ellipse([200, 200, 312, 312], fill=(220, 180, 60), outline=outline, width=4)
        draw.ellipse([220, 220, 292, 292], fill=(240, 205, 90))
        draw.ellipse([244, 244, 268, 268], fill=(190, 150, 40))
    else:
        draw.polygon([(220, 260), (256, 180), (300, 260), (292, 320), (228, 320)], fill=(120, 120, 135), outline=outline, width=4)
        draw.polygon([(256, 180), (288, 235), (256, 250)], fill=(170, 170, 185))


def _draw_creature(img, rng, pal, outline):
    draw = ImageDraw.Draw(img)
    body = rng.choice([(*pal[1], 255), (140, 110, 90, 255), (100, 130, 110, 255), (150, 80, 90, 255)])
    # quadruped silhouette
    draw.ellipse([140, 230, 340, 340], fill=body, outline=outline, width=4)
    draw.rounded_rectangle([150, 320, 210, 400], 16, fill=body, outline=outline, width=3)
    draw.rounded_rectangle([270, 320, 330, 400], 16, fill=body, outline=outline, width=3)
    # head
    draw.ellipse([300, 200, 380, 280], fill=body, outline=outline, width=4)
    # eyes
    draw.ellipse([332, 228, 352, 246], fill=(255, 240, 200), outline=outline, width=2)
    draw.ellipse([336, 233, 348, 243], fill=(40, 30, 30))
    # ears / horns
    draw.polygon([(330, 205), (322, 160), (352, 196)], fill=body, outline=outline)
    draw.polygon([(360, 205), (368, 160), (392, 200)], fill=body, outline=outline)
    # tail
    draw.arc([110, 200, 240, 320], 100, 240, fill=outline, width=10)


def _draw_environment(img, rng, pal, outline):
    kind = rng.choice(["house", "tree", "tower", "rock"])
    draw = ImageDraw.Draw(img)
    if kind == "house":
        draw.rectangle([140, 260, 372, 400], fill=(190, 150, 110), outline=outline, width=4)
        draw.polygon([(120, 260), (256, 150), (392, 260)], fill=(140, 70, 55), outline=outline, width=4)
        draw.rectangle([236, 320, 276, 400], fill=(90, 60, 40), outline=outline, width=3)
        draw.rectangle([170, 290, 210, 320], fill=(*pal[1], 255), outline=outline, width=3)
        draw.rectangle([302, 290, 342, 320], fill=(*pal[1], 255), outline=outline, width=3)
    elif kind == "tree":
        draw.rectangle([238, 280, 274, 420], fill=(110, 75, 50), outline=outline, width=4)
        draw.ellipse([120, 120, 392, 330], fill=(80, 160, 90), outline=outline, width=4)
        draw.ellipse([160, 170, 350, 310], fill=(110, 190, 110))
    elif kind == "tower":
        draw.rectangle([200, 140, 312, 420], fill=(170, 160, 150), outline=outline, width=4)
        draw.polygon([(180, 140), (256, 60), (332, 140)], fill=(130, 60, 55), outline=outline, width=4)
        draw.rectangle([236, 300, 276, 360], fill=(80, 60, 45), outline=outline, width=3)
        draw.rectangle([230, 190, 282, 240], fill=(*pal[1], 255), outline=outline, width=3)
    else:
        draw.polygon([(150, 420), (220, 260), (300, 380), (350, 290), (400, 420)], fill=(140, 140, 150), outline=outline, width=4)
        draw.polygon([(220, 260), (256, 320), (200, 340)], fill=(180, 180, 190))


def _draw_terrain(img, rng, pal, outline):
    draw = ImageDraw.Draw(img)
    base = rng.choice([(96, 150, 84), (196, 170, 110), (200, 200, 205), (110, 130, 150)])
    draw.rectangle([64, 64, 448, 448], fill=base)
    for _ in range(rng.randint(8, 16)):
        x = rng.randint(70, 430)
        y = rng.randint(70, 430)
        r = rng.randint(8, 22)
        shade = tuple(min(255, c + rng.randint(-25, 25)) for c in base)
        draw.ellipse([x, y, x + r, y + r], fill=shade + (255,))
    draw.rectangle([64, 64, 448, 448], outline=(60, 60, 70, 255), width=4)


def _draw_ui(img, rng, pal, outline):
    draw = ImageDraw.Draw(img)
    frame = (60, 45, 35)
    draw.rounded_rectangle([96, 120, 416, 392], 20, fill=(30, 32, 40, 235), outline=frame, width=8)
    draw.rounded_rectangle([116, 140, 396, 372], 12, fill=(42, 46, 56, 255), outline=(90, 70, 50, 255), width=4)
    draw.rounded_rectangle([136, 160, 376, 200], 8, fill=(*pal[1], 255), outline=(20, 22, 28, 255), width=3)
    draw.rounded_rectangle([136, 220, 376, 260], 8, fill=(70, 74, 86, 255), outline=(20, 22, 28, 255), width=3)
    draw.rounded_rectangle([136, 280, 376, 320], 8, fill=(70, 74, 86, 255), outline=(20, 22, 28, 255), width=3)
    draw.rounded_rectangle([136, 340, 376, 372], 8, fill=(160, 70, 70, 255), outline=(20, 22, 28, 255), width=3)


def _draw_icon(img, rng, pal, outline):
    draw = ImageDraw.Draw(img)
    draw.ellipse([140, 140, 372, 372], fill=(*pal[1], 255), outline=outline, width=6)
    star = []
    for i in range(10):
        ang = math.pi * i / 5 - math.pi / 2
        r = 105 if i % 2 == 0 else 45
        star.append((256 + r * math.cos(ang), 256 + r * math.sin(ang)))
    draw.polygon(star, fill=(255, 247, 214, 255), outline=(150, 100, 20, 255), width=3)


def _draw_effect(img, rng, pal, outline):
    draw = ImageDraw.Draw(img)
    color = rng.choice([(255, 150, 60, 255), (110, 200, 255, 255), (190, 120, 255, 255), (110, 255, 160, 255)])
    draw.ellipse([150, 150, 362, 362], fill=color)
    draw.ellipse([180, 180, 332, 332], fill=(255, 255, 255, 100))
    for _ in range(rng.randint(8, 14)):
        x = rng.randint(100, 400)
        y = rng.randint(100, 400)
        r = rng.randint(6, 20)
        draw.ellipse([x, y, x + r, y + r], fill=(color[0], color[1], color[2], 180))
    draw.ellipse([210, 210, 302, 302], fill=(255, 255, 255, 220))


def _draw_furniture(img, rng, pal, outline):
    draw = ImageDraw.Draw(img)
    wood = rng.choice([(150, 105, 60), (120, 85, 50), (170, 125, 75)])
    draw.rounded_rectangle([140, 200, 372, 240], 8, fill=wood, outline=outline, width=4)
    draw.rounded_rectangle([150, 240, 190, 400], 8, fill=(90, 60, 35), outline=outline, width=3)
    draw.rounded_rectangle([322, 240, 362, 400], 8, fill=(90, 60, 35), outline=outline, width=3)
    draw.rounded_rectangle([140, 170, 372, 210], 8, fill=(wood[0] + 20, wood[1] + 20, wood[2] + 20, 255), outline=outline, width=3)


def _draw_default(img, rng, pal, outline):
    draw = ImageDraw.Draw(img)
    draw.rounded_rectangle([140, 140, 372, 372], 40, fill=(*pal[1], 255), outline=outline, width=5)
    draw.ellipse([200, 200, 312, 312], fill=(255, 255, 255, 90))


DRAWERS = {
    "character": _draw_character, "npc": _draw_npc, "weapon": _draw_weapon,
    "armor": _draw_armor, "item": _draw_item, "creature": _draw_creature,
    "environment": _draw_environment, "terrain": _draw_terrain, "ui": _draw_ui,
    "icon": _draw_icon, "effect": _draw_effect, "furniture": _draw_furniture,
    "default": _draw_default,
}


def render_asset(asset_id: str, prompt: str, kind: str, size: int) -> Image.Image:
    pal = PALETTES.get(kind, PALETTES["default"])
    rng = random.Random(hash((asset_id, kind)) & 0xFFFFFFFF)
    seed_rng = random.Random(abs(hash(asset_id)) & 0xFFFFFFFF)
    rng = seed_rng
    base = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    big = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    draw = ImageDraw.Draw(big)
    outline = (30, 30, 40, 255)
    # soft radial glow backdrop
    glow = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    gd = ImageDraw.Draw(glow)
    gd.ellipse([96, 96, 416, 416], fill=(*pal[1], 40))
    glow = glow.filter(ImageFilter.GaussianBlur(40))
    big.alpha_composite(glow)
    DRAWERS[kind](big, rng, pal, outline)
    if size != 512:
        big = big.resize((size, size), Image.LANCZOS)
    return big


def load_prompts(path: Path) -> list[tuple[str, str]]:
    data = json.loads(path.read_text(encoding="utf-8"))
    entries = []
    if isinstance(data, dict):
        for k, v in data.items():
            if isinstance(v, str):
                entries.append((k, v))
    elif isinstance(data, list):
        for item in data:
            if isinstance(item, dict):
                p = item.get("path") or item.get("file") or item.get("name") or "asset.png"
                entries.append((p, item.get("prompt", str(item))))
    return entries


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--prompts", default=str(DEFAULT_PROMPTS))
    ap.add_argument("--size", type=int, default=512)
    ap.add_argument("--force", action="store_true")
    args = ap.parse_args()

    entries = load_prompts(Path(args.prompts))
    if not entries:
        entries = [(f"asset_{i:04d}.png", "generic game asset") for i in range(393)]

    written = 0
    skipped = 0
    for rel, prompt in entries:
        rel = rel.replace("Assets/GoonzuGame/", "")
        out_path = ASSETS_ROOT / rel
        if out_path.suffix.lower() not in {".png", ".jpg", ".jpeg"}:
            out_path = out_path.with_suffix(".png")
        if out_path.exists() and not args.force:
            skipped += 1
            continue
        out_path.parent.mkdir(parents=True, exist_ok=True)
        kind = classify(rel, prompt)
        img = render_asset(out_path.stem, prompt, kind, args.size)
        img.save(out_path, "PNG")
        written += 1

    print(f"Generated {written} PNG assets ({skipped} skipped/already present) in {ASSETS_ROOT}")


if __name__ == "__main__":
    main()
