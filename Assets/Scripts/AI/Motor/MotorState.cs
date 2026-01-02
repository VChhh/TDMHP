using UnityEngine;

namespace TDMHP.AI.Motor
{
    public readonly struct MotorState
    {
        public readonly bool isMoving;
        public readonly Vector3 velocity;
        public readonly Vector3 desiredDirection;

        public MotorState(bool isMoving, Vector3 velocity, Vector3 desiredDirection)
        {
            this.isMoving = isMoving;
            this.velocity = velocity;
            this.desiredDirection = desiredDirection;
        }

        public static MotorState Idle => new(false, Vector3.zero, Vector3.zero);
    }
}
