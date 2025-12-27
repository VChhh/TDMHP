using UnityEngine;
using TDMHP.Debugging;
using TDMHP.Combat;
using TDMHP.Combat.Aiming;

namespace TDMHP.Debugging.Visualizers
{
    public sealed class AimDebugVisualizer : MonoBehaviour
    {
        [SerializeField] private PlayerActionController _player;
        [SerializeField] private float _crossSize = 0.5f;
        [SerializeField] private float _lineHeight = 0.15f;
        [SerializeField] private Color _color = new Color(0.2f, 1f, 0.2f, 1f);

        private void Reset()
        {
            _player = GetComponent<PlayerActionController>();
        }

        private void LateUpdate()
        {
            if (_player == null) return;

            AimProvider aim = _player.Aim;
            if (aim == null || !aim.HasAim) return;

            Vector3 p = _player.transform.position + Vector3.up * _lineHeight;
            Vector3 a = aim.AimWorldPoint + Vector3.up * _lineHeight;

            DebugDraw.Line(p, a, _color, 0f, DebugDrawChannel.Aim, depthTest: true);
            DebugDraw.Cross(a, _crossSize, _color, 0f, DebugDrawChannel.Aim, depthTest: true);
        }
    }
}
