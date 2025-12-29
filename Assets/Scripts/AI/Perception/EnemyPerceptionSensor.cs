// Note: find player by tag, may require update for more complex scenarios

using UnityEngine;

namespace TDMHP.AI.Perception
{
    public sealed class EnemyPerceptionSensor : MonoBehaviour
    {
        [Header("Target acquisition")]
        [SerializeField] private string _playerTag = "Player";

        public Transform FindPlayer()
        {
            var go = GameObject.FindGameObjectWithTag(_playerTag);
            return go != null ? go.transform : null;
        }

        public bool IsInRange(Transform self, Transform target, float range)
        {
            if (self == null || target == null) return false;
            return (target.position - self.position).sqrMagnitude <= range * range;
        }
    }
}
