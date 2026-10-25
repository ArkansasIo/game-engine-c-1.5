using UnityEngine;

namespace GoonzuUI.Windows
{
    // Optional: for advanced docking/anchoring, not required for basic GoonZu layout
    public sealed class UIDock : MonoBehaviour
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private Vector2 anchorMin;
        [SerializeField] private Vector2 anchorMax;
        [SerializeField] private Vector2 pivot;
        [SerializeField] private Vector2 offset;

        private void Awake()
        {
            if (!target) return;
            target.anchorMin = anchorMin;
            target.anchorMax = anchorMax;
            target.pivot = pivot;
            target.anchoredPosition = offset;
        }
    }
}
