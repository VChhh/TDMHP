using System;

namespace TDMHP.Debugging
{
    [Flags]
    public enum DebugDrawChannel
    {
        None   = 0,
        Aim    = 1 << 0,
        Camera = 1 << 1,
        Combat = 1 << 2,
        Damage = 1 << 3,
        All    = ~0
    }
}
