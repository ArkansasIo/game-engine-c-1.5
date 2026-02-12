using System;

namespace GoonzuGame.Audio
{
    public class AudioManager
        public void PlayMusic(string track) {
            System.Console.WriteLine($"Playing music track: {track}");
        }
        public void StopMusic(string track) {
            System.Console.WriteLine($"Stopping music track: {track}");
        }
        public void SetVolume(int volume) {
            System.Console.WriteLine($"Setting volume to: {volume}");
        }
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
