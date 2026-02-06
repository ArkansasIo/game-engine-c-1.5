namespace GameEngine.System
{
    /// <summary>
    /// Base class for all engine systems. Provides lifecycle hooks and system metadata.
    /// </summary>
    public abstract class System
    {
        /// <summary>
        /// The name of the system (for identification and debugging).
        /// </summary>
        public string SystemName;
        /// <summary>
        /// The version of the system implementation.
        /// </summary>
        public string Version;
        /// <summary>
        /// The author or maintainer of the system.
        /// </summary>
        public string Author;
        /// <summary>
        /// Called when the system is initialized.
        /// </summary>
        public abstract void Initialize();
        /// <summary>
        /// Called every frame to update the system.
        /// </summary>
        public abstract void Update();
        /// <summary>
        /// Called when the system is being shut down or destroyed.
        /// </summary>
        public abstract void Shutdown();
    }
}
