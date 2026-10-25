#!/usr/bin/env bash
set -euo pipefail
mkdir -p "${GAME_STATE_DIR:-/var/lib/game-engine}"
printf '%s\n' "$(date -u +%FT%TZ) world maintenance completed" >> "${GAME_STATE_DIR:-/var/lib/game-engine}/maintenance.log"