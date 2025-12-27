using UnityEngine;

namespace TDMHP.Combat.HitDetection
{
    public static class HitQueryNonAlloc
    {
        public struct HitResult
        {
            public int hitCount;
            public Vector3 center;
            public Quaternion rotation;

            // For instrumentation/debug draw:
            public float radius;
            public Vector3 halfExtents;
            public float capsuleHeight;
            public HitShapeType type;
        }

        public static HitResult Query(
            in HitShape shape,
            Transform origin,
            Collider[] buffer,
            int layerMask,
            QueryTriggerInteraction qti)
        {
            // Transform shape from local to world
            Vector3 center = origin.TransformPoint(shape.localCenter);
            Quaternion rot = origin.rotation * shape.LocalRotation;

            HitResult r = new HitResult
            {
                type = shape.type,
                center = center,
                rotation = rot,
                radius = shape.radius,
                halfExtents = shape.halfExtents,
                capsuleHeight = shape.capsuleHeight,
                hitCount = 0
            };

            switch (shape.type)
            {
                case HitShapeType.Sphere:
                    r.hitCount = Physics.OverlapSphereNonAlloc(center, shape.radius, buffer, layerMask, qti);
                    break;

                case HitShapeType.Box:
                    r.hitCount = Physics.OverlapBoxNonAlloc(center, shape.halfExtents, buffer, rot, layerMask, qti);
                    break;

                case HitShapeType.Capsule:
                    // We’ll implement next step (after Box). For now, keep safe.
                    // r.hitCount = ...
                    r.hitCount = 0;
                    break;
            }

            return r;
        }
    }
}
