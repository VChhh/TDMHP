// deprecated file
// using UnityEngine;

// namespace TDMHP.Combat.Hit
// {
//     public sealed class Health : MonoBehaviour
//     {
//         [SerializeField] private float _maxHealth = 100f;
//         public float Current { get; private set; }

//         private void Awake()
//         {
//             Current = _maxHealth;
//         }

//         public void TakeDamage(float amount, GameObject source)
//         {
//             if (amount <= 0f) return;

//             Current -= amount;
//             Debug.Log($"[Health] {name} took {amount} from {(source ? source.name : "null")} => {Current:0.0} HP");

//             if (Current <= 0f)
//             {
//                 Current = 0f;
//                 Debug.Log($"[Health] {name} died.");
//                 // Later: death event, ragdoll, disable AI, etc.
//             }
//         }
//     }
// }
