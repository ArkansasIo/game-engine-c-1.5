using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GoonzuUI.Inventory
{
    public sealed class InventorySlotView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text countText;

        public void Render(ItemStack stack)
        {
            if (stack.IsEmpty)
            {
                if (icon) { icon.enabled = false; icon.sprite = null; }
                if (countText) countText.text = "";
                return;
            }

            if (icon)
            {
                icon.enabled = true;
                icon.sprite = stack.def.icon;
            }

            if (countText)
                countText.text = (stack.def.stackable && stack.count > 1) ? stack.count.ToString() : "";
        }
    }
}
