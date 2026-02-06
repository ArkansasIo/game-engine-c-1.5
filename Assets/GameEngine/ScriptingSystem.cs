using System;
using System.Collections.Generic;

namespace GameEngine
{
    public class ScriptingSystem : System
    {
        private readonly Dictionary<string, Action> scripts = new();

        // Register a script by name
        public void RegisterScript(string name, Action script)
        {
            scripts[name] = script;
        }

        // Run a script by name
        public void RunScript(string name)
        {
            if (scripts.TryGetValue(name, out var script))
                script?.Invoke();
        }

        // Remove a script
        public void RemoveScript(string name)
        {
            scripts.Remove(name);
        }

        // Example: Run all scripts (could be used for cutscenes, triggers, etc.)
        public void RunAll()
        {
            foreach (var script in scripts.Values)
                script?.Invoke();
        }
    }
}
