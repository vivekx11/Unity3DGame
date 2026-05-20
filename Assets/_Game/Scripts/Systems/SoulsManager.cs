using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// Manages souls (currency) dropped on death and retrieval.
    /// Editor tips: Ensure this manager is present in the scene as a singleton. Configure drop prefab.
    /// Design notes: Drops a collectible pickup with amount; retrievable once.
    /// </summary>
    public class SoulsManager : MonoBehaviour
    {
        public static SoulsManager Instance { get; private set; }
        public GameObject soulPrefab;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this);
            Instance = this;
        }

        public void DropSouls(Vector3 position, int amount)
        {
            if (amount <= 0) return;
            var go = Instantiate(soulPrefab, position, Quaternion.identity);
            var pickup = go.GetComponent<SoulPickup>();
            if (pickup != null) pickup.amount = amount;
            // drop physics
            var rb = go.GetComponent<Rigidbody>();
            if (rb != null) rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        }
    }

    public class SoulPickup : MonoBehaviour
    {
        public int amount;
        public float life = 60f; // despawn

        private void Start()
        {
            Destroy(gameObject, life);
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<Game.Player.PlayerController>();
            if (player != null)
            {
                // Add to player currency (not implemented) - raise event
                Destroy(gameObject);
            }
        }
    }
}
