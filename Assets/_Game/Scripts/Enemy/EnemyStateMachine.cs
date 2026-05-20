using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Combat;

namespace Game.Enemy
{
    /// <summary>
    /// Simple HFSM for enemies. States: Idle, Patrol, Alert, Chase, Attack, Stagger, Death.
    /// Editor tips: Attach to enemy root. Requires Animator and NavMeshAgent (optional). Configure detectionRadius.
    /// Design notes: This implementation keeps logic readable for production and uses coroutines for state transitions. AIManager can later stagger update ticks.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class EnemyStateMachine : MonoBehaviour, IDamageable
    {
        public enum State { Idle, Patrol, Alert, Chase, Attack, Stagger, Death }

        public State state = State.Idle;
        public Animator animator;
        public float detectionRadius = 8f;
        public float attackRange = 2f;
        public float health = 100f;
        public float poise = 50f;

        private Transform player;
        private float lastSeenTime;
        private float patrolTimer;

        private void Reset()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (animator == null) animator = GetComponent<Animator>();
            player = Camera.main.transform; // naive; replace with player reference in production
            StartCoroutine(StateLoop());
        }

        private IEnumerator StateLoop()
        {
            while (state != State.Death)
            {
                switch (state)
                {
                    case State.Idle:
                        yield return Idle();
                        break;
                    case State.Patrol:
                        yield return Patrol();
                        break;
                    case State.Alert:
                        yield return Alert();
                        break;
                    case State.Chase:
                        yield return Chase();
                        break;
                    case State.Attack:
                        yield return Attack();
                        break;
                    case State.Stagger:
                        yield return Stagger();
                        break;
                }
                yield return null;
            }
        }

        private IEnumerator Idle()
        {
            // simple detection
            animator.SetBool("IsMoving", false);
            if (Vector3.Distance(transform.position, player.position) < detectionRadius)
            {
                state = State.Alert;
                yield break;
            }
            // idle wait
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator Patrol()
        {
            animator.SetBool("IsMoving", true);
            // naive patrol timer
            patrolTimer += Time.deltaTime;
            if (patrolTimer > 5f) patrolTimer = 0f;
            if (Vector3.Distance(transform.position, player.position) < detectionRadius)
            {
                state = State.Alert;
                yield break;
            }
            yield return null;
        }

        private IEnumerator Alert()
        {
            animator.SetTrigger("Alert");
            lastSeenTime = Time.time;
            state = State.Chase;
            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator Chase()
        {
            animator.SetBool("IsMoving", true);
            var dist = Vector3.Distance(transform.position, player.position);
            if (dist < attackRange)
            {
                state = State.Attack;
                yield break;
            }
            // naive movement towards player
            transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime * 2f);
            yield return null;
        }

        private IEnumerator Attack()
        {
            animator.SetTrigger("Attack");
            // attack windup
            yield return new WaitForSeconds(0.8f);
            // damage application would be via animation events calling RegisterAttack on this component
            state = State.Chase;
            yield return null;
        }

        private IEnumerator Stagger()
        {
            animator.SetTrigger("Stagger");
            yield return new WaitForSeconds(1f);
            // after stagger, return to chase or idle
            state = State.Chase;
        }

        public void TakeDamage(DamageData data)
        {
            health -= data.physicalDamage + data.fireDamage;
            poise -= data.poiseDamage;
            if (poise <= 0)
            {
                poise = 50f; // reset poise
                state = State.Stagger;
            }
            if (health <= 0)
            {
                state = State.Death;
                Die();
            }
            else
            {
                state = State.Chase;
            }
        }

        private void Die()
        {
            animator.SetTrigger("Die");
            // disable collider/navigation and drop loot etc.
            Destroy(gameObject, 5f);
        }
    }
}
