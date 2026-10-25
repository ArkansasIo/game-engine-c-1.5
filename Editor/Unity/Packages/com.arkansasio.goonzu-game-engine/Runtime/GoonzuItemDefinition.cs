using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine;

public enum GoonzuItemCategory
{
    Material,
    Consumable,
    Weapon,
    Armor,
    Tool,
    Quest,
    Currency
}

[CreateAssetMenu(menuName = "Goonzu/C 1.5 Item Definition", fileName = "GoonzuItem")]
public sealed class GoonzuItemDefinition : ScriptableObject
{
    [SerializeField] private string itemId = string.Empty;
    [SerializeField] private string displayName = string.Empty;
    [SerializeField] private GoonzuItemCategory category = GoonzuItemCategory.Material;
    [SerializeField] private bool stackable = true;
    [SerializeField, Min(1)] private int maxStack = 99;
    [SerializeField, Min(0f)] private float weight;
    [SerializeField, Min(0)] private long baseValue;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public GoonzuItemCategory Category => category;
    public bool Stackable => stackable;
    public int MaxStack => stackable ? Mathf.Max(1, maxStack) : 1;
    public float Weight => Mathf.Max(0f, weight);
    public long BaseValue => System.Math.Max(0, baseValue);

    public bool IsValid(out string error)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            error = "Item ID is required.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(displayName))
        {
            error = $"Item '{itemId}' is missing a display name.";
            return false;
        }
        if (stackable && maxStack <= 0)
        {
            error = $"Item '{itemId}' must have a positive max stack.";
            return false;
        }
        error = string.Empty;
        return true;
    }
}
