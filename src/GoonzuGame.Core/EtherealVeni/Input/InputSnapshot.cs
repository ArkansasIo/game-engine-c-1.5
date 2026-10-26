namespace EtherealVeni.Input;

public enum InputAction
{
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,
    Interact,
    Attack,
    Special,
    Jump,
    Menu,
    Pause
}

public sealed class InputSnapshot
{
    private readonly HashSet<InputAction> _pressed = new();

    public bool IsPressed(InputAction action) => _pressed.Contains(action);

    public void SetPressed(InputAction action, bool pressed)
    {
        if (pressed) _pressed.Add(action);
        else _pressed.Remove(action);
    }

    public void Clear() => _pressed.Clear();
}
