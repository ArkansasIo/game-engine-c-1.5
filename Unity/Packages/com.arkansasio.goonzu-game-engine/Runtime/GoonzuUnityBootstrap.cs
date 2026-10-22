using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine;

/// <summary>
/// Unity-side entry point. It does not replace GameEngine.cpp; it hosts the same data-driven world model inside Unity.
/// </summary>
public sealed class GoonzuUnityBootstrap : MonoBehaviour
{
    [Header("World")]
    [SerializeField] private TextAsset seedWorldJson;
    [SerializeField] private GoonzuWorldDefinition worldDefinition;
    [SerializeField] private Transform worldRoot;
    [SerializeField] private GoonzuIsometricGrid grid;

    [Header("Presentation")]
    [SerializeField] private Camera worldCamera;
    [SerializeField] private float townMarkerHeight = 0.1f;
    [SerializeField] private float playerElevation = 0.15f;

    public GoonzuWorldDefinition World { get; private set; }

    private void Awake()
    {
        if (worldDefinition != null)
            World = worldDefinition;
        else if (seedWorldJson != null)
            World = GoonzuWorldLoader.LoadJson(seedWorldJson);
        else
            Debug.LogWarning("GoonzuUnityBootstrap: assign a world definition or seedWorldJson.");

        if (grid == null)
            grid = GetComponentInChildren<GoonzuIsometricGrid>();
        if (worldRoot == null)
            worldRoot = transform;
        if (worldCamera == null)
            worldCamera = Camera.main;

        if (World != null)
            BuildWorldMarkers();
    }

    private void BuildWorldMarkers()
    {
        for (var i = 0; i < World.towns.Length; i++)
        {
            var town = World.towns[i];
            var marker = GameObject.CreatePrimitive(PrimitiveType.Quad);
            marker.name = $"Town_{town.id}";
            marker.transform.SetParent(worldRoot, false);
            marker.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            marker.transform.position = grid != null
                ? grid.TileToWorld(i * 4, i * 2, townMarkerHeight)
                : new Vector3(i * 2f, townMarkerHeight, i * 2f);
            marker.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        }

        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player_Adventurer_01";
        player.transform.SetParent(worldRoot, false);
        player.transform.position = grid != null
            ? grid.TileToWorld(0, 0, playerElevation)
            : new Vector3(0f, playerElevation, 0f);
    }

    public void FocusCamera(Vector3 worldPosition)
    {
        if (worldCamera == null) return;
        worldCamera.transform.position = worldPosition + new Vector3(0f, 12f, -10f);
        worldCamera.transform.rotation = Quaternion.Euler(50f, 0f, 0f);
    }
}
