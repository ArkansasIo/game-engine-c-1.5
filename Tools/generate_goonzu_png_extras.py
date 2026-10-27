#!/usr/bin/env python3
"""Generate the remaining GoonZu PNG assets that exist only as .txt placeholders."""
from __future__ import annotations

import random
from pathlib import Path

from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / "Assets" / "GoonzuGame"

PALETTES = {
    "character": ((37, 53, 74), (121, 184, 255), (255, 205, 148)),
    "weapon":    ((49, 52, 59), (199, 206, 219), (120, 85, 45)),
    "terrain":   ((60, 50, 31), (96, 150, 84), (255, 244, 206)),
    "environment": ((25, 54, 47), (190, 150, 110), (140, 70, 55)),
}


def draw_character(size, rng, pal):
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    s = size / 512.0
    o = (30, 30, 40, 255)
    cloth = (150, 100, 60)
    skin = (232, 190, 152)
    cx = size // 2
    cy = int(300 * s)
    def rrect(box, r, fill, w=3):
        draw.rounded_rectangle([int(v * s) for v in box], int(r * s), fill=fill, outline=o, width=w)
    def ell(box, fill, w=3):
        draw.ellipse([int(v * s) for v in box], fill=fill, outline=o, width=w)
    rrect([cx - 46, cy + 40, cx - 12, cy + 190], 18, (60, 55, 70))
    rrect([cx + 12, cy + 40, cx + 46, cy + 190], 18, (60, 55, 70))
    rrect([cx - 50, cy + 165, cx - 8, cy + 196], 12, (70, 45, 35))
    rrect([cx + 8, cy + 165, cx + 50, cy + 196], 12, (70, 45, 35))
    rrect([cx - 82, cy - 30, cx - 52, cy + 60], 16, cloth)
    rrect([cx + 52, cy - 30, cx + 82, cy + 60], 16, cloth)
    ell([cx - 88, cy + 55, cx - 50, cy + 92], skin)
    ell([cx + 50, cy + 55, cx + 88, cy + 92], skin)
    rrect([cx - 58, cy - 40, cx + 58, cy + 60], 24, cloth, 4)
    rrect([cx - 56, cy + 28, cx + 56, cy + 46], 0, (80, 60, 40))
    ell([cx - 44, cy - 128, cx + 44, cy - 44], skin, 4)
    draw.pieslice([int(v * s) for v in [cx - 48, cy - 134, cx + 48, cy - 64]], 180, 360,
                  fill=(90, 60, 35) if rng.random() > 0.5 else (220, 200, 160))
    ell([cx - 26, cy - 96, cx - 8, cy - 82], (255, 255, 255), 2)
    ell([cx + 8, cy - 96, cx + 26, cy - 82], (255, 255, 255), 2)
    draw.ellipse([int((cx - 20) * s), int((cy - 93) * s), int((cx - 12) * s), int((cy - 85) * s)], fill=(40, 40, 60))
    draw.ellipse([int((cx + 14) * s), int((cy - 93) * s), int((cx + 22) * s), int((cy - 85) * s)], fill=(40, 40, 60))
    return img


def draw_weapon(size, rng, pal):
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    s = size / 512.0
    o = (30, 30, 40, 255)
    steel = (200, 208, 220)
    draw.polygon([(int(256 * s), int(80 * s)), (int(276 * s), int(100 * s)), (int(256 * s), int(420 * s)),
                  (int(236 * s), int(100 * s))], fill=steel, outline=o)
    draw.rectangle([int(230 * s), int(400 * s), int(282 * s), int(416 * s)], fill=(120, 85, 45), outline=o)
    draw.rectangle([int(248 * s), int(416 * s), int(264 * s), int(448 * s)], fill=(90, 60, 35), outline=o)
    return img


def draw_terrain(size, rng, pal):
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    base = (96, 150, 84, 255)
    draw.rectangle([0, 0, size - 1, size - 1], fill=base)
    for _ in range(30):
        x, y = rng.randint(0, size - 30), rng.randint(0, size - 30)
        r = rng.randint(6, 20)
        shade = tuple(min(255, max(0, c + rng.randint(-30, 30))) for c in base[:3])
        draw.ellipse([x, y, x + r, y + r], fill=shade + (255,))
    return img


def draw_house(size, rng, pal):
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    s = size / 512.0
    o = (30, 30, 40, 255)
    draw.rectangle([int(140 * s), int(260 * s), int(372 * s), int(400 * s)], fill=(190, 150, 110), outline=o, width=4)
    draw.polygon([(int(120 * s), int(260 * s)), (int(256 * s), int(150 * s)), (int(392 * s), int(260 * s))],
                 fill=(140, 70, 55), outline=o)
    draw.rectangle([int(236 * s), int(320 * s), int(276 * s), int(400 * s)], fill=(90, 60, 40), outline=o, width=3)
    draw.rectangle([int(170 * s), int(290 * s), int(210 * s), int(320 * s)], fill=(121, 184, 255), outline=o, width=3)
    draw.rectangle([int(302 * s), int(290 * s), int(342 * s), int(320 * s)], fill=(121, 184, 255), outline=o, width=3)
    return img


def main():
    extras = {
        "Buildings/house_01.png": ("environment", draw_house),
        "Characters/player_female.png": ("character", draw_character),
        "Characters/player_male.png": ("character", draw_character),
        "Items/Weapons/sword.png": ("weapon", draw_weapon),
        "Tiles/grass_tile.png": ("terrain", draw_terrain),
    }
    n = 0
    for rel, (kind, fn) in extras.items():
        out = ASSETS / rel
        if out.exists():
            continue
        rng = random.Random(rel)
        img = fn(512, rng, PALETTES[kind])
        out.parent.mkdir(parents=True, exist_ok=True)
        img.save(out, "PNG")
        n += 1
    print(f"Generated {n} extra PNG assets")


if __name__ == "__main__":
    main()
