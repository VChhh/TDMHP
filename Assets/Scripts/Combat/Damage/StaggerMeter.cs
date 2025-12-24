using System;
using UnityEngine;

namespace TDMHP.Combat.Damage
{
    public sealed class StaggerMeter : MonoBehaviour
    {
        [SerializeField] private float _maxPoise = 50f;
        [SerializeField] private float _regenPerSecond = 10f;

        public float Max => _maxPoise;
        public float Current { get; private set; }

        public event Action<float, float> OnPoiseChanged; // (current, max)
        public event Action OnBroken;

        private void Awake()
        {
            Current = _maxPoise;
        }

        private void Update()
        {
            // Simple regen (tune later; often delayed regen is better)
            if (Current < _maxPoise)
            {
                Current = Mathf.Min(_maxPoise, Current + _regenPerSecond * Time.deltaTime);
                OnPoiseChanged?.Invoke(Current, _maxPoise);
            }
        }

        /// <returns>true if stagger triggered</returns>
        public bool ApplyStagger(float amount)
        {
            if (amount <= 0f) return false;

            Current -= amount;
            OnPoiseChanged?.Invoke(Current, _maxPoise);

            if (Current <= 0f)
            {
                Current = _maxPoise; // reset after break (classic “poise break” loop)
                OnPoiseChanged?.Invoke(Current, _maxPoise);
                OnBroken?.Invoke();
                return true;
            }

            return false;
        }
    }
}
