using UnityEngine;
using Item = GoonzuGame.Items.Item;
using GoonzuGame.Items;

namespace GoonzuGame.Weapons
{
    public class WeaponDef : Weapon
    {
        public string PvPType;
        public string PvEType;
        public string Theme;
        public bool IsEquipped;
        public string Proficiency;

        public WeaponDef(string name, int attack, int speed, ItemRarity rarity, string pvpType, string pveType, string theme, string proficiency = "")
            : base(name, $"A powerful {theme} weapon", 100, rarity, attack, speed)
        {
            PvPType = pvpType;
            PvEType = pveType;
            Theme = theme;
            Proficiency = proficiency;
            IsEquipped = false;
        }

        public void Equip()
        {
            IsEquipped = true;
            Debug.Log($"Equipped weapon: {Name}");
        }

        public void Unequip()
        {
            IsEquipped = false;
            Debug.Log($"Unequipped weapon: {Name}");
        }

        public bool CheckProficiency(string characterProficiency)
        {
            return Proficiency == characterProficiency;
        }

        public int AttackRoll(int strMod)
        {
            return RollD20() + strMod;
        }

        public int DamageRoll(int diceCount, int diceType, int strMod)
        {
            int total = 0;
            for (int i = 0; i < diceCount; i++)
                total += RollDice(diceType);
            return total + strMod;
        }

        public int RollD20() => RollDice(20);
        public int RollDice(int sides)
        {
            return Random.Range(1, sides + 1);
        }

        public void Display()
        {
            Debug.Log($"Weapon: {Name}, Attack: {Damage}, Speed: {AttackSpeed}, PvPType: {PvPType}, PvEType: {PvEType}, Theme: {Theme}, Proficiency: {Proficiency}, Equipped: {IsEquipped}");
        }
    }
}