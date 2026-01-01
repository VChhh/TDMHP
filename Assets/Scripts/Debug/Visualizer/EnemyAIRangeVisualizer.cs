using System.Reflection;
using UnityEngine;
using TDMHP.Debugging;
using TDMHP.AI;

namespace TDMHP.Debugging.Visualizers
{
    /// <summary>
    /// Draws enemy AI ranges (attack, alerted, lose) using DebugDraw.
    /// Can read an assigned EnemyBrainAsset directly or extract it from an EnemyBrainRunner on the same GameObject.
    /// Uses reflection to remain loosely coupled to concrete brain asset types.
    /// </summary>
    public sealed class EnemyAIRangeVisualizer : MonoBehaviour
    {
        [Header("Source")]
        [Tooltip("Optional: assign the EnemyBrainRunner on the same GameObject (will be found automatically if empty)")]
        [SerializeField] private EnemyBrainRunner _runner;
        [Tooltip("Optional: assign the brain asset directly to avoid reflection")]
        [SerializeField] private EnemyBrainAsset _brainAsset;

        [Header("Drawing")]
        [SerializeField] private bool _draw = true;
        [SerializeField] private Color _attackColor = new Color(1f, 0.3f, 0.3f, 1f);
        [SerializeField] private Color _alertColor = new Color(1f, 0.9f, 0.3f, 1f);
        [SerializeField] private Color _loseColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        [SerializeField] private float _heightOffset = 0.02f;
        [SerializeField] private int _segments = 48;
        [SerializeField] private DebugDrawChannel _channel = DebugDrawChannel.Combat;

        private void Reset()
        {
            if (_runner == null) _runner = GetComponent<EnemyBrainRunner>();
        }

        private void LateUpdate()
        {
            if (!_draw) return;

            var asset = _brainAsset ?? GetBrainAssetFromRunner(_runner);
            if (asset == null) return;

            float alerted = ReadFloatFieldSafe(asset, "alertedRange");
            float attack = ReadFloatFieldSafe(asset, "attackRange");
            float lose = ReadFloatFieldSafe(asset, "loseRange");

            Vector3 c = transform.position + Vector3.up * _heightOffset;
            if (attack > 0f) DebugDraw.CircleXZ(c, attack, _attackColor, _segments, 0f, _channel, depthTest: false);
            if (alerted > 0f) DebugDraw.CircleXZ(c, alerted, _alertColor, _segments, 0f, _channel, depthTest: false);
            if (lose > 0f) DebugDraw.CircleXZ(c, lose, _loseColor, _segments, 0f, _channel, depthTest: false);
        }

        private EnemyBrainAsset GetBrainAssetFromRunner(EnemyBrainRunner runner)
        {
            if (runner == null) runner = GetComponent<EnemyBrainRunner>();
            if (runner == null) return null;

            var field = runner.GetType().GetField("_brainAsset", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null) return null;
            return field.GetValue(runner) as EnemyBrainAsset;
        }

        private float ReadFloatFieldSafe(object src, string name)
        {
            if (src == null) return 0f;
            var f = src.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (f == null) return 0f;
            var v = f.GetValue(src);
            if (v is float fval) return fval;
            return 0f;
        }
    }
}
