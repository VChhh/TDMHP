using UnityEngine;
using UnityEngine.InputSystem;

namespace TDMHP.Combat.Aiming
{
    /// <summary>
    /// Mouse/touch pointer aim: reads <Pointer>/position (screen) and projects to ground (raycast or plane).
    /// New Input System only.
    /// </summary>
    public sealed class PointerAimProvider : AimProvider
    {
        [Header("Refs")]
        [SerializeField] private UnityEngine.Camera _camera;

        [Header("Input")]
        [Tooltip("Bind to a Vector2 action like <Pointer>/position.")]
        [SerializeField] private InputActionReference _pointerPosition;

        [Header("Ground projection")]
        [SerializeField] private bool _usePhysicsRaycast = true;
        [SerializeField] private LayerMask _groundMask = ~0;
        [SerializeField] private float _groundY = 0f;

        [Header("Stability")]
        [Tooltip("If pointer ray misses, keep last valid aim point instead of losing aim.")]
        [SerializeField] private bool _keepLastValid = true;

        private Vector3 _aimWorld;
        private bool _hasAim;

        public override bool HasAim => _hasAim;
        public override Vector3 AimWorldPoint => _aimWorld;

        private void Reset()
        {
            _camera = UnityEngine.Camera.main;
        }

        private void Awake()
        {
            if (_camera == null) _camera = UnityEngine.Camera.main;
        }

        private void OnEnable()
        {
            _pointerPosition?.action?.Enable();
        }

        private void OnDisable()
        {
            _pointerPosition?.action?.Disable();
        }

        private void LateUpdate()
        {
            if (_camera == null)
            {
                _hasAim = false;
                return;
            }

            if (!TryGetPointerScreenPosition(out var screenPos))
            {
                _hasAim = false;
                return;
            }

            if (TryProjectToWorld(screenPos, out var world))
            {
                _aimWorld = world;
                _hasAim = true;
            }
            else
            {
                _hasAim = _keepLastValid && _hasAim;
            }
        }

        private bool TryGetPointerScreenPosition(out Vector2 screenPos)
        {
            if (_pointerPosition != null && _pointerPosition.action != null)
            {
                screenPos = _pointerPosition.action.ReadValue<Vector2>();
                return true;
            }

            // Fallback still within New Input System
            if (Mouse.current != null)
            {
                screenPos = Mouse.current.position.ReadValue();
                return true;
            }

            screenPos = default;
            return false;
        }

        private bool TryProjectToWorld(Vector2 screenPos, out Vector3 world)
        {
            Ray ray = _camera.ScreenPointToRay(screenPos);

            if (_usePhysicsRaycast)
            {
                if (Physics.Raycast(ray, out var hit, 500f, _groundMask, QueryTriggerInteraction.Ignore))
                {
                    world = hit.point;
                    return true;
                }
            }

            // Plane fallback at y = _groundY
            Plane plane = new Plane(Vector3.up, new Vector3(0f, _groundY, 0f));
            if (plane.Raycast(ray, out float enter))
            {
                world = ray.GetPoint(enter);
                return true;
            }

            world = default;
            return false;
        }
    }
}
