namespace GameEngine.V15.Dialogue;

public sealed record DialogueChoice(string Text,string NextNodeId);
public sealed class DialogueNode
{
    public required string Id {get;init;}
    public required string Speaker {get;init;}
    public required string Text {get;init;}
    public List<DialogueChoice> Choices {get;}=new();
}
public sealed class DialogueGraph
{
    private readonly Dictionary<string,DialogueNode> _nodes=new();
    public void Add(DialogueNode node)=>_nodes[node.Id]=node;
    public DialogueNode? Get(string id)=>_nodes.TryGetValue(id,out var n)?n:null;
}
