using System;

namespace TDMHP.Combat.Feedback
{
    public sealed class FeedbackEventBus
    {
        public event Action<HitFeedbackEvent> Hit;

        public void Publish(in HitFeedbackEvent e) => Hit?.Invoke(e);
    }
}
