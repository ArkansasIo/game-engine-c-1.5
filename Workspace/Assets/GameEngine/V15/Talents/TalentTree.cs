namespace GameEngine.V15.Talents;

public sealed record TalentNode(string Id,string Name,int MaxRank=1);
public sealed class TalentTree
{
    private readonly Dictionary<string,int> _ranks=new();
    public bool Unlock(TalentNode node){var rank=_ranks.GetValueOrDefault(node.Id);if(rank>=node.MaxRank)return false;_ranks[node.Id]=rank+1;return true;}
    public int Rank(string id)=>_ranks.GetValueOrDefault(id);
}
