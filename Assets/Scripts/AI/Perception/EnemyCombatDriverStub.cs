using UnityEngine;

namespace TDMHP.AI.Combat
{
    public sealed class EnemyCombatDriverStub : MonoBehaviour
    {
        public bool CanAttack => false;

        public void RequestAttack()
        {
            // no-op until you implement enemy attacks
        }
    }
}
