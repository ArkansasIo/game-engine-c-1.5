namespace GameEngine.V15.RPG;

public enum EquipmentSlot { Head,Chest,Hands,Legs,Feet,MainHand,OffHand,Accessory1,Accessory2 }
public sealed record ItemDefinition(string Id,string Name,int MaxStack=1,float Weight=0);
public sealed class Equipment
{
    private readonly Dictionary<EquipmentSlot,ItemDefinition?> _slots = Enum.GetValues<EquipmentSlot>().ToDictionary(x=>x,x=>(ItemDefinition?)null);
    public bool Equip(EquipmentSlot slot,ItemDefinition item){_slots[slot]=item;return true;}
    public ItemDefinition? Get(EquipmentSlot slot)=>_slots[slot];
}
