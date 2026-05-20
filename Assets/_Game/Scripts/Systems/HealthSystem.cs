using UnityEngine;
using Game.Combat;

namespace Game.Core
{
    /// <summary>
    /// Health and poise system. Implements IDamageable and damage pipeline: Raw -> Apply Stats -> Armor -> Poise -> Apply.
    /// Editor tips: Attach to all actors that can take damage. Configure armorReduction and poise values.
    /// Design notes: Keep lightweight and deterministic. Heavy methods should be profiled; use ProfilerMarker in production.
    /// </summary>
    public class HealthSystem : MonoBehaviour, IDamageable
    {
        public float maxHealth = 100f;
        public float health = 100f;
        public float poise = 100f;
        public float armorReduction = 0.2f; // 20% reduction

        private bool invulnerable;

        private void Start()
        {
            health = maxHealth;
        }

        /// <summary>
        /// Sets temporary invulnerability (used by dodge)
        /// </summary>
        public void SetInvulnerable(bool v)
        {
            invulnerable = v;
        }

        public void TakeDamage(DamageData data)
        {
            if (invulnerable) return;

            // Raw damage
            float raw = data.physicalDamage + data.fireDamage;

            // Here stat scaling from owner would be applied (omitted for brevity)

            // Armor reduction
            float afterArmor = raw * (1f - armorReduction);

            // Poise
            poise -= data.poiseDamage;
            if (poise <= 0f)
            {
                // Apply stagger
                var enemy = GetComponent<Game.Enemy.EnemyStateMachine>();
                if (enemy) enemy.state = Game.Enemy.EnemyStateMachine.State.Stagger;
                poise = 100f; // reset
            }

            health -= afterArmor;
            if (health <= 0f) Die();
        }

        private void Die()
        {
            // death visuals handled elsewhere
            Destroy(gameObject, 5f);
        }
    }
}
