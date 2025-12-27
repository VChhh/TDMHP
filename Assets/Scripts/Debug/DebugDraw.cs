using UnityEngine;

namespace TDMHP.Debugging
{
    /// <summary>Static API used by visualizers.</summary>
    public static class DebugDraw
    {
        public static void Line(Vector3 a, Vector3 b, Color c, float seconds = 0f, DebugDrawChannel ch = DebugDrawChannel.All, bool depthTest = true)
        {
            RuntimeDebugDraw.Instance?.AddLine(a, b, c, seconds, ch, depthTest);
        }

        public static void Ray(Vector3 origin, Vector3 dir, Color c, float seconds = 0f, DebugDrawChannel ch = DebugDrawChannel.All, bool depthTest = true)
        {
            Line(origin, origin + dir, c, seconds, ch, depthTest);
        }

        public static void Cross(Vector3 p, float size, Color c, float seconds = 0f, DebugDrawChannel ch = DebugDrawChannel.All, bool depthTest = true)
        {
            var s = size * 0.5f;
            Line(p + Vector3.right * s, p - Vector3.right * s, c, seconds, ch, depthTest);
            Line(p + Vector3.forward * s, p - Vector3.forward * s, c, seconds, ch, depthTest);
        }

        public static void CircleXZ(Vector3 center, float radius, Color c, int segments = 24, float seconds = 0f, DebugDrawChannel ch = DebugDrawChannel.All, bool depthTest = true)
        {
            RuntimeDebugDraw.Instance?.AddCircleXZ(center, radius, c, segments, seconds, ch, depthTest);
        }

        public static void Sphere(Vector3 center, float radius, Color c, int segments = 18, float seconds = 0f, DebugDrawChannel ch = DebugDrawChannel.All, bool depthTest = true)
        {
            RuntimeDebugDraw.Instance?.AddSphere(center, radius, c, segments, seconds, ch, depthTest);
        }
    }
}
