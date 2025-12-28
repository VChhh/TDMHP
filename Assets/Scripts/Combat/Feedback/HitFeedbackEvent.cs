using UnityEngine;

namespace TDMHP.Combat.Feedback
{
    public readonly struct HitFeedbackEvent
    {
        public readonly GameObject attacker;
        public readonly GameObject target;

        public readonly Vector3 point;
        public readonly Vector3 direction;

        public readonly bool isCrit;
        public readonly bool isHeavy;
        public readonly bool didStagger;
        public readonly bool didKill;

        public readonly float damageApplied;

        public readonly HitFeedbackSpec spec;

        public HitFeedbackEvent(
            GameObject attacker,
            GameObject target,
            Vector3 point,
            Vector3 direction,
            bool isCrit,
            bool isHeavy,
            bool didStagger,
            bool didKill,
            float damageApplied,
            HitFeedbackSpec spec)
        {
            this.attacker = attacker;
            this.target = target;
            this.point = point;
            this.direction = direction;
            this.isCrit = isCrit;
            this.isHeavy = isHeavy;
            this.didStagger = didStagger;
            this.didKill = didKill;
            this.damageApplied = damageApplied;
            this.spec = spec;
        }
    }
}
