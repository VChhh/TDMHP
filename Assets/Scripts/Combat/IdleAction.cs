using TDMHP.Input;
using TDMHP.Combat.Weapons;

namespace TDMHP.Combat
{
    public sealed class IdleAction : PlayerAction
    {
        private static readonly CombatIntent[] PressPriority =
        {
            CombatIntent.Dodge,
            CombatIntent.HeavyAttack,
            CombatIntent.LightAttack
        };

        public IdleAction(PlayerActionController controller) : base(controller) {}

        public override void Tick(float dt)
        {
            // Free locomotion
            C.Motor.TickMove(
                moveInput: C.Input.Move,
                speedMultiplier: C.Input.SprintHeld ? 1.25f : 1f,
                allowRotate: true,
                turnMultiplier: 1f
            );

            if (!C.Buffer.TryConsumeFirst(PressPriority, InputPhase.Pressed, out var e))
                return;

            switch (e.Intent)
            {
                case CombatIntent.Dodge:
                    C.SwitchTo(new DodgeAction(C));
                    return;

                case CombatIntent.LightAttack:
                case CombatIntent.HeavyAttack:
                {
                    WeaponData w = C.Weapon;
                    if (w == null) return;

                    var entry = w.GetEntryMove(e.Intent);
                    if (entry == null) return;

                    C.SwitchTo(new AttackAction(C, entry));
                    return;
                }
            }
        }
    }
}
