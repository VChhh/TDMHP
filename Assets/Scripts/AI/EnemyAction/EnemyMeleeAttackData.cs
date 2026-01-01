using UnityEngine;
using TDMHP.Combat.Damage;
using TDMHP.Combat.HitDetection;

namespace TDMHP.AI.Combat
{
    [CreateAssetMenu(menuName = "TDMHP/AI/Enemy Melee Attack", fileName = "EnemyMeleeAttack_")]
    public sealed class EnemyMeleeAttackData : ScriptableObject
    {
        [Header("Hitbox")]
        public HitboxProfile hitboxProfile;

        [Header("Damage")]
        public DamageType damageType = DamageType.Slash;
        public float baseDamage = 10f;
        public float baseStagger = 10f;

        [Header("Timing (unscaled seconds)")]
        [Tooltip("Delay before active frames begin.")]
        public float windup = 0.15f;

        [Tooltip("Duration of active hit query window.")]
        public float active = 0.10f;

        [Tooltip("Total action lock after active ends (no-op for now, but useful later).")]
        public float recovery = 0.30f;

        [Header("Rules")]
        public float cooldown = 0.75f;
        public float preferredRange = 1.8f;
        public bool faceTargetOnStart = true;

        public float ActiveStartOffset => Mathf.Max(0f, windup);
        public float ActiveEndOffset => Mathf.Max(0f, windup + active);
        public float TotalLock => Mathf.Max(0f, windup + active + recovery);
    }
}
