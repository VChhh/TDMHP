using System;
using UnityEngine;

namespace TDMHP.Combat.Resources
{
    [Serializable]
    public struct ResourceCost
    {
        public ResourceId id;
        [Min(0f)] public float amount;
    }
}
