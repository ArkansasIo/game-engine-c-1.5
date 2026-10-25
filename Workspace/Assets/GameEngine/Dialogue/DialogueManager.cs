using System;

namespace GameEngine.Dialogue
{
    public class DialogueManager
    {
        public void Initialize()
        {
            Console.WriteLine("Dialogue system initialized.");
        }

        public void StartDialogue(string npcName)
        {
            Console.WriteLine($"Dialogue started with {npcName}.");
        }

        public void Next()
        {
            Console.WriteLine("Dialogue advanced to next line.");
        }

        public void EndDialogue()
        {
            Console.WriteLine("Dialogue ended.");
        }
    }
}
