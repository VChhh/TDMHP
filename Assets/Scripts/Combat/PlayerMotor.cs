using UnityEngine;

namespace TDMHP.Combat
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _turnSpeed = 720f; // degrees/sec

        [Header("Optional camera for camera-relative movement")]
        [SerializeField] private Transform _cameraTransform;

        private CharacterController _cc;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
        }

        public void TickMove(Vector2 moveInput, float speedMultiplier, bool allowRotate, float turnMultiplier = 1f)
        {
            Vector3 world = ToWorld(moveInput);
            if (world.sqrMagnitude > 1f) world.Normalize();

            // Move (top-down: keep y = 0)
            Vector3 delta = world * (_moveSpeed * speedMultiplier) * Time.deltaTime;
            _cc.Move(delta);

            // Rotate toward move direction (if allowed)
            if (allowRotate && world.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(world, Vector3.up);
                float turn = _turnSpeed * turnMultiplier * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turn);
            }
        }

        private Vector3 ToWorld(Vector2 move)
        {
            Vector3 v = new Vector3(move.x, 0f, move.y);

            if (_cameraTransform == null)
                return v;

            // Camera-relative movement projected onto XZ plane
            Vector3 f = _cameraTransform.forward; f.y = 0f; f.Normalize();
            Vector3 r = _cameraTransform.right;   r.y = 0f; r.Normalize();
            return r * move.x + f * move.y;
        }
    }
}