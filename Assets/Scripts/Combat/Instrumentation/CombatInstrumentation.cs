using System;

namespace TDMHP.Combat.Instrumentation
{
    /// <summary>
    /// Debug instrumentation hook points. In non-dev builds, this compiles to no-ops.
    /// Combat code can safely call Publish* without depending on any debug assembly.
    /// </summary>
    public static class CombatInstrumentation
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public static event Action<HitQueryDebugEvent> HitQuery;

        public static void PublishHitQuery(in HitQueryDebugEvent e)
        {
            HitQuery?.Invoke(e);
        }
#else
        // No-op in release builds
        public static event Action<HitQueryDebugEvent> HitQuery { add { } remove { } }
        public static void PublishHitQuery(in HitQueryDebugEvent e) { }
#endif
    }
}
