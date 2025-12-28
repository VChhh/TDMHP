namespace TDMHP.Combat.Feedback
{
    public interface IFeedbackModule
    {
        void Bind(FeedbackEventBus bus);
        void Unbind(FeedbackEventBus bus);
    }
}
