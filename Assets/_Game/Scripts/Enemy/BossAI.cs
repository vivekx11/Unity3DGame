using System.Collections;
using UnityEngine;
using Game.Combat;

namespace Game.Enemy
{
    /// <summary>
    /// Boss AI with two phases. Phase switch at 50% HP.
    /// Editor tips: Configure phase thresholds and assign unique animation triggers for phase moves.
    /// Design notes: This class extends EnemyStateMachine behavior and adds phase logic and cooldowns.
    /// </summary>
    [RequireComponent(typeof(EnemyStateMachine))]
    public class BossAI : MonoBehaviour, IDamageable
    {
        public EnemyStateMachine baseAI;
        public Animator animator;

        public float maxHealth = 1000f;
        public float health;
        private bool phaseTwo;

        private void Reset()
        {
            baseAI = GetComponent<EnemyStateMachine>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (baseAI == null) baseAI = GetComponent<EnemyStateMachine>();
            if (animator == null) animator = GetComponent<Animator>();
            health = maxHealth;
            StartCoroutine(AISequence());
        }

        private IEnumerator AISequence()
        {
            while (health > 0)
            {
                if (!phaseTwo && health <= maxHealth * 0.5f)
                {
                    EnterPhaseTwo();
                }

                // choose a move based on phase
                if (!phaseTwo)
                {
                    animator.SetTrigger("LightSlam");
                    yield return new WaitForSeconds(3f);
                    animator.SetTrigger("Swipe");
                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    animator.SetTrigger("Phase2_AOE");
                    yield return new WaitForSeconds(2f);
                    animator.SetTrigger("Phase2_Rush");
                    yield return new WaitForSeconds(1.5f);
                }
                yield return null;
            }
            Die();
        }

        private void EnterPhaseTwo()
        {
            phaseTwo = true;
            animator.SetTrigger("PhaseTwo");
            // increase aggression via baseAI settings
            baseAI.detectionRadius *= 1.2f;
        }

        public void TakeDamage(DamageData data)
        {
            health -= data.physicalDamage + data.fireDamage;
            if (health <= 0) Die();
        }

        private void Die()
        {
            animator.SetTrigger("Die");
            Destroy(gameObject, 5f);
        }
    }
}
