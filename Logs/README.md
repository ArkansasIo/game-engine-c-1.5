# Ethereal Veni Logs

[DOCS] Central index for development history, chat-derived requirements, update records, and patch records.

## Structure

```text
Logs/
├── README.md
├── Chat/
│   ├── README.md
│   └── 2026-10-25.md
├── Updates/
│   ├── README.md
│   └── 2026-10-25.md
└── Patches/
    ├── README.md
    └── 2026-10-25.md
```

## Rules

- `[CHAT]` records requirements or decisions originating from the development conversation.
- `[UPDATE]` records completed repository changes.
- `[PATCH]` records patch/change-set summaries and validation notes.
- Logs are append-oriented. Existing historical records should not be rewritten merely to change product names.
- Secrets, credentials, access tokens, private user data, and proprietary SDK contents must never be written to these logs.
- Generated binaries and build caches do not belong in `Logs/`.

## Current product identity

- Engine: **Ethereal Veni**
- Game: **Ethereo Pandora**
