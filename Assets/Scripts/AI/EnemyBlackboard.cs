using UnityEngine;

namespace TDMHP.AI
{
    public sealed class EnemyBlackboard : MonoBehaviour
    {
        [Header("Target")]
        public Transform target;

        [Header("Runtime flags")]
        public bool isStaggered;
        public bool isDead;

        [Header("Timers")]
        public double staggerEndTime;
    }
}
