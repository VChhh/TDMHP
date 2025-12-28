using UnityEngine;

namespace TDMHP.Combat.Feedback
{
    public sealed class HitSfxModule : MonoBehaviour, IFeedbackModule
    {
        [SerializeField] private FeedbackHub _hub;
        [SerializeField] private AudioSource _audioSource;

        private void Awake()
        {
            if (_hub == null) _hub = GetComponentInParent<FeedbackHub>();
            if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
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
            // Debug log for testing
            Debug.Log("HitSfxModule received HitFeedbackEvent, playing SFX.");
            if (_audioSource == null || e.spec.sfx == null) return;
            _audioSource.PlayOneShot(e.spec.sfx);
        }
    }
}
