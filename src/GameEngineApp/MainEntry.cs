using System;

namespace GameEngineApp
{
    class MainEntry
    {
        static void Main(string[] args)
                    // Demo: Biome
                    var biome = new GoonzuGame.Biomes.BiomeDef("Forest", "Lush green forest biome");
                    biome.Display();

                    // Demo: City
                    var city = new GoonzuGame.Cities.CityDef("Arkansas City", "A bustling trade hub");
                    city.Display();

                    // Demo: Town
                    var town = new GoonzuGame.Towns.TownDef("Goonzu Town", "A peaceful village");
                    town.Display();

                    // Demo: Dungeon
                    var dungeon = new GoonzuGame.Dungeons.DungeonDef("Goblin Cave", "Dark and dangerous goblin lair");
                    dungeon.Display();

                    // Demo: Trial
                    var trial = new GoonzuGame.Trials.TrialDef("Trial of Fire", "Survive the flames");
                    trial.Display();

                    // Demo: Raid
                    var raid = new GoonzuGame.Raids.RaidDef("Dragon's Lair", "Defeat the mighty dragon");
                    raid.Display();

                    // Demo: Zone
                    var zone = new GoonzuGame.Zones.ZoneDef(
                        "Forest Zone",
                        biome.Name,
                        city.Name,
                        town.Name,
                        new System.Collections.Generic.List<string> { dungeon.Name },
                        new System.Collections.Generic.List<string> { trial.Name },
                        new System.Collections.Generic.List<string> { raid.Name });
                    zone.Display();
        {
            Console.WriteLine("Game Engine App Started!");
            var mainWindow = new GoonzuGame.GUI.GoonzuMainWindow();
            mainWindow.ShowAll();
            var hud = new GoonzuGame.GUI.GoonzuHUD();
            hud.ShowHUD();
            var world = new GoonzuGame.World.GoonzuWorld();
            world.RenderWorld();

            // Demo: Boss
            var boss = new GoonzuGame.Bosses.BossDef(
                "Dragon King", 100, 50000, 1200, 800,
                new System.Collections.Generic.List<string> { "Fire Breath", "Tail Swipe" }, "Fire",
                24, 16, 22, 12, 14, 18);
            boss.DisplayStats();
            Console.WriteLine($"Boss Initiative: {boss.Initiative()}");
            Console.WriteLine($"Boss Attack Roll: {boss.AttackRoll()}");
            Console.WriteLine($"Boss Damage: {boss.DamageRoll(2, 12)}");

            // Demo: Mob
            var mob = new GoonzuGame.Mobs.MobDef(
                "Goblin Scout", 10, 500, 40, 20,
                new System.Collections.Generic.List<string> { "Quick Strike" }, "Forest",
                12, 14, 10, 8, 10, 6);
            mob.DisplayStats();
            Console.WriteLine($"Mob Initiative: {mob.Initiative()}");
            Console.WriteLine($"Mob Attack Roll: {mob.AttackRoll()}");
            Console.WriteLine($"Mob Damage: {mob.DamageRoll(1, 6)}");

            // Demo: Enemy
            var enemy = new GoonzuGame.Enemies.EnemyDef(
                "Undead Warrior", 25, 2000, 120, 60,
                new System.Collections.Generic.List<string> { "Curse", "Shield Bash" }, "Undead",
                16, 10, 14, 8, 12, 6);
            enemy.DisplayStats();
            Console.WriteLine($"Enemy Initiative: {enemy.Initiative()}");
            Console.WriteLine($"Enemy Attack Roll: {enemy.AttackRoll()}");
            Console.WriteLine($"Enemy Damage: {enemy.DamageRoll(1, 8)}");

            // Demo: Item
            var item = new GoonzuGame.Items.ItemDef("Potion of Healing", "Rare", "Restoration", "Heals 50 HP", 100);
            item.Display();
            item.Use();
            item.Equip();
            item.Unequip();
            Console.WriteLine($"Item Value: {item.CalculateValue()}");

            // Demo: Equipment
            var equipment = new GoonzuGame.Equipment.EquipmentDef("Helm of Thunder", "Head", 30, 80, "Epic", "Thunder");
            equipment.Display();
            equipment.Equip();
            equipment.Unequip();
            Console.WriteLine($"Equipment Value: {equipment.CalculateValue()}");

            // Demo: Weapon
            var weapon = new GoonzuGame.Weapons.WeaponDef("Sword of Flames", 50, 10, "Legendary", "Sword", "Sword", "Fire", "Sword");
            weapon.Display();
            weapon.Equip();
            weapon.Unequip();
            Console.WriteLine($"Weapon Attack Roll: {weapon.AttackRoll(7)}");
            Console.WriteLine($"Weapon Damage: {weapon.DamageRoll(2, 6, 7)}");

            // Demo: Armor
            var armor = new GoonzuGame.Armor.ArmorDef("Plate of Light", 60, 100, "Rare", "Plate", "Plate", "Light", "Plate");
            armor.Display();
            armor.Equip();
            armor.Unequip();
            Console.WriteLine($"Armor AC: {armor.CalculateAC(2)}");

            Console.WriteLine("Game Engine App Shutting Down.");
        }
    }
}
