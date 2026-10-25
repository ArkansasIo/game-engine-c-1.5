using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame
{
    [System.Serializable]
    public class CharacterStats
    {
        public int level = 1;
        public int experience = 0;
        public int experienceToNext = 100;

        // Base stats
        public int maxHealth = 100;
        public int currentHealth = 100;
        public int maxMana = 50;
        public int currentMana = 50;
        public int strength = 10;
        public int dexterity = 10;
        public int intelligence = 10;
        public int wisdom = 10;
        public int constitution = 10;
        public int charisma = 10;

        // Derived stats
        public int attackPower { get { return strength * 2; } }
        public int magicPower { get { return intelligence * 2; } }
        public int defense { get { return constitution; } }
        public int speed { get { return dexterity; } }

        // Combat stats
        public float attackSpeed = 1.0f;
        public float movementSpeed = 5.0f;
        public float criticalChance = 0.05f;
        public float criticalMultiplier = 1.5f;

        public void LevelUp()
        {
            level++;
            experience -= experienceToNext;
            experienceToNext = level * 100;

            // Increase base stats based on class
            maxHealth += 10;
            currentHealth = maxHealth;
            maxMana += 5;
            currentMana = maxMana;
        }

        public void TakeDamage(int damage)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
        }

        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }

        public void RestoreMana(int amount)
        {
            currentMana = Mathf.Min(maxMana, currentMana + amount);
        }

        public bool IsDead()
        {
            return currentHealth <= 0;
        }
    }

    public enum CharacterRace
    {
        Human,
        Elf,
        Dwarf,
        Orc,
        Halfling,
        Gnome,
        HalfElf,
        Tiefling
    }

    public enum CharacterClass
    {
        Warrior,
        Mage,
        Rogue,
        Cleric,
        Paladin,
        Ranger,
        Bard,
        Necromancer,
        Druid,
        Monk
    }

    public enum CharacterGender
    {
        Male,
        Female
    }

    [RequireComponent(typeof(GoonzuSpriteAnimator))]
    public class GoonzuCharacter : MonoBehaviour
    {
        [Header("Character Identity")]
        public string characterName;
        public CharacterRace race;
        public CharacterClass characterClass;
        public CharacterGender gender;

        [Header("Character Stats")]
        public CharacterStats stats = new CharacterStats();

        [Header("Equipment")]
        public GoonzuItem weapon;
        public GoonzuItem armor;
        public List<GoonzuItem> inventory = new List<GoonzuItem>();

        [Header("State")]
        public bool isMoving = false;
        public bool isAttacking = false;
        public bool isCasting = false;
        public Vector3 targetPosition;

        private GoonzuSpriteAnimator animator;
        private Rigidbody2D rb;
        private string assetName;

        void Awake()
        {
            animator = GetComponent<GoonzuSpriteAnimator>();
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0; // Top-down game
                rb.freezeRotation = true;
            }
        }

        void Start()
        {
            InitializeCharacter();
        }

        void Update()
        {
            UpdateMovement();
            UpdateAnimations();
        }

        void InitializeCharacter()
        {
            // Generate asset name based on race, gender, and class
            assetName = $"{race.ToString().ToLower()}_{gender.ToString().ToLower()}_{characterClass.ToString().ToLower()}";

            // Load character animations
            animator.LoadCharacterAnimations(assetName);

            // Apply racial bonuses
            ApplyRacialBonuses();

            // Apply class bonuses
            ApplyClassBonuses();

            // Set default animation
            animator.PlayAnimation($"{assetName}_idle");
        }

        void ApplyRacialBonuses()
        {
            switch (race)
            {
                case CharacterRace.Human:
                    // Humans get +1 to all stats
                    stats.strength += 1;
                    stats.dexterity += 1;
                    stats.intelligence += 1;
                    stats.wisdom += 1;
                    stats.constitution += 1;
                    stats.charisma += 1;
                    break;
                case CharacterRace.Elf:
                    stats.dexterity += 3;
                    stats.intelligence += 2;
                    stats.movementSpeed += 1;
                    break;
                case CharacterRace.Dwarf:
                    stats.constitution += 3;
                    stats.strength += 2;
                    stats.movementSpeed -= 0.5f;
                    break;
                case CharacterRace.Orc:
                    stats.strength += 3;
                    stats.constitution += 2;
                    stats.intelligence -= 1;
                    break;
                case CharacterRace.Halfling:
                    stats.dexterity += 2;
                    stats.charisma += 2;
                    stats.strength -= 1;
                    break;
                case CharacterRace.Gnome:
                    stats.intelligence += 2;
                    stats.dexterity += 1;
                    stats.constitution -= 1;
                    break;
                case CharacterRace.HalfElf:
                    stats.charisma += 2;
                    stats.dexterity += 1;
                    stats.intelligence += 1;
                    break;
                case CharacterRace.Tiefling:
                    stats.intelligence += 1;
                    stats.charisma += 2;
                    stats.wisdom -= 1;
                    break;
            }
        }

        void ApplyClassBonuses()
        {
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    stats.strength += 4;
                    stats.constitution += 3;
                    stats.maxHealth += 20;
                    stats.currentHealth = stats.maxHealth;
                    break;
                case CharacterClass.Mage:
                    stats.intelligence += 4;
                    stats.wisdom += 2;
                    stats.maxMana += 30;
                    stats.currentMana = stats.maxMana;
                    break;
                case CharacterClass.Rogue:
                    stats.dexterity += 4;
                    stats.speed += 2;
                    stats.criticalChance += 0.1f;
                    break;
                case CharacterClass.Cleric:
                    stats.wisdom += 4;
                    stats.constitution += 2;
                    stats.maxMana += 20;
                    stats.currentMana = stats.maxMana;
                    break;
                case CharacterClass.Paladin:
                    stats.strength += 3;
                    stats.charisma += 2;
                    stats.constitution += 2;
                    stats.maxHealth += 15;
                    stats.currentHealth = stats.maxHealth;
                    break;
                case CharacterClass.Ranger:
                    stats.dexterity += 3;
                    stats.wisdom += 2;
                    stats.movementSpeed += 1;
                    break;
                case CharacterClass.Bard:
                    stats.charisma += 4;
                    stats.dexterity += 2;
                    stats.maxMana += 15;
                    stats.currentMana = stats.maxMana;
                    break;
                case CharacterClass.Necromancer:
                    stats.intelligence += 3;
                    stats.wisdom += 3;
                    stats.constitution -= 1;
                    stats.maxMana += 25;
                    stats.currentMana = stats.maxMana;
                    break;
                case CharacterClass.Druid:
                    stats.wisdom += 3;
                    stats.constitution += 2;
                    stats.intelligence += 1;
                    stats.maxMana += 20;
                    stats.currentMana = stats.maxMana;
                    break;
                case CharacterClass.Monk:
                    stats.dexterity += 3;
                    stats.wisdom += 2;
                    stats.constitution += 2;
                    stats.speed += 1;
                    break;
            }
        }

        void UpdateMovement()
        {
            if (isMoving && Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                Vector3 direction = (targetPosition - transform.position).normalized;
                rb.MovePosition(transform.position + direction * stats.movementSpeed * Time.deltaTime);

                // Flip sprite based on movement direction
                if (direction.x > 0)
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                else if (direction.x < 0)
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                isMoving = false;
            }
        }

        void UpdateAnimations()
        {
            if (stats.IsDead())
            {
                if (!animator.IsPlaying($"{assetName}_death"))
                {
                    animator.PlayAnimation($"{assetName}_death");
                }
                return;
            }

            if (isAttacking)
            {
                if (!animator.IsPlaying($"{assetName}_attack"))
                {
                    animator.PlayAnimation($"{assetName}_attack", () => { isAttacking = false; });
                }
            }
            else if (isCasting)
            {
                if (!animator.IsPlaying($"{assetName}_cast"))
                {
                    animator.PlayAnimation($"{assetName}_cast", () => { isCasting = false; });
                }
            }
            else if (isMoving)
            {
                if (!animator.IsPlaying($"{assetName}_walk"))
                {
                    animator.PlayAnimation($"{assetName}_walk");
                }
            }
            else
            {
                if (!animator.IsPlaying($"{assetName}_idle"))
                {
                    animator.PlayAnimation($"{assetName}_idle");
                }
            }
        }

        // Public API methods
        public void MoveTo(Vector3 position)
        {
            targetPosition = position;
            isMoving = true;
        }

        public void Attack()
        {
            if (!isAttacking && !isCasting && !stats.IsDead())
            {
                isAttacking = true;
            }
        }

        public void CastSpell()
        {
            if (!isAttacking && !isCasting && !stats.IsDead() && stats.currentMana > 10)
            {
                isCasting = true;
                stats.currentMana -= 10;
            }
        }

        public void TakeDamage(int damage)
        {
            stats.TakeDamage(damage);

            if (!stats.IsDead() && !animator.IsPlaying($"{assetName}_hurt"))
            {
                animator.PlayAnimation($"{assetName}_hurt");
            }
        }

        public void Heal(int amount)
        {
            stats.Heal(amount);
        }

        public void GainExperience(int exp)
        {
            stats.experience += exp;
            if (stats.experience >= stats.experienceToNext)
            {
                stats.LevelUp();
                Debug.Log($"{characterName} leveled up to {stats.level}!");
            }
        }

        public void EquipWeapon(GoonzuItem newWeapon)
        {
            if (newWeapon != null && newWeapon.itemType == GoonzuItem.ItemType.Weapon)
            {
                weapon = newWeapon;
                // Apply weapon bonuses to stats
                stats.attackPower += (int)newWeapon.GetProperty("damage", 0);
            }
        }

        public void EquipArmor(GoonzuItem newArmor)
        {
            if (newArmor != null && newArmor.itemType == GoonzuItem.ItemType.Armor)
            {
                armor = newArmor;
                // Apply armor bonuses to stats
                stats.defense += (int)newArmor.GetProperty("defense", 0);
            }
        }

        public void AddToInventory(GoonzuItem item)
        {
            inventory.Add(item);
        }

        public bool RemoveFromInventory(GoonzuItem item)
        {
            return inventory.Remove(item);
        }

        public string GetAssetName()
        {
            return assetName;
        }
    }
}