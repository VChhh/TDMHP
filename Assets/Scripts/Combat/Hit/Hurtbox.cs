using UnityEngine;

namespace TDMHP.Combat.Hit
{
    /// <summary>
    /// Put this on the enemy root (or a child). The collider that gets hit can be a child;
    /// we find Hurtbox via GetComponentInParent.
    /// </summary>
    public sealed class Hurtbox : MonoBehaviour
    {
        [SerializeField] private Health _health;

        private void Reset()
        {
            _health = GetComponentInParent<Health>();
        }

        public void ReceiveHit(in HitInfo hit)
        {
            if (_health != null)
            {
                _health.TakeDamage(hit.damage, hit.attacker);
            }
            else
            {
                Debug.Log($"[Hurtbox] {name} got hit for {hit.damage} (no Health assigned).");
            }
        }
    }
}
