using System;
using UnityEngine;

namespace TDMHP.Combat.Damage
{
    public sealed class HealthComponent : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 100f;
        public float Max => _maxHealth;
        public float Current { get; private set; }

        public bool IsDead => Current <= 0f;

        public event Action<float, float> OnHealthChanged; // (current, max)
        public event Action OnDied;

        private void Awake()
        {
            Current = _maxHealth;
        }

        public void ApplyDamage(float amount)
        {
            if (IsDead || amount <= 0f) return;

            Current = Mathf.Max(0f, Current - amount);
            OnHealthChanged?.Invoke(Current, _maxHealth);

            if (Current <= 0f)
                OnDied?.Invoke();
        }
    }
}
