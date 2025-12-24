using UnityEngine;

namespace TDMHP.Combat.Damage
{
    public sealed class HitReactionReceiver : MonoBehaviour
    {
        public void React(in DamageRequest req, in DamageResult res)
        {
            // MVP: logs. Later: play animation, interrupt AI, apply knockback, etc.
            Debug.Log($"[React] {name} -> {res.reaction} (dmg={res.damageApplied:0.0}, stagger={res.staggerApplied:0.0}, critical={res.critical})");
        }
    }
}
