using UnityEngine;

namespace TDMHP.Combat.HitDetection
{
    /// <summary>
    /// One unified shape definition. Stored in local-space of an origin transform (weapon, emitter, or character).
    /// </summary>
    [System.Serializable]
    public struct HitShape
    {
        public HitShapeType type;

        [Header("Local space (relative to origin)")]
        public Vector3 localCenter;
        public Vector3 localEuler; // local rotation for box/capsule axis

        [Header("Sphere / Capsule")]
        public float radius;

        [Header("Box")]
        public Vector3 halfExtents;

        [Header("Capsule")]
        public float capsuleHeight; // total height along local up axis

        public Quaternion LocalRotation => Quaternion.Euler(localEuler);
    }
}
