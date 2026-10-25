using UnityEngine;
using UnityEngine.EventSystems;

namespace GoonzuUI.Windows
{
    public sealed class DragHandle : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private RectTransform target; // the window root RectTransform
        [SerializeField] private Canvas canvas;

        private Vector2 _startLocalPointerPos;
        private Vector2 _startAnchoredPos;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!target || !canvas) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                target, eventData.position, eventData.pressEventCamera, out _startLocalPointerPos);
            _startAnchoredPos = target.anchoredPosition;

            target.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!target || !canvas) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                target, eventData.position, eventData.pressEventCamera, out var localPointerPos);

            var delta = localPointerPos - _startLocalPointerPos;
            target.anchoredPosition = _startAnchoredPos + delta;
        }
    }
}
