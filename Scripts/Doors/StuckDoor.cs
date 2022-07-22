using UnityEngine;

/// <summary>
/// StuckDoor controls the door in the Stuck Door Puzzle.
/// </summary>
public class StuckDoor : Door {

    [Tooltip("The AudioClip to play when the door is attempting to open.")]
    [SerializeField] protected AudioClip openAttemptClip;

    [Tooltip("The AudioClip to play when the door is attempting to close.")]
    [SerializeField] protected AudioClip closeAttemptClip;

    protected override void Start() {
        base.Start();
        audioSource.clip = closeAttemptClip;
    }

    public void PlayOpenAttemptSound() {
        audioSource.clip = openAttemptClip;
        audioSource.Play();
    }

    public void PlayCloseAttemptSound() {
        audioSource.clip = closeAttemptClip;
        audioSource.Play();
    }

    /// <summary>
    /// Opens the door completely.
    /// </summary>
    public override void OpenDoor() {
        base.OpenDoor();
        audioSource.clip = openSound;
        audioSource.Play();
    }

    /// <summary>
    /// Closes the door completely.
    /// For the stuck door puzzle, we prevent that from ever happening.
    /// </summary>
    public override void CloseDoor() {
        // completely prevent from closing to ensure the player never gets... stuck
    }
}
