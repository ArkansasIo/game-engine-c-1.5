using System;

namespace GoonzuGame.Audio
{
    public class AudioManager
    {
        public List<string> PlayingSounds { get; set; }

        public AudioManager()
        {
            PlayingSounds = new List<string>();
        }

        public void PlaySound(string soundName)
        {
            PlayingSounds.Add(soundName);
            Console.WriteLine($"Playing sound: {soundName}");
        }

        public void StopSound(string soundName)
        {
            PlayingSounds.Remove(soundName);
            Console.WriteLine($"Stopped sound: {soundName}");
        }
    }
}
