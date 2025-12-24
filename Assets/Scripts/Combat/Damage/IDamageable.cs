namespace TDMHP.Combat.Damage
{
    public interface IDamageable
    {
        DamageResult ApplyDamage(in DamageRequest request);
    }
}
