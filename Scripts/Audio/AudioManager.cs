using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    private static AudioManager _instance;
    public static AudioManager Instance { get => _instance; }

    [SerializeField] private Transform player;

    [Space(10)]

    [SerializeField] private AudioMixer mainMix;
    [SerializeField] private string mainMixVolumeParam = "MasterVolume";

    [Space(10)]

    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup musicGroup;

    [Space(10)]

    [SerializeField] private SoundFx[] soundFxs;
    public SoundFx[] SoundFxs { get => soundFxs; }

    [SerializeField] private Music[] musicTracks;
    public Music[] MusicTracks { get => musicTracks; }

    /// <summary>
    /// The currently playing music.
    /// </summary>
    public Music CurrentMusic { get; private set; }

    void Awake() {
        // singleton
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }

        foreach (SoundFx soundFx in soundFxs) {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = sfxGroup;
            soundFx.InitialiseSound(audioSource);
        }

        foreach (Music music in musicTracks) {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = musicGroup;
            music.InitialiseSound(audioSource);
        }
    }

    /// <summary>
    /// Unmutes all audio in the game.
    /// </summary>
    /// <param name="doFade">Whether to fade the audio in.</param>
    /// <param name="fadeTime">The duration of the fade. Only applicable if <c>doFade</c> is <c>true</c>.</param>
    public void UnmuteAllAudio(bool doFade = false, float fadeTime = 3f) {
        if (!doFade) {
            mainMix.SetFloat(mainMixVolumeParam, 0f);
        } else {
            StartCoroutine(FadeInAllAudio(fadeTime));
        }
    }

    /// <summary>
    /// Fades all audio up to a volume of 0dB.
    /// </summary>
    /// <param name="fadeLength">The length of the fade.</param>
    /// <returns>IEnumerator; this is a coroutine.</returns>
    private IEnumerator FadeInAllAudio(float fadeLength) {
        mainMix.GetFloat(mainMixVolumeParam, out float currVolume);
        while (currVolume < -0.01f) {
            mainMix.GetFloat(mainMixVolumeParam, out currVolume);
            // unsure if we need to multiply by Time.deltaTime
            mainMix.SetFloat(mainMixVolumeParam, currVolume + (1 / fadeLength));
            yield return null;
        }

        mainMix.SetFloat(mainMixVolumeParam, 0f);
    }

    /// <summary>
    /// Mutes all audio in the game.
    /// </summary>
    /// <param name="doFade">Whether to fade the audio out.</param>
    /// <param name="fadeTime">The duration of the fade. Only applicable if <c>doFade</c> is <c>true</c>.</param>
    public void MuteAllAudio(bool doFade = false, float fadeTime = 3f) {
        if (!doFade) {
            mainMix.SetFloat(mainMixVolumeParam, -80f);
        } else {
            StartCoroutine(FadeOutAllAudio(fadeTime));
        }
    }

    /// <summary>
    /// Fades all audio down to a volume of -80dB.
    /// </summary>
    /// <param name="fadeLength">The length of the fade.</param>
    /// <returns>IEnumerator; this is a coroutine.</returns>
    private IEnumerator FadeOutAllAudio(float fadeLength) {
        mainMix.GetFloat(mainMixVolumeParam, out float currVolume);
        while (currVolume > -80f) {
            mainMix.GetFloat(mainMixVolumeParam, out currVolume);
            // unsure if we need to use Time.deltaTime
            mainMix.SetFloat(mainMixVolumeParam, currVolume - (1 / fadeLength));
            yield return null;
        }

        mainMix.SetFloat(mainMixVolumeParam, -80f);
    }

    /// <summary>
    /// Plays the sound FX with the given name.
    /// </summary>
    /// <param name="name">The name of the sound FX to play.</param>
    /// <param name="doFade">If true, fades the sound FX in. Else, it starts at default volume.</param>
    /// <param name="fadeTime">The length of the fade in.</param>
    public void PlaySoundFx(string name, bool doFade = false, float fadeTime = 1f) {
        SoundFx soundFx = System.Array.Find(soundFxs, soundFx => soundFx.Name == name);

        if (soundFx == null) {
            Debug.LogError($"SoundFx [{name}] not found in AudioManager.");
        }

        soundFx.Play();
        if (doFade) {
            StartCoroutine(RaiseVolume(soundFx, 0, soundFx.DefaultVolume, fadeTime));
        } else {
            soundFx.Volume = soundFx.DefaultVolume;
        }
    }

    /// <summary>
    /// Stops the sound FX with the given name.
    /// </summary>
    /// <param name="name">The name of the sound FX to stop.</param>
    /// <param name="doFade">If true, fades the sound FX out before stopping. Else, it simply stops.</param>
    /// <param name="fadeTime">The length of the fade out.</param>
    public void StopSoundFx(string name, bool doFade = false, float fadeTime = 1f) {
        SoundFx soundFx = System.Array.Find(soundFxs, soundFx => soundFx.Name == name);
        
        if (soundFx == null) {
            Debug.LogError($"SoundFx [{name}] not found in AudioManager.");
        }

        soundFx.Stop();
        if (doFade) {
            StartCoroutine(StopSoundWithFade(soundFx, fadeTime));
        } else {
            soundFx.Stop();
        }
    }

    /// <summary>
    /// Plays the music with the given name.
    /// </summary>
    /// <param name="name">The name of the music to play.</param>
    /// <param name="doFade">If true, fades the music in. Else, music starts at default volume.</param>
    /// <param name="fadeTime">The length of the fade in.</param>
    public void PlayMusic(string name, bool doFade = false, float fadeTime = 1f) {
        Music music = System.Array.Find(musicTracks, music => music.Name == name);

        if (music == null) {
            Debug.LogError($"Music [{name}] not found in AudioManager.");
            return;
        }

        CurrentMusic = music;
        music.Play();
        if (doFade) {
            StartCoroutine(RaiseVolume(music, 0, music.DefaultVolume, fadeTime));
        } else {
            music.Volume = music.DefaultVolume;
        }

    }

    /// <summary>
    /// Pauses the given music.
    /// </summary>
    /// <param name="music">The music to pause.</param>
    /// <param name="doFade">If true, fades the music out before pausing. Else, music simply pauses.</param>
    /// <param name="fadeTime">The length of the fade out.</param>
    public void PauseMusic(Music music, bool doFade = false, float fadeTime = 1f) {
        if (doFade) {
            StartCoroutine(PauseMusicWithFade(music, fadeTime));
        } else {
            music.Pause();
            CurrentMusic = null;
        }
    }


    /// <summary>
    /// Resumes the given paused music.
    /// </summary>
    /// <param name="music">The music to resume.</param>
    /// <param name="doFade">If true, fades the music in. Else, music simply resumes.</param>
    /// <param name="fadeTime">The length of the fade in.</param>
    public void ResumeMusic(Music music, bool doFade = false, float fadeTime = 1f) {
        if (!doFade) {
            music.Play();
            music.Volume = music.DefaultVolume;
            return;
        }

        music.Play();
        StartCoroutine(RaiseVolume(music, 0, music.DefaultVolume, fadeTime));
    }

    /// <summary>
    /// Stops the given music.
    /// </summary>
    /// <param name="music">The music to stop.</param>
    /// <param name="doFade">If true, fades the music out before stopping. Else, music simply stops.</param>
    /// <param name="fadeTime">The length of the fade out.</param>
    public void StopMusic(Music music, bool doFade = false, float fadeTime = 1f) {
        if (doFade) {
            StartCoroutine(StopSoundWithFade(music, fadeTime));
        } else {
            music.Stop();
            CurrentMusic = null;
        }
    }

    /// <summary>
    /// Fades out and then pauses the given music.
    /// </summary>
    /// <param name="music">The music to pause.</param>
    /// <param name="fadeTime">The length of the fade out.</param>
    /// <returns>IEnumerator, as this method is a coroutine.</returns>
    private IEnumerator PauseMusicWithFade(Music music, float fadeTime) {
        StartCoroutine(LowerVolume(music, music.Volume, 0, fadeTime));
        yield return new WaitUntil(() => music.Volume <= 0);
        PauseMusic(music);
    }

    /// <summary>
    /// Fades out and then stops the given sound.
    /// </summary>
    /// <param name="sound">The sound to stop.</param>
    /// <param name="fadeTime">The length of the fade out.</param>
    /// <returns>IEnumerator, as this method is a coroutine.</returns>
    private IEnumerator StopSoundWithFade(Sound sound, float fadeTime) {
        StartCoroutine(LowerVolume(sound, sound.Volume, 0, fadeTime));
        yield return new WaitUntil(() => sound.Volume <= 0);
        sound.Stop();
    }

    /// <summary>
    /// Raises the volume of the given sound.
    /// </summary>
    /// <param name="sound">The sound whose volume to raise.</param>
    /// <param name="fromVolume">The volume from which to raise the sound.</param>
    /// <param name="toVolume">The volume to raise the sound to.</param>
    /// <param name="raiseTime">The time taken to raise the volume.</param>
    /// <returns>IEnumerator, as this method is a coroutine.</returns>
    private IEnumerator RaiseVolume(Sound sound, float fromVolume, float toVolume, float raiseTime) {
        sound.Volume = fromVolume;
        while (sound.Volume < toVolume) {
            sound.Volume += (1 / raiseTime) * Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Lowers the volume of the given sound.
    /// </summary>
    /// <param name="sound">The sound whose volume to lower.</param>
    /// <param name="fromVolume">The volume from which to lower the sound.</param>
    /// <param name="toVolume">The volume to lower the sound to.</param>
    /// <param name="lowerTime">The time taken to lower the volume.</param>
    /// <returns>IEnumerator, as this method is a coroutine.</returns>
    private IEnumerator LowerVolume(Sound sound, float fromVolume, float toVolume, float lowerTime) {
        sound.Volume = fromVolume;
        while (sound.Volume > toVolume) {
            sound.Volume -= (1 / lowerTime) * Time.deltaTime;
            yield return null;
        }
    }
}
