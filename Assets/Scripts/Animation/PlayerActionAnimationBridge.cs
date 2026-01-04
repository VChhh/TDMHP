// Scripts/Animation/PlayerActionAnimationBridge.cs
using UnityEngine;
using TDMHP.Combat;

namespace TDMHP.Animation
{
    public sealed class PlayerActionAnimationBridge : MonoBehaviour
    {
        [SerializeField] private PlayerActionController _actions;
        [SerializeField] private CharacterAnimator _anim;

        void Reset()
        {
            _actions = GetComponent<PlayerActionController>();
            _anim = GetComponent<CharacterAnimator>();
        }

        void OnEnable()
        {
            if (_actions == null) _actions = GetComponent<PlayerActionController>();
            if (_anim == null) _anim = GetComponent<CharacterAnimator>();

            if (_actions != null)
                _actions.OnActionChanged += HandleActionChanged;
        }

        void OnDisable()
        {
            if (_actions != null)
                _actions.OnActionChanged -= HandleActionChanged;
        }

        void HandleActionChanged(PlayerAction prev, PlayerAction next)
        {
            if (_anim == null || next == null) return;

            // Attack: use AttackMoveData.name as the key (data-driven)
            if (next is AttackAction atk && atk.MoveData != null)
            {
                // key should match an entry in AnimationLibrary
                Debug.Log($"PlayerActionAnimationBridge: Playing attack animation '{atk.MoveData.name}'");
                _anim.PlayKey(atk.MoveData.name);
                return;
            }

            // Optional: dodge etc by type name (works before you add more exposure)
            var typeName = next.GetType().Name;
            if (_anim.PlayKey(typeName)) return;

            // If nothing mapped, do nothing (locomotion blend tree keeps running)
        }
    }
}
