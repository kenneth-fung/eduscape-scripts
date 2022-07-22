using UnityEngine;

/// <summary>
/// SoundFxSource is responsible for modulating the volume of the AudioSource on the object it is attached to.
/// Said object's position is taken to be the source of the sound effect.
/// </summary>
public class SoundFxSource : MonoBehaviour {

    private AudioSource audioSource;

    [Tooltip("The sphere within which the sound plays at its default volume.")]
    [SerializeField] private SphereCollider defaultVolumeSphere;

    [Tooltip("The sphere within which the volume will decrease towards zero as the player moves towards its edge." +
        "This should be larger than the default volume sphere.")]
    [SerializeField] private SphereCollider decreasingVolumeSphere;

    private float defaultVolume;
    public float DefaultVolume { get => defaultVolume; }

    public bool DoModulateVolume { get; set; }

    /// <summary>
    /// The distance between the edge of the area of default volume and the area of zero volume.
    /// </summary>
    private float decreasingVolumeDistance;

    void Awake() {
        audioSource = GetComponent<AudioSource>();

        if (decreasingVolumeSphere.radius < defaultVolumeSphere.radius) {
            decreasingVolumeSphere.radius = defaultVolumeSphere.radius;
        }

        decreasingVolumeDistance = decreasingVolumeSphere.radius - defaultVolumeSphere.radius;

        defaultVolume = audioSource.volume;

        audioSource.volume = 0f;
    }

    void Start() {
        DoModulateVolume = true;
    }

    // Update is called once per frame
    void Update() {
        if (DoModulateVolume && audioSource.isPlaying) {
            ModulateVolume();
        }
    }

    /// <summary>
    /// Adjusts the volume of the sound based on proximity to the player.
    /// </summary>
    private void ModulateVolume() {
        // we scale the distance based on the global scale of the object
        float scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        float distanceToPlayer = Vector3.Distance(transform.position, LevelManager.Instance.PlayerHead.transform.position);
        if (distanceToPlayer <= defaultVolumeSphere.radius * scale) {
            // maintain volume at default volume
            audioSource.volume = defaultVolume;
            if (!audioSource.isPlaying) {
                audioSource.Play();
            }

            return;
        }

        float decreasingVolumeRadius = decreasingVolumeSphere.radius * scale;
        if (distanceToPlayer >= decreasingVolumeRadius) {
            // exceeded total radius
            audioSource.volume = 0f;
            return;
        }

        // decrease volume
        audioSource.volume = (decreasingVolumeRadius - distanceToPlayer) / (decreasingVolumeDistance * scale) * defaultVolume;
        if (!audioSource.isPlaying) {
            audioSource.Play();
        }
    }
}
