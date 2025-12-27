using UnityEngine;

namespace TDMHP.Combat.HitDetection
{
    [CreateAssetMenu(menuName = "TDMHP/Combat/Hitbox Profile", fileName = "HitboxProfile_")]
    public sealed class HitboxProfile : ScriptableObject
    {
        [Header("Shape")]
        public HitShape shape;

        [Header("Query")]
        public LayerMask targetMask = ~0;
        public QueryTriggerInteraction queryTriggers = QueryTriggerInteraction.Ignore;

        [Header("Rules")]
        [Tooltip("If true, colliders under the owner root will be ignored.")]
        public bool ignoreOwnerHierarchy = true;

        [Tooltip("If true, dedup is per Damageable (preferred). If false, per Collider.")]
        public bool dedupPerDamageable = true;
    }
}
