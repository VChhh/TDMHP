using UnityEngine;
using TDMHP.Combat.Damage;

namespace TDMHP.Combat.Emitters
{
    public readonly struct MeleeEmissionPayload
    {
        public readonly GameObject attacker;
        public readonly DamageType damageType;
        public readonly float baseDamage;
        public readonly float baseStagger;

        public MeleeEmissionPayload(GameObject attacker, DamageType damageType, float baseDamage, float baseStagger)
        {
            this.attacker = attacker;
            this.damageType = damageType;
            this.baseDamage = baseDamage;
            this.baseStagger = baseStagger;
        }
    }
}
