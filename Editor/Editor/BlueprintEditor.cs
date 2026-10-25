using System;
using System.Collections.Generic;

namespace EtherealVeni.Editor;

public enum GraphNodeKind { Event, Action, Condition, Branch, Variable, Ability, Math, Script }

public sealed record BlueprintNode(string Id, GraphNodeKind Kind, string Name);
public sealed record BlueprintLink(string FromNode, string FromPin, string ToNode, string ToPin);

public sealed class BlueprintGraph
{
    public string Name { get; init; } = "NewGraph";
    public List<BlueprintNode> Nodes { get; } = new();
    public List<BlueprintLink> Links { get; } = new();

    public void Connect(string fromNode, string fromPin, string toNode, string toPin)
    {
        if (Nodes.FindIndex(n => n.Id.Equals(fromNode, StringComparison.Ordinal)) < 0 ||
            Nodes.FindIndex(n => n.Id.Equals(toNode, StringComparison.Ordinal)) < 0)
            throw new InvalidOperationException("Both Blueprint nodes must exist before connecting them.");

        Links.Add(new BlueprintLink(fromNode, fromPin, toNode, toPin));
    }
}

public sealed class BlueprintEditorPanel : IEditorPanel
{
    public string Id => "blueprint";
    public string Title => "Blueprint Graph";
    public BlueprintGraph Graph { get; } = new();
}
