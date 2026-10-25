"""Minimal Blender -> EAF manifest exporter for Ethereal Veni."""
import json
from pathlib import Path

try:
    import bpy
except ImportError:
    bpy = None


def build_manifest(output: Path) -> dict:
    objects = []
    if bpy is not None:
        for obj in bpy.context.scene.objects:
            objects.append({
                "name": obj.name,
                "type": obj.type,
                "location": list(obj.location),
                "scale": list(obj.scale),
            })
    return {
        "format": "EAF",
        "version": 1,
        "scene": bpy.context.scene.name if bpy is not None else "Standalone",
        "objects": objects,
    }


def export(output_file: str) -> None:
    path = Path(output_file)
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(build_manifest(path), indent=2), encoding="utf-8")


if __name__ == "__main__":
    export("EthereoPandora.eaf.json")
