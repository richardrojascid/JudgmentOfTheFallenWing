using System;
using UnityEngine;

namespace JudgmentOfTheFallenWing.Combat
{
    public struct DamageInfo
    {
        public float Amount;
        public Vector2 Knockback;
        public GameObject Source;
        public bool IsCritical;

        public DamageInfo(float amount, Vector2 knockback, GameObject source, bool isCritical = false)
        {
            Amount = amount;
            Knockback = knockback;
            Source = source;
            IsCritical = isCritical;
        }
    }

    public interface IDamageable
    {
        void TakeDamage(DamageInfo damage);
        bool IsAlive { get; }
    }
}
