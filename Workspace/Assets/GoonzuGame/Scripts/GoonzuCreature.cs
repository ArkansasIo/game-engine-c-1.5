using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame
{
    public enum CreatureType
    {
        Dragon,
        Griffin,
        Phoenix,
        Unicorn,
        WarHorse,
        DireWolf,
        GiantSpider,
        Troll,
        Ogre,
        Goblin,
        OrcWarrior,
        SkeletonWarrior,
        Zombie,
        Vampire,
        Werewolf,
        Minotaur,
        Centaur,
        Harpy,
        Mermaid,
        Golem,
        Elemental,
        Demon,
        Angel,
        Fairy,
        Beast
    }

    public enum NPCType
    {
        Merchant,
        Blacksmith,
        Guard,
        Innkeeper,
        Priest,
        Mayor,
        Farmer,
        Hunter,
        Beggar,
        Noble
    }

    public enum AIBehavior
    {
        Passive,
        Aggressive,
        Defensive,
        Patrol,
        Flee,
        Trader,
        Guard,
        Healer
    }

    [RequireComponent(typeof(GoonzuSpriteAnimator))]
    public class GoonzuCreature : MonoBehaviour
    {
        [Header("Creature Identity")]
        public string creatureName;
        public CreatureType creatureType;
        public AIBehavior aiBehavior = AIBehavior.Passive;

        [Header("Creature Stats")]
        public CharacterStats stats = new CharacterStats();

        [Header("AI Settings")]
        public float detectionRange = 5f;
        public float attackRange = 2f;
        public float patrolSpeed = 2f;
        public float chaseSpeed = 3f;
        public Vector3[] patrolPoints;
        private int currentPatrolIndex = 0;

        [Header("Loot")]
        public List<GoonzuItem> lootTable = new List<GoonzuItem>();
        public int goldDrop = 0;

        private GoonzuSpriteAnimator animator;
        private Rigidbody2D rb;
        private GoonzuCharacter target;
        private string assetName;
        private Vector3 startPosition;
        private bool isAlerted = false;

        void Awake()
        {
            animator = GetComponent<GoonzuSpriteAnimator>();
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0;
                rb.freezeRotation = true;
            }
        }

        void Start()
        {
            InitializeCreature();
            startPosition = transform.position;
        }

        void Update()
        {
            UpdateAI();
            UpdateAnimations();
        }

        void InitializeCreature()
        {
            assetName = creatureType.ToString().ToLower();

            // Load creature animations
            animator.LoadCharacterAnimations(assetName);

            // Apply creature-specific stats
            ApplyCreatureStats();

            // Initialize loot table
            GenerateLootTable();

            // Set default animation
            animator.PlayAnimation($"{assetName}_idle");

            // Enter combat system
            GoonzuCombatManager.Instance.EnterCombat(this as GoonzuCharacter);
        }

        void ApplyCreatureStats()
        {
            // Base stats for creatures
            stats.maxHealth = 50;
            stats.currentHealth = 50;
            stats.maxMana = 0;
            stats.currentMana = 0;
            stats.strength = 8;
            stats.dexterity = 6;
            stats.intelligence = 4;
            stats.wisdom = 4;
            stats.constitution = 8;
            stats.charisma = 2;

            // Modify stats based on creature type
            switch (creatureType)
            {
                case CreatureType.Dragon:
                    stats.maxHealth = 500;
                    stats.currentHealth = 500;
                    stats.strength = 25;
                    stats.defense = 15;
                    stats.level = 20;
                    goldDrop = 1000;
                    break;

                case CreatureType.Goblin:
                    stats.maxHealth = 30;
                    stats.currentHealth = 30;
                    stats.strength = 6;
                    stats.defense = 2;
                    stats.level = 2;
                    goldDrop = 5;
                    break;

                case CreatureType.OrcWarrior:
                    stats.maxHealth = 80;
                    stats.currentHealth = 80;
                    stats.strength = 12;
                    stats.defense = 8;
                    stats.level = 5;
                    goldDrop = 25;
                    break;

                case CreatureType.SkeletonWarrior:
                    stats.maxHealth = 40;
                    stats.currentHealth = 40;
                    stats.strength = 8;
                    stats.defense = 5;
                    stats.level = 3;
                    goldDrop = 10;
                    break;

                case CreatureType.Troll:
                    stats.maxHealth = 150;
                    stats.currentHealth = 150;
                    stats.strength = 18;
                    stats.constitution = 15;
                    stats.level = 8;
                    goldDrop = 50;
                    break;

                case CreatureType.WarHorse:
                    stats.maxHealth = 60;
                    stats.currentHealth = 60;
                    stats.strength = 10;
                    stats.movementSpeed = 6f;
                    stats.level = 1;
                    goldDrop = 0;
                    break;

                case CreatureType.Vampire:
                    stats.maxHealth = 100;
                    stats.currentHealth = 100;
                    stats.strength = 15;
                    stats.dexterity = 12;
                    stats.level = 10;
                    goldDrop = 75;
                    break;

                case CreatureType.Werewolf:
                    stats.maxHealth = 90;
                    stats.currentHealth = 90;
                    stats.strength = 16;
                    stats.dexterity = 10;
                    stats.level = 7;
                    goldDrop = 40;
                    break;

                default:
                    // Default creature stats
                    stats.level = 3;
                    goldDrop = 15;
                    break;
            }
        }

        void GenerateLootTable()
        {
            // Generate loot based on creature level and type
            int level = stats.level;

            // Common drops
            if (Random.value < 0.3f)
            {
                GoonzuItem loot = GoonzuItemManager.Instance.GenerateRandomItem(level, GoonzuItem.ItemType.Material);
                if (loot != null) lootTable.Add(loot);
            }

            // Weapon drops for combat creatures
            if ((creatureType == CreatureType.OrcWarrior || creatureType == CreatureType.SkeletonWarrior ||
                 creatureType == CreatureType.Troll) && Random.value < 0.1f)
            {
                GoonzuItem weapon = GoonzuItemManager.Instance.GenerateRandomItem(level, GoonzuItem.ItemType.Weapon);
                if (weapon != null) lootTable.Add(weapon);
            }

            // Rare drops for high-level creatures
            if (level >= 10 && Random.value < 0.05f)
            {
                GoonzuItem rareItem = GoonzuItemManager.Instance.GenerateRandomItem(level, null);
                rareItem.rarity = GoonzuItem.ItemRarity.Rare;
                lootTable.Add(rareItem);
            }
        }

        void UpdateAI()
        {
            if (stats.IsDead()) return;

            // Find nearby players
            GoonzuCharacter nearestPlayer = FindNearestPlayer();

            switch (aiBehavior)
            {
                case AIBehavior.Passive:
                    // Do nothing unless attacked
                    if (isAlerted && nearestPlayer != null)
                    {
                        aiBehavior = AIBehavior.Defensive;
                    }
                    break;

                case AIBehavior.Aggressive:
                    if (nearestPlayer != null)
                    {
                        float distance = Vector3.Distance(transform.position, nearestPlayer.transform.position);

                        if (distance <= attackRange)
                        {
                            // Attack
                            GoonzuCombatManager.Instance.PerformAttack(this as GoonzuCharacter, nearestPlayer);
                        }
                        else if (distance <= detectionRange)
                        {
                            // Chase
                            MoveTowards(nearestPlayer.transform.position, chaseSpeed);
                        }
                    }
                    break;

                case AIBehavior.Defensive:
                    if (nearestPlayer != null)
                    {
                        float distance = Vector3.Distance(transform.position, nearestPlayer.transform.position);

                        if (distance <= attackRange)
                        {
                            // Attack when threatened
                            GoonzuCombatManager.Instance.PerformAttack(this as GoonzuCharacter, nearestPlayer);
                        }
                        else if (distance > detectionRange * 1.5f)
                        {
                            // Return to passive
                            aiBehavior = AIBehavior.Passive;
                            isAlerted = false;
                        }
                    }
                    break;

                case AIBehavior.Patrol:
                    Patrol();
                    break;

                case AIBehavior.Flee:
                    if (nearestPlayer != null)
                    {
                        FleeFrom(nearestPlayer.transform.position);
                    }
                    break;
            }
        }

        void Patrol()
        {
            if (patrolPoints.Length == 0) return;

            Vector3 targetPoint = patrolPoints[currentPatrolIndex];
            float distance = Vector3.Distance(transform.position, targetPoint);

            if (distance < 0.5f)
            {
                // Move to next patrol point
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                targetPoint = patrolPoints[currentPatrolIndex];
            }

            MoveTowards(targetPoint, patrolSpeed);
        }

        void MoveTowards(Vector3 position, float speed)
        {
            Vector3 direction = (position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * speed * Time.deltaTime);

            // Flip sprite based on movement direction
            if (direction.x > 0)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        void FleeFrom(Vector3 threatPosition)
        {
            Vector3 fleeDirection = (transform.position - threatPosition).normalized;
            Vector3 fleePosition = transform.position + fleeDirection * 3f;
            MoveTowards(fleePosition, chaseSpeed);
        }

        GoonzuCharacter FindNearestPlayer()
        {
            // Find all players in scene (simplified - would need proper player tagging)
            GoonzuCharacter[] allCharacters = FindObjectsOfType<GoonzuCharacter>();
            GoonzuCharacter nearest = null;
            float minDistance = detectionRange;

            foreach (var character in allCharacters)
            {
                if (character != this && !character.stats.IsDead())
                {
                    float distance = Vector3.Distance(transform.position, character.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = character;
                    }
                }
            }

            return nearest;
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

            // Determine animation based on movement and state
            bool isMoving = rb.velocity.magnitude > 0.1f;

            if (isMoving)
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

        // Combat interface (implements GoonzuCharacter for combat system)
        public void TakeDamage(int damage)
        {
            stats.TakeDamage(damage);
            isAlerted = true;

            if (!stats.IsDead())
            {
                animator.PlayAnimation($"{assetName}_hurt");
            }
        }

        public void Attack()
        {
            if (!animator.IsPlaying($"{assetName}_attack"))
            {
                animator.PlayAnimation($"{assetName}_attack");
            }
        }

        public void CastSpell()
        {
            // Creatures don't cast spells by default
        }

        public void MoveTo(Vector3 position)
        {
            // Handled by AI
        }

        public void GainExperience(int exp)
        {
            // Creatures don't gain experience
        }

        public void Heal(int amount)
        {
            stats.Heal(amount);
        }

        public void AddToInventory(GoonzuItem item)
        {
            lootTable.Add(item);
        }

        public bool RemoveFromInventory(GoonzuItem item)
        {
            return lootTable.Remove(item);
        }

        // Public API methods
        public void Alert()
        {
            isAlerted = true;
        }

        public void SetAIBehavior(AIBehavior newBehavior)
        {
            aiBehavior = newBehavior;
        }

        public void SetPatrolPoints(Vector3[] points)
        {
            patrolPoints = points;
        }

        public List<GoonzuItem> GetLoot()
        {
            return new List<GoonzuItem>(lootTable);
        }

        public int GetGoldDrop()
        {
            return goldDrop;
        }

        public void OnDeath()
        {
            // Drop loot
            foreach (var item in lootTable)
            {
                // Create item pickup at location
                Debug.Log($"Dropped: {item.itemName}");
            }

            // Disable creature
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            GoonzuCombatManager.Instance.ExitCombat(this as GoonzuCharacter);
        }
    }
}