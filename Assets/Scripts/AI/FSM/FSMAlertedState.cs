namespace TDMHP.AI.FSM.States
{
    public sealed class FSMAlertedState : IFSMEnemyState
    {
        private readonly FSMBrainAsset _cfg;
        public FSMEnemyStateId Id => FSMEnemyStateId.Alerted;

        public FSMAlertedState(FSMBrainAsset cfg) { _cfg = cfg; }

        public void OnEnter(TDMHP.AI.EnemyContext ctx) { }

        public FSMEnemyStateId Tick(TDMHP.AI.EnemyContext ctx, float dt)
        {
            if (ctx.bb.isDead) return FSMEnemyStateId.Dead;
            if (ctx.bb.isStaggered) return FSMEnemyStateId.Staggered;

            if (ctx.bb.target == null) ctx.bb.target = ctx.perception.FindPlayer();
            if (ctx.bb.target == null) return FSMEnemyStateId.Idle;

            // stationary dummy: just face the target
            ctx.motor.FaceTowards(ctx.bb.target.position);

            // attack if in range
            if (ctx.perception.IsInRange(ctx.tr, ctx.bb.target, _cfg.attackRange)) // or use attack.preferredRange via driver
            {
                ctx.combat.TryAttackBest(ctx.bb.target);
            }

            // lose aggro if too far
            if (!ctx.perception.IsInRange(ctx.tr, ctx.bb.target, _cfg.loseRange))
                return FSMEnemyStateId.Idle;

            return FSMEnemyStateId.Alerted;
        }

        public void OnExit(TDMHP.AI.EnemyContext ctx) { }
    }
}

