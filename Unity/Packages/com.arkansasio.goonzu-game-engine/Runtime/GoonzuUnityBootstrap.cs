using System.Collections.Generic;
using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine;

/// <summary>Unity scene bridge for a data-driven isometric world prototype.</summary>
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
    [SerializeField] private bool focusCameraOnStart = true;

    private readonly List<GameObject> spawnedObjects = new();

    public GoonzuWorldDefinition World { get; private set; }
    public GameObject PlayerObject { get; private set; }

    private void Awake()
    {
        if (grid == null)
            grid = GetComponentInChildren<GoonzuIsometricGrid>();
        if (worldRoot == null)
            worldRoot = transform;
        if (worldCamera == null)
            worldCamera = Camera.main;

        try
        {
            if (worldDefinition != null)
                World = worldDefinition;
            else if (seedWorldJson != null)
                World = GoonzuWorldLoader.LoadJson(seedWorldJson);
            else
                Debug.LogWarning("GoonzuUnityBootstrap: assign a world definition or seedWorldJson.");
        }
        catch (System.Exception exception)
        {
            Debug.LogError("GoonzuUnityBootstrap: world load failed. " + exception.Message, this);
            return;
        }

        if (World == null)
            return;

        BuildWorldMarkers();
        if (focusCameraOnStart && PlayerObject != null)
            FocusCamera(PlayerObject.transform.position);
    }

    private void OnDestroy()
    {
        ClearGeneratedObjects();
    }

    private void ClearGeneratedObjects()
    {
        for (var i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] != null)
                Destroy(spawnedObjects[i]);
        }
        spawnedObjects.Clear();
        PlayerObject = null;
    }

    private void BuildWorldMarkers()
    {
        if (worldRoot == null || World.towns == null)
            return;

        ClearGeneratedObjects();

        for (var i = 0; i < World.towns.Length; i++)
        {
            var town = World.towns[i];
            if (town == null)
                continue;

            var marker = GameObject.CreatePrimitive(PrimitiveType.Quad);
            marker.name = "Town_" + town.id;
            marker.transform.SetParent(worldRoot, false);
            marker.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            marker.transform.position = GetTownWorldPosition(town, i, townMarkerHeight);
            marker.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            spawnedObjects.Add(marker);
        }

        PlayerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        PlayerObject.name = "Player_Adventurer_01";
        PlayerObject.transform.SetParent(worldRoot, false);
        PlayerObject.transform.position = grid != null
            ? grid.TileToWorld(0, 0, playerElevation)
            : new Vector3(0f, playerElevation, 0f);
        spawnedObjects.Add(PlayerObject);

        // CharacterController provides a safe starting point for the prototype player controller.
        var controller = PlayerObject.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.35f;
        controller.center = new Vector3(0f, 0.9f, 0f);
        PlayerObject.AddComponent<GoonzuPlayerController>();
    }

    private Vector3 GetTownWorldPosition(GoonzuTown town, int fallbackIndex, float elevation)
    {
        if (grid == null)
            return town.hasGridPosition
                ? new Vector3(town.gridX, elevation, town.gridY)
                : new Vector3(fallbackIndex * 2f, elevation, fallbackIndex * 2f);

        var x = town.hasGridPosition ? town.gridX : fallbackIndex * 4;
        var y = town.hasGridPosition ? town.gridY : fallbackIndex * 2;
        return grid.TileToWorld(x, y, elevation);
    }

    public void FocusCamera(Vector3 worldPosition)
    {
        if (worldCamera == null)
            return;

        worldCamera.transform.position = worldPosition + new Vector3(0f, 12f, -10f);
        worldCamera.transform.rotation = Quaternion.Euler(50f, 0f, 0f);
    }
}
