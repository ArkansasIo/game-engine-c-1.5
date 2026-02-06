using UnityEngine;
using GoonzuUI.Windows;

namespace GoonzuUI.Inventory
{
    public sealed class InventoryWindow : UIWindow
    {
        [SerializeField] private InventorySlotView[] slotViews;

        private InventoryModel _model;

        public void Bind(InventoryModel model)
        {
            _model = model;
            _model.SlotChanged += OnSlotChanged;

            // initial render
            for (int i = 0; i < slotViews.Length && i < _model.SlotCount; i++)
                slotViews[i].Render(_model.Get(i));
        }

        private void OnDestroy()
        {
            if (_model != null) _model.SlotChanged -= OnSlotChanged;
        }

        private void OnSlotChanged(int index)
        {
            if (_model == null) return;
            if (index < 0 || index >= slotViews.Length) return;
            slotViews[index].Render(_model.Get(index));
        }
    }
}
