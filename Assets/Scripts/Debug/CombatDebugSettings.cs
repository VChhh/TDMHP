using UnityEngine;

namespace TDMHP.Debugging
{
    [CreateAssetMenu(menuName = "TDMHP/Debug/Combat Debug Settings", fileName = "CombatDebugSettings")]
    public sealed class CombatDebugSettings : ScriptableObject
    {
        public bool enabled = true;

        [Tooltip("Draw only in development builds (recommended).")]
        public bool onlyInDevelopmentBuild = true;

        public DebugDrawChannel channels = DebugDrawChannel.All;

        [Header("Default Colors")]
        public Color aimColor = new Color(0.2f, 1f, 0.2f, 1f);
        public Color invulnColor = new Color(1f, 0.8f, 0.2f, 1f);
        public Color cameraColor = new Color(0.4f, 0.7f, 1f, 1f);
    }
}
