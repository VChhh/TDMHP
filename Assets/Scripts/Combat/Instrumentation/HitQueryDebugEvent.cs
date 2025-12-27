using UnityEngine;

namespace TDMHP.Combat.Instrumentation
{
    /// <summary>
    /// Debug-only snapshot of a hit query. Intended for immediate consumption (same frame).
    /// Keep it lightweight: no strings required.
    /// </summary>
    public readonly struct HitQueryDebugEvent
    {
        public readonly Transform emitter;         // who is querying
        public readonly HitQueryShapeType shape;

        public readonly Vector3 center;
        public readonly Quaternion rotation;       // for box/capsule orientation

        // Sphere
        public readonly float radius;

        // Box
        public readonly Vector3 halfExtents;

        // Capsule (axis = rotation * Vector3.up)
        public readonly float capsuleHeight;

        // Results (optional)
        public readonly Collider[] hits;           // buffer used by NonAlloc query (valid during callback)
        public readonly int hitCount;

        public readonly double time;               // Time.unscaledTimeAsDouble
        public readonly int attackId;              // optional identifier (hash, move index, etc.)

        public HitQueryDebugEvent(
            Transform emitter,
            HitQueryShapeType shape,
            Vector3 center,
            Quaternion rotation,
            float radius,
            Vector3 halfExtents,
            float capsuleHeight,
            Collider[] hits,
            int hitCount,
            double time,
            int attackId)
        {
            this.emitter = emitter;
            this.shape = shape;
            this.center = center;
            this.rotation = rotation;
            this.radius = radius;
            this.halfExtents = halfExtents;
            this.capsuleHeight = capsuleHeight;
            this.hits = hits;
            this.hitCount = hitCount;
            this.time = time;
            this.attackId = attackId;
        }
    }
}
