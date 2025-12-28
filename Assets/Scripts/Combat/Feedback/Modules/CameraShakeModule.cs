using UnityEngine;

namespace TDMHP.Combat.Feedback
{
    public sealed class CameraShakeModule : MonoBehaviour, IFeedbackModule
    {
        [SerializeField] private FeedbackHub _hub;

        private void Awake()
        {
            if (_hub == null) _hub = GetComponentInParent<FeedbackHub>();
        }

        private void OnEnable()  => Bind(_hub?.Bus);
        private void OnDisable() => Unbind(_hub?.Bus);

        public void Bind(FeedbackEventBus bus)
        {
            if (bus == null) return;
            bus.Hit += OnHit;
        }

        public void Unbind(FeedbackEventBus bus)
        {
            if (bus == null) return;
            bus.Hit -= OnHit;
        }

        private void OnHit(HitFeedbackEvent e)
        {
            // Placeholder: route into your camera system.
            // Example options:
            // - Cinemachine Impulse
            // - Custom perlin shake
            // - Your own CameraRig event bus
            float strength = e.spec.cameraShakeStrength;
            if (strength <= 0f) return;

            // TODO: implement your chosen shake backend.
            // Debug log for testing
            Debug.Log($"CameraShakeModule received HitFeedbackEvent, shake strength: {strength}");
        }
    }
}
