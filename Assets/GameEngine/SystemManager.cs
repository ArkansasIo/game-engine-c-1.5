using System.Collections.Generic;

namespace GameEngine
{
    public class SystemManager
    {
        private readonly List<System> systems = new();

        public void Register(System system)
        {
            systems.Add(system);
        }

        public void InitializeAll()
        {
            foreach (var system in systems)
                system.Initialize();
        }

        public void UpdateAll(float deltaTime)
        {
            foreach (var system in systems)
                system.Update(deltaTime);
        }

        public void PauseAll()
        {
            foreach (var system in systems)
                system.Pause();
        }

        public void ResumeAll()
        {
            foreach (var system in systems)
                system.Resume();
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
