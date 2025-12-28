using UnityEngine;
using TDMHP.Combat.Damage;

namespace TDMHP.Combat.Feedback
{
    /// Only class that knows about DamageSystem.
    public sealed class FeedbackHub : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private DamageSystem _damageSystem;

        [Header("Bus + Profile")]
        [SerializeField] private HitFeedbackProfile _profile;

        // Keep bus as an object so modules can bind to it.
        public FeedbackEventBus Bus { get; private set; } = new FeedbackEventBus();

        private void Awake()
        {
            if (_damageSystem == null) _damageSystem = DamageSystem.Instance;
        }

        private void OnEnable()
        {
            if (_damageSystem == null) _damageSystem = DamageSystem.Instance;
            if (_damageSystem != null) _damageSystem.OnDamageResolved += OnDamageResolved;
        }

        private void OnDisable()
        {
            if (_damageSystem != null) _damageSystem.OnDamageResolved -= OnDamageResolved;
        }

        private void OnDamageResolved(DamageRequest req, DamageResult res)
        {
            // Adjust field names to your actual DamageResult/DamageRequest.
            if (!res.applied) return;

            var attacker = req.attacker != null ? req.attacker.gameObject : null;
            var target = req.target != null ? req.target.gameObject : null;

            Vector3 point = req.point;
            Vector3 dir = req.direction.sqrMagnitude > 0.0001f ? req.direction.normalized : Vector3.forward;

            var e0 = new HitFeedbackEvent(
                attacker: attacker,
                target: target,
                point: point,
                direction: dir,
                isCrit: res.critical,
                isHeavy: false,     // FIXME: no heavy info in DamageResult yet, need to be change either here or in DamageSystem
                didStagger: res.staggered,
                didKill: res.killed,
                damageApplied: res.damageApplied,
                spec: default
            );

            var spec = _profile != null ? _profile.Resolve(in e0) : default;

            var e = new HitFeedbackEvent(
                attacker, target, point, dir,
                e0.isCrit, e0.isHeavy, e0.didStagger, e0.didKill,
                e0.damageApplied,
                spec
            );

            Bus.Publish(in e);
        }
    }
}
