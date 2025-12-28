using UnityEngine;

namespace TDMHP.Combat.Feedback
{
    [System.Serializable]
    public struct HitFeedbackSpec
    {
        [Header("VFX")]
        public ParticleSystem sparkPrefab;

        [Header("SFX")]
        public AudioClip sfx;

        [Header("Feel")]
        public float hitstopSeconds;
        public float cameraShakeStrength;
    }
}
