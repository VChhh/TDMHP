using UnityEngine;

namespace TDMHP.Combat.Hit
{
    public readonly struct HitInfo
    {
        public readonly GameObject attacker;
        public readonly GameObject target;
        public readonly float damage;
        public readonly Vector3 point;
        public readonly Vector3 direction; // attacker -> target

        public HitInfo(GameObject attacker, GameObject target, float damage, Vector3 point, Vector3 direction)
        {
            this.attacker = attacker;
            this.target = target;
            this.damage = damage;
            this.point = point;
            this.direction = direction;
        }
    }
}
