using UnityEngine;
using UnityEngine.InputSystem;

namespace TDMHP.Input
{
    /// <summary>
    /// Testing: press LightAttack, then press G within the buffer window to consume it.
    /// Set buffer window to ~1.0s for easy testing.
    /// </summary>
    public sealed class IntentBufferTester : MonoBehaviour
    {
        [SerializeField] private IntentBuffer _buffer;

        private void Reset()
        {
            _buffer = GetComponent<IntentBuffer>();
        }

        private void Update()
        {
            if (Keyboard.current == null) return;

            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                if (_buffer != null && _buffer.TryConsume(CombatIntent.LightAttack, out var e))
                    Debug.Log($"[BufferTester] Consumed buffered: {e}");
                else
                    Debug.Log("[BufferTester] Nothing buffered for LightAttack.");
            }
        }
    }
}