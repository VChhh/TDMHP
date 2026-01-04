// Scripts/Animation/LocomotionAnimDriver.cs
using UnityEngine;

namespace TDMHP.Animation
{
    public sealed class LocomotionAnimDriver : MonoBehaviour
    {
        [SerializeField] private CharacterAnimator _characterAnimator;
        [SerializeField] private CharacterControllerMotionSource _motion;
        [SerializeField] private float _maxSpeed = 6f;

        void Reset()
        {
            _characterAnimator = GetComponent<CharacterAnimator>();
            _motion = GetComponent<CharacterControllerMotionSource>();
        }

        void Awake()
        {
            if (_characterAnimator == null) _characterAnimator = GetComponent<CharacterAnimator>();
            if (_motion == null) _motion = GetComponent<CharacterControllerMotionSource>();
        }

        void Update()
        {
            if (_characterAnimator == null || _motion == null) return;

            Vector3 v = _motion.Velocity;
            v.y = 0f;

            Vector3 fwd = _motion.Forward; fwd.y = 0f;
            if (fwd.sqrMagnitude < 0.0001f) fwd = Vector3.forward;
            fwd.Normalize();

            Vector3 right = Vector3.Cross(Vector3.up, fwd);

            float speed = v.magnitude;
            float denom = Mathf.Max(0.001f, _maxSpeed);

            float moveX = Vector3.Dot(v, right) / denom;
            float moveY = Vector3.Dot(v, fwd) / denom;
            float speed01 = Mathf.Clamp01(speed / denom);
            bool moving = speed > 0.05f;

            _characterAnimator.SetLocomotion(moveX, moveY, speed01, moving);
        }
    }
}
