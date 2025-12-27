using UnityEngine;
using TDMHP.Combat.Emitters;
using TDMHP.Combat.HitDetection;

namespace TDMHP.Combat.Damage
{
    public sealed class MeleeHitDamageRouter : MonoBehaviour
    {
        [SerializeField] private MeleeHitDetector _detector;
        [SerializeField] private EmitterSystem _emitterSystem;
        [SerializeField] private DamageSystem _damageSystem;

        private void Reset()
        {
            _detector = GetComponentInChildren<MeleeHitDetector>();
            _damageSystem = FindFirstObjectByType<DamageSystem>();
            _emitterSystem = FindFirstObjectByType<EmitterSystem>();
        }

        private void OnEnable()
        {
            if (_detector != null) _detector.OnHitCandidate += OnHitCandidate;
        }

        private void OnDisable()
        {
            if (_detector != null) _detector.OnHitCandidate -= OnHitCandidate;
        }

        private void OnHitCandidate(HitCandidate c)
        {
            if (_damageSystem == null) _damageSystem = DamageSystem.Instance;
            if (_damageSystem == null || _emitterSystem == null) return;

            if (!_emitterSystem.TryGetMeleePayload(c.attackId, out var payload))
                return;

            // We apply damage to a Hurtbox (your DamageSystem expects Hurtbox) :contentReference[oaicite:4]{index=4}
            var hurtbox = c.collider != null ? c.collider.GetComponentInParent<Hurtbox>() : null;
            if (hurtbox == null) return;

            Vector3 point = c.collider.ClosestPoint(c.queryCenter);

            Vector3 attackerPos = payload.attacker != null ? payload.attacker.transform.position : transform.position;
            Vector3 dir = point - attackerPos;
            if (dir.sqrMagnitude < 1e-6f) dir = (hurtbox.transform.position - attackerPos);
            dir.Normalize();

            _damageSystem.TryApplyHit(
                attacker: payload.attacker,
                hurtbox: hurtbox,
                damageType: payload.damageType,
                baseDamage: payload.baseDamage,
                baseStaggerDamage: payload.baseStagger,
                point: point,
                direction: dir,
                out _ // you can use result for extra feedback later
            );
        }
    }
}
