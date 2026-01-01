using UnityEngine;
using TDMHP.AI.Perception;
using TDMHP.AI.Motor;
using TDMHP.AI.Combat;

namespace TDMHP.AI
{
    public sealed class EnemyContext
    {
        public GameObject go { get; }
        public Transform tr { get; }

        public EnemyBlackboard bb { get; }
        public EnemyPerceptionSensor perception { get; }
        public EnemyMotorStub motor { get; }
        public EnemyCombatDriver combat { get; }

        public EnemyContext(
            GameObject go,
            EnemyBlackboard bb,
            EnemyPerceptionSensor perception,
            EnemyMotorStub motor,
            EnemyCombatDriver combat)
        {
            this.go = go;
            this.tr = go.transform;
            this.bb = bb;
            this.perception = perception;
            this.motor = motor;
            this.combat = combat;
        }
    }
}
