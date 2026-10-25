using System;
using System.Collections.Generic;

namespace EtherealVeni.Editor;

public sealed class EditorApplication
{
    private readonly Dictionary<string, IEditorPanel> _panels = new(StringComparer.OrdinalIgnoreCase);

    public string EngineName => "Ethereal Veni";
    public string GameName => "Ethereo Pandora";
    public WorkspaceSession Workspace { get; private set; } = new();

    public void RegisterPanel(IEditorPanel panel)
    {
        ArgumentNullException.ThrowIfNull(panel);
        _panels[panel.Id] = panel;
    }

    public bool TryGetPanel(string id, out IEditorPanel? panel) => _panels.TryGetValue(id, out panel);

    public IReadOnlyCollection<IEditorPanel> Panels => _panels.Values;

    public void OpenWorkspace(string projectId)
    {
        Workspace = new WorkspaceSession { ProjectId = projectId };
    }
}

public interface IEditorPanel
{
    string Id { get; }
    string Title { get; }
}

public sealed class WorkspaceSession
{
    public string ProjectId { get; init; } = "EthereoPandora";
    public string LayoutId { get; set; } = "GameDevelopment";
    public string ActiveScene { get; set; } = "MainWorld";
}
