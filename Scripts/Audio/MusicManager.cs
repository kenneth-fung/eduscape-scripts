using System.Collections;
using UnityEngine;

/// <summary>
/// MusicManager exposes methods that play music specific to this (space station) level.
/// </summary>
public class MusicManager : MonoBehaviour {

    private static MusicManager _instance;
    public static MusicManager Instance { get => _instance; }

    [SerializeField] private bool playMusic = true;

    [Space(10)]

    [SerializeField] private bool useSongForIntro = false;
    [SerializeField] private AudioSource[] introRadioAudioSources;

    [SerializeField] private AudioClip introMusic;
    [SerializeField] private float introMusicVolume;
    
    [SerializeField] private AudioClip introSong;
    [SerializeField] private float introSongVolume;
    
    [SerializeField] private AudioClip introEnd;

    public bool UseSongForIntro { get => useSongForIntro; }

    private void Awake() {
        // singleton
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }

    public void PlayIntroMusic(float fadeInLength) {
        if (!playMusic) {
            return;
        }

        foreach (AudioSource source in introRadioAudioSources) {
            source.loop = !useSongForIntro;
            source.clip = useSongForIntro ? introSong : introMusic;
            StartCoroutine(FadeInIntroMusic(source, fadeInLength));
        }
    }

    private IEnumerator FadeInIntroMusic(AudioSource source, float fadeInLength) {
        source.volume = 0f;
        source.Play();

        float targetVolume = useSongForIntro ? introSongVolume : introMusicVolume;
        float elapsedTime = 0f;
        while (elapsedTime < fadeInLength) {
            source.volume += (targetVolume - source.volume) / (fadeInLength - elapsedTime) * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        source.volume = targetVolume;
    }

    public void StopIntroMusic() {
        foreach (AudioSource source in introRadioAudioSources) {
            source.Stop();
            source.volume = 1f;
            source.PlayOneShot(introEnd);
        }
    }

    public void PlayStationMusic() {
        if (!playMusic) {
            return;
        }

        AudioManager.Instance.PlayMusic("Station");
    }

    public void StopStationMusic() {
        if (playMusic && AudioManager.Instance.CurrentMusic?.Name == "Station") {
            AudioManager.Instance.StopMusic(AudioManager.Instance.CurrentMusic);
        }
    }

    public void PlayExteriorMusic() {
        if (!playMusic || (AudioManager.Instance.CurrentMusic != null && AudioManager.Instance.CurrentMusic.Name == "Exterior")) {
            return;
        }

        AudioManager.Instance.PlayMusic("Exterior");
    }

    public void StopExteriorMusic(bool doFade) {
        if (AudioManager.Instance.CurrentMusic?.Name == "Exterior") {
            AudioManager.Instance.StopMusic(AudioManager.Instance.CurrentMusic, doFade, 5f);
        }
    }

    public void PlayShipRiseMusic() {
        if (!playMusic) {
            return;
        }

        AudioManager.Instance.PlayMusic("Ship Rise");
    }

    public void PlayShipEvadeMusic() {
        if (!playMusic) {
            return;
        }

        AudioManager.Instance.PlayMusic("Ship Evade");
    }

    public void PlayVictoryMusic() {
        if (!playMusic) {
            return;
        }

        AudioManager.Instance.PlayMusic("Victory", true, 5f);
    }
}
