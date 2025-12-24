namespace TDMHP.Combat.Damage
{
    public readonly struct DamageResult
    {
        public readonly bool applied;
        public readonly float damageApplied;
        public readonly float staggerApplied;

        public readonly float healthAfter;
        public readonly float staggerAfter;

        public readonly bool killed;
        public readonly bool staggered;
        public readonly bool critical;

        public readonly HitReactionType reaction;

        public DamageResult(
            bool applied,
            float damageApplied,
            float staggerApplied,
            float healthAfter,
            float staggerAfter,
            bool killed,
            bool staggered,
            bool critical,
            HitReactionType reaction)
        {
            this.applied = applied;
            this.damageApplied = damageApplied;
            this.staggerApplied = staggerApplied;
            this.healthAfter = healthAfter;
            this.staggerAfter = staggerAfter;
            this.killed = killed;
            this.staggered = staggered;
            this.critical = critical;
            this.reaction = reaction;
        }
    }
}
