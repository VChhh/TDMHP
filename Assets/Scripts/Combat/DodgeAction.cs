using UnityEngine;

namespace TDMHP.Combat
{
    public sealed class DodgeAction : PlayerAction
    {
        private const float Duration = 0.35f;
        private float _t;

        public DodgeAction(PlayerActionController controller) : base(controller) {}

        public override void Enter()
        {
            _t = 0f;
            Debug.Log("[Dodge] Enter");
        }

        public override void Tick(float dt)
        {
            _t += dt;

            // Simple: fast move, full turn control
            C.Motor.TickMove(
                moveInput: C.Input.Move,
                speedMultiplier: 2.0f,
                allowRotate: true,
                turnMultiplier: 1f
            );

            if (_t >= Duration)
                C.SwitchTo(new IdleAction(C));
        }
    }
}
