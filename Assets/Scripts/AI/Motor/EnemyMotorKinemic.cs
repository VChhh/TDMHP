using UnityEngine;
using TDMHP.UnscaledTime;

namespace TDMHP.AI.Motor
{
    /// Simple transform-based motor for prototypes.
    /// Later you can swap to NavMesh or CharacterController without changing AI.
    [DisallowMultipleComponent]
    public sealed class EnemyMotorKinematic : MonoBehaviour, IEnemyMotor
    {
        [Header("Move")]
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private float accel = 25f;
        [SerializeField] private float decel = 30f;

        [Header("Rotate")]
        [SerializeField] private float turnSpeedDeg = 720f;

        [Header("Impulse")]
        [SerializeField] private float impulseDamping = 8f;

        [Header("Time Source (optional)")]
        [SerializeField] private CombatClock _clock;
        private float DeltaTime => _clock != null ? _clock.DeltaTime : Time.deltaTime;
        private float _speedMul = 1f;

        private bool _hasTarget;
        private Vector3 _targetPos;
        private float _stopDist = 0.1f;

        private Vector3 _vel;          // world velocity
        private Vector3 _impulseVel;   // world velocity

        public MotorState State { get; private set; } = MotorState.Idle;

        public void SetMoveTarget(Vector3 worldPos, float stopDistance)
        {
            _hasTarget = true;
            _targetPos = worldPos;
            _stopDist = Mathf.Max(0f, stopDistance);
        }

        public void ClearMoveTarget()
        {
            _hasTarget = false;
        }

        public void Stop()
        {
            _hasTarget = false;
            _vel = Vector3.zero;
            State = MotorState.Idle;
        }

        public void FaceTowards(Vector3 worldPos)
        {
            var dir = worldPos - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;

            RotateTowards(dir.normalized, Time.deltaTime);
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _speedMul = Mathf.Clamp(multiplier, 0f, 10f);
        }

        public void AddImpulse(Vector3 worldVelocity)
        {
            _impulseVel += worldVelocity;
        }

        private void Update()
        {
            float dt = DeltaTime;
            Tick(dt);
        }

        private void Tick(float dt)
        {
            // decay impulse
            if (_impulseVel.sqrMagnitude > 0.00001f)
            {
                _impulseVel = Vector3.Lerp(_impulseVel, Vector3.zero, 1f - Mathf.Exp(-impulseDamping * dt));
            }

            Vector3 desiredDir = Vector3.zero;
            bool wantsMove = false;

            if (_hasTarget)
            {
                Vector3 to = _targetPos - transform.position;
                to.y = 0f;
                float dist = to.magnitude;

                if (dist > _stopDist)
                {
                    desiredDir = to / Mathf.Max(0.0001f, dist);
                    wantsMove = true;
                }
                else
                {
                    wantsMove = false;
                }
            }

            float targetSpeed = wantsMove ? moveSpeed * _speedMul : 0f;
            Vector3 targetVel = desiredDir * targetSpeed;

            // accelerate/decelerate
            float rate = wantsMove ? accel : decel;
            _vel = Vector3.MoveTowards(_vel, targetVel, rate * dt);

            // face movement direction if moving (optional, but nice default)
            if (_vel.sqrMagnitude > 0.01f)
            {
                RotateTowards(_vel.normalized, dt);
            }

            Vector3 totalVel = _vel + _impulseVel;
            transform.position += totalVel * dt;

            bool moving = totalVel.sqrMagnitude > 0.01f;
            State = new MotorState(moving, totalVel, desiredDir);
        }

        private void RotateTowards(Vector3 dir, float dt)
        {
            if (dir.sqrMagnitude < 0.0001f) return;

            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                turnSpeedDeg * dt
            );
        }
    }
}
