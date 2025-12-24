using System;
using UnityEngine;

namespace TDMHP.Combat.Damage
{
    /// <summary>
    /// Target-side: HP + poise + reaction selection.
    /// </summary>
    public sealed class DamageableCharacter : MonoBehaviour, IDamageable
    {
        [Header("Components")]
        [SerializeField] private HealthComponent _health;
        [SerializeField] private StaggerMeter _stagger;
        [SerializeField] private Invulnerability _invuln;
        [SerializeField] private HitReactionReceiver _reaction;

        public event Action<DamageRequest, DamageResult> OnDamaged;
        public event Action<DamageRequest> OnStaggered;
        public event Action<DamageRequest> OnDied;

        private void Reset()
        {
            _health = GetComponent<HealthComponent>();
            _stagger = GetComponent<StaggerMeter>();
            _invuln = GetComponent<Invulnerability>();
            _reaction = GetComponent<HitReactionReceiver>();
        }

        private void Awake()
        {
            if (_health == null) _health = GetComponent<HealthComponent>();
            if (_stagger == null) _stagger = GetComponent<StaggerMeter>();
            if (_invuln == null) _invuln = GetComponent<Invulnerability>();
            if (_reaction == null) _reaction = GetComponent<HitReactionReceiver>();

            if (_health == null)
                Debug.LogError("[DamageableCharacter] Missing HealthComponent.", this);
        }

        public DamageResult ApplyDamage(in DamageRequest request)
        {
            // Extra safety: even though DamageSystem checks too
            if (_invuln != null && _invuln.IsInvulnerable)
            {
                var inv = new DamageResult(
                    applied: false,
                    damageApplied: 0f,
                    staggerApplied: 0f,
                    healthAfter: _health != null ? _health.Current : 0f,
                    staggerAfter: _stagger != null ? _stagger.Current : 0f,
                    killed: false,
                    staggered: false,
                    critical: request.isCritical,
                    reaction: HitReactionType.Invulnerable
                );

                _reaction?.React(request, inv);
                OnDamaged?.Invoke(request, inv);
                return inv;
            }

            if (_health == null || _health.IsDead)
            {
                var dead = new DamageResult(false, 0f, 0f,
                    _health != null ? _health.Current : 0f,
                    _stagger != null ? _stagger.Current : 0f,
                    killed: true, staggered: false, critical: request.isCritical,
                    reaction: HitReactionType.Death);

                return dead;
            }

            // Apply HP damage
            _health.ApplyDamage(request.damage);
            bool killedNow = _health.IsDead;

            // Apply stagger/poise
            bool staggeredNow = false;
            if (!killedNow && _stagger != null)
                staggeredNow = _stagger.ApplyStagger(request.staggerDamage);

            HitReactionType reaction =
                killedNow ? HitReactionType.Death :
                staggeredNow ? HitReactionType.Stagger :
                HitReactionType.Hit;

            var result = new DamageResult(
                applied: true,
                damageApplied: request.damage,
                staggerApplied: request.staggerDamage,
                healthAfter: _health.Current,
                staggerAfter: _stagger != null ? _stagger.Current : 0f,
                killed: killedNow,
                staggered: staggeredNow,
                critical: request.isCritical,
                reaction: reaction
            );

            _reaction?.React(request, result);
            OnDamaged?.Invoke(request, result);

            if (staggeredNow) OnStaggered?.Invoke(request);
            if (killedNow) OnDied?.Invoke(request);

            return result;
        }
    }
}
