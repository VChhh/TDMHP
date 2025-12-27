namespace TDMHP.Combat.Emitters
{
    /// <summary>Handle returned by EmitterSystem to allow cancellation.</summary>
    public readonly struct EmissionHandle
    {
        public readonly int id;

        public EmissionHandle(int id) => this.id = id;
        public bool IsValid => id != 0;
    }
}
