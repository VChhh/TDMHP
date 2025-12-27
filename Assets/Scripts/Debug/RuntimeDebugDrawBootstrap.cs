using UnityEngine;

namespace TDMHP.Debugging
{
    public static class RuntimeDebugDrawBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Boot()
        {
            if (RuntimeDebugDraw.Instance != null) return;

            var go = new GameObject("RuntimeDebugDraw");
            go.AddComponent<RuntimeDebugDraw>();
        }
    }
}
