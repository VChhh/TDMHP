using UnityEngine;

namespace TDMHP.AI.BT
{
    [CreateAssetMenu(menuName = "TDMHP/AI/Behavior Tree")]
    public sealed class BehaviorTreeAsset : TDMHP.AI.EnemyBrainAsset
    {
        [SerializeReference] public BTNode root;

        public override TDMHP.AI.IEnemyBrain CreateBrain()
            => new BehaviorTreeBrain(this);
    }
}
