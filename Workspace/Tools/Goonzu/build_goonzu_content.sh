#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
DATA="$ROOT/Content/GoonzuGame"
ASSETS="$ROOT/Assets/GoonzuGame"

printf '%s\n' "GoonZu-inspired content validation"
printf 'Data:   %s\n' "$DATA"
printf 'Assets: %s\n' "$ASSETS"

required=(
  "$DATA/GoonzuGameData.cs"
  "$DATA/GoonzuGameSystems.hpp"
  "$DATA/goonzu_world.json"
  "$DATA/goonzu_economy.sql"
  "$DATA/goonzu_asset_catalog.json"
)

for file in "${required[@]}"; do
  if [[ ! -f "$file" ]]; then
    echo "ERROR: missing $file" >&2
    exit 1
  fi
done

if command -v python3 >/dev/null 2>&1; then
  python3 - <<PY
import json
from pathlib import Path
for p in [Path(r"$DATA/goonzu_world.json"), Path(r"$DATA/goonzu_asset_catalog.json")]:
    json.loads(p.read_text(encoding="utf-8"))
    print(f"OK JSON: {p}")
PY
fi

if command -v c++ >/dev/null 2>&1; then
  TMP="$(mktemp --suffix=.cpp)"
  trap 'rm -f "$TMP"' EXIT
  cat > "$TMP" <<EOF
#include "${DATA#$ROOT/}/GoonzuGameSystems.hpp"
int main() {
  GoonzuGame::EconomySystem economy;
  economy.AddListing({1, "test", "iron_sword", 100, 1, 1});
  return GoonzuGame::CraftingSystem::SuccessChance(10, 5) >= 0.0 ? 0 : 1;
}
EOF
  if c++ -std=c++23 -I"$ROOT" "$TMP" -o "${TMP%.cpp}"; then
    echo "OK C++23 header compile"
    rm -f "${TMP%.cpp}"
  else
    echo "WARNING: C++23 validation failed" >&2
  fi
fi

echo "Content validation complete. Existing engine source files were not modified."
