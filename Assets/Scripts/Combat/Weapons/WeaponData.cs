using System.Collections.Generic;
using TDMHP.Input;
using UnityEngine;

namespace TDMHP.Combat.Weapons
{
    [CreateAssetMenu(menuName = "TDMHP/Combat/Weapon", fileName = "Weapon_")]
    public sealed class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        public string weaponId = "weapon";
        public CombatInputMode inputMode = CombatInputMode.Melee;

        [Header("Entry Moves (from Idle)")]
        public AttackMoveData lightEntry;
        public AttackMoveData heavyEntry;

        [Header("Combo Graph (from move -> input -> move)")]
        public List<ComboTransition> transitions = new();

        public AttackMoveData GetEntryMove(CombatIntent intent)
        {
            return intent switch
            {
                CombatIntent.LightAttack => lightEntry,
                CombatIntent.HeavyAttack => heavyEntry,
                _ => null
            };
        }

        public AttackMoveData GetNextMove(AttackMoveData currentMove, CombatIntent intent)
        {
            if (currentMove == null) return null;

            // Simple linear search is fine for MVP. Later we can pre-build a dictionary.
            for (int i = 0; i < transitions.Count; i++)
            {
                var tr = transitions[i];
                if (tr.from == currentMove && tr.intent == intent)
                    return tr.to;
            }

            return null;
        }
    }
}
