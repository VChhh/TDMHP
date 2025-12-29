// Assets/Scripts/AI/EnemyEvents.cs

using System;
using UnityEngine;
using TDMHP.Combat.Damage;

namespace TDMHP.AI
{
    public enum EnemyEventType { Damaged, Staggered, Died }

    public readonly struct EnemyEvent
    {
        public readonly EnemyEventType type;
        public EnemyEvent(EnemyEventType type) { this.type = type; }
    }

    public sealed class EnemyEvents : MonoBehaviour
    {
        public event Action<EnemyEvent> EventRaised;

        private DamageableCharacter _damageable;

        private void Awake()
        {
            _damageable = GetComponent<DamageableCharacter>();
        }

        private void OnEnable()
        {
            if (_damageable == null) return;
            _damageable.OnDamaged += HandleDamaged;
            _damageable.OnStaggered += HandleStaggered;
            _damageable.OnDied += HandleDied;
        }

        private void OnDisable()
        {
            if (_damageable == null) return;
            _damageable.OnDamaged -= HandleDamaged;
            _damageable.OnStaggered -= HandleStaggered;
            _damageable.OnDied -= HandleDied;
        }

        private void HandleDamaged(DamageRequest req, DamageResult res)
            => EventRaised?.Invoke(new EnemyEvent(EnemyEventType.Damaged));

        private void HandleStaggered(DamageRequest req)
            => EventRaised?.Invoke(new EnemyEvent(EnemyEventType.Staggered));

        private void HandleDied(DamageRequest req)
            => EventRaised?.Invoke(new EnemyEvent(EnemyEventType.Died));
    }
}
