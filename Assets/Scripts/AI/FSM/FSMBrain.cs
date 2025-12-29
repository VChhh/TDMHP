using System.Collections.Generic;

namespace TDMHP.AI.FSM
{
    public sealed class FSMBrain : TDMHP.AI.IEnemyBrain
    {
        private readonly FSMBrainAsset _cfg;
        private TDMHP.AI.EnemyContext _ctx;

        private readonly Dictionary<FSMEnemyStateId, IFSMEnemyState> _states = new();
        private IFSMEnemyState _current;

        public FSMBrain(FSMBrainAsset cfg)
        {
            _cfg = cfg;

            // register built-in states
            _states[FSMEnemyStateId.Idle] = new States.FSMIdleState(cfg);
            _states[FSMEnemyStateId.Alerted] = new States.FSMAlertedState(cfg);
            _states[FSMEnemyStateId.Staggered] = new States.FSMStaggeredState(cfg);
            _states[FSMEnemyStateId.Dead] = new States.FSMDeadState();
        }

        public void Initialize(TDMHP.AI.EnemyContext ctx)
        {
            _ctx = ctx;
            TransitionTo(FSMEnemyStateId.Idle);
        }

        public void Tick(float dt)
        {
            if (_ctx?.bb == null) return;
            if (_ctx.bb.isDead) return;

            var next = _current?.Tick(_ctx, dt) ?? _current.Id;
            if (next != _current.Id)
                TransitionTo(next);
        }

        public void OnEvent(in TDMHP.AI.EnemyEvent e)
        {
            if (_ctx?.bb == null) return;

            if (e.type == TDMHP.AI.EnemyEventType.Died)
            {
                _ctx.bb.isDead = true;
                TransitionTo(FSMEnemyStateId.Dead);
                return;
            }

            if (e.type == TDMHP.AI.EnemyEventType.Staggered)
            {
                _ctx.bb.isStaggered = true;
                _ctx.bb.staggerEndTime = UnityEngine.Time.time + _cfg.staggerLockSeconds;
                TransitionTo(FSMEnemyStateId.Staggered);
                return;
            }
        }

        public void Shutdown()
        {
            _current?.OnExit(_ctx);
            _current = null;
        }

        internal void TransitionTo(FSMEnemyStateId id)
        {
            if (_current != null && _current.Id == id) return;

            _current?.OnExit(_ctx);
            _current = _states[id];
            _current.OnEnter(_ctx);
        }

        internal bool HasTargetInRange(float range)
        {
            var t = _ctx.bb.target;
            if (t == null) return false;
            return _ctx.perception.IsInRange(_ctx.tr, t, range);
        }

        internal void EnsureTarget()
        {
            if (_ctx.bb.target != null) return;
            _ctx.bb.target = _ctx.perception.FindPlayer();
        }
    }
}
