using UnityEngine;
using UnityEngine.InputSystem;

namespace TDMHP.Combat.Damage
{
    public sealed class DamageTestDriver : MonoBehaviour
    {
        [SerializeField] private GameObject _attacker;
        [SerializeField] private MonoBehaviour _targetDamageable; // drag DamageableCharacter here
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _staggerDamage = 15f;

        private void Update()
        {
            if (Keyboard.current == null) return;
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                if (DamageSystem.Instance == null)
                {
                    Debug.LogError("[DamageTest] No DamageSystem in scene.");
                    return;
                }

                if (!(_targetDamageable is IDamageable d))
                {
                    Debug.LogError("[DamageTest] Target is not IDamageable.");
                    return;
                }

                var targetGo = _targetDamageable.gameObject;
                var req = new DamageRequest(
                    attacker: _attacker != null ? _attacker : gameObject,
                    target: targetGo,
                    damageType: DamageType.Slash,
                    damage: _damage,
                    staggerDamage: _staggerDamage,
                    isCritical: false,
                    point: targetGo.transform.position,
                    direction: (targetGo.transform.position - transform.position).normalized,
                    time: Time.unscaledTimeAsDouble
                );

                var res = d.ApplyDamage(req);
                Debug.Log($"[DamageTest] Applied -> {res.reaction}, HP after={res.healthAfter:0.0}");
            }
        }
    }
}
