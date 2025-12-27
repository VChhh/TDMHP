using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TDMHP.Debugging
{
    /// <summary>
    /// Draws debug primitives in Game View (SRP + built-in).
    /// Use DebugDraw.* to enqueue primitives with an optional TTL.
    /// </summary>
    [DefaultExecutionOrder(-10000)]
    public sealed class RuntimeDebugDraw : MonoBehaviour
    {
        public static RuntimeDebugDraw Instance { get; private set; }

        [SerializeField] private CombatDebugSettings _settings;

        private struct LineCmd
        {
            public Vector3 a, b;
            public Color c;
            public double expireAt;
            public DebugDrawChannel channel;
            public bool depthTest;
        }

        private readonly List<LineCmd> _lines = new(2048);

        private Material _matDepth;
        private Material _matNoDepth;
        private bool _useSrp;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _useSrp = GraphicsSettings.currentRenderPipeline != null;
            EnsureMaterials();
        }

        private void OnEnable()
        {
            if (_useSrp)
                RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }

        private void OnDisable()
        {
            if (_useSrp)
                RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (_matDepth != null) Destroy(_matDepth);
            if (_matNoDepth != null) Destroy(_matNoDepth);
        }

        private bool ShouldDraw(DebugDrawChannel channel)
        {
            if (_settings == null) return Debug.isDebugBuild;

            if (!_settings.enabled) return false;
            if (_settings.onlyInDevelopmentBuild && !Debug.isDebugBuild) return false;

            return (_settings.channels & channel) != 0;
        }

        private void EnsureMaterials()
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            if (shader == null)
            {
                Debug.LogWarning("[RuntimeDebugDraw] Could not find Hidden/Internal-Colored shader.");
                return;
            }

            _matDepth = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            _matNoDepth = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };

            // Depth tested
            _matDepth.SetInt("_ZWrite", 0);
            _matDepth.SetInt("_Cull", (int)CullMode.Off);
            _matDepth.SetInt("_ZTest", (int)CompareFunction.LessEqual);

            // Always visible
            _matNoDepth.SetInt("_ZWrite", 0);
            _matNoDepth.SetInt("_Cull", (int)CullMode.Off);
            _matNoDepth.SetInt("_ZTest", (int)CompareFunction.Always);
        }

        private void CleanupExpired(double now)
        {
            for (int i = _lines.Count - 1; i >= 0; i--)
            {
                if (_lines[i].expireAt <= now)
                    _lines.RemoveAt(i);
            }
        }

        public void AddLine(Vector3 a, Vector3 b, Color c, float seconds, DebugDrawChannel ch, bool depthTest)
        {
            if (!ShouldDraw(ch)) return;
            if (_matDepth == null || _matNoDepth == null) EnsureMaterials();
            if (_matDepth == null) return;

            double now = Time.unscaledTimeAsDouble;
            double expire = seconds <= 0f ? now + 0.0001 : now + seconds;

            _lines.Add(new LineCmd
            {
                a = a, b = b, c = c,
                expireAt = expire,
                channel = ch,
                depthTest = depthTest
            });
        }

        public void AddCircleXZ(Vector3 center, float radius, Color c, int segments, float seconds, DebugDrawChannel ch, bool depthTest)
        {
            segments = Mathf.Max(8, segments);
            float step = Mathf.PI * 2f / segments;

            Vector3 prev = center + new Vector3(radius, 0f, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float ang = step * i;
                Vector3 next = center + new Vector3(Mathf.Cos(ang) * radius, 0f, Mathf.Sin(ang) * radius);
                AddLine(prev, next, c, seconds, ch, depthTest);
                prev = next;
            }
        }

        public void AddSphere(Vector3 center, float radius, Color c, int segments, float seconds, DebugDrawChannel ch, bool depthTest)
        {
            // 3 great circles
            AddCircleXZ(center, radius, c, segments, seconds, ch, depthTest);

            // XY
            segments = Mathf.Max(8, segments);
            float step = Mathf.PI * 2f / segments;
            Vector3 prev = center + new Vector3(radius, 0f, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float ang = step * i;
                Vector3 next = center + new Vector3(Mathf.Cos(ang) * radius, Mathf.Sin(ang) * radius, 0f);
                AddLine(prev, next, c, seconds, ch, depthTest);
                prev = next;
            }

            // YZ
            prev = center + new Vector3(0f, 0f, radius);
            for (int i = 1; i <= segments; i++)
            {
                float ang = step * i;
                Vector3 next = center + new Vector3(0f, Mathf.Sin(ang) * radius, Mathf.Cos(ang) * radius);
                AddLine(prev, next, c, seconds, ch, depthTest);
                prev = next;
            }
        }

        // Built-in pipeline path
        private void OnRenderObject()
        {
            if (_useSrp) return;
            RenderForCamera(Camera.current);
        }

        // SRP path (URP/HDRP)
        private void OnEndCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            RenderForCamera(cam);
        }

        private void RenderForCamera(Camera cam)
        {
            if (cam == null) return;
            if (_lines.Count == 0) return;

            double now = Time.unscaledTimeAsDouble;
            CleanupExpired(now);
            if (_lines.Count == 0) return;

            // Draw depth-tested then non-depth-tested
            DrawLines(_matDepth, depthTest: true);
            DrawLines(_matNoDepth, depthTest: false);
        }

        private void DrawLines(Material mat, bool depthTest)
        {
            mat.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);
            GL.Begin(GL.LINES);

            for (int i = 0; i < _lines.Count; i++)
            {
                var cmd = _lines[i];
                if (cmd.depthTest != depthTest) continue;
                GL.Color(cmd.c);
                GL.Vertex(cmd.a);
                GL.Vertex(cmd.b);
            }

            GL.End();
            GL.PopMatrix();
        }
    }
}
