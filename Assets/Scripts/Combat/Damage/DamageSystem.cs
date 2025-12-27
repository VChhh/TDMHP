using System;
using UnityEngine;
using TDMHP.Combat.HitDetection; // for Hurtbox

namespace TDMHP.Combat.Damage
{
    public sealed class DamageSystem : MonoBehaviour
    {
        public static DamageSystem Instance { get; private set; }

        [Header("Rules")]
        [SerializeField] private bool _friendlyFire = false;

        [Header("Damage modifiers (MVP)")]
        [SerializeField] private float _globalDamageMultiplier = 1f;
        [SerializeField] private float _critChance = 0.05f;
        [SerializeField] private float _critMultiplier = 1.5f;

        public event Action<DamageRequest, DamageResult> OnDamageResolved;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public bool TryApplyHit(
            GameObject attacker,
            Hurtbox hurtbox,
            DamageType damageType,
            float baseDamage,
            float baseStaggerDamage,
            Vector3 point,
            Vector3 direction,
            out DamageResult result)
        {
            result = default;

            if (attacker == null || hurtbox == null)
                return false;

            IDamageable target = hurtbox.Damageable;
            if (target == null)
                return false;

            GameObject targetGo = hurtbox.DamageableGameObject;

            // Team filtering
            if (!_friendlyFire)
            {
                Team aTeam = attacker.GetComponentInParent<Team>();
                Team tTeam = targetGo != null ? targetGo.GetComponentInParent<Team>() : null;

                if (aTeam != null && tTeam != null && aTeam.teamId != TeamId.Neutral && aTeam.teamId == tTeam.teamId)
                    return false;
            }

            // Invulnerability check (can also be enforced in DamageableCharacter)
            var inv = targetGo != null ? targetGo.GetComponentInParent<Invulnerability>() : null;
            if (inv != null && inv.IsInvulnerable)
            {
                var reqInv = new DamageRequest(attacker, targetGo, damageType, 0f, 0f, false, point, direction, Time.unscaledTimeAsDouble);
                result = new DamageResult(false, 0f, 0f,
                    healthAfter: (targetGo.GetComponentInParent<HealthComponent>()?.Current) ?? 0f,
                    staggerAfter: (targetGo.GetComponentInParent<StaggerMeter>()?.Current) ?? 0f,
                    killed: false, staggered: false, critical: false,
                    reaction: HitReactionType.Invulnerable);

                OnDamageResolved?.Invoke(reqInv, result);
                return false;
            }

            // Damage pipeline (MVP)
            float dmg = Mathf.Max(0f, baseDamage) * Mathf.Max(0f, _globalDamageMultiplier);
            float stag = Mathf.Max(0f, baseStaggerDamage);

            bool crit = UnityEngine.Random.value < Mathf.Clamp01(_critChance);
            if (crit) dmg *= Mathf.Max(1f, _critMultiplier);

            var req = new DamageRequest(attacker, targetGo, damageType, dmg, stag, crit, point, direction, Time.unscaledTimeAsDouble);
            result = target.ApplyDamage(req);

            OnDamageResolved?.Invoke(req, result);
            return result.applied;
        }
    }
}
