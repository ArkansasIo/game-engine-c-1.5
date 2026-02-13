using System;
using System.Collections.Generic;

namespace GoonzuGame.Audio
{
    [System.Serializable]
    public class Sound
    {
        public string Name;
        public string FilePath;
        public bool IsLooping;
        public float Volume;

        public Sound(string name, string filePath, bool looping = false, float volume = 1.0f)
        {
            Name = name;
            FilePath = filePath;
            IsLooping = looping;
            Volume = volume;
        }
    }

    public class AudioManager
    {
        public static AudioManager Instance { get; } = new AudioManager();

        public float MasterVolume = 1.0f;
        public Dictionary<string, Sound> Sounds = new Dictionary<string, Sound>();
        public List<string> PlayingSounds = new List<string>();

        private AudioManager()
        {
            InitializeSounds();
        }

        private void InitializeSounds()
        {
            // Add default sounds
            Sounds["BackgroundMusic"] = new Sound("BackgroundMusic", "music/bg.wav", true, 0.5f);
            Sounds["Attack"] = new Sound("Attack", "sfx/attack.wav", false, 0.8f);
            Sounds["Heal"] = new Sound("Heal", "sfx/heal.wav", false, 0.7f);
            Sounds["LevelUp"] = new Sound("LevelUp", "sfx/levelup.wav", false, 1.0f);
        }

        public void PlaySound(string name)
        {
            if (Sounds.ContainsKey(name))
            {
                PlayingSounds.Add(name);
                Console.WriteLine($"Playing sound: {name} (Volume: {Sounds[name].Volume * MasterVolume})");
                // In a real implementation, load and play the audio file
            }
            else
            {
                Console.WriteLine($"Sound not found: {name}");
            }
        }

        public void StopSound(string name)
        {
            if (PlayingSounds.Contains(name))
            {
                PlayingSounds.Remove(name);
                Console.WriteLine($"Stopped sound: {name}");
                // Stop the audio playback
            }
        }

        public void PlayMusic(string name)
        {
            // Stop current music if any
            foreach (var sound in PlayingSounds)
            {
                if (Sounds.ContainsKey(sound) && Sounds[sound].IsLooping)
                {
                    StopSound(sound);
                }
            }
            PlaySound(name);
        }

        public void SetMasterVolume(float volume)
        {
            MasterVolume = MathfClamp(volume, 0f, 1f);
            Console.WriteLine($"Master volume set to: {MasterVolume}");
        }

        public void Mute()
        {
            MasterVolume = 0f;
            Console.WriteLine("Audio muted");
        }

        public void Unmute()
        {
            MasterVolume = 1f;
            Console.WriteLine("Audio unmuted");
        }

        // Helper method for Mathf.Clamp
        private float MathfClamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
