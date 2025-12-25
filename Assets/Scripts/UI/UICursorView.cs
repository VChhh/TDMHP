using UnityEngine;
using TDMHP.Combat.Aiming;

namespace TDMHP.UI
{
    /// <summary>
    /// Positions a RectTransform at the ScreenCursor's screen position.
    /// Works best with Canvas = Screen Space - Overlay.
    /// </summary>
    public sealed class UICursorView : MonoBehaviour
    {
        [SerializeField] private ScreenCursor _cursor;
        [SerializeField] private RectTransform _cursorRect;

        private void Reset()
        {
            _cursorRect = transform as RectTransform;
        }

        private void LateUpdate()
        {
            if (_cursor == null || _cursorRect == null) return;

            // For Screen Space - Overlay, screen position == rect position
            _cursorRect.position = _cursor.ScreenPosition;
        }
    }
}
