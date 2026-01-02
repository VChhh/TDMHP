using System.Collections.Generic;
using UnityEngine;
using TDMHP.Combat.HitDetection;
using TDMHP.UnscaledTime;


namespace TDMHP.Combat.Emitters
{
    /// <summary>
    /// Schedules and ticks hit emission windows in absolute unscaled time.
    /// Independent from actions and damage; it only drives hit detectors.
    /// </summary>
    [DefaultExecutionOrder(-50)]
    public sealed class EmitterSystem : MonoBehaviour
    {
        [SerializeField] private CombatClock _clock;


        private sealed class MeleeEmission
        {
            public int id;
            public bool cancelled;

            public MeleeHitDetector detector;
            public HitboxProfile profile;

            public double startTime;
            public double endTime;

            public MeleeEmissionPayload payload;

        }

        private readonly List<MeleeEmission> _melee = new(64);
        private int _nextId = 1;


        private void OnEnable()
        {
            if (_clock == null) _clock = FindFirstObjectByType<CombatClock>();
        }

        /// <summary>
        /// Schedule a melee hit query window (exact timing).
        /// startOffset/endOffset are seconds from now (unscaled).
        /// </summary>
        public EmissionHandle ScheduleMelee(MeleeHitDetector detector, HitboxProfile profile, float startOffset, float endOffset, MeleeEmissionPayload payload)
        {
            if (detector == null || profile == null) return default;
            if (endOffset <= startOffset) return default;

            double now = _clock != null ? _clock.Now : Time.unscaledTimeAsDouble;

            var e = new MeleeEmission
            {
                id = _nextId++,
                cancelled = false,
                detector = detector,
                profile = profile,
                startTime = now + startOffset,
                endTime = now + endOffset,
                payload = payload
            };

            _melee.Add(e);
            return new EmissionHandle(e.id);
        }

        public void Cancel(EmissionHandle handle)
        {
            if (!handle.IsValid) return;
            for (int i = 0; i < _melee.Count; i++)
            {
                if (_melee[i].id == handle.id)
                {
                    _melee[i].cancelled = true;
                    return;
                }
            }
        }

        private void Update()
        {
            double now = _clock != null ? _clock.Now : Time.unscaledTimeAsDouble;

            // Iterate backwards so we can remove safely
            for (int i = _melee.Count - 1; i >= 0; i--)
            {
                var e = _melee[i];

                if (e.cancelled || e.detector == null || e.profile == null)
                {
                    _melee.RemoveAt(i);
                    continue;
                }

                if (now >= e.endTime)
                {
                    _melee.RemoveAt(i);
                    continue;
                }

                if (now >= e.startTime)
                {
                    // attackId = emission id (stable during this window)
                    e.detector.TickHitQuery(e.profile, e.id);
                }
            }
        }

        public bool TryGetMeleePayload(int attackId, out MeleeEmissionPayload payload)
        {
            for (int i = 0; i < _melee.Count; i++)
            {
                var e = _melee[i];
                if (e.id == attackId && !e.cancelled)
                {
                    payload = e.payload;
                    return true;
                }
            }

            payload = default;
            return false;
        }
    }
}
