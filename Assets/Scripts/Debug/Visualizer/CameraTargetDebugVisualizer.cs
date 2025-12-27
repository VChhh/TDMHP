using UnityEngine;
using TDMHP.Debugging;

namespace TDMHP.Debugging.Visualizers
{
    public sealed class CameraTargetDebugVisualizer : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private float _height = 0.2f;
        [SerializeField] private Color _color = new Color(0.4f, 0.7f, 1f, 1f);

        private void LateUpdate()
        {
            if (_player == null) return;

            Vector3 p = _player.position + Vector3.up * _height;
            Vector3 t = transform.position + Vector3.up * _height;

            DebugDraw.Line(p, t, _color, 0f, DebugDrawChannel.Camera, depthTest: true);
            DebugDraw.Cross(t, 0.35f, _color, 0f, DebugDrawChannel.Camera, depthTest: true);
        }
    }
}
