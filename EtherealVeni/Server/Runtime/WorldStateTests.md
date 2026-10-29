# Manual acceptance checklist

These checks require running the server locally. They are not automated test results.

1. GET /health returns HTTP 200 and a running status.
2. POST /api/v1/players/join with {"username":"TestHero"} returns HTTP 201 and a generated player ID.
3. GET /api/v1/players/{id} returns a level 1 player snapshot.
4. POST /api/v1/world/move accepts a nearby finite position and rejects extreme coordinates or implausible displacement.
5. POST /api/v1/combat/attack rejects unknown players, self-attacks, defeated attackers, defeated targets and targets outside melee range; successful attacks apply server-defined damage.
6. POST /api/v1/progression/experience updates XP and level and rejects negative or excessive XP.
7. POST /api/v1/chat rejects unknown players and oversized messages, sanitizes control characters and returns HTTP 429 for rapid repeated messages.
8. Restarting clears player and chat state; this is expected until durable persistence is connected.
