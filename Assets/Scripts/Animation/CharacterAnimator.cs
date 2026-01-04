// Scripts/Animation/CharacterAnimator.cs
using UnityEngine;

namespace TDMHP.Animation
{
    public sealed class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationLibrary _library;

        [Header("Time")]
        [SerializeField] private bool _useUnscaledAnimatorTime = true;

        public Animator Animator => _animator;
        public AnimationLibrary Library => _library;

        void Reset()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        void Awake()
        {
            if (_animator == null) _animator = GetComponentInChildren<Animator>();
            if (_animator != null)
                _animator.updateMode = _useUnscaledAnimatorTime
                    ? AnimatorUpdateMode.UnscaledTime
                    : AnimatorUpdateMode.Normal;
        }

        public bool PlayKey(string key)
        {
            if (_animator == null || _library == null) return false;
            if (!_library.TryGet(key, out var e) || e == null) return false;

            // Debug.Log($"CharacterAnimator: Playing animation '{e.stateName}' for key '{key}'");

            _animator.speed = Mathf.Max(0.01f, e.speed);
            int hash = Animator.StringToHash(e.stateName);
            _animator.CrossFadeInFixedTime(hash, Mathf.Max(0f, e.crossFade), e.layer);
            return true;
        }

        public void SetLocomotion(float moveX, float moveY, float speed01, bool isMoving)
        {
            if (_animator == null || _library == null) return;

            if (!string.IsNullOrEmpty(_library.moveXParam)) _animator.SetFloat(_library.moveXParam, moveX);
            if (!string.IsNullOrEmpty(_library.moveYParam)) _animator.SetFloat(_library.moveYParam, moveY);
            if (!string.IsNullOrEmpty(_library.speed01Param)) _animator.SetFloat(_library.speed01Param, speed01);
            if (!string.IsNullOrEmpty(_library.isMovingParam)) _animator.SetBool(_library.isMovingParam, isMoving);
        }
    }
}
