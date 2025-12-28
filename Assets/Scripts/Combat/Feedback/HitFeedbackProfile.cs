using System.Collections.Generic;
using UnityEngine;

namespace TDMHP.Combat.Feedback
{
    [CreateAssetMenu(menuName = "TDMHP/Combat/Feedback/Hit Feedback Profile")]
    public sealed class HitFeedbackProfile : ScriptableObject
    {
        [System.Serializable]
        public class Rule
        {
            [Header("Match (leave false to ignore)")]
            public bool requireCrit;
            public bool requireHeavy;
            public bool requireStagger;
            public bool requireKill;

            [Header("Output")]
            public HitFeedbackSpec spec;

            public bool Matches(in HitFeedbackEvent e)
            {
                if (requireCrit && !e.isCrit) return false;
                if (requireHeavy && !e.isHeavy) return false;
                if (requireStagger && !e.didStagger) return false;
                if (requireKill && !e.didKill) return false;
                return true;
            }
        }

        [SerializeField] private HitFeedbackSpec _defaultSpec;
        [SerializeField] private List<Rule> _rules = new();

        public HitFeedbackSpec Resolve(in HitFeedbackEvent e)
        {
            for (int i = 0; i < _rules.Count; i++)
                if (_rules[i].Matches(in e))
                    return _rules[i].spec;

            return _defaultSpec;
        }
    }
}
