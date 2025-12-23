using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDMHP.Input
{
    /// <summary>
    /// Stores recent input intents for a short window (e.g., 0.2s).
    /// The action system will poll/consume buffered intents when allowed (cancel/link windows).
    /// </summary>
    public sealed class IntentBuffer : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private InputReader _reader;

        [Header("Buffer Window (seconds)")]
        [SerializeField] private float _defaultBufferSeconds = 0.20f;

        [Tooltip("Optional per-intent overrides. Leave empty to use Default Buffer Seconds.")]
        [SerializeField] private IntentOverride[] _overrides;

        [Serializable]
        private struct IntentOverride
        {
            public CombatIntent intent;
            public float bufferSeconds;
        }

        private struct Entry
        {
            public InputIntentEvent evt;
            public bool consumed;
        }

        // Key = (intent, phase) packed into an int.
        private readonly Dictionary<int, Entry> _entries = new();
        private readonly Dictionary<CombatIntent, float> _overrideLookup = new();

        private void Reset()
        {
            _reader = GetComponent<InputReader>();
        }

        private void Awake()
        {
            _overrideLookup.Clear();
            if (_overrides != null)
            {
                foreach (var o in _overrides)
                    _overrideLookup[o.intent] = Mathf.Max(0f, o.bufferSeconds);
            }
        }

        private void OnEnable()
        {
            if (_reader == null)
            {
                Debug.LogError("[IntentBuffer] Missing InputReader reference.", this);
                enabled = false;
                return;
            }

            _reader.OnIntent += OnIntent;
        }

        private void OnDisable()
        {
            if (_reader != null)
                _reader.OnIntent -= OnIntent;
        }

        private void OnIntent(InputIntentEvent e)
        {
            // We buffer BOTH Pressed and Released to stay generic.
            // Later you can choose to only consume Pressed for most gameplay.
            var key = MakeKey(e.Intent, e.Phase);

            _entries[key] = new Entry
            {
                evt = e,
                consumed = false
            };
        }

        public bool HasBuffered(CombatIntent intent, InputPhase phase = InputPhase.Pressed)
        {
            return TryPeek(intent, phase, out _);
        }

        public bool TryPeek(CombatIntent intent, InputPhase phase, out InputIntentEvent e)
        {
            var key = MakeKey(intent, phase);
            if (!_entries.TryGetValue(key, out var entry))
            {
                e = default;
                return false;
            }

            if (entry.consumed)
            {
                e = default;
                return false;
            }

            double now = Time.unscaledTimeAsDouble;
            float window = GetWindowSeconds(intent);

            if (now - entry.evt.Time > window)
            {
                // Expired
                _entries.Remove(key);
                e = default;
                return false;
            }

            e = entry.evt;
            return true;
        }

        public bool TryConsume(CombatIntent intent, out InputIntentEvent e)
        {
            return TryConsume(intent, InputPhase.Pressed, out e);
        }

        public bool TryConsume(CombatIntent intent, InputPhase phase, out InputIntentEvent e)
        {
            var key = MakeKey(intent, phase);
            if (!TryPeek(intent, phase, out e))
                return false;

            var entry = _entries[key];
            entry.consumed = true;
            _entries[key] = entry;
            return true;
        }

        /// <summary>
        /// Consume the first available buffered intent from a priority-ordered list.
        /// Useful when multiple inputs could be valid and you want deterministic priority.
        /// </summary>
        public bool TryConsumeFirst(ReadOnlySpan<CombatIntent> priorityList, InputPhase phase, out InputIntentEvent e)
        {
            for (int i = 0; i < priorityList.Length; i++)
            {
                if (TryConsume(priorityList[i], phase, out e))
                    return true;
            }

            e = default;
            return false;
        }

        public bool TryConsumeFirst(CombatIntent[] priorityList, InputPhase phase, out InputIntentEvent e)
        {
            if (priorityList == null)
            {
                e = default;
                return false;
            }

            for (int i = 0; i < priorityList.Length; i++)
            {
                if (TryConsume(priorityList[i], phase, out e))
                    return true;
            }

            e = default;
            return false;
        }

        public void ClearAll()
        {
            _entries.Clear();
        }

        public void ClearIntent(CombatIntent intent)
        {
            _entries.Remove(MakeKey(intent, InputPhase.Pressed));
            _entries.Remove(MakeKey(intent, InputPhase.Released));
        }

        private float GetWindowSeconds(CombatIntent intent)
        {
            if (_overrideLookup.TryGetValue(intent, out var s))
                return s;

            return Mathf.Max(0f, _defaultBufferSeconds);
        }

        private static int MakeKey(CombatIntent intent, InputPhase phase)
        {
            // Pack into int: [intent bits][phase bit]
            int p = (phase == InputPhase.Pressed) ? 0 : 1;
            return ((int)intent << 1) | p;
        }
    }
}