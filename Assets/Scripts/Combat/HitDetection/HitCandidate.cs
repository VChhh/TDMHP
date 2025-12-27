using UnityEngine;

namespace TDMHP.Combat.HitDetection
{
    public readonly struct HitCandidate
    {
        public readonly int attackId;
        public readonly Collider collider;
        public readonly Vector3 queryCenter;

        public HitCandidate(int attackId, Collider collider, Vector3 queryCenter)
        {
            this.attackId = attackId;
            this.collider = collider;
            this.queryCenter = queryCenter;
        }
    }
}
