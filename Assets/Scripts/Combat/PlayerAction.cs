namespace TDMHP.Combat
{
    public abstract class PlayerAction
    {
        protected readonly PlayerActionController C;

        protected PlayerAction(PlayerActionController controller)
        {
            C = controller;
        }

        public virtual void Enter() { }
        public abstract void Tick(float dt);
        public virtual void Exit() { }
    }
}