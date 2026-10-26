namespace EtherealVeni.World;

public sealed class WorldState
{
    private readonly Dictionary<string, Scene> _scenes = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, Scene> Scenes => _scenes;
    public Scene? ActiveScene { get; private set; }

    public void Register(Scene scene)
    {
        ArgumentNullException.ThrowIfNull(scene);
        _scenes[scene.Id] = scene;
    }

    public Scene LoadScene(string id)
    {
        if (!_scenes.TryGetValue(id, out var scene))
            throw new KeyNotFoundException($"Scene '{id}' was not registered.");
        ActiveScene?.Unload();
        scene.Load();
        ActiveScene = scene;
        return scene;
    }

    public bool TryGetScene(string id, out Scene? scene) => _scenes.TryGetValue(id, out scene);
}
