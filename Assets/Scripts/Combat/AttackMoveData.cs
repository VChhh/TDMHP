using UnityEngine;
using TDMHP.Combat.Hit;

namespace TDMHP.Combat
{
    [CreateAssetMenu(menuName = "TDMHP/Combat/Attack Move", fileName = "AttackMove_")]
    public sealed class AttackMoveData : ScriptableObject
    {
        [Header("Timing (seconds)")]
        [Min(0.05f)] public float totalDuration = 0.9f;

        [Tooltip("When the hit becomes active (seconds from start).")]
        [Min(0f)] public float activeStart = 0.25f;

        [Tooltip("When the hit stops being active (seconds from start).")]
        [Min(0f)] public float activeEnd = 0.45f;

        [Header("Windows (seconds from start)")]
        public float comboLinkStart = 0.55f;
        public float comboLinkEnd = 0.80f;

        public float dodgeCancelStart = 0.60f;
        public float dodgeCancelEnd = 0.90f;

        [Header("Movement/Turn constraints")]
        [Range(0f, 1f)] public float moveMultiplier = 0.2f;
        [Range(0f, 1f)] public float turnMultiplier = 0.2f;

        [Header("Hit")]
        public HitboxProfile hitbox;
        [Min(0f)] public float damage = 10f;


        // [Header("Combo routing")]
        // public AttackMoveData nextLight;
        // public AttackMoveData nextHeavy;
    }
}
