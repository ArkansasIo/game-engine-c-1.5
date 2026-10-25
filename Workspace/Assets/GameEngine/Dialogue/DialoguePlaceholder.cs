namespace GameEngine.Dialogue
{
    /// <summary>
    /// DialogueManager handles conversations, NPC interactions, and branching dialogue trees.
    /// </summary>
    public class DialogueManager
    {
        private Dictionary<string, List<string>> dialogueData = new Dictionary<string, List<string>>();
        private int currentLine = 0;
        private string currentNpc = null;

        public void Initialize()
        {
            // Load dialogue data (stub)
            dialogueData["Guard"] = new List<string> { "Halt! Who goes there?", "You may pass." };
            dialogueData["Merchant"] = new List<string> { "Welcome! Looking for something special?", "Safe travels!" };
        }

        public void StartDialogue(string npcName)
        {
            if (dialogueData.ContainsKey(npcName))
            {
                currentNpc = npcName;
                currentLine = 0;
                ShowCurrentLine();
            }
        }

        public void Next()
        {
            if (currentNpc != null && currentLine + 1 < dialogueData[currentNpc].Count)
            {
                currentLine++;
                ShowCurrentLine();
            }
            else
            {
                EndDialogue();
            }
        }

        public void EndDialogue()
        {
            currentNpc = null;
            currentLine = 0;
            // Clean up resources or notify UI
        }

        private void ShowCurrentLine()
        {
            if (currentNpc != null)
            {
                string line = dialogueData[currentNpc][currentLine];
                // Display line to UI (stub)
                System.Console.WriteLine($"{currentNpc}: {line}");
            }
        }
    }
}
