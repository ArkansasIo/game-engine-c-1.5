using System.Text.Json;

namespace EtherealVeni.Configuration;

public static class EngineSettingsLoader
{
    public static async Task<EngineSettings> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path)) return new EngineSettings();
        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<EngineSettings>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }, cancellationToken) ?? new EngineSettings();
    }

    public static async Task SaveAsync(string path, EngineSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        var directory = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, settings, new JsonSerializerOptions { WriteIndented = true }, cancellationToken);
    }
}
