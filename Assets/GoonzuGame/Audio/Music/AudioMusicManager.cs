using System;

namespace GoonzuGame.Audio.Music
{
    using System.Collections.Generic;

    public class AudioMusicManager
    {
        public List<string> MusicTracks { get; set; }
        public List<string> SFX { get; set; }
        public float Volume { get; set; }

        public AudioMusicManager()
        {
            MusicTracks = new List<string>();
            SFX = new List<string>();
            Volume = 1.0f;
        }

        public void PlayMusic(string trackName)
        {
            MusicTracks.Add(trackName);
            Console.WriteLine($"Playing music track: {trackName}");
        }

        public void PlaySFX(string sfxName)
        {
            SFX.Add(sfxName);
            Console.WriteLine($"Playing sound effect: {sfxName}");
        }

        public void SetVolume(float volume)
        {
            Volume = volume;
            Console.WriteLine($"Audio volume set to: {volume}");
        }

        public void ChangeMusic(string newTrack)
        {
            Console.WriteLine($"Changing music to: {newTrack}");
        }
    }
}
