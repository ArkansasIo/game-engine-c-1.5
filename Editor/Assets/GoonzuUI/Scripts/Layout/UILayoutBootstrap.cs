using UnityEngine;

namespace GoonzuUI.Layout
{
    // Put this on a scene GameObject and link the RectTransforms.
    public sealed class UILayoutBootstrap : MonoBehaviour
    {
        [Header("Docked HUD Rects")]
        [SerializeField] private RectTransform topLeftHUD;
        [SerializeField] private RectTransform topRightMinimap;
        [SerializeField] private RectTransform bottomLeftChat;
        [SerializeField] private RectTransform bottomCenterHotbar;
        [SerializeField] private RectTransform bottomRightButtons;

        [Header("Offsets")]
        [SerializeField] private Vector2 margin = new(12, 12);

        private void Awake()
        {
            DockTopLeft(topLeftHUD);
            DockTopRight(topRightMinimap);
            DockBottomLeft(bottomLeftChat);
            DockBottomCenter(bottomCenterHotbar);
            DockBottomRight(bottomRightButtons);
        }

        private void DockTopLeft(RectTransform rt)
        {
            if (!rt) return;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(margin.x, -margin.y);
        }

        private void DockTopRight(RectTransform rt)
        {
            if (!rt) return;
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-margin.x, -margin.y);
        }

        private void DockBottomLeft(RectTransform rt)
        {
            if (!rt) return;
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0, 0);
            rt.anchoredPosition = new Vector2(margin.x, margin.y);
        }

        private void DockBottomCenter(RectTransform rt)
        {
            if (!rt) return;
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.anchoredPosition = new Vector2(0, margin.y);
        }

        private void DockBottomRight(RectTransform rt)
        {
            if (!rt) return;
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(1, 0);
            rt.anchoredPosition = new Vector2(-margin.x, margin.y);
        }
    }
}
