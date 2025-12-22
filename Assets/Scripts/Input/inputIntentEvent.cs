namespace TDMHP.Input
{
    /// <summary>
    /// High-level input event emitted by the InputReader.
    /// This is what the buffer/action system will consume.
    /// </summary>
    public readonly struct InputIntentEvent
    {
        public readonly CombatIntent Intent;
        public readonly InputPhase Phase;
        public readonly double Time;   // InputSystem timestamp (seconds since startup)
        public readonly float Value;   // Useful for analog triggers; default 1

        public InputIntentEvent(CombatIntent intent, InputPhase phase, double time, float value = 1f)
        {
            Intent = intent;
            Phase = phase;
            Time = time;
            Value = value;
        }

        public override string ToString() => $"{Time:F3} {Intent} {Phase} v={Value:F2}";
    }
}
