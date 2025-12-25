using UnityEngine;

namespace TDMHP.Combat.Aiming
{
    /// <summary>
    /// Projects ScreenCursor (screen pos) to a world aim point on ground.
    /// This becomes the single aim source for player facing + camera look-ahead.
    /// </summary>
    public sealed class CursorAimProvider : AimProvider
    {
        [Header("Refs")]
        [SerializeField] private ScreenCursor _cursor;
        [SerializeField] private Camera _camera;

        [Header("Ground projection")]
        [SerializeField] private bool _usePhysicsRaycast = true;
        [SerializeField] private LayerMask _groundMask = ~0;
        [SerializeField] private float _groundY = 0f;

        [Header("Stability")]
        [SerializeField] private bool _keepLastValid = true;

        private Vector3 _aimWorld;
        private bool _hasAim;

        public override bool HasAim => _hasAim;
        public override Vector3 AimWorldPoint => _aimWorld;

        private void Reset()
        {
            _camera = Camera.main;
        }

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_cursor == null || _camera == null)
            {
                _hasAim = false;
                return;
            }

            Vector2 screenPos = _cursor.ScreenPosition;
            if (TryProjectToWorld(screenPos, out Vector3 world))
            {
                _aimWorld = world;
                _hasAim = true;
            }
            else
            {
                _hasAim = _keepLastValid && _hasAim;
            }
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
