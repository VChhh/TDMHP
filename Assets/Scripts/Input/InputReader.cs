using System;
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

        private void Awake()
        {
            _actions = new TDMHPInputActions();
            _mode = _startingMode;
        }

        private void OnEnable()
        {
            EnableMapsForMode(_mode);
            HookButtonCallbacks();
        }

        private void OnDisable()
        {
            UnhookButtonCallbacks();
            _actions.Disable();
        }

        private void Update()
        {
            // Read continuous values every frame (more stable than relying only on callbacks).
            Move = _actions.Common.Move.ReadValue<Vector2>();
            Look = _actions.Common.Look.ReadValue<Vector2>();
        }

        public void SetCombatMode(CombatInputMode mode)
        {
            if (_mode == mode) return;
            _mode = mode;

            // Keep Common always enabled; switch combat map.
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

        private void HookButtonCallbacks()
        {
            // Common
            HookButton(_actions.Common.Dodge, CombatIntent.Dodge);
            HookButton(_actions.Common.Interact, CombatIntent.Interact);
            HookButton(_actions.Common.Sprint, CombatIntent.Sprint);
            HookButton(_actions.Common.Pause, CombatIntent.Pause);

            // Melee
            HookButton(_actions.Melee.LightAttack, CombatIntent.LightAttack);
            HookButton(_actions.Melee.HeavyAttack, CombatIntent.HeavyAttack);

            // Ranged
            HookButton(_actions.Ranged.Aim, CombatIntent.Aim);
            HookButton(_actions.Ranged.Shoot, CombatIntent.Shoot);
            HookButton(_actions.Ranged.Reload, CombatIntent.Reload);
        }

        private void UnhookButtonCallbacks()
        {
            // Common
            UnhookButton(_actions.Common.Dodge, CombatIntent.Dodge);
            UnhookButton(_actions.Common.Interact, CombatIntent.Interact);
            UnhookButton(_actions.Common.Sprint, CombatIntent.Sprint);
            UnhookButton(_actions.Common.Pause, CombatIntent.Pause);

            // Melee
            UnhookButton(_actions.Melee.LightAttack, CombatIntent.LightAttack);
            UnhookButton(_actions.Melee.HeavyAttack, CombatIntent.HeavyAttack);

            // Ranged
            UnhookButton(_actions.Ranged.Aim, CombatIntent.Aim);
            UnhookButton(_actions.Ranged.Shoot, CombatIntent.Shoot);
            UnhookButton(_actions.Ranged.Reload, CombatIntent.Reload);
        }

        private void HookButton(InputAction action, CombatIntent intent)
        {
            action.performed += ctx => RaiseIntent(intent, InputPhase.Pressed, ctx);
            action.canceled  += ctx => RaiseIntent(intent, InputPhase.Released, ctx);
        }

        private void UnhookButton(InputAction action, CombatIntent intent)
        {
            action.performed -= ctx => RaiseIntent(intent, InputPhase.Pressed, ctx);
            action.canceled  -= ctx => RaiseIntent(intent, InputPhase.Released, ctx);
            // Note: lambdas can’t be unsubscribed this way reliably in general.
            // We’ll improve this in the next iteration by caching delegates per action.
        }

        private void RaiseIntent(CombatIntent intent, InputPhase phase, InputAction.CallbackContext ctx)
        {
            float value = 1f;
            if (ctx.control is AxisControl)
            {
                value = ctx.ReadValue<float>();
            }

            OnIntent?.Invoke(new InputIntentEvent(intent, phase, ctx.time, value));
        }
    }
}