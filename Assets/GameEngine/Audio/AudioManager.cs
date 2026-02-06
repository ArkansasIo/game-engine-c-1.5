using UnityEngine;

namespace GameEngine.Audio
{
    /// <summary>
    /// Manages background music, sound effects, and audio playback for the game.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public AudioSource musicSource;
        public AudioSource sfxSource;

        public AudioClip titleMusic;
        public AudioClip[] gameMusic;
        public AudioClip[] sfxClips;
        /// <summary>
        /// The audio source used for playing background music.
        /// </summary>
        public AudioSource musicSource;
        
        /// <summary>
        /// The audio source used for playing sound effects.
        /// </summary>
        public AudioSource sfxSource;
        
        /// <summary>
        /// The audio clip for the title screen music.
        /// </summary>
        public AudioClip titleMusic;
        
        /// <summary>
        /// The array of audio clips used as background music tracks during gameplay.
        /// </summary>
        public AudioClip[] gameMusic;
        
        /// <summary>
        /// The array of audio clips used for sound effects.
        /// </summary>
        public AudioClip[] sfxClips;

        /// <summary>
        /// Plays background music for a given track name.
        /// </summary>
        /// <summary>
        /// Plays background music for a given track name.
        /// </summary>
        /// <param name="trackName">The name of the music track to play.</param>
        public void PlayMusic(string trackName)
        {
            if (musicSource && gameMusic.Length > 0)
            {
                int index = Array.FindIndex(gameMusic, clip => clip.name == trackName);
                if (index >= 0)
                {
                    musicSource.clip = gameMusic[index];
                    musicSource.loop = true;
                    musicSource.Play();
                }
            }
        }

        /// <summary>
        /// Plays a sound effect by name.
        /// </summary>
        /// <summary>
        /// Plays a sound effect by name.
        /// </summary>
        /// <param name="sfxName">The name of the sound effect to play.</param>
        public void PlaySFX(string sfxName)
        {
            if (sfxSource && sfxClips.Length > 0)
            {
                int index = Array.FindIndex(sfxClips, clip => clip.name == sfxName);
                if (index >= 0)
                {
                    sfxSource.PlayOneShot(sfxClips[index]);
                }
            }
        }

        /// <summary>
        /// Stops the currently playing music.
        /// </summary>
        /// <summary>
        /// Stops the currently playing music.
        /// </summary>
        public void StopMusic()
        {
            if (musicSource)
                musicSource.Stop();
        }

        /// <summary>
        /// Adjusts the master volume for all audio.
        /// </summary>
        /// <summary>
        /// Adjusts the master volume for all audio.
        /// </summary>
        /// <param name="volume">The new volume level (0.0 to 1.0).</param>
        public void SetVolume(float volume)
        {
            if (musicSource)
                musicSource.volume = volume;
        }
    }
}
