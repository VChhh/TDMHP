using UnityEngine;

namespace TDMHP.AI.Motor
{
    /// Stable motor API for brains. Internally forwards to an implementation.
    [DisallowMultipleComponent]
    public sealed class EnemyMotorDriver : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _implBehaviour;

        private IEnemyMotor _impl;

        public MotorState State => _impl != null ? _impl.State : MotorState.Idle;

        private void Awake()
        {
            ResolveImpl();
        }

        private void OnValidate()
        {
            ResolveImpl();
        }

        private void ResolveImpl()
        {
            _impl = _implBehaviour as IEnemyMotor;

            if (_impl == null && _implBehaviour != null)
                Debug.LogError($"{name}: Motor impl must implement IEnemyMotor.", this);

            // Auto-pick if not assigned
            if (_impl == null)
            {
                var candidates = GetComponents<MonoBehaviour>();
                for (int i = 0; i < candidates.Length; i++)
                {
                    if (candidates[i] is IEnemyMotor m)
                    {
                        _impl = m;
                        _implBehaviour = candidates[i];
                        break;
                    }
                }
            }
        }

        public void MoveTo(Vector3 worldPos, float stopDistance = 0.1f)
            => _impl?.SetMoveTarget(worldPos, stopDistance);

        public void Stop() => _impl?.Stop();

        public void ClearMoveTarget() => _impl?.ClearMoveTarget();

        public void FaceTowards(Vector3 worldPos) => _impl?.FaceTowards(worldPos);

        public void SetSpeedMultiplier(float multiplier) => _impl?.SetSpeedMultiplier(multiplier);

        public void AddImpulse(Vector3 worldVel) => _impl?.AddImpulse(worldVel);
    }
}
