namespace GoonzuUI.Inventory
{
    [System.Serializable]
    public struct ItemStack
    {
        public ItemDef def;
        public int count;

        public bool IsEmpty => def == null || count <= 0;
    }
}
