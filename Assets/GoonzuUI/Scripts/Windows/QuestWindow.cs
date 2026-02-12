using System;
using System.Collections.Generic;
using GoonzuGame.Quests;

namespace GoonzuGame.GUI
{
    public class QuestWindow : UIWindow
    {
        public List<Quest> ActiveQuests { get; set; }
        public List<Quest> CompletedQuests { get; set; }
        public QuestWindow()
        {
            ActiveQuests = new List<Quest>();
            CompletedQuests = new List<Quest>();
        }
        public override void Show()
        {
            Console.WriteLine("Showing QuestWindow");
            DisplayActiveQuests();
            DisplayCompletedQuests();
        }
        public void DisplayActiveQuests()
        {
            Console.WriteLine("Active Quests:");
            foreach (var quest in ActiveQuests)
                Console.WriteLine($"- {quest.Title}: {quest.Description}");
        }
        public void DisplayCompletedQuests()
        {
            Console.WriteLine("Completed Quests:");
            foreach (var quest in CompletedQuests)
                Console.WriteLine($"- {quest.Title}");
        }
        public void AddQuest(Quest quest)
        {
            ActiveQuests.Add(quest);
            Console.WriteLine($"Added quest: {quest.Title}");
        }
        public void CompleteQuest(Quest quest)
        {
            ActiveQuests.Remove(quest);
            CompletedQuests.Add(quest);
            Console.WriteLine($"Completed quest: {quest.Title}");
        }
    }
}