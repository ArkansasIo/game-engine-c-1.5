# CSS.C1.5

CSS.C1.5 is Ethereal Veni's engine-owned styling language for editor and game UI. It intentionally uses a CSS-like declaration model while remaining a C#-friendly engine contract.

## Example

```css
Panel {
    width: 640px;
    height: 360px;
    padding: 16px;
}

Button.Primary {
    background: #263a55;
    text: #ffffff;
}
```

The current parser provides a small deterministic foundation. Future compiler stages will add variables, inheritance, states, animation, responsive constraints, themes, and runtime style caching.
