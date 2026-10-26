# Goonzu Game Asset Pack

This directory is generated from the repository's `goonzu_prompts.json` prompt catalog.

## Generate all image assets

From the repository root:

```bash
python tools/generate_goonzu_assets.py
```

The generator creates one scalable SVG per discovered prompt entry, `manifest.json`, and `goonzu_asset_atlas.svg`.

If the prompt catalog cannot be resolved to individual entries, the generator falls back to a complete 393-asset pack so the engine always has a deterministic visual set to work with.

## Asset categories

The generator classifies assets into character, NPC, enemy, monster, environment, terrain, item, weapon, armor, UI, icon, and default categories. Each category has a consistent palette and silhouette language suitable for replacing the temporary art with production art later without changing asset references.

## Production art workflow

The SVG output is intentionally dependency-free and source-controlled. It can be rasterized to PNG/WebP/AVIF for runtime platforms or replaced by final art while preserving the generated manifest IDs.
