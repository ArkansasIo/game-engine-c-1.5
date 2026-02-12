using System;
using System.Collections.Generic;

namespace GoonzuGame.Graphics
{
    public class Engine2D
    {
        public List<string> Sprites { get; set; }
        public Engine2D()
        {
            Sprites = new List<string>();
        }
        public void LoadSprite(string spriteName)
        {
            Sprites.Add(spriteName);
            Console.WriteLine($"Loaded 2D sprite: {spriteName}");
        }
        public void RenderSprite(string spriteName, int x, int y)
        {
            Console.WriteLine($"Rendering 2D sprite '{spriteName}' at ({x},{y})");
        }
        public void AnimateSprite(string spriteName, string animation)
        {
            Console.WriteLine($"Animating 2D sprite '{spriteName}' with '{animation}'");
        }
    }
}
