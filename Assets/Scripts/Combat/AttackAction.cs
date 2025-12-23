using UnityEngine;
using TDMHP.Input;
using TDMHP.Combat.Weapons;
using TDMHP.Combat.Hit;


namespace TDMHP.Combat
{
    public sealed class AttackAction : PlayerAction
    {
        private readonly AttackMoveData _move;
        private float _t;
        private bool _hitRunning;

        public AttackAction(PlayerActionController controller, AttackMoveData move) : base(controller)
        {
            _move = move;
        }

        public override void Enter()
        {
            _t = 0f;
            _hitRunning = false;
            if (_move != null)
                Debug.Log($"[Attack] Enter {_move.name}");
        }

        public override void Tick(float dt)
        {
            if (_move == null)
            {
                C.SwitchTo(new IdleAction(C));
                return;
            }

            _t += dt;

            // Constrained locomotion during attacks (commitment)
            C.Motor.TickMove(
                moveInput: C.Input.Move,
                speedMultiplier: _move.moveMultiplier,
                allowRotate: true,
                turnMultiplier: _move.turnMultiplier
            );

            // Hit active window
            bool inActive = _t >= _move.activeStart && _t <= _move.activeEnd && _move.activeEnd > _move.activeStart;

            if (inActive)
            {
                if (!_hitRunning)
                {
                    _hitRunning = true;

                    if (C.HitDetector != null && _move.hitbox != null)
                        C.HitDetector.BeginSwing(_move.hitbox, C.gameObject, _move.damage);
                }

                C.HitDetector?.TickActive();
            }
            else if (_hitRunning && _t > _move.activeEnd)
            {
                // Leaving active window
                C.HitDetector?.EndSwing();
                _hitRunning = false;
            }

            
            // 1) Dodge cancel window
            if (InWindow(_t, _move.dodgeCancelStart, _move.dodgeCancelEnd))
            {
                if (C.Buffer.TryConsume(CombatIntent.Dodge, out _))
                {
                    C.SwitchTo(new DodgeAction(C));
                    return;
                }
            }

            // 2) Combo link window
            if (InWindow(_t, _move.comboLinkStart, _move.comboLinkEnd))
            {
                WeaponData w = C.Weapon;

                // Heavy priority, then Light (you can change per weapon later)
                if (TryChain(w, CombatIntent.HeavyAttack)) return;
                if (TryChain(w, CombatIntent.LightAttack)) return;
            }

            // 3) End
            if (_t >= _move.totalDuration)
                C.SwitchTo(new IdleAction(C));
        }

        public override void Exit()
        {
            if (_hitRunning && C.HitDetector != null)
                C.HitDetector.EndSwing();

            _hitRunning = false;
        }

        private bool TryChain(WeaponData weapon, CombatIntent intent)
        {
            if (!C.Buffer.TryConsume(intent, out _))
                return false;

            // Route via weapon combo graph first
            AttackMoveData next = weapon != null ? weapon.GetNextMove(_move, intent) : null;

            // Optional legacy fallback: if you previously used move.nextLight/nextHeavy
            // (If you want to remove those fields later, delete this fallback.)
            // if (next == null)
            // {
            //     if (intent == CombatIntent.HeavyAttack) next = _move.nextHeavy;
            //     else if (intent == CombatIntent.LightAttack) next = _move.nextLight;
            // }

            if (next == null)
                return false;

            C.SwitchTo(new AttackAction(C, next));
            return true;
        }

        private static bool InWindow(float t, float start, float end)
        {
            return t >= start && t <= end && end > start;
        }
    }
}
