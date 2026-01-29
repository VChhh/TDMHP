using System;
using System.Collections.Generic;
using TDMHP.Combat;
using TDMHP.Input;
using UnityEngine;

namespace TDMHP.Combat.Weapons
{
    [Serializable]
    public struct MoveNodeLayout
    {
        public AttackMoveData move;
        public Vector2 position;
    }

    [Serializable]
    public struct ConnectionNodeLayout
    {
        public string id;
        public Vector2 position;
    }

    public enum GraphEndpointType
    {
        EntryLight,
        EntryHeavy,
        Move,
        Joint
    }

    [Serializable]
    public struct GraphEndpoint
    {
        public GraphEndpointType type;
        public AttackMoveData move;
        public string jointId;
    }

    [Serializable]
    public struct GraphEdgeData
    {
        public GraphEndpoint from;
        public GraphEndpoint to;
        public bool hasIntent;
        public CombatIntent intent;
    }

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

        [Header("Editor Layout (serialized for the graph editor)")]
        [SerializeField] public List<MoveNodeLayout> nodeLayout = new();
        [SerializeField] public List<ConnectionNodeLayout> connectionLayout = new();
        [SerializeField] public List<GraphEdgeData> graphEdges = new();

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
