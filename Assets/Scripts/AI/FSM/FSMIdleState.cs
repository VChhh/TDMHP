namespace TDMHP.AI.FSM.States
{
    public sealed class FSMIdleState : IFSMEnemyState
    {
        private readonly FSMBrainAsset _cfg;
        public FSMEnemyStateId Id => FSMEnemyStateId.Idle;

        public FSMIdleState(FSMBrainAsset cfg) { _cfg = cfg; }

        public void OnEnter(TDMHP.AI.EnemyContext ctx) { }

        public FSMEnemyStateId Tick(TDMHP.AI.EnemyContext ctx, float dt)
        {
            if (ctx.bb.isDead) return FSMEnemyStateId.Dead;

            if(ctx.bb.target == null)
            {
                UnityEngine.Debug.Log("Searching for player...");
                ctx.bb.target = ctx.perception.FindPlayer();
            }
            if (ctx.bb.target != null && ctx.perception.IsInRange(ctx.tr, ctx.bb.target, _cfg.alertedRange))
                return FSMEnemyStateId.Alerted;
            return FSMEnemyStateId.Idle;
        }

        public void OnExit(TDMHP.AI.EnemyContext ctx) { }
    }
}
