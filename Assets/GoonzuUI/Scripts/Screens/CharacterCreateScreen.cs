using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GoonzuUI.Screens
{
    public sealed class CharacterCreateScreen : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private Dropdown classDropdown;
        [SerializeField] private Button createButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Transform slotPanel;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private int maxSlots = 10;

        private void Start()
        {
            PopulateSlots();
            if (createButton) createButton.onClick.AddListener(OnCreateClicked);
            if (cancelButton) cancelButton.onClick.AddListener(OnCancelClicked);
        }

        private void PopulateSlots()
        {
            for (int i = 0; i < maxSlots; i++)
            {
                Instantiate(slotPrefab, slotPanel);
            }
        }

        private void OnCreateClicked()
        {
            string charName = nameInput ? nameInput.text : "";
            string charClass = classDropdown ? classDropdown.options[classDropdown.value].text : "";
            // TODO: Add logic to create and save new character
        }

        private void OnCancelClicked()
        {
            // TODO: Return to previous screen
        }
    }
}
