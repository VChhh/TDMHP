using UnityEngine;
using TDMHP.Combat.Damage;

namespace TDMHP.Combat
{
    public sealed class DodgeAction : PlayerAction
    {
        private float _t;

        public DodgeAction(PlayerActionController controller) : base(controller) {}

        public override void Enter()
        {
            _t = 0f;

            // Spend resources (from your resource system)
            if (C.Resources != null)
            {
                if (!C.Resources.TrySpend(C.DodgeCosts, out var missing, out var amt))
                {
                    C.Reject($"Not enough {missing} to Dodge");
                    C.SwitchTo(new IdleAction(C));
                    return;
                }
            }

            var data = C.DodgeData;
            if (data == null)
            {
                Debug.LogWarning("[Dodge] Missing DodgeData on PlayerActionController.", C.gameObject);
                C.SwitchTo(new IdleAction(C));
                return;
            }

            // Schedule exact i-frames in absolute time
            if (C.Invulnerability != null && data.iFrameEnd > data.iFrameStart)
            {
                double t0 = Time.unscaledTimeAsDouble;
                double start = t0 + data.iFrameStart;
                double end   = t0 + data.iFrameEnd;

                C.Invulnerability.AddWindow(start, end);
            }

            Debug.Log("[Dodge] Enter");
        }

        public override void Tick(float dt)
        {
            var data = C.DodgeData;
            if (data == null)
            {
                C.SwitchTo(new IdleAction(C));
                return;
            }

            _t += dt;

            // Dodge movement
            C.Motor.TickMove(
                moveInput: C.Input.Move,
                speedMultiplier: data.speedMultiplier,
                allowRotate: false,
                turnMultiplier: 1f
            );

            if (C.Aim != null && C.Aim.HasAim)
                C.Motor.TickFaceTowards(C.Aim.AimWorldPoint, 1f);

            if (_t >= data.duration)
                C.SwitchTo(new IdleAction(C));
        }
    }
}
