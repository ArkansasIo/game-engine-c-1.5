using NUnit.Framework;
using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine.Tests;

public sealed class GoonzuIsometricGridTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void TileToWorldAndWorldToTileRoundTrip(bool centerTiles)
    {
        var go = new GameObject("IsometricGridTest");
        try
        {
            var grid = go.AddComponent<GoonzuIsometricGrid>();
            grid.tileWidth = 64f;
            grid.tileHeight = 32f;
            grid.centerTiles = centerTiles;

            var coordinates = new[] { (0, 0), (1, 0), (0, 1), (8, 4), (-3, 7) };
            foreach (var (x, y) in coordinates)
            {
                var world = grid.TileToWorld(x, y, 5f);
                var tile = grid.WorldToTile(world);
                Assert.That(tile.x, Is.EqualTo(x), $"x mismatch for {x},{y}");
                Assert.That(tile.y, Is.EqualTo(y), $"y mismatch for {x},{y}");
            }
        }
        finally
        {
            Object.DestroyImmediate(go);
        }
    }
}
