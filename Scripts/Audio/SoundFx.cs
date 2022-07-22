using UnityEngine;

[System.Serializable]
public class SoundFx : Sound {

    [Tooltip("A random audio clip will be chosen from this list each time the sound is played.")]
    [SerializeField] private AudioClip[] audioClips;

    /// <summary>
    /// Sets the sound's audio source to the given AudioSource.
    /// Also sets the source's volume and loop setting to the sound's default volume and loop setting.
    /// </summary>
    /// <remarks>The source's audio clip is set to the sound effect's first audio clip.</remarks>
    /// <param name="audioSource">The AudioSource to set this sound's audio source to.</param>
    public override void InitialiseSound(AudioSource audioSource) {
        if (audioClips.Length == 0) {
            Debug.LogError($"SoundFx [{name}] has no audio clips set.");
            return;
        }

        audioSource.clip = audioClips[0];
        base.InitialiseSound(audioSource);
    }

    /// <summary>
    /// Plays the sound FX. If multiple audio clips are available, a random one is played.
    /// </summary>
    public override void Play() {
        if (audioClips.Length > 1) {
            audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        }
    }
}
