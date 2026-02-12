using System;

namespace GoonzuGame.Graphics
{
    using System.Collections.Generic;

    public class GraphicsSettings
    {
        public int ResolutionWidth { get; set; }
        public int ResolutionHeight { get; set; }
        public bool Fullscreen { get; set; }
        public string Quality { get; set; }
    }

    public class GraphicsManager
    {
        public List<string> RenderedSprites { get; set; }
        public List<string> RenderedModels { get; set; }
        public GraphicsSettings Settings { get; set; }

        public GraphicsManager()
        {
            RenderedSprites = new List<string>();
            RenderedModels = new List<string>();
            Settings = new GraphicsSettings { ResolutionWidth = 1920, ResolutionHeight = 1080, Fullscreen = true, Quality = "High" };
        }

        public void RenderSprite(string spriteName)
        {
            RenderedSprites.Add(spriteName);
            Console.WriteLine($"Rendering sprite: {spriteName}");
        }

        public void RenderModel(string modelName)
        {
            RenderedModels.Add(modelName);
            Console.WriteLine($"Rendering model: {modelName}");
        }

        public void SetGraphicsSettings(GraphicsSettings settings)
        {
            Settings = settings;
            Console.WriteLine($"Graphics settings updated: {settings.ResolutionWidth}x{settings.ResolutionHeight}, Fullscreen: {settings.Fullscreen}, Quality: {settings.Quality}");
        }

        public void ApplyEffect(string effectName)
        {
            Console.WriteLine($"Applying graphics effect: {effectName}");
        }
    }
}
