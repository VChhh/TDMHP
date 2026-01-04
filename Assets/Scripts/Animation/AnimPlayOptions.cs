// Scripts/Animation/AnimPlayOptions.cs
namespace TDMHP.Animation
{
    public enum AnimClock { ScaledTime, UnscaledTime }

    public struct AnimPlayOptions
    {
        public float CrossFade;     // override default if > 0
        public float Speed;         // 1 = normal
        public int Priority;        // higher overrides lower
        public float HoldSeconds;   // if > 0, auto-release override after this time
        public AnimClock Clock;     // scaled/unscaled for HoldSeconds timer

        public static AnimPlayOptions Default => new AnimPlayOptions
        {
            CrossFade = 0f,
            Speed = 1f,
            Priority = 0,
            HoldSeconds = 0f,
            Clock = AnimClock.ScaledTime
        };
    }
}
