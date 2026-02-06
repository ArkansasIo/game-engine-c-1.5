using UnityEngine;
using GoonzuUI.Core;
using GoonzuUI.Windows;
using GoonzuUI.HUD;
using GoonzuUI.Chat;
using GoonzuUI.Inventory;

public sealed class UIRuntimeRoot : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private UIWindowManager windowManager;
    [SerializeField] private GoonzuUI.Core.UIInputRouter inputRouter;

    [Header("HUD")]
    [SerializeField] private SystemButtonsHUD systemButtons;

    [Header("Chat")]
    [SerializeField] private ChatController chat;

    [Header("Inventory")]
    [SerializeField] private InventoryWindow inventoryWindow;

    private UIEventBus _bus;
    private InventoryModel _inventory;

    private void Awake()
    {
        _bus = new UIEventBus();

        windowManager.Initialize(_bus);
        inputRouter.Initialize(_bus);
        systemButtons.Initialize(_bus);
        chat.Initialize(_bus);

        // Demo inventory
        _inventory = new InventoryModel(slotCount: 40);
        inventoryWindow.Bind(_inventory);

        // Demo: append a system line
        _bus.Publish(new ChatAppendEvent("General", "Welcome to the world.", "System"));
    }
}
