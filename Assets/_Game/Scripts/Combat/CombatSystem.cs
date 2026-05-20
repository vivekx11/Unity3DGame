// CombatSystem

using System.Collections.Generic;
using UnityEngine;
using Game.Combat;

namespace Game.Combat
{
    /// <summary>
    /// High-level combat system on the player. Handles attack requests, chains, parry, riposte and manages hitbox pooling.
    /// Editor tips: Attach to player object. Configure Animator and assign attack animations with Animation Events calling RegisterHit on this component.
    /// Design notes: This class focuses on orchestration; weapon specifics come from ScriptableObjects.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CombatSystem : MonoBehaviour
    {
        public Animator animator;
        public WeaponData equippedWeapon;

        private int comboStep = 0;
        private float lastAttackTime = 0f;
        public float comboWindow = 0.6f;

        private HitboxManager hitboxManager;

        private void Reset()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (animator == null) animator = GetComponent<Animator>();
            hitboxManager = HitboxManager.Instance;
        }

        public void RequestLightAttack()
        {
            if (Time.time - lastAttackTime > comboWindow) comboStep = 0;
            if (equippedWeapon == null) return;

            animator.SetTrigger("LightAttack");
            lastAttackTime = Time.time;
            comboStep = (comboStep + 1) % equippedWeapon.lightComboCount;
        }

        public void RequestHeavyAttack()
        {
            if (equippedWeapon == null) return;
            animator.SetTrigger("HeavyAttack");
            lastAttackTime = Time.time;
            comboStep = 0;
        }

        /// <summary>
        /// Called from Animation Event to spawn a hitbox at the weapon's position.
        /// Animation provides the timing for active frames.
        /// </summary>
        public void RegisterHit()
        {
            if (equippedWeapon == null) return;
            var hb = hitboxManager.Get();
            hb.Activate(transform, equippedWeapon.hitboxRadius, equippedWeapon.damage, equippedWeapon.poise);
        }
    }

    /// <summary>
    /// Simple ScriptableObject representing weapon data.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Weapons/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName;
        public float damage = 20f;
        public float poise = 10f;
        public int lightComboCount = 3;
        public float hitboxRadius = 0.8f;
    }

    /// <summary>
    /// Very small pooled hitbox that detects collisions in active windows and applies DamageData to IDamageable targets.
    /// Editor tips: a single prefab with sphere collider and kinematic rigidbody can be used if desired, but this implementation uses Physics.OverlapSphere for simplicity.
    /// </summary>
    public class Hitbox : MonoBehaviour
    {
        private bool active;
        private float radius;
        private float damage;
        private float poise;
        private Transform origin;
        private float duration = 0.15f;

        public void Activate(Transform origin, float radius, float damage, float poise)
        {
            this.origin = origin;
            this.radius = radius;
            this.damage = damage;
            this.poise = poise;
            active = true;
            StartCoroutine(Life());
            DoHit();
        }

        private System.Collections.IEnumerator Life()
        {
            yield return new WaitForSeconds(duration);
            active = false;
            gameObject.SetActive(false);
        }

        private void DoHit()
        {
            var hits = Physics.OverlapSphere(origin.position, radius, LayerMask.GetMask("Default"));
            foreach (var col in hits)
            {
                var dmg = new DamageData { physicalDamage = damage, poiseDamage = poise, hitDirection = (col.transform.position - origin.position).normalized, type = DamageType.Slash };
                var target = col.GetComponentInParent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(dmg);
                }
            }
        }
    }

    /// <summary>
    /// Simple singleton pooled manager for hitboxes.
    /// </summary>
    public class HitboxManager : MonoBehaviour
    {
        public static HitboxManager Instance { get; private set; }
        public Hitbox prefab;
        public int poolSize = 10;
        private Queue<Hitbox> pool = new Queue<Hitbox>();

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this);
            Instance = this;
            for (int i = 0; i < poolSize; i++)
            {
                var go = Instantiate(prefab, transform);
                go.gameObject.SetActive(false);
                pool.Enqueue(go);
            }
        }

        public Hitbox Get()
        {
            if (pool.Count == 0)
            {
                var go = Instantiate(prefab, transform);
                return go;
            }
            var hb = pool.Dequeue();
            hb.gameObject.SetActive(true);
            return hb;
        }

        public void Recycle(Hitbox hb)
        {
            hb.gameObject.SetActive(false);
            pool.Enqueue(hb);
        }
    }
}
