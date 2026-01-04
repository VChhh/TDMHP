// // Scripts/Animation/AnimationMarkerRelay.cs
// using UnityEngine;

// namespace TDMHP.Animation
// {
//     public class AnimationMarkerRelay : MonoBehaviour
//     {
//         [SerializeField] private CharacterAnimator characterAnimator;

//         private void Reset()
//         {
//             characterAnimator = GetComponentInParent<CharacterAnimator>();
//         }

//         // Call this from Unity Animation Events:
//         // Event function: EmitMarker
//         // String parameter: e.g. "fx:swing" / "sfx:step" / "debug:active_on"
//         public void EmitMarker(string markerId)
//         {
//             if (!characterAnimator) characterAnimator = GetComponentInParent<CharacterAnimator>();
//             if (!characterAnimator) return;

//             characterAnimator.RaiseMarker(markerId);
//         }
//     }
// }
