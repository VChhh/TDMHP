namespace TDMHP.AI.FSM.States
{
    public sealed class FSMDeadState : IFSMEnemyState
    {
        public FSMEnemyStateId Id => FSMEnemyStateId.Dead;

        public void OnEnter(TDMHP.AI.EnemyContext ctx) { ctx.motor.Stop(); }
        public FSMEnemyStateId Tick(TDMHP.AI.EnemyContext ctx, float dt) => FSMEnemyStateId.Dead;
        public void OnExit(TDMHP.AI.EnemyContext ctx) { }
    }
}
