using UnityEngine;

namespace TDMHP.Combat
{
    [CreateAssetMenu(menuName = "TDMHP/Combat/Dodge Data", fileName = "Dodge_")]
    public sealed class DodgeData : ScriptableObject
    {
        [Header("Timing (seconds)")]
        [Min(0.05f)] public float duration = 0.35f;

        [Header("Movement")]
        [Min(0f)] public float speedMultiplier = 2.0f;

        [Header("I-Frames window (seconds from dodge start)")]
        [Tooltip("When invulnerability starts.")]
        [Min(0f)] public float iFrameStart = 0.05f;

        [Tooltip("When invulnerability ends.")]
        [Min(0f)] public float iFrameEnd = 0.20f;

        public float IFrameDuration => Mathf.Max(0f, iFrameEnd - iFrameStart);
    }
}
