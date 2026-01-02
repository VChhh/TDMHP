using System;
using UnityEngine;
using TDMHP.Combat.Emitters;
using TDMHP.Combat.HitDetection;
using TDMHP.UnscaledTime; // optional; driver works without it

namespace TDMHP.AI.Combat
{
    [DisallowMultipleComponent]
    public sealed class EnemyCombatDriver : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private EmitterSystem _emitterSystem;
        [SerializeField] private CombatClock _clock;                 // optional
        [SerializeField] private MeleeHitDetector _meleeDetector;    // usually child
        [SerializeField] private Transform _faceOrigin;              // default = transform

        [Header("Attacks")]
        [SerializeField] private EnemyMeleeAttackData[] _meleeAttacks;

        [Header("Runtime (read-only)")]
        [SerializeField] private bool _isLocked;
        [SerializeField] private double _lockUntil;
        [SerializeField] private double _cooldownUntil;

        private EmissionHandle _activeEmission; // cancel on stagger/death

        public bool IsLocked => _isLocked && Now < _lockUntil;
        public bool CanAttack => !IsLocked && Now >= _cooldownUntil && _meleeAttacks != null && _meleeAttacks.Length > 0;

        public event Action<EnemyMeleeAttackData> OnAttackStarted;
        public event Action OnAttackFinished;

        private double Now => _clock != null ? _clock.Now : Time.unscaledTimeAsDouble;
        public double CombatNow => Now; // public accessor for unscaled time

        private void Reset()
        {
            _emitterSystem = FindFirstObjectByType<EmitterSystem>();
            _clock = FindFirstObjectByType<CombatClock>();
            _meleeDetector = GetComponentInChildren<MeleeHitDetector>();
            _faceOrigin = transform;
        }

        private void Awake()
        {
            if (_emitterSystem == null) _emitterSystem = FindFirstObjectByType<EmitterSystem>();
            if (_clock == null) _clock = FindFirstObjectByType<CombatClock>();
            if (_meleeDetector == null) _meleeDetector = GetComponentInChildren<MeleeHitDetector>();
            if (_faceOrigin == null) _faceOrigin = transform;
        }

        private void Update()
        {
            if (_isLocked && Now >= _lockUntil)
            {
                _isLocked = false;
                OnAttackFinished?.Invoke();
            }
        }

        public void CancelCurrentAttack()
        {
            if (_activeEmission.IsValid && _emitterSystem != null)
                _emitterSystem.Cancel(_activeEmission);

            _activeEmission = default;
            _isLocked = false;
            _lockUntil = Now;
        }

        /// <summary>
        /// Pick an attack by distance (simple MVP).
        /// You can replace with weights / BT tasks later.
        /// </summary>
        public EnemyMeleeAttackData ChooseAttack(Transform target)
        {
            if (_meleeAttacks == null || _meleeAttacks.Length == 0) return null;
            if (target == null) return _meleeAttacks[0];

            float d = Vector3.Distance(transform.position, target.position);

            EnemyMeleeAttackData best = _meleeAttacks[0];
            float bestAbs = Mathf.Abs(d - best.preferredRange);

            for (int i = 1; i < _meleeAttacks.Length; i++)
            {
                var a = _meleeAttacks[i];
                if (a == null) continue;
                float abs = Mathf.Abs(d - a.preferredRange);
                if (abs < bestAbs)
                {
                    best = a;
                    bestAbs = abs;
                }
            }

            return best;
        }

        public bool TryAttackBest(Transform target)
        {
            var atk = ChooseAttack(target);
            return TryStartMelee(atk, target);
        }

        public bool TryStartMelee(EnemyMeleeAttackData atk, Transform target)
        {
            if (atk == null) return false;
            if (!CanAttack) return false;
            if (_emitterSystem == null || _meleeDetector == null || atk.hitboxProfile == null) return false;

            // Face target for stationary dummy (optional)
            if (atk.faceTargetOnStart && target != null && _faceOrigin != null)
            {
                Vector3 dir = target.position - _faceOrigin.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.0001f)
                    _faceOrigin.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
            }

            // Schedule hit window (active frames)
            var payload = new MeleeEmissionPayload(
                attacker: gameObject,
                damageType: atk.damageType,
                baseDamage: atk.baseDamage,
                baseStagger: atk.baseStagger
            );

            _activeEmission = _emitterSystem.ScheduleMelee(
                detector: _meleeDetector,
                profile: atk.hitboxProfile,
                startOffset: atk.ActiveStartOffset,
                endOffset: atk.ActiveEndOffset,
                payload: payload
            );

            // Lock + cooldown (unscaled)
            double now = Now;
            _isLocked = true;
            _lockUntil = now + atk.TotalLock;
            _cooldownUntil = now + Mathf.Max(0f, atk.cooldown);

            OnAttackStarted?.Invoke(atk);
            return true;
        }
    }
}
