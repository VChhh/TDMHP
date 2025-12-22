using UnityEngine;

namespace TDMHP.Input
{
    public sealed class InputDebugLogger : MonoBehaviour
    {
        [SerializeField] private InputReader _reader;

        private void Reset()
        {
            _reader = GetComponent<InputReader>();
        }

        private void OnEnable()
        {
            if (_reader == null) return;
            _reader.OnIntent += HandleIntent;
        }

        private void OnDisable()
        {
            if (_reader == null) return;
            _reader.OnIntent -= HandleIntent;
        }

        private void HandleIntent(InputIntentEvent e)
        {
            Debug.Log(e.ToString());
        }
    }
}