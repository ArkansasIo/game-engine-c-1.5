using System;

namespace GoonzuGame.Skills
{
    [System.Serializable]
    public enum SkillType
    {
        Offensive,
        Defensive,
        Utility,
        Passive
    }

    public class Skill
    {
        public string Name;
        public string Description;
        public SkillType Type;
        public int Level;
        public int MaxLevel;
        public int ManaCost;
        public float Cooldown;
        public float LastUsedTime;

        public Skill(string name, string description, SkillType type, int manaCost, float cooldown, int maxLevel = 5)
        {
            Name = name;
            Description = description;
            Type = type;
            Level = 1;
            MaxLevel = maxLevel;
            ManaCost = manaCost;
            Cooldown = cooldown;
            LastUsedTime = Cooldown; // Ready to use
        }

        public bool CanUse(GoonzuGame.Characters.Character character)
        {
            return character.Mana >= ManaCost && LastUsedTime >= Cooldown;
        }

        public virtual void Use(GoonzuGame.Characters.Character caster, GoonzuGame.Characters.Character target = null)
        {
            if (!CanUse(caster)) return;

            caster.Mana -= ManaCost;
            LastUsedTime = 0;

            Console.WriteLine($"{caster.Name} used {Name}!");
            // Specific skill effects implemented in subclasses
        }

        public void LevelUp()
        {
            if (Level < MaxLevel)
            {
                Level++;
                ManaCost += 5; // Example scaling
                Console.WriteLine($"{Name} leveled up to {Level}!");
            }
        }

        public void UpdateCooldown(float deltaTime)
        {
            if (LastUsedTime < Cooldown)
            {
                LastUsedTime += deltaTime;
            }
        }
    }

    public class OffensiveSkill : Skill
    {
        public int Damage;

        public OffensiveSkill(string name, string description, int damage, int manaCost, float cooldown)
            : base(name, description, SkillType.Offensive, manaCost, cooldown)
        {
            Damage = damage;
        }

        public override void Use(GoonzuGame.Characters.Character caster, GoonzuGame.Characters.Character target = null)
        {
            base.Use(caster, target);
            if (target != null)
            {
                int totalDamage = Damage + (caster.BaseStats.Strength - 10);
                target.TakeDamage(totalDamage);
                Console.WriteLine($"{Name} dealt {totalDamage} damage to {target.Name}!");
            }
        }
    }

    public class DefensiveSkill : Skill
    {
        public int HealAmount;

        public DefensiveSkill(string name, string description, int healAmount, int manaCost, float cooldown)
            : base(name, description, SkillType.Defensive, manaCost, cooldown)
        {
            HealAmount = healAmount;
        }

        public override void Use(GoonzuGame.Characters.Character caster, GoonzuGame.Characters.Character target = null)
        {
            base.Use(caster, target);
            var healTarget = target ?? caster;
            healTarget.Health = Math.Min(healTarget.Health + HealAmount, healTarget.MaxHealth);
            Console.WriteLine($"{Name} healed {healTarget.Name} for {HealAmount} HP!");
        }
    }

    public class UtilitySkill : Skill
    {
        public UtilitySkill(string name, string description, int manaCost, float cooldown)
            : base(name, description, SkillType.Utility, manaCost, cooldown)
        {
        }

        // Implement specific utility effects
    }

    public class PassiveSkill : Skill
    {
        public PassiveSkill(string name, string description)
            : base(name, description, SkillType.Passive, 0, 0)
        {
        }

        // Passive effects applied automatically
    }
}
