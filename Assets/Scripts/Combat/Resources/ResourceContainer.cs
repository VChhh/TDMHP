using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDMHP.Combat.Resources
{
    public sealed class ResourceContainer : MonoBehaviour
    {
        [SerializeField] private List<ResourcePool> _pools = new();

        private readonly Dictionary<ResourceId, ResourcePool> _map = new();

        public event Action<ResourceId, float> OnSpendFailed; // (missingId, missingAmount)

        private void Awake()
        {
            _map.Clear();
            for (int i = 0; i < _pools.Count; i++)
            {
                var p = _pools[i];
                if (p == null) continue;

                p.Initialize();
                _map[p.id] = p;
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            foreach (var kv in _map)
                kv.Value.Tick(dt);
        }

        public float Get(ResourceId id) => _map.TryGetValue(id, out var p) ? p.Current : 0f;
        public float GetMax(ResourceId id) => _map.TryGetValue(id, out var p) ? p.Max : 0f;

        public bool CanAfford(ResourceCost[] costs)
        {
            if (costs == null || costs.Length == 0) return true;

            for (int i = 0; i < costs.Length; i++)
            {
                var c = costs[i];
                if (!_map.TryGetValue(c.id, out var p)) return false;
                if (!p.CanSpend(c.amount)) return false;
            }
            return true;
        }

        public bool TrySpend(ResourceCost[] costs, out ResourceId missingId, out float missingAmount)
        {
            missingId = default;
            missingAmount = 0f;

            if (costs == null || costs.Length == 0) return true;

            // First pass: check all
            for (int i = 0; i < costs.Length; i++)
            {
                var c = costs[i];
                if (!_map.TryGetValue(c.id, out var p))
                {
                    missingId = c.id;
                    missingAmount = c.amount;
                    OnSpendFailed?.Invoke(missingId, missingAmount);
                    return false;
                }

                if (!p.CanSpend(c.amount))
                {
                    missingId = c.id;
                    missingAmount = c.amount;
                    OnSpendFailed?.Invoke(missingId, missingAmount);
                    return false;
                }
            }

            // Second pass: spend all (safe because we checked)
            for (int i = 0; i < costs.Length; i++)
            {
                var c = costs[i];
                _map[c.id].TrySpend(c.amount);
            }

            return true;
        }
    }
}
