namespace TDMHP.AI.FSM.States
{
    public sealed class FSMAttackingState : IFSMEnemyState
    {
        private readonly FSMBrainAsset _cfg;
        public FSMEnemyStateId Id => FSMEnemyStateId.Attacking;

        private bool _started;

        public FSMAttackingState(FSMBrainAsset cfg) { _cfg = cfg; }

        public void OnEnter(TDMHP.AI.EnemyContext ctx)
        {
            _started = false;

            if (ctx.bb.isDead) return;
            if (ctx.bb.isStaggered) return;

            if (ctx.bb.target == null) ctx.bb.target = ctx.perception.FindPlayer();
            if (ctx.bb.target == null) return;

            // usually you don't want to chase during the committed attack
            ctx.motor.Stop();
            ctx.motor.FaceTowards(ctx.bb.target.position);

            // start the attack exactly once on enter
            _started = ctx.combat.TryAttackBest(ctx.bb.target);
        }

        public FSMEnemyStateId Tick(TDMHP.AI.EnemyContext ctx, float dt)
        {
            if (ctx.bb.isDead) return FSMEnemyStateId.Dead;
            if (ctx.bb.isStaggered) return FSMEnemyStateId.Staggered;

            if (!_started) return FSMEnemyStateId.Alerted;

            // keep facing for readability (even if stationary)
            if (ctx.bb.target != null)
                ctx.motor.FaceTowards(ctx.bb.target.position);

            // Attack ends when driver unlocks (windup+active+recovery complete)
            if (!ctx.combat.IsLocked)
                return FSMEnemyStateId.Alerted;

            // optional: if you want to drop aggro immediately when too far, do it here.
            // otherwise let Alerted handle loseRange.
            return FSMEnemyStateId.Attacking;
        }

        public void OnExit(TDMHP.AI.EnemyContext ctx)
        {
            _started = false;
        }
    }
}

