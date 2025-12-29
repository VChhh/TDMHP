namespace TDMHP.AI.BT
{
    public sealed class BTContext
    {
        public readonly TDMHP.AI.EnemyContext enemy;
        public BTContext(TDMHP.AI.EnemyContext enemy) { this.enemy = enemy; }
    }
}
