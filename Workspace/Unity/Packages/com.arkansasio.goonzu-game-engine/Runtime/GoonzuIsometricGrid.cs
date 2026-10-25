using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine;

/// <summary>Converts logical tile coordinates to Unity world coordinates for a 2:1 isometric grid.</summary>
public sealed class GoonzuIsometricGrid : MonoBehaviour
{
    [Min(0.01f)] public float tileWidth = 1f;
    [Min(0.01f)] public float tileHeight = 0.5f;
    public bool centerTiles = true;

    public Vector3 TileToWorld(int x, int y, float elevation = 0f)
    {
        var worldX = (x - y) * tileWidth * 0.5f;
        var worldZ = (x + y) * tileHeight;
        if (centerTiles)
        {
            worldX += tileWidth * 0.5f;
            worldZ += tileHeight * 0.5f;
        }
        return new Vector3(worldX, elevation, worldZ);
    }

    public Vector2Int WorldToTile(Vector3 world)
    {
        var x = world.x / tileWidth + world.z / (2f * tileHeight);
        var y = world.z / (2f * tileHeight) - world.x / tileWidth;
        return new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }
}
