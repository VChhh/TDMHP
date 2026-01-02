using UnityEngine;

namespace TDMHP.AI.Motor
{
    public interface IEnemyMotor
    {
        MotorState State { get; }

        void SetMoveTarget(Vector3 worldPos, float stopDistance);
        void ClearMoveTarget();

        void FaceTowards(Vector3 worldPos);
        void Stop();

        void SetSpeedMultiplier(float multiplier);

        // Optional but useful for combat feel
        void AddImpulse(Vector3 worldVelocity); // knockback etc.
    }
}
