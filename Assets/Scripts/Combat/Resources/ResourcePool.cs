using System;
using UnityEngine;

namespace TDMHP.Combat.Resources
{
    [Serializable]
    public sealed class ResourcePool
    {
        public ResourceId id;

        [Min(0f)] public float max = 100f;
        [Min(0f)] public float start = 100f;

        [Header("Regen")]
        [Min(0f)] public float regenPerSecond = 25f;

        [Tooltip("After spending, wait this long before regen resumes.")]
        [Min(0f)] public float regenDelayAfterSpend = 0.5f;

        public float Current => _current;
        public float Max => max;

        public event Action<ResourceId, float, float> OnChanged; // (id, current, max)

        [NonSerialized] private float _current;
        [NonSerialized] private float _regenDelayRemaining;

        public void Initialize()
        {
            max = Mathf.Max(0f, max);
            start = Mathf.Clamp(start, 0f, max);
            _current = start;
            _regenDelayRemaining = 0f;

            OnChanged?.Invoke(id, _current, max);
        }

        public void Tick(float dt)
        {
            if (dt <= 0f) return;

            if (_regenDelayRemaining > 0f)
            {
                _regenDelayRemaining = Mathf.Max(0f, _regenDelayRemaining - dt);
                return;
            }

            if (regenPerSecond <= 0f) return;
            if (_current >= max) return;

            _current = Mathf.Min(max, _current + regenPerSecond * dt);
            OnChanged?.Invoke(id, _current, max);
        }

        public bool CanSpend(float amount)
        {
            if (amount <= 0f) return true;
            return _current >= amount;
        }

        public bool TrySpend(float amount)
        {
            if (!CanSpend(amount)) return false;

            if (amount > 0f)
            {
                _current -= amount;
                _regenDelayRemaining = Mathf.Max(_regenDelayRemaining, regenDelayAfterSpend);
                OnChanged?.Invoke(id, _current, max);
            }

            return true;
        }

        public void Add(float amount)
        {
            if (amount <= 0f) return;
            _current = Mathf.Min(max, _current + amount);
            OnChanged?.Invoke(id, _current, max);
        }
    }
}
