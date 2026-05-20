//typesss.cs

using System;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Types of damage for the pipeline.
    /// </summary>
    public enum DamageType { Slash, Pierce, Blunt, Magic }

    /// <summary>
    /// Struct passed into <see cref="IDamageable"/>. Keep it a struct to avoid allocations on fast paths.
    /// </summary>
    [Serializable]
    public struct DamageData
    {
        public float physicalDamage;
        public float fireDamage;
        public float poiseDamage;
        public Vector3 hitDirection;
        public DamageType type;

        public static DamageData Zero => new DamageData { physicalDamage = 0, fireDamage = 0, poiseDamage = 0, hitDirection = Vector3.zero, type = DamageType.Slash };
    }

    /// <summary>
    /// Objects that can receive damage implement this. Separation allows non-Mono behaviours to be damageable in the future.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(DamageData data);
    }
}
