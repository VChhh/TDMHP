// Scripts/Animation/AnimationLibrary.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDMHP.Animation
{
    [CreateAssetMenu(menuName = "TDMHP/Animation/Animation Library", fileName = "AnimLibrary_")]
    public sealed class AnimationLibrary : ScriptableObject
    {
        [Serializable]
        public sealed class Entry
        {
            [Tooltip("Gameplay key. Example: 'Idle', 'Dodge', or AttackMoveData.name")]
            public string key;

            [Tooltip("Animator state name. Prefer FULL path: 'Base Layer.Attack_Slash1'")]
            public string stateName;

            public int layer = 0;
            public float crossFade = 0.08f;
            public float speed = 1f;
        }

        [Header("Locomotion Params (optional)")]
        public string moveXParam = "MoveX";
        public string moveYParam = "MoveY";
        public string speed01Param = "Speed01";
        public string isMovingParam = "IsMoving";

        public List<Entry> entries = new();

        Dictionary<string, Entry> _map;

        public bool TryGet(string key, out Entry entry)
        {
            _map ??= Build();
            return _map.TryGetValue(key, out entry);
        }

        Dictionary<string, Entry> Build()
        {
            var d = new Dictionary<string, Entry>(StringComparer.Ordinal);
            foreach (var e in entries)
            {
                if (e == null || string.IsNullOrWhiteSpace(e.key)) continue;
                d[e.key] = e;
            }
            return d;
        }
    }
}
