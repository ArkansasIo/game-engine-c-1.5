using System.Collections.Generic;
using System;

namespace GoonzuGame.AI
{
    [System.Serializable]
    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Flee,
        Dead
    }

    public class AIController
    {
        public AIState CurrentState = AIState.Idle;
        public GoonzuGame.Characters.Character AICharacter;
        public GoonzuGame.Characters.Character Target;
        public float DetectionRange = 10f;
        public float AttackRange = 2f;
        public float PatrolSpeed = 2f;
        public float ChaseSpeed = 4f;

        private Vector3 patrolPoint;
        private float stateTimer;
        private System.Random random = new System.Random();

        public AIController(GoonzuGame.Characters.Character aiCharacter)
        {
            AICharacter = aiCharacter;
            SetPatrolPoint();
        }

        public void Update(float deltaTime)
        {
            switch (CurrentState)
            {
                case AIState.Idle:
                    IdleBehavior(deltaTime);
                    break;
                case AIState.Patrol:
                    PatrolBehavior(deltaTime);
                    break;
                case AIState.Chase:
                    ChaseBehavior(deltaTime);
                    break;
                case AIState.Attack:
                    AttackBehavior(deltaTime);
                    break;
                case AIState.Flee:
                    FleeBehavior(deltaTime);
                    break;
                case AIState.Dead:
                    // Do nothing
                    break;
            }
        }

        private void IdleBehavior(float deltaTime)
        {
            // Look for targets
            Target = FindTarget();
            if (Target != null)
            {
                CurrentState = AIState.Chase;
            }
            else if (random.NextDouble() < 0.01f) // 1% chance per update
            {
                CurrentState = AIState.Patrol;
                SetPatrolPoint();
            }
        }

        private void PatrolBehavior(float deltaTime)
        {
            if (Vector3.Distance(AICharacter.Position, patrolPoint) < 1f)
            {
                CurrentState = AIState.Idle;
            }
            else
            {
                Vector3 direction = (patrolPoint - AICharacter.Position).Normalized();
                AICharacter.Move(direction * PatrolSpeed * deltaTime);
            }

            Target = FindTarget();
            if (Target != null)
            {
                CurrentState = AIState.Chase;
            }
        }

        private void ChaseBehavior(float deltaTime)
        {
            if (Target == null || Target.Health <= 0)
            {
                CurrentState = AIState.Idle;
                return;
            }

            float distance = Vector3.Distance(AICharacter.Position, Target.Position);
            if (distance <= AttackRange)
            {
                CurrentState = AIState.Attack;
            }
            else
            {
                Vector3 direction = (Target.Position - AICharacter.Position).Normalized();
                AICharacter.Move(direction * ChaseSpeed * deltaTime);
            }
        }

        private void AttackBehavior(float deltaTime)
        {
            if (Target == null || Target.Health <= 0)
            {
                CurrentState = AIState.Idle;
                return;
            }

            float distance = Vector3.Distance(AICharacter.Position, Target.Position);
            if (distance > AttackRange)
            {
                CurrentState = AIState.Chase;
            }
            else
            {
                AICharacter.Attack(Target);
                // After attack, maybe go back to chase or idle
                CurrentState = AIState.Chase;
            }
        }

        private void FleeBehavior(float deltaTime)
        {
            // Implement flee logic
            Vector3 fleeDirection = (AICharacter.Position - Target.Position).Normalized();
            AICharacter.Move(fleeDirection * ChaseSpeed * deltaTime);

            if (Vector3.Distance(AICharacter.Position, Target.Position) > DetectionRange * 2)
            {
                CurrentState = AIState.Idle;
            }
        }

        private GoonzuGame.Characters.Character FindTarget()
        {
            // Simple target finding - assume player is available
            var player = GoonzuGame.Characters.CharacterManager.Instance.PlayerCharacter;
            if (player != null && Vector3.Distance(AICharacter.Position, player.Position) <= DetectionRange)
            {
                return player;
            }
            return null;
        }

        private void SetPatrolPoint()
        {
            float x = (float)(random.NextDouble() * 20 - 10);
            float z = (float)(random.NextDouble() * 20 - 10);
            patrolPoint = AICharacter.Position + new Vector3(x, 0, z);
        }
    }

    public class AIManager
    {
        public static AIManager Instance { get; } = new AIManager();

        public List<AIController> AIControllers = new List<AIController>();

        private AIManager() {}

        public void RegisterAI(AIController ai)
        {
            AIControllers.Add(ai);
        }

        public void UnregisterAI(AIController ai)
        {
            AIControllers.Remove(ai);
        }

        public void UpdateAI(float deltaTime)
        {
            foreach (var ai in AIControllers)
            {
                ai.Update(deltaTime);
            }
        }
    }
}
