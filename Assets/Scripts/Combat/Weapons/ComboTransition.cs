using System;
using TDMHP.Input;
using UnityEngine;

namespace TDMHP.Combat.Weapons
{
    [Serializable]
    public struct ComboTransition
    {
        [Tooltip("Current move you are executing.")]
        public AttackMoveData from;

        [Tooltip("Input that triggers this transition (e.g., LightAttack, HeavyAttack).")]
        public CombatIntent intent;

        [Tooltip("Next move to switch into when the link window is open.")]
        public AttackMoveData to;
    }
}
