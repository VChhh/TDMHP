namespace TDMHP.AI
{
    public interface IEnemyBrain
    {
        void Initialize(EnemyContext ctx);
        void Tick(float dt);
        void OnEvent(in EnemyEvent e); // stagger, damaged, died, etc.
        void Shutdown();
    }
}
