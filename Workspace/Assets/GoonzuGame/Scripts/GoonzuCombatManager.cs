using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame
{
    public enum CombatState
    {
        Idle,
        Attacking,
        Defending,
        Casting,
        Dead
    }

    [System.Serializable]
    public class CombatAction
    {
        public string actionName;
        public CombatActionType actionType;
        public int damage = 0;
        public int manaCost = 0;
        public float cooldown = 0f;
        public float range = 1f;
        public string animationTrigger;
        public System.Action<GoonzuCharacter, GoonzuCharacter> executeAction;

        public enum CombatActionType
        {
            Attack,
            Spell,
            Ability,
            Item
        }

        public CombatAction(string name, CombatActionType type, int dmg = 0, int mana = 0, float cd = 0f)
        {
            actionName = name;
            actionType = type;
            damage = dmg;
            manaCost = mana;
            cooldown = cd;
        }
    }

    public class GoonzuCombatManager : MonoBehaviour
    {
        private static GoonzuCombatManager instance;
        public static GoonzuCombatManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GoonzuCombatManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GoonzuCombatManager");
                        instance = go.AddComponent<GoonzuCombatManager>();
                    }
                }
                return instance;
            }
        }

        private List<GoonzuCharacter> combatants = new List<GoonzuCharacter>();
        private Dictionary<GoonzuCharacter, CombatState> combatStates = new Dictionary<GoonzuCharacter, CombatState>();
        private Dictionary<GoonzuCharacter, float> actionCooldowns = new Dictionary<GoonzuCharacter, float>();

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            UpdateCooldowns();
        }

        void UpdateCooldowns()
        {
            List<GoonzuCharacter> keys = new List<GoonzuCharacter>(actionCooldowns.Keys);
            foreach (var character in keys)
            {
                if (actionCooldowns[character] > 0)
                {
                    actionCooldowns[character] -= Time.deltaTime;
                    if (actionCooldowns[character] <= 0)
                    {
                        actionCooldowns[character] = 0;
                    }
                }
            }
        }

        // Public API methods
        public void EnterCombat(GoonzuCharacter character)
        {
            if (!combatants.Contains(character))
            {
                combatants.Add(character);
                combatStates[character] = CombatState.Idle;
                actionCooldowns[character] = 0f;
                Debug.Log($"{character.characterName} entered combat");
            }
        }

        public void ExitCombat(GoonzuCharacter character)
        {
            if (combatants.Contains(character))
            {
                combatants.Remove(character);
                combatStates.Remove(character);
                actionCooldowns.Remove(character);
                Debug.Log($"{character.characterName} exited combat");
            }
        }

        public bool IsInCombat(GoonzuCharacter character)
        {
            return combatants.Contains(character);
        }

        public void PerformAttack(GoonzuCharacter attacker, GoonzuCharacter target)
        {
            if (!CanPerformAction(attacker, CombatState.Attacking)) return;
            if (!IsValidTarget(attacker, target)) return;

            combatStates[attacker] = CombatState.Attacking;

            // Calculate damage
            int baseDamage = attacker.stats.attackPower;
            if (attacker.weapon != null)
            {
                baseDamage += (int)attacker.weapon.GetProperty("damage", 0);
            }

            // Critical hit check
            bool isCritical = Random.value < attacker.stats.criticalChance;
            if (isCritical)
            {
                baseDamage = Mathf.RoundToInt(baseDamage * attacker.stats.criticalMultiplier);
            }

            // Defense calculation
            int defense = target.stats.defense;
            if (target.armor != null)
            {
                defense += (int)target.armor.GetProperty("defense", 0);
            }

            int finalDamage = Mathf.Max(1, baseDamage - defense);

            // Apply damage
            target.TakeDamage(finalDamage);

            // Experience gain
            if (target.stats.IsDead())
            {
                attacker.GainExperience(target.stats.level * 10);
            }
            else
            {
                attacker.GainExperience(5);
            }

            // Trigger animations
            attacker.Attack();
            if (isCritical)
            {
                // Play critical hit effect
                PlayEffect("critical_hit", attacker.transform.position);
            }

            // Reset combat state after animation
            StartCoroutine(ResetCombatState(attacker, 0.5f));

            Debug.Log($"{attacker.characterName} attacked {target.characterName} for {finalDamage} damage" +
                     (isCritical ? " (CRITICAL!)" : ""));
        }

        public void PerformSpell(GoonzuCharacter caster, GoonzuCharacter target, string spellName)
        {
            if (!CanPerformAction(caster, CombatState.Casting)) return;
            if (!IsValidTarget(caster, target)) return;
            if (caster.stats.currentMana < 10) return; // Basic mana check

            combatStates[caster] = CombatState.Casting;

            // Spell effects based on class
            int spellDamage = 0;
            string effectName = "";

            switch (caster.characterClass)
            {
                case CharacterClass.Mage:
                    spellDamage = caster.stats.magicPower;
                    effectName = "fire_spell";
                    break;
                case CharacterClass.Cleric:
                    spellDamage = caster.stats.magicPower / 2;
                    effectName = "heal_spell";
                    // Healing spell
                    target.Heal(spellDamage);
                    spellDamage = 0;
                    break;
                case CharacterClass.Necromancer:
                    spellDamage = caster.stats.magicPower;
                    effectName = "poison_cloud";
                    break;
                case CharacterClass.Druid:
                    spellDamage = caster.stats.magicPower;
                    effectName = "lightning_spell";
                    break;
            }

            caster.stats.currentMana -= 10;

            if (spellDamage > 0)
            {
                target.TakeDamage(spellDamage);
            }

            // Play spell effect
            PlayEffect(effectName, target.transform.position);

            // Trigger casting animation
            caster.CastSpell();

            // Reset combat state after animation
            StartCoroutine(ResetCombatState(caster, 1f));

            Debug.Log($"{caster.characterName} cast {spellName} on {target.characterName}" +
                     (spellDamage > 0 ? $" for {spellDamage} damage" : " (healing)"));
        }

        public void UseItem(GoonzuCharacter user, GoonzuCharacter target, GoonzuItem item)
        {
            if (item.itemType != GoonzuItem.ItemType.Consumable) return;
            if (!user.inventory.Contains(item)) return;

            string effect = (string)item.GetProperty("effect", "");
            int power = (int)item.GetProperty("power", 0);

            switch (effect)
            {
                case "heal":
                    target.Heal(power);
                    PlayEffect("heal_spell", target.transform.position);
                    break;
                case "restore_mana":
                    target.stats.RestoreMana(power);
                    PlayEffect("sparkle_effect", target.transform.position);
                    break;
                case "buff_strength":
                    // Temporary buff (would need buff system)
                    PlayEffect("buff_aura", target.transform.position);
                    break;
            }

            // Remove item from inventory
            user.RemoveFromInventory(item);

            Debug.Log($"{user.characterName} used {item.itemName} on {target.characterName}");
        }

        bool CanPerformAction(GoonzuCharacter character, CombatState requiredState)
        {
            if (!combatStates.ContainsKey(character)) return false;
            if (character.stats.IsDead()) return false;
            if (actionCooldowns.ContainsKey(character) && actionCooldowns[character] > 0) return false;

            return combatStates[character] == CombatState.Idle;
        }

        bool IsValidTarget(GoonzuCharacter attacker, GoonzuCharacter target)
        {
            if (target == null || target.stats.IsDead()) return false;

            // Check range (simplified - would need proper distance checking)
            float distance = Vector3.Distance(attacker.transform.position, target.transform.position);
            return distance <= 2f; // Basic melee range
        }

        void PlayEffect(string effectName, Vector3 position)
        {
            // Create temporary effect object
            GameObject effectObj = new GameObject($"Effect_{effectName}");
            effectObj.transform.position = position;

            var animator = effectObj.AddComponent<GoonzuSpriteAnimator>();
            animator.LoadEffectAnimation(effectName);

            // Auto-destroy after animation
            Destroy(effectObj, 2f);
        }

        IEnumerator ResetCombatState(GoonzuCharacter character, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (combatStates.ContainsKey(character))
            {
                combatStates[character] = CombatState.Idle;
            }
        }

        // AI Combat Logic
        public void PerformAIAction(GoonzuCharacter aiCharacter)
        {
            if (!IsInCombat(aiCharacter) || aiCharacter.stats.IsDead()) return;

            // Find nearest enemy (simplified)
            GoonzuCharacter target = FindNearestEnemy(aiCharacter);
            if (target == null) return;

            // Simple AI decision making
            float rand = Random.value;

            if (rand < 0.7f)
            {
                // 70% chance to attack
                PerformAttack(aiCharacter, target);
            }
            else if (rand < 0.9f && aiCharacter.stats.currentMana >= 10)
            {
                // 20% chance to cast spell (if enough mana)
                PerformSpell(aiCharacter, target, "basic_spell");
            }
            else
            {
                // 10% chance to defend or use item
                // For now, just attack
                PerformAttack(aiCharacter, target);
            }
        }

        GoonzuCharacter FindNearestEnemy(GoonzuCharacter character)
        {
            GoonzuCharacter nearest = null;
            float minDistance = float.MaxValue;

            foreach (var combatant in combatants)
            {
                if (combatant != character && !combatant.stats.IsDead())
                {
                    float distance = Vector3.Distance(character.transform.position, combatant.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = combatant;
                    }
                }
            }

            return nearest;
        }

        public List<GoonzuCharacter> GetCombatants()
        {
            return new List<GoonzuCharacter>(combatants);
        }

        public CombatState GetCombatState(GoonzuCharacter character)
        {
            return combatStates.ContainsKey(character) ? combatStates[character] : CombatState.Idle;
        }
    }
}