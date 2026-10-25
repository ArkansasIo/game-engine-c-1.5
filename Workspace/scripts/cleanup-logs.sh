#!/usr/bin/env bash
set -euo pipefail
LOG_DIR="${GAME_LOG_DIR:-/var/log/game-engine}"
if [ -d "$LOG_DIR" ]; then find "$LOG_DIR" -type f -name '*.log' -mtime +30 -delete; fi