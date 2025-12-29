namespace TDMHP.AI.FSM
{
    public interface IFSMEnemyState
    {
        FSMEnemyStateId Id { get; }
        void OnEnter(TDMHP.AI.EnemyContext ctx);
        FSMEnemyStateId Tick(TDMHP.AI.EnemyContext ctx, float dt);
        void OnExit(TDMHP.AI.EnemyContext ctx);
    }
}
