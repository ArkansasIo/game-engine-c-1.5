using UnityEngine;

namespace GoonzuGame.Monsters
{
    [System.Serializable]
    public class Monster : MonoBehaviour
    {
        public int MonsterId;
        public string Name;
        public int Level;
        public int Health;
        public int MaxHealth;
        public int AttackPower;
        public int Defense;
        public GoonzuGame.Items.Item Loot;

        public Monster() {}

        public Monster(int id, string name, int level, int health, int attack, int defense)
        {
            MonsterId = id;
            Name = name;
            Level = level;
            MaxHealth = health;
            Health = MaxHealth;
            AttackPower = attack;
            Defense = defense;
        }

        public void Attack(GoonzuGame.Characters.Character target)
        {
            int damage = Mathf.Max(1, AttackPower - target.BaseStats.Constitution);
            target.TakeDamage(damage);
            Debug.Log($"{Name} attacked {target.Name} for {damage} damage!");
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log($"{Name} died!");
            DropLoot();
            // Remove from scene
            Destroy(gameObject);
        }

        public void DropLoot()
        {
            if (Loot != null)
            {
                // Drop item at position
                Debug.Log($"{Name} dropped {Loot.Name}");
                // In real implementation, instantiate item pickup
            }
        }

        public void Initialize(int id, string name, int level, int health, int attack, int defense)
        {
            MonsterId = id;
            Name = name;
            Level = level;
            MaxHealth = health;
            Health = MaxHealth;
            AttackPower = attack;
            Defense = defense;
        }
    }

    public class MonsterManager : MonoBehaviour
    {
        public static MonsterManager Instance { get; private set; }

        public List<Monster> MonsterPrefabs = new List<Monster>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SpawnMonster(string monsterName, Vector3 position)
        {
            var prefab = MonsterPrefabs.Find(m => m.Name == monsterName);
            if (prefab != null)
            {
                Instantiate(prefab, position, Quaternion.identity);
                Debug.Log($"Spawned {monsterName} at {position}");
            }
        }

        public void SpawnRandomMonster(Vector3 position)
        {
            if (MonsterPrefabs.Count > 0)
            {
                var randomMonster = MonsterPrefabs[Random.Range(0, MonsterPrefabs.Count)];
                Instantiate(randomMonster, position, Quaternion.identity);
            }
        }
    }
}
