// Scripts/Animation/IMotionSource.cs
using UnityEngine;

namespace TDMHP.Animation
{
    public interface IMotionSource
    {
        Vector3 WorldVelocity { get; }   // e.g. from motor
        Vector3 WorldForward  { get; }   // facing direction
        bool IsGrounded { get; }
    }
}
