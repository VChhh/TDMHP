using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace TDMHP.Input
{
    /// <summary>
    /// Owns raw input -> high-level intents.
    /// No gameplay decisions
    /// </summary>
    public sealed class InputReader : MonoBehaviour
    {
        public event Action<InputIntentEvent> OnIntent;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }

        public bool SprintHeld => _actions != null && _actions.Common.Sprint.IsPressed();
        public bool AimHeld => _actions != null && _actions.Ranged.Aim.IsPressed();

        [SerializeField] private CombatInputMode _startingMode = CombatInputMode.Melee;

        private TDMHPInputActions _actions;
        private CombatInputMode _mode;

        private readonly Dictionary<InputAction, CombatIntent> _actionToIntent = new();

        private void Awake()
        {
            _actions = new TDMHPInputActions();
            _mode = _startingMode;

            BuildActionMap();
        }

        private void OnEnable()
        {
            EnableMapsForMode(_mode);

            foreach (var kv in _actionToIntent)
            {
                kv.Key.performed += OnActionPerformed;
                kv.Key.canceled  += OnActionCanceled;
            }
        }

        private void OnDisable()
        {
            foreach (var kv in _actionToIntent)
            {
                kv.Key.performed -= OnActionPerformed;
                kv.Key.canceled  -= OnActionCanceled;
            }

            _actions.Disable();
        }

        private void Update()
        {
            Move = _actions.Common.Move.ReadValue<Vector2>();
            Look = _actions.Common.Look.ReadValue<Vector2>();
        }

        public void SetCombatMode(CombatInputMode mode)
        {
            if (_mode == mode) return;
            _mode = mode;
            EnableMapsForMode(_mode);
        }

        private void EnableMapsForMode(CombatInputMode mode)
        {
            _actions.Disable();

            _actions.Common.Enable();

            switch (mode)
            {
                case CombatInputMode.Melee:
                    _actions.Melee.Enable();
                    break;
                case CombatInputMode.Ranged:
                    _actions.Ranged.Enable();
                    break;
                case CombatInputMode.None:
                default:
                    break;
            }
        }

        private void BuildActionMap()
        {
            _actionToIntent.Clear();

            // Common
            _actionToIntent[_actions.Common.Dodge]    = CombatIntent.Dodge;
            _actionToIntent[_actions.Common.Interact] = CombatIntent.Interact;
            _actionToIntent[_actions.Common.Sprint]   = CombatIntent.Sprint;
            _actionToIntent[_actions.Common.Pause]    = CombatIntent.Pause;

            // Melee
            _actionToIntent[_actions.Melee.LightAttack] = CombatIntent.LightAttack;
            _actionToIntent[_actions.Melee.HeavyAttack] = CombatIntent.HeavyAttack;

            // Ranged
            _actionToIntent[_actions.Ranged.Aim]    = CombatIntent.Aim;
            _actionToIntent[_actions.Ranged.Shoot]  = CombatIntent.Shoot;
            _actionToIntent[_actions.Ranged.Reload] = CombatIntent.Reload;
        }

        private void OnActionPerformed(InputAction.CallbackContext ctx)
        {
            if (!_actionToIntent.TryGetValue(ctx.action, out var intent))
                return;

            RaiseIntent(intent, InputPhase.Pressed, ctx);
        }

        private void OnActionCanceled(InputAction.CallbackContext ctx)
        {
            if (!_actionToIntent.TryGetValue(ctx.action, out var intent))
                return;

            RaiseIntent(intent, InputPhase.Released, ctx);
        }

        private void RaiseIntent(CombatIntent intent, InputPhase phase, InputAction.CallbackContext ctx)
        {
            // Use Unity time for buffer comparisons (simple + consistent).
            double t = Time.unscaledTimeAsDouble;

            float value = 1f;
            // ReadValue<float>() works for buttons/triggers; for buttons it's usually 0/1.
            try { value = ctx.ReadValue<float>(); }
            catch { value = 1f; }

            OnIntent?.Invoke(new InputIntentEvent(intent, phase, t, value));
        }
    }
}