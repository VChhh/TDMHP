using UnityEngine;

namespace TDMHP.Combat.Damage
{
    public readonly struct DamageRequest
    {
        public readonly GameObject attacker;
        public readonly GameObject target;

        public readonly DamageType damageType;
        public readonly float damage;
        public readonly float staggerDamage;

        public readonly bool isCritical;

        public readonly Vector3 point;
        public readonly Vector3 direction; // attacker -> target (flattened OK)

        public readonly double time;

        public DamageRequest(
            GameObject attacker,
            GameObject target,
            DamageType damageType,
            float damage,
            float staggerDamage,
            bool isCritical,
            Vector3 point,
            Vector3 direction,
            double time)
        {
            this.attacker = attacker;
            this.target = target;
            this.damageType = damageType;
            this.damage = damage;
            this.staggerDamage = staggerDamage;
            this.isCritical = isCritical;
            this.point = point;
            this.direction = direction;
            this.time = time;
        }
    }
}
