namespace EtherealVeni.Configuration;

public sealed class EngineSettings
{
    public string ProductName { get; set; } = "Ethereal Veni";
    public string GameName { get; set; } = "Ethereo Pandora";
    public int TargetFramesPerSecond { get; set; } = 60;
    public double FixedUpdateHz { get; set; } = 60;
    public bool HeadlessServer { get; set; }
    public string SaveDirectory { get; set; } = "Saves";
    public string AssetDirectory { get; set; } = "Assets";
    public string LogDirectory { get; set; } = "Logs";
}
