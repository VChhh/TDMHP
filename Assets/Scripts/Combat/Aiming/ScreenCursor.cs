using UnityEngine;
using UnityEngine.InputSystem;

namespace TDMHP.Combat.Aiming
{
    /// <summary>
    /// Maintains a single screen-space cursor for both mouse and controller.
    /// - Pointer mode: cursor follows <Pointer>/position
    /// - Stick mode: cursor moves by right stick * speed * dt
    /// </summary>
    public sealed class ScreenCursor : MonoBehaviour
    {
        public enum CursorMode { PointerAbsolute, StickRelative }

        [Header("Input (New Input System)")]
        [Tooltip("Optional PlayerInput to auto-switch modes based on control scheme.")]
        [SerializeField] private PlayerInput _playerInput;

        [Tooltip("Bind to <Pointer>/position (Vector2).")]
        [SerializeField] private InputActionReference _pointerPosition;

        [Tooltip("Bind to <Gamepad>/rightStick (Vector2).")]
        [SerializeField] private InputActionReference _aimStick;

        [Header("Mode")]
        [SerializeField] private CursorMode _mode = CursorMode.PointerAbsolute;

        [Tooltip("If PlayerInput is present, auto switch mode based on control scheme.")]
        [SerializeField] private bool _autoModeFromControlScheme = true;

        [Header("Stick settings")]
        [Min(0f)] [SerializeField] private float _stickSpeedPixelsPerSecond = 1400f;
        [Min(0f)] [SerializeField] private float _stickDeadzone = 0.15f;

        [Header("Bounds")]
        [SerializeField] private float _edgePaddingPixels = 8f;

        [Header("Presentation")]
        [SerializeField] private bool _hideHardwareCursorInStickMode = true;

        public Vector2 ScreenPosition => _screenPos;
        public CursorMode Mode => _mode;

        private Vector2 _screenPos;

        private void Reset()
        {
            _playerInput = GetComponentInParent<PlayerInput>();
        }

        private void Awake()
        {
            if (_playerInput == null) _playerInput = GetComponentInParent<PlayerInput>();

            // Start centered
            _screenPos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            ApplyCursorVisibility();
            ApplyAutoMode();
        }

        private void OnEnable()
        {
            _pointerPosition?.action?.Enable();
            _aimStick?.action?.Enable();

            if (_playerInput != null)
                _playerInput.onControlsChanged += OnControlsChanged;
        }

        private void OnDisable()
        {
            if (_playerInput != null)
                _playerInput.onControlsChanged -= OnControlsChanged;

            _pointerPosition?.action?.Disable();
            _aimStick?.action?.Disable();
        }

        private void Update()
        {
            ApplyAutoMode();

            switch (_mode)
            {
                case CursorMode.PointerAbsolute:
                    TickPointer();
                    break;

                case CursorMode.StickRelative:
                    TickStick();
                    break;
            }

            ClampToScreen();
        }

        private void TickPointer()
        {
            if (_pointerPosition == null || _pointerPosition.action == null) return;
            _screenPos = _pointerPosition.action.ReadValue<Vector2>();
        }

        private void TickStick()
        {
            if (_aimStick == null || _aimStick.action == null) return;

            Vector2 stick = _aimStick.action.ReadValue<Vector2>();
            if (stick.magnitude < _stickDeadzone) return;

            float dt = Time.unscaledDeltaTime;
            _screenPos += stick * (_stickSpeedPixelsPerSecond * dt);
        }

        private void ClampToScreen()
        {
            float minX = _edgePaddingPixels;
            float minY = _edgePaddingPixels;
            float maxX = Mathf.Max(minX, Screen.width - _edgePaddingPixels);
            float maxY = Mathf.Max(minY, Screen.height - _edgePaddingPixels);

            _screenPos.x = Mathf.Clamp(_screenPos.x, minX, maxX);
            _screenPos.y = Mathf.Clamp(_screenPos.y, minY, maxY);
        }

        private void OnControlsChanged(PlayerInput pi)
        {
            ApplyAutoMode();
            ApplyCursorVisibility();
        }

        private void ApplyAutoMode()
        {
            if (!_autoModeFromControlScheme || _playerInput == null) return;

            // Common default scheme names: "Keyboard&Mouse", "Gamepad"
            string scheme = _playerInput.currentControlScheme ?? "";
            bool gamepad = scheme.ToLower().Contains("gamepad");

            _mode = gamepad ? CursorMode.StickRelative : CursorMode.PointerAbsolute;
        }

        private void ApplyCursorVisibility()
        {
            if (!_hideHardwareCursorInStickMode) return;

            bool stickMode = _mode == CursorMode.StickRelative;
            Cursor.visible = !stickMode;
            Cursor.lockState = CursorLockMode.None;
        }

        public void SetMode(CursorMode mode)
        {
            _mode = mode;
            ApplyCursorVisibility();
        }

        public void WarpTo(Vector2 screenPos)
        {
            _screenPos = screenPos;
            ClampToScreen();
        }
    }
}
