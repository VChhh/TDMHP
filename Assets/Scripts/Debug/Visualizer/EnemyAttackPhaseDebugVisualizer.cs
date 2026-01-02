using UnityEngine;
using TDMHP.Debugging;
using TDMHP.AI.Combat; // EnemyCombatDriver + EnemyMeleeAttackData

namespace TDMHP.Debugging.Visualizers
{
    /// <summary>
    /// Draws simple phase visualization for enemy attacks:
    /// - Windup ring (orange-ish)
    /// - Recovery ring (blue-ish)
    /// Active is already covered by HitQueryDebugDrawer; optional toggle here.
    ///
    /// Attach to the enemy root that has EnemyCombatDriver.
    /// </summary>
    public sealed class EnemyAttackPhaseDebugVisualizer : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private EnemyCombatDriver _combat;

        [Header("Draw")]
        [SerializeField] private bool _drawWindup = true;
        [SerializeField] private bool _drawRecovery = true;
        [SerializeField] private bool _drawActiveRing = false; // optional (active is usually shown by HitQueryDebugDrawer)

        [SerializeField] private float _ttlSeconds = 0.05f;
        [SerializeField] private float _yLift = 0.05f;
        [SerializeField] private int _segments = 24;

        [Tooltip("If true, ring radius uses attack.preferredRange; otherwise uses fallbackRadius.")]
        [SerializeField] private bool _useAttackPreferredRange = true;

        [SerializeField] private float _fallbackRadius = 1.8f;

        [Header("Colors")]
        [SerializeField] private Color _windupColor = new Color(1.00f, 0.65f, 0.20f, 1f);
        [SerializeField] private Color _activeColor  = new Color(1.00f, 0.25f, 0.25f, 1f);
        [SerializeField] private Color _recoveryColor= new Color(0.35f, 0.70f, 1.00f, 1f);

        [Header("Progress spoke")]
        [SerializeField] private bool _drawProgressSpoke = true;
        [SerializeField] private float _spokeScale = 1.0f;

        private struct Timeline
        {
            public bool valid;
            public double tStart;
            public double tWindupEnd;
            public double tActiveEnd;
            public double tRecoveryEnd;
            public float radius;
        }

        private Timeline _tl;

        private double Now => Time.unscaledTimeAsDouble;

        private void Reset()
        {
            _combat = GetComponent<EnemyCombatDriver>();
        }

        private void Awake()
        {
            if (_combat == null) _combat = GetComponent<EnemyCombatDriver>();
        }

        private void OnEnable()
        {
            if (_combat != null)
            {
                _combat.OnAttackStarted += OnAttackStarted;
                _combat.OnAttackFinished += OnAttackFinished;
            }
        }

        private void OnDisable()
        {
            if (_combat != null)
            {
                _combat.OnAttackStarted -= OnAttackStarted;
                _combat.OnAttackFinished -= OnAttackFinished;
            }
        }

        private void Update()
        {
            if (!_tl.valid) return;

            double now = Now;
            if (now >= _tl.tRecoveryEnd)
            {
                _tl.valid = false;
                return;
            }

            Vector3 c = transform.position + Vector3.up * _yLift;

            // Phase ranges:
            // Windup:   [tStart, tWindupEnd)
            // Active:   [tWindupEnd, tActiveEnd)
            // Recovery: [tActiveEnd, tRecoveryEnd)
            bool inWindup = now < _tl.tWindupEnd;
            bool inActive = !inWindup && now < _tl.tActiveEnd;
            bool inRecovery = now >= _tl.tActiveEnd;

            float r = Mathf.Max(0.05f, _tl.radius);

            if (inWindup && _drawWindup)
            {
                DrawRingAndSpoke(c, r, _windupColor, now, _tl.tStart, _tl.tWindupEnd);
            }
            else if (inActive && _drawActiveRing)
            {
                DrawRingAndSpoke(c, r, _activeColor, now, _tl.tWindupEnd, _tl.tActiveEnd);
            }
            else if (inRecovery && _drawRecovery)
            {
                DrawRingAndSpoke(c, r, _recoveryColor, now, _tl.tActiveEnd, _tl.tRecoveryEnd);
            }
        }

        private void DrawRingAndSpoke(Vector3 center, float radius, Color color, double now, double phaseStart, double phaseEnd)
        {
            DebugDraw.CircleXZ(center, radius, color, _segments, _ttlSeconds, DebugDrawChannel.Combat, depthTest: true);

            if (!_drawProgressSpoke) return;

            float denom = (float)(phaseEnd - phaseStart);
            float t01 = denom <= 0.0001f ? 1f : Mathf.Clamp01((float)((now - phaseStart) / (phaseEnd - phaseStart)));

            // rotate a spoke around Y to indicate progress
            float ang = t01 * 360f;
            Vector3 dir = Quaternion.Euler(0f, ang, 0f) * Vector3.forward;

            DebugDraw.Line(
                center,
                center + dir * (radius * _spokeScale),
                color,
                _ttlSeconds,
                DebugDrawChannel.Combat,
                depthTest: false
            );
        }

        private void OnAttackStarted(EnemyMeleeAttackData atk)
        {
            double t0 = Now;

            // Use attack ranges when available
            float r = _fallbackRadius;
            if (_useAttackPreferredRange && atk != null && atk.preferredRange > 0f)
                r = atk.preferredRange;

            _tl = new Timeline
            {
                valid = true,
                tStart = t0,
                tWindupEnd = t0 + Mathf.Max(0f, atk != null ? atk.windup : 0f),
                tActiveEnd = t0 + Mathf.Max(0f, atk != null ? (atk.windup + atk.active) : 0f),
                tRecoveryEnd = t0 + Mathf.Max(0f, atk != null ? atk.TotalLock : 0f),
                radius = r
            };
        }

        private void OnAttackFinished()
        {
            // If you want the ring to persist through full recovery regardless of "finished" signal,
            // comment this out. For now we clear when the driver says done.
            _tl.valid = false;
        }

        /// <summary>
        /// Optional helper for cancels: call this from your stagger/death interrupt bridge if you want.
        /// </summary>
        public void ClearNow()
        {
            _tl.valid = false;
        }
    }
}
