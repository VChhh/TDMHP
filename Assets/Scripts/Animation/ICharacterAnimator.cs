// Scripts/Animation/ICharacterAnimator.cs
using System;

namespace TDMHP.Animation
{
    public interface ICharacterAnimator
    {
        event Action<string> OnMarker; // e.g. "fx:swing", "sfx:step"

        bool Play(string key, AnimPlayOptions options = default);
        void StopOverride(int layer = -1);

        // locomotion parameters (generic)
        void SetLocomotion(float moveX, float moveY, float speed01, bool isMoving);

        // optional generic parameters for special cases
        void SetFloat(string param, float value);
        void SetBool(string param, bool value);
        void SetInt(string param, int value);
        void SetTrigger(string param);
    }
}
