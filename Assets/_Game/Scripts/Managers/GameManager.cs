using UnityEngine;
using Game.Systems;

namespace Game.Managers
{
    /// <summary>
    /// Central game manager. Handles bonfire resting, respawn, and central references.
    /// Editor tips: Make this a scene singleton and assign player reference.
    /// Design notes: Keeps world-level operations here; systems register with GameManager for events.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Game.Player.PlayerController player;
        public Transform spawnPoint;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this);
            Instance = this;
        }

        public void RespawnPlayer()
        {
            if (player == null) return;
            player.transform.position = spawnPoint.position;
            var hs = player.GetComponent<Game.Core.HealthSystem>();
            if (hs != null) hs.health = hs.maxHealth;
        }

        public void RestAtBonfire(Transform bonfire)
        {
            // restore player HP and reset enemies. In production, send events to enemy spawners.
            var hs = player.GetComponent<Game.Core.HealthSystem>();
            if (hs != null) hs.health = hs.maxHealth;
        }
    }
}
