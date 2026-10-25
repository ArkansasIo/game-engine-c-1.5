using System.Collections.Generic;

namespace GameEngine
{
    public class SystemManager
    {
        private readonly List<System> systems = new();

        public void Register(System system)
        {
            systems.Add(system);
            System.Console.WriteLine($"Registered system: {system.GetType().Name}");
        }

        public void InitializeAll()
        {
            foreach (var system in systems)
            {
                system.Initialize();
                System.Console.WriteLine($"Initialized system: {system.GetType().Name}");
            }
        }

        public void UpdateAll(float deltaTime)
        {
            foreach (var system in systems)
            {
                system.Update(deltaTime);
                System.Console.WriteLine($"Updated system: {system.GetType().Name}");
            }
        }

        public void PauseAll()
        {
            foreach (var system in systems)
            {
                system.Pause();
                System.Console.WriteLine($"Paused system: {system.GetType().Name}");
            }
        }

        public void ResumeAll()
        {
            foreach (var system in systems)
            {
                system.Resume();
                System.Console.WriteLine($"Resumed system: {system.GetType().Name}");
            }
        }

        public void ShutdownAll()
        {
            foreach (var system in systems)
                system.Shutdown();
        }

        public void BroadcastEvent(string eventType, object eventData)
        {
            foreach (var system in systems)
                system.HandleEvent(eventType, eventData);
        }
    }
}
