using System.Collections.Generic;
using UnityEngine;
using TDMHP.Combat.Damage;

namespace TDMHP.Combat.Hit
{
    /// <summary>
    /// Performs non-alloc overlap checks during active frames.
    /// One hit per Hurtbox per swing.
    /// </summary>
    public sealed class MeleeHitDetector : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private int _maxColliders = 16;

        private Collider[] _results;
        private readonly HashSet<int> _hitThisSwing = new();

        private HitboxProfile _profile;
        private GameObject _attacker;

        private float _damage;
        private float _staggerDamage;
        private DamageType _damageType;

        private bool _active;

        private void Awake()
        {
            _results = new Collider[Mathf.Max(4, _maxColliders)];
        }

        public void BeginSwing(HitboxProfile profile, GameObject attacker, float damage, float staggerDamage, DamageType damageType)
        {
            _profile = profile;
            _attacker = attacker;
            _damage = damage;
            _staggerDamage = staggerDamage;
            _damageType = damageType;

            _active = (_profile != null);
            _hitThisSwing.Clear();
        }

        public void EndSwing()
        {
            _active = false;
            _profile = null;
            _attacker = null;
            _damage = 0f;
            _hitThisSwing.Clear();
        }

        public void TickActive()
        {
            if (!_active || _profile == null || _attacker == null)
                return;

            // calculate the world space center of the hitbox
            // given the localCenter is the attacker's local space
            // use the attacker's transform, not this component's transform,
            // so the hitbox stays locked to the character correctly.
            Vector3 center = _attacker.transform.TransformPoint(_profile.localCenter);
            var query = _profile.includeTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

            int count = Physics.OverlapSphereNonAlloc(center, _profile.radius, _results, _profile.hitMask, query);
            for (int i = 0; i < count; i++)
            {
                Collider col = _results[i];
                if (col == null) continue;

                // Find Hurtbox on this collider or its parents
                Hurtbox hb = col.GetComponentInParent<Hurtbox>();
                if (hb == null) continue;

                int id = hb.GetInstanceID();
                if (_hitThisSwing.Contains(id))
                    continue;

                _hitThisSwing.Add(id);

                Vector3 point = col.ClosestPoint(center);
                Vector3 dir = (hb.transform.position - transform.position);
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.0001f) dir.Normalize();

                var ds = DamageSystem.Instance;
                if (ds != null)
                {
                    ds.TryApplyHit(
                        attacker: _attacker,
                        hurtbox: hb,
                        damageType: _damageType,
                        baseDamage: _damage,
                        baseStaggerDamage: _staggerDamage,
                        point: point,
                        direction: dir,
                        out _);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_profile == null || !_profile.drawGizmos || _attacker == null) return;

            // Match the runtime hit detection position by using the attacker's transform.
            Vector3 center = _attacker.transform.TransformPoint(_profile.localCenter);
            Gizmos.DrawWireSphere(center, _profile.radius);
        }
    }
}
