namespace TDMHP.AI.BT
{
    public sealed class BehaviorTreeBrain : TDMHP.AI.IEnemyBrain
    {
        private readonly BehaviorTreeAsset _asset;
        private BTContext _ctx;
        private bool _running;
        private bool _staggeredLock;

        public BehaviorTreeBrain(BehaviorTreeAsset asset) { _asset = asset; }

        public void Initialize(TDMHP.AI.EnemyContext ctx)
        {
            _ctx = new BTContext(ctx);
            _asset.root?.Reset();
            _running = true;
            _staggeredLock = false;
        }

        public void Tick(float dt)
        {
            if (!_running) return;
            if (_staggeredLock) return; // simplest “stagger interrupt” gate

            var r = _asset.root?.Tick(_ctx, dt) ?? BTStatus.Failure;
            if (r != BTStatus.Running)
                _asset.root?.Reset();
        }

        public void OnEvent(in TDMHP.AI.EnemyEvent e)
        {
            if (e.type == TDMHP.AI.EnemyEventType.Died)
            {
                _running = false;
                _ctx.enemy.motor.Stop();
                return;
            }

            if (e.type == TDMHP.AI.EnemyEventType.Staggered)
            {
                _staggeredLock = true;
                _ctx.enemy.motor.Stop();
                // You can clear this lock when your stagger ends (best via another event/flag).
            }
        }

        public void Shutdown() { _running = false; }
    }
}
