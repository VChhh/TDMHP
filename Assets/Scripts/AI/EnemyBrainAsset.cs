using UnityEngine;

namespace TDMHP.AI
{
    public abstract class EnemyBrainAsset : ScriptableObject
    {
        public abstract IEnemyBrain CreateBrain();
    }
}
