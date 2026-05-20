using System.Collections;
using UnityEngine;
using Game.Combat;
using Game.Core;

namespace Game.Player
{
    /// <summary>
    /// Root-motion based player controller handling locomotion, stamina, and dodge with i-frames.
    /// Editor tips: Attach to the Player root that contains Animator (root motion enabled) and a CapsuleCollider. Set layers for "Default" and "Invulnerable" if using collision ignoring.
    /// Design notes: Uses Animator parameters (Speed, MoveX, MoveY, IsSprinting, IsDodging, AttackTrigger) and reads input centrally so animations drive final position (root motion).
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        public Animator animator;
        public HealthSystem health;
        public CombatSystem combatSystem;

        [Header("Stats")]
        public float walkSpeed = 1f; // used by blend tree
        public float runSpeed = 2f;
        public float sprintMultiplier = 1.5f;

        [Header("Stamina")]
        public float maxStamina = 100f;
        public float stamina;
        public float staminaRegenRate = 10f;
        public float staminaRegenDelay = 1f;
        private float lastStaminaUseTime = -10f;

        [Header("Dodge")]
        public float dodgeStaminaCost = 20f;
        public float dodgeIFrameWindow = 0.6f;
        public float dodgeCooldown = 0.5f;
        private bool isDodging;
        private bool canDodge = true;

        private Vector2 moveInput;
        private bool isSprinting;

        private void Reset()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (animator == null) animator = GetComponent<Animator>();
            stamina = maxStamina;
            if (health == null) health = GetComponent<HealthSystem>();
            if (combatSystem == null) combatSystem = GetComponent<CombatSystem>();
        }

        private void Update()
        {
            ReadInput();
            UpdateAnimatorParameters();
            RegenerateStamina();
        }

        private void ReadInput()
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            isSprinting = Input.GetKey(KeyCode.LeftShift);

            if (Input.GetButtonDown("Fire1"))
            {
                if (combatSystem != null) combatSystem.RequestLightAttack();
            }
            if (Input.GetButtonDown("Fire2"))
            {
                if (combatSystem != null) combatSystem.RequestHeavyAttack();
            }

            if (Input.GetButtonDown("Jump")) // mapped to dodge/roll
            {
                if (canDodge && stamina >= dodgeStaminaCost)
                {
                    StartCoroutine(PerformDodge());
                }
            }
        }

        private void UpdateAnimatorParameters()
        {
            float speed = moveInput.magnitude * (isSprinting ? runSpeed * sprintMultiplier : runSpeed);
            animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
            animator.SetFloat("MoveX", moveInput.x, 0.1f, Time.deltaTime);
            animator.SetFloat("MoveY", moveInput.y, 0.1f, Time.deltaTime);
            animator.SetBool("IsSprinting", isSprinting);
        }

        private void RegenerateStamina()
        {
            if (Time.time - lastStaminaUseTime < staminaRegenDelay) return;
            if (stamina < maxStamina)
            {
                stamina += staminaRegenRate * Time.deltaTime;
                stamina = Mathf.Min(stamina, maxStamina);
            }
        }

        /// <summary>
        /// Requests stamina consumption from other systems.
        /// </summary>
        public bool TryUseStamina(float amount)
        {
            if (stamina < amount) return false;
            stamina -= amount;
            lastStaminaUseTime = Time.time;
            return true;
        }

        private IEnumerator PerformDodge()
        {
            canDodge = false;
            isDodging = true;

            // consume stamina
            if (!TryUseStamina(dodgeStaminaCost))
            {
                isDodging = false;
                canDodge = true;
                yield break;
            }

            // trigger animation
            animator.SetTrigger("Dodge");
            if (health != null) health.SetInvulnerable(true);

            // I-frame window controlled by animation event is preferred; here we guard for minimum window
            yield return new WaitForSeconds(dodgeIFrameWindow);

            if (health != null) health.SetInvulnerable(false);
            isDodging = false;

            // cooldown
            yield return new WaitForSeconds(dodgeCooldown);
            canDodge = true;
        }

        private void OnAnimatorMove()
        {
            // Allow root motion to drive transform.
            // Non-physics movement: animator.deltaPosition and deltaRotation applied directly.
            if (animator == null) return;
            transform.position += animator.deltaPosition;
            transform.rotation *= animator.deltaRotation;
        }
    }
}
