using UnityEngine;
using TDMHP.AI;

namespace TDMHP.AI.FSM
{
    [CreateAssetMenu(menuName = "TDMHP/AI/FSM Brain")]
    public sealed class FSMBrainAsset : EnemyBrainAsset
    {
        [Header("Alerted")]
        public float alertedRange = 6f;
        public float chaseRange = 5f;
        public float attackRange = 2f;
        public float loseRange = 8f;

        [Header("Stagger")]
        public float staggerLockSeconds = 0.6f;

        public override IEnemyBrain CreateBrain() => new FSMBrain(this);
    }
}
