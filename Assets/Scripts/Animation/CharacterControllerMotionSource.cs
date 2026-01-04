// Scripts/Animation/CharacterControllerMotionSource.cs
using UnityEngine;

namespace TDMHP.Animation
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class CharacterControllerMotionSource : MonoBehaviour
    {
        private CharacterController _cc;

        public Vector3 Velocity => _cc != null ? _cc.velocity : Vector3.zero;
        public Vector3 Forward => transform.forward;

        void Awake() => _cc = GetComponent<CharacterController>();
    }
}
