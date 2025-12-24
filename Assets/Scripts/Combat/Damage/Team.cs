using UnityEngine;

namespace TDMHP.Combat.Damage
{
    public enum TeamId
    {
        Neutral = 0,
        Player = 1,
        Enemy = 2
    }

    public sealed class Team : MonoBehaviour
    {
        public TeamId teamId = TeamId.Neutral;
    }
}
