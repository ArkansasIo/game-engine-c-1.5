#!/usr/bin/env bash
set -euo pipefail
: "${GAME_DB_HOST:?GAME_DB_HOST is required}"
: "${GAME_DB_USER:?GAME_DB_USER is required}"
: "${GAME_DB_PASSWORD:?GAME_DB_PASSWORD is required}"
DB_NAME="${GAME_DB_NAME:-game_engine}"
OUT="${BACKUP_DIR:-/var/backups/game-engine}"
mkdir -p "$OUT"
STAMP=$(date -u +%Y%m%dT%H%M%SZ)
MYSQL_PWD="$GAME_DB_PASSWORD" mysqldump --single-transaction --routines --triggers -h "$GAME_DB_HOST" -u "$GAME_DB_USER" "$DB_NAME" | gzip > "$OUT/${DB_NAME}_${STAMP}.sql.gz"
find "$OUT" -type f -name '*.sql.gz' -mtime +14 -delete