using System;
using System.Collections.Generic;

namespace GameEngine.Scripting.Lua;

/// <summary>
/// Controlled C# boundary for Lua gameplay scripts.
/// The concrete Lua VM adapter can implement this contract without exposing
/// arbitrary host-process APIs to scripts.
/// </summary>
public interface ILuaBridge
{
    void RegisterType(string name, Type type);
    void RegisterFunction(string name, Delegate function);
    object? Call(string functionName, params object?[] arguments);
    void SetGlobal(string name, object? value);
    object? GetGlobal(string name);
}

public sealed class LuaBridgeRegistry
{
    private readonly Dictionary<string, Type> _types = new();
    private readonly Dictionary<string, Delegate> _functions = new();

    public void RegisterType<T>(string name) => _types[name] = typeof(T);

    public void RegisterFunction(string name, Delegate function) => _functions[name] = function;

    public bool TryGetType(string name, out Type? type) => _types.TryGetValue(name, out type);

    public bool TryGetFunction(string name, out Delegate? function) => _functions.TryGetValue(name, out function);
}
