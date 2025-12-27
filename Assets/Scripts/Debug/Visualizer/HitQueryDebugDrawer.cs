using UnityEngine;
using TDMHP.Combat.Instrumentation;
using TDMHP.Debugging; // uses DebugDraw + channels

namespace TDMHP.Debugging.Visualizers
{
    /// <summary>
    /// Subscribes to CombatInstrumentation.HitQuery and visualizes hit query shapes + hit points.
    /// Add this once in the scene (e.g., on GameSystems).
    /// </summary>
    public sealed class HitQueryDebugDrawer : MonoBehaviour
    {
        [Header("Draw")]
        [SerializeField] private bool _drawShape = true;
        [SerializeField] private bool _drawHitPoints = true;

        [SerializeField] private float _ttlSeconds = 0.05f;      // keep short; event fires every frame during active hit
        [SerializeField] private int _segments = 24;

        [Header("Colors")]
        [SerializeField] private Color _shapeColor = new Color(1f, 0.25f, 0.25f, 1f);
        [SerializeField] private Color _hitColor   = new Color(1f, 1f, 0.25f, 1f);

        [Header("Hit point marker")]
        [SerializeField] private float _hitCrossSize = 0.25f;
        [SerializeField] private float _yLift = 0.05f;

        private void OnEnable()
        {
            CombatInstrumentation.HitQuery += OnHitQuery;
        }

        private void OnDisable()
        {
            CombatInstrumentation.HitQuery -= OnHitQuery;
        }

        private void OnHitQuery(HitQueryDebugEvent e)
        {
            if (!_drawShape && !_drawHitPoints) return;

            // Draw shape
            if (_drawShape)
            {
                DrawShape(e);
            }

            // Draw hit points
            if (_drawHitPoints && e.hits != null && e.hitCount > 0)
            {
                Vector3 c = e.center + Vector3.up * _yLift;

                int n = Mathf.Min(e.hitCount, e.hits.Length);
                for (int i = 0; i < n; i++)
                {
                    Collider col = e.hits[i];
                    if (col == null) continue;

                    Vector3 p = col.ClosestPoint(e.center);
                    p.y = Mathf.Max(p.y, c.y);

                    DebugDraw.Line(c, p + Vector3.up * _yLift, _hitColor, _ttlSeconds, DebugDrawChannel.Combat, depthTest: true);
                    DebugDraw.Cross(p + Vector3.up * _yLift, _hitCrossSize, _hitColor, _ttlSeconds, DebugDrawChannel.Combat, depthTest: false);
                }
            }
        }

        private void DrawShape(HitQueryDebugEvent e)
        {
            Vector3 c = e.center + Vector3.up * _yLift;

            switch (e.shape)
            {
                case HitQueryShapeType.Sphere:
                    DebugDraw.Sphere(c, e.radius, _shapeColor, _segments, _ttlSeconds, DebugDrawChannel.Combat, depthTest: true);
                    break;

                case HitQueryShapeType.Box:
                    DrawBox(c, e.rotation, e.halfExtents, _shapeColor, _ttlSeconds);
                    break;

                case HitQueryShapeType.Capsule:
                    DrawCapsule(c, e.rotation, e.radius, e.capsuleHeight, _shapeColor, _ttlSeconds);
                    break;
            }
        }

        private void DrawBox(Vector3 center, Quaternion rot, Vector3 halfExtents, Color color, float ttl)
        {
            // 8 corners in world
            Vector3 r = rot * Vector3.right;
            Vector3 u = rot * Vector3.up;
            Vector3 f = rot * Vector3.forward;

            Vector3 ex = r * halfExtents.x;
            Vector3 ey = u * halfExtents.y;
            Vector3 ez = f * halfExtents.z;

            Vector3 c000 = center - ex - ey - ez;
            Vector3 c001 = center - ex - ey + ez;
            Vector3 c010 = center - ex + ey - ez;
            Vector3 c011 = center - ex + ey + ez;
            Vector3 c100 = center + ex - ey - ez;
            Vector3 c101 = center + ex - ey + ez;
            Vector3 c110 = center + ex + ey - ez;
            Vector3 c111 = center + ex + ey + ez;

            // bottom
            DebugDraw.Line(c000, c001, color, ttl, DebugDrawChannel.Combat, true);
            DebugDraw.Line(c001, c101, color, ttl, DebugDrawChannel.Combat, true);
            DebugDraw.Line(c101, c100, color, ttl, DebugDrawChannel.Combat, true);
            DebugDraw.Line(c100, c000, color, ttl, DebugDrawChannel.Combat, true);

            // top
            DebugDraw.Line(c010, c011, color, ttl, DebugDrawChannel.Combat, true);
            DebugDraw.Line(c011, c111, color, ttl, DebugDrawChannel.Combat, true);
            DebugDraw.Line(c111, c110, color, ttl, DebugDrawChannel.Combat, true);
            DebugDraw.Line(c110, c010, color, ttl, DebugDrawChannel.Combat, true);

            // verticals
            DebugDraw.Line(c000, c010, color, ttl, DebugDrawChannel.Combat, true);
            DebugDraw.Line(c001, c011, color, ttl, DebugDrawChannel.Combat, true);
            DebugDraw.Line(c100, c110, color, ttl, DebugDrawChannel.Combat, true);
            DebugDraw.Line(c101, c111, color, ttl, DebugDrawChannel.Combat, true);
        }

        private void DrawCapsule(Vector3 center, Quaternion rot, float radius, float height, Color color, float ttl)
        {
            // Capsule axis = rot * up
            Vector3 up = rot * Vector3.up;
            Vector3 right = rot * Vector3.right;
            Vector3 forward = rot * Vector3.forward;

            float half = Mathf.Max(0f, height * 0.5f);
            float stem = Mathf.Max(0f, half - radius);

            Vector3 a = center + up * stem;
            Vector3 b = center - up * stem;

            // End spheres (approx as circles in 3 planes)
            DrawOrientedCircle(a, right, forward, radius, color, ttl);
            DrawOrientedCircle(b, right, forward, radius, color, ttl);

            DrawOrientedCircle(a, right, up, radius, color, ttl);
            DrawOrientedCircle(b, right, up, radius, color, ttl);

            DrawOrientedCircle(a, forward, up, radius, color, ttl);
            DrawOrientedCircle(b, forward, up, radius, color, ttl);

            // Side lines (4 generatrix lines)
            DebugDraw.Line(a + right * radius, b + right * radius, color, ttl, DebugDrawChannel.Combat, true);
            DebugDraw.Line(a - right * radius, b - right * radius, color, ttl, DebugDrawChannel.Combat, true);
            DebugDraw.Line(a + forward * radius, b + forward * radius, color, ttl, DebugDrawChannel.Combat, true);
            DebugDraw.Line(a - forward * radius, b - forward * radius, color, ttl, DebugDrawChannel.Combat, true);
        }

        private void DrawOrientedCircle(Vector3 center, Vector3 axisA, Vector3 axisB, float radius, Color color, float ttl)
        {
            // axisA/axisB should be roughly orthonormal
            int seg = Mathf.Max(12, _segments);
            float step = Mathf.PI * 2f / seg;

            Vector3 prev = center + axisA * radius;
            for (int i = 1; i <= seg; i++)
            {
                float ang = step * i;
                Vector3 next = center + (axisA * Mathf.Cos(ang) + axisB * Mathf.Sin(ang)) * radius;
                DebugDraw.Line(prev, next, color, ttl, DebugDrawChannel.Combat, true);
                prev = next;
            }
        }
    }
}
