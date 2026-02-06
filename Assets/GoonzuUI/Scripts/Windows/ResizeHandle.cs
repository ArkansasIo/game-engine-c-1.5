using UnityEngine;
using UnityEngine.EventSystems;

namespace GoonzuUI.Windows
{
    public sealed class ResizeHandle : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private Vector2 minSize = new Vector2(100, 100);
        [SerializeField] private Vector2 maxSize = new Vector2(1000, 1000);

        private Vector2 _startPointerPos;
        private Vector2 _startSize;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!target) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                target, eventData.position, eventData.pressEventCamera, out _startPointerPos);
            _startSize = target.sizeDelta;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!target) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                target, eventData.position, eventData.pressEventCamera, out var localPointerPos);
            var delta = localPointerPos - _startPointerPos;
            var newSize = _startSize + new Vector2(delta.x, -delta.y);
            newSize = Vector2.Max(minSize, Vector2.Min(maxSize, newSize));
            target.sizeDelta = newSize;
        }
    }
}
