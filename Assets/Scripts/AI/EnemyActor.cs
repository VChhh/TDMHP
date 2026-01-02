using UnityEngine;
using TDMHP.AI.Perception;
using TDMHP.AI.Motor;
using TDMHP.AI.Combat;

namespace TDMHP.AI
{
    [DisallowMultipleComponent]
    public sealed class EnemyActor : MonoBehaviour
    {
        [SerializeField] private EnemyBlackboard _bb;
        [SerializeField] private EnemyPerceptionSensor _perception;
        [SerializeField] private EnemyMotorDriver _motor;
        [SerializeField] private EnemyCombatDriver _combat;

        private void Reset()
        {
            _bb = GetComponent<EnemyBlackboard>();
            if (_bb == null) _bb = gameObject.AddComponent<EnemyBlackboard>();

            _perception = GetComponent<EnemyPerceptionSensor>();
            if (_perception == null) _perception = gameObject.AddComponent<EnemyPerceptionSensor>();

            _motor = GetComponent<EnemyMotorDriver>();
            if (_motor == null) _motor = gameObject.AddComponent<EnemyMotorDriver>();

            _combat = GetComponent<EnemyCombatDriver>();
            if (_combat == null) _combat = gameObject.AddComponent<EnemyCombatDriver>();
        }

        public EnemyContext BuildContext()
        {
            if (_bb == null) _bb = GetComponent<EnemyBlackboard>();
            if (_perception == null) _perception = GetComponent<EnemyPerceptionSensor>();
            if (_motor == null) _motor = GetComponent<EnemyMotorDriver>();
            if (_combat == null) _combat = GetComponent<EnemyCombatDriver>();

            return new EnemyContext(gameObject, _bb, _perception, _motor, _combat);
        }
    }
}
