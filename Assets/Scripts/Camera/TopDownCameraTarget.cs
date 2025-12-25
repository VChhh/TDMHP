using UnityEngine;
using TDMHP.Combat.Aiming;

namespace TDMHP.Camera
{
    /// <summary>
    /// Cinemachine follows this transform.
    /// We move it around the player with optional look-ahead toward the current aim point.
    /// Uses AimProvider so it works with mouse, virtual cursor, lock-on, etc.
    /// </summary>
    public sealed class TopDownCameraTarget : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _player;
        [SerializeField] private AimProvider _aim;

        [Header("Look-ahead")]
        [SerializeField] private bool _enableLookAhead = true;

        [Tooltip("Max distance the follow target offsets toward aim (meters).")]
        [Min(0f)] [SerializeField] private float _maxLookAheadDistance = 3.0f;

        [Tooltip("0 = none, 1 = full max distance.")]
        [Range(0f, 1f)] [SerializeField] private float _lookAheadWeight = 0.8f;

        [Header("Smoothing")]
        [Min(0f)] [SerializeField] private float _smoothTime = 0.08f;

        private Vector3 _velocity;

        private void Reset()
        {
            // Try auto-find AimProvider on player if the script is on a child
            if (_player == null) _player = transform.parent;
            if (_aim == null && _player != null) _aim = _player.GetComponentInChildren<AimProvider>();
        }

        private void Update()
        {
            if (_player == null) return;

            Vector3 playerPos = _player.position;
            Vector3 desired = playerPos;

            if (_enableLookAhead && _aim != null && _aim.HasAim && _lookAheadWeight > 0f && _maxLookAheadDistance > 0f)
            {
                Vector3 aim = _aim.AimWorldPoint;
                Vector3 offset = aim - playerPos;
                offset.y = 0f;

                if (offset.sqrMagnitude > 0.0001f)
                {
                    float max = _maxLookAheadDistance * _lookAheadWeight;
                    if (offset.magnitude > max) offset = offset.normalized * max;
                    desired = playerPos + offset;
                }
            }

            desired.y = playerPos.y;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, _smoothTime);
        }

        // Expandability hooks
        public void SetLookAheadEnabled(bool enabled) => _enableLookAhead = enabled;
        public void SetLookAheadWeight(float w) => _lookAheadWeight = Mathf.Clamp01(w);
        public void SetPlayer(Transform player) => _player = player;
        public void SetAim(AimProvider aim) => _aim = aim;
    }
}
