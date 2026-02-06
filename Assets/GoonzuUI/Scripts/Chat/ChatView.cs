using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GoonzuUI.Chat
{
    public sealed class ChatView : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown channelDropdown;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private TMP_Text output;

        public string SelectedChannel => channelDropdown ? channelDropdown.options[channelDropdown.value].text : "General";

        public void SetOutput(string text)
        {
            if (output) output.text = text;
        }

        public void ClearInput()
        {
            if (input) input.text = "";
            if (input) input.ActivateInputField();
        }

        public string ConsumeInput()
        {
            return input ? input.text : "";
        }
    }
}
