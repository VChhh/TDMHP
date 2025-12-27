using UnityEngine;

namespace TDMHP.Combat.UnscaledTime
{
    /// <summary>
    /// Shared combat time source. Based on unscaled time.
    /// Later you can add hitstop by setting timeScale = 0.
    /// </summary>
    public sealed class CombatClock : MonoBehaviour
    {
        [Range(0f, 2f)]
        [SerializeField] private float _timeScale = 1f;

        public double Now { get; private set; }          // combat-time seconds
        public float DeltaTime { get; private set; }     // combat-time delta this frame

        private double _lastUnscaled;

        private void Awake()
        {
            _lastUnscaled = Time.unscaledTimeAsDouble;
            Now = 0.0;
            DeltaTime = 0f;
        }

        private void Update()
        {
            double uNow = Time.unscaledTimeAsDouble;
            double uDt = uNow - _lastUnscaled;
            _lastUnscaled = uNow;

            double dt = uDt * _timeScale;
            DeltaTime = (float)dt;
            Now += dt;
        }

        public void SetTimeScale(float s) => _timeScale = Mathf.Clamp(s, 0f, 2f);
    }
}
