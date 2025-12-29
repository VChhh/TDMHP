namespace TDMHP.AI.BT
{
    public abstract class BTNode
    {
        public virtual void OnStart(BTContext ctx) { }
        public abstract BTStatus Tick(BTContext ctx, float dt);
        public virtual void OnStop(BTContext ctx) { }
        public virtual void Reset() { }
    }
}
