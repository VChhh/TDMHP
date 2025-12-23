using UnityEngine;

namespace TDMHP.Combat.Hit
{
    [CreateAssetMenu(menuName = "TDMHP/Combat/Hitbox Profile", fileName = "Hitbox_")]
    public sealed class HitboxProfile : ScriptableObject
    {
        [Header("Shape")]
        [Min(0.01f)] public float radius = 0.6f;

        [Tooltip("Sphere center in ATTACKER LOCAL SPACE. Example: (0, 1, 1.2) = 1m up, 1.2m forward.")]
        public Vector3 localCenter = new Vector3(0f, 1f, 1.2f);

        [Header("Filter")]
        public LayerMask hitMask = ~0;

        [Tooltip("Detect trigger colliders too (common for hurtboxes).")]
        public bool includeTriggers = true;

        [Header("Debug")]
        public bool drawGizmos = true;
    }
}
