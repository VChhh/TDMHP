using UnityEngine;

namespace TDMHP.Combat.Aiming
{
    /// <summary>
    /// Abstract aim source. Can be mouse, virtual cursor, lock-on, etc.
    /// </summary>
    public abstract class AimProvider : MonoBehaviour
    {
        /// <summary>True if we have a valid aim point this frame.</summary>
        public abstract bool HasAim { get; }

        /// <summary>World-space aim point (typically on ground plane).</summary>
        public abstract Vector3 AimWorldPoint { get; }
    }
}
