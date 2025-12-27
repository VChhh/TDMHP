using UnityEngine;
using TDMHP.Combat.Damage;

namespace TDMHP.Combat.HitDetection
{
    public sealed class Hurtbox : MonoBehaviour
    {
        [Tooltip("Optional. If empty, we auto-find an IDamageable in parents.")]
        [SerializeField] private MonoBehaviour _damageableBehaviour;

        public IDamageable Damageable { get; private set; }
        public GameObject DamageableGameObject { get; private set; }

        private void Awake()
        {
            ResolveDamageable();
        }

        private void OnValidate()
        {
            // Helps in editor too
            ResolveDamageable();
        }

        private void ResolveDamageable()
        {
            Damageable = _damageableBehaviour as IDamageable;
            DamageableGameObject = _damageableBehaviour != null ? _damageableBehaviour.gameObject : null;

            if (Damageable != null) return;

            // Find any MonoBehaviour in parent chain that implements IDamageable
            Transform t = transform;
            while (t != null)
            {
                var mbs = t.GetComponents<MonoBehaviour>();
                for (int i = 0; i < mbs.Length; i++)
                {
                    if (mbs[i] is IDamageable d)
                    {
                        Damageable = d;
                        DamageableGameObject = mbs[i].gameObject;
                        _damageableBehaviour = mbs[i];
                        return;
                    }
                }
                t = t.parent;
            }
        }
    }
}
