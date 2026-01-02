namespace TDMHP.AI.FSM.States
{
    public sealed class FSMStaggeredState : IFSMEnemyState
    {
        private readonly FSMBrainAsset _cfg;
        public FSMEnemyStateId Id => FSMEnemyStateId.Staggered;

        public FSMStaggeredState(FSMBrainAsset cfg) { _cfg = cfg; }

        public void OnEnter(TDMHP.AI.EnemyContext ctx)
        {
            ctx.motor.Stop();
        }

        public FSMEnemyStateId Tick(TDMHP.AI.EnemyContext ctx, float dt)
        {
            if (ctx.bb.isDead) return FSMEnemyStateId.Dead;

            // simple stagger lock; later replace with “stagger ended” event or animation state
            if (ctx.combat.CombatNow >= ctx.bb.staggerEndTime)
            {
                ctx.bb.isStaggered = false;
                return FSMEnemyStateId.Alerted;
            }

            return FSMEnemyStateId.Staggered;
        }

        public void OnExit(TDMHP.AI.EnemyContext ctx) { }
    }
}
