using UnityEngine;
using GoonzuUI.Core;

namespace GoonzuUI.Core
{
    public sealed class UIInputRouter : MonoBehaviour
    {
        private UIEventBus _bus;

        public void Initialize(UIEventBus bus) => _bus = bus;

        private void Update()
        {
            if (_bus == null) return;

            if (Input.GetKeyDown(KeyCode.I)) _bus.Publish(new ToggleWindowEvent(UIIds.Inventory));
            if (Input.GetKeyDown(KeyCode.K)) _bus.Publish(new ToggleWindowEvent(UIIds.Skills));
            if (Input.GetKeyDown(KeyCode.L)) _bus.Publish(new ToggleWindowEvent(UIIds.Quests));
            if (Input.GetKeyDown(KeyCode.Escape)) _bus.Publish(new ToggleWindowEvent(UIIds.Options));
        }
    }
}
