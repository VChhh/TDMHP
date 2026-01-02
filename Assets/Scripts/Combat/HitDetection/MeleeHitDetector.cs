using System;
using System.Collections.Generic;
using UnityEngine;
using TDMHP.Combat.Instrumentation;
using TDMHP.Combat.Damage;

namespace TDMHP.Combat.HitDetection
{
    /// <summary>
    /// Runs non-alloc overlap queries during active frames.
    /// Publishes debug instrumentation, and emits hit candidates to whoever consumes them (Emitter/Damage pipeline).
    /// </summary>
    public sealed class MeleeHitDetector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _origin;              // where shape local space is defined
        [SerializeField] private Transform _ownerRoot;           // used for ignoreOwnerHierarchy filtering

        [Header("Buffers")]
        [SerializeField] private int _bufferSize = 32;

        private Collider[] _hits;

        // Dedup (per attack)
        private int _lastAttackId = int.MinValue;

        // Dedup keys: either collider instance id OR damageable instance id.
        private readonly HashSet<int> _dedupSet = new HashSet<int>(128);

        /// <summary>
        /// Fired for each accepted hit candidate (after filters + dedup).
        /// Your emitter system can subscribe and route to DamageSystem.
        /// </summary>
        public event Action<HitCandidate> OnHitCandidate;

        private void Reset()
        {
            _origin = transform;
            _ownerRoot = transform.root;
        }

        private void Awake()
        {
            if (_origin == null) _origin = transform;
            if (_ownerRoot == null) _ownerRoot = transform.root;

            _hits = new Collider[Mathf.Max(8, _bufferSize)];
        }

        /// <summary>
        /// Call once per active-frame tick. attackId should be stable for the whole swing (e.g., move instance id).
        /// </summary>
        public void TickHitQuery(HitboxProfile profile, int attackId)
        {
            if (profile == null) return;
            if (_hits == null || _hits.Length != Mathf.Max(8, _bufferSize))
                _hits = new Collider[Mathf.Max(8, _bufferSize)];

            // Reset dedup when attack changes
            if (attackId != _lastAttackId)
            {
                _lastAttackId = attackId;
                _dedupSet.Clear();
            }

            // Query
            var res = HitQueryNonAlloc.Query(
                shape: profile.shape,
                origin: _origin,
                buffer: _hits,
                layerMask: profile.targetMask,
                qti: profile.queryTriggers
            );

            // TODO: apply damage through a separate layer/system
            // done below by a router

            // Publish instrumentation (debug-only hook; no-op in release)
            PublishInstrumentation(profile, attackId, res);

            // Route candidates
            int n = Mathf.Min(res.hitCount, _hits.Length);
            for (int i = 0; i < n; i++)
            {
                Collider col = _hits[i];
                if (col == null) continue;

                if (profile.ignoreOwnerHierarchy && _ownerRoot != null)
                {
                    if (col.transform.IsChildOf(_ownerRoot))
                        continue;
                }

                int key = profile.dedupPerDamageable
                    ? GetDamageableKey(col)
                    : col.GetInstanceID();

                if (key == 0) key = col.GetInstanceID();

                if (_dedupSet.Contains(key))
                    continue;

                _dedupSet.Add(key);
                OnHitCandidate?.Invoke(new HitCandidate(attackId, col, res.center));
            }
        }

        private int GetDamageableKey(Collider col)
        {
            // Prefer dedup per damageable root (avoids multi-collider enemies multi-hitting)
            // Replace "IDamageable" with your actual interface/type if needed.
            var dmg = col.GetComponentInParent<IDamageable>();
            if (dmg == null) return 0;

            if (dmg is Component c) return c.GetInstanceID();
            return dmg.GetHashCode();
        }

        private void PublishInstrumentation(HitboxProfile profile, int attackId, HitQueryNonAlloc.HitResult res)
        {
            // Map HitShapeType -> instrumentation enum
            HitQueryShapeType shapeType = res.type switch
            {
                HitShapeType.Sphere  => HitQueryShapeType.Sphere,
                HitShapeType.Box     => HitQueryShapeType.Box,
                HitShapeType.Capsule => HitQueryShapeType.Capsule,
                _ => HitQueryShapeType.Sphere
            };

            CombatInstrumentation.PublishHitQuery(
                new HitQueryDebugEvent(
                    emitter: _origin,
                    shape: shapeType,
                    center: res.center,
                    rotation: res.rotation,
                    radius: res.radius,
                    halfExtents: res.halfExtents,
                    capsuleHeight: res.capsuleHeight,
                    hits: _hits,
                    hitCount: res.hitCount,
                    time: Time.unscaledTimeAsDouble,
                    attackId: attackId
                )
            );
        }
    }
}
