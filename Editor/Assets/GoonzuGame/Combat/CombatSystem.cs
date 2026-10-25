using System.Collections;
using System;

namespace GoonzuGame.Combat
{
    public enum CombatState
    {
        NotInCombat,
        PlayerTurn,
        EnemyTurn,
        Victory,
        Defeat
    }

    public class CombatSystem
    {
        public static CombatSystem Instance { get; private set; }

        public CombatState State = CombatState.NotInCombat;
        public GoonzuGame.Characters.Character Player;
        public GoonzuGame.Characters.Character Enemy;

        private static CombatSystem _instance;
        public static CombatSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CombatSystem();
                }
                return _instance;
            }
        }

        public void StartCombat(GoonzuGame.Characters.Character player, GoonzuGame.Characters.Character enemy)
        {
            Player = player;
            Enemy = enemy;
            State = CombatState.PlayerTurn;
            Console.WriteLine("Combat started!");
        }

        public void EndCombat(bool playerWon)
        {
            if (playerWon)
            {
                State = CombatState.Victory;
                Player.GainExperience(50); // Example
                GoonzuGame.Achievements.AchievementManager.Instance.TrackProgress("First Kill", 1);
            }
            else
            {
                State = CombatState.Defeat;
                // Handle defeat
            }
            Console.WriteLine(playerWon ? "Player won!" : "Player defeated!");
            State = CombatState.NotInCombat;
        }

        public void PlayerAttack()
        {
            if (State == CombatState.PlayerTurn)
            {
                Player.Attack(Enemy);
                if (Enemy.Health <= 0)
                {
                    EndCombat(true);
                }
                else
                {
                    State = CombatState.EnemyTurn;
                    EnemyTurn();
                }
            }
        }

        private void EnemyTurn()
        {
            // Simulate delay
            System.Threading.Thread.Sleep(1000); // 1 second delay
            Enemy.Attack(Player);
            if (Player.Health <= 0)
            {
                EndCombat(false);
            }
            else
            {
                State = CombatState.PlayerTurn;
            }
        }

        public void PlayerDefend()
        {
            if (State == CombatState.PlayerTurn)
            {
                // Reduce damage or something
                Console.WriteLine("Player defended!");
                State = CombatState.EnemyTurn;
                EnemyTurn();
            }
        }

        public void PlayerUseItem(GoonzuGame.Items.Item item)
        {
            if (State == CombatState.PlayerTurn)
            {
                Player.UseItem(item);
                State = CombatState.EnemyTurn;
                EnemyTurn();
            }
        }
    }
}
