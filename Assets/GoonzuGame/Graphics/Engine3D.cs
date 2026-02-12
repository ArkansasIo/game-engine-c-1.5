using System;
using System.Collections.Generic;

namespace GoonzuGame.Graphics
{
    public class Engine3D
    {
        public List<string> Models { get; set; }
        public Engine3D()
        {
            Models = new List<string>();
        }
        public void LoadModel(string modelName)
        {
            Models.Add(modelName);
            Console.WriteLine($"Loaded 3D model: {modelName}");
        }
        public void RenderModel(string modelName, float x, float y, float z)
        {
            Console.WriteLine($"Rendering 3D model '{modelName}' at ({x},{y},{z})");
        }
        public void ApplyMaterial(string modelName, string material)
        {
            Console.WriteLine($"Applying material '{material}' to 3D model '{modelName}'");
        }
        public void AnimateModel(string modelName, string animation)
        {
            Console.WriteLine($"Animating 3D model '{modelName}' with '{animation}'");
        }
    }
}
