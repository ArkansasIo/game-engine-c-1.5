# evai — Ethereal Veni External AI Development Terminal

`evai` is the external, engine-independent AI development interface for Ethereal Veni. It connects to a local Ollama service and uses the same project/toolchain concepts as the native editor.

## Goals

- Work without launching the engine editor.
- Read and modify engine or game source through explicit tools.
- Run `evbuild` and inspect diagnostics.
- Show a planned change/diff before applying writes.
- Support engine, game, or read-only permission scopes.
- Keep Ollama replaceable behind a provider interface.

## Example

```text
$ evai
Ethereal Veni AI
> Add an inventory component to Ethereo Pandora.
```

The first implementation is intentionally conservative: it exposes project context and a dry-run planning mode before destructive filesystem operations are enabled.
