using System.Collections;
using UnityEngine;

public class DoorPanel : MonoBehaviour {

    public enum Status { OPENED, CLOSED, JAMMED, OFF }
    protected Status status;

    [SerializeField] protected Door door;

    [SerializeField] protected float jamDuration = 0.5f;

    [Header("Materials")]

    [SerializeField] protected MeshRenderer mesh;
    [SerializeField] protected Material openMaterial;
    [SerializeField] protected Material closedMaterial;
    [SerializeField] protected Material jamMaterial;
    [SerializeField] protected Material offMaterial;

    protected Material materialBeforeSwitchedOff;

    [Header("Audio")]

    [SerializeField] protected AudioClip activateSound;
    [SerializeField] protected AudioClip deactivateSound;
    [SerializeField] protected AudioClip jamSound;

    protected AudioSource audioSource;

    protected virtual void Awake() {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    // Start is called before the first frame update
    protected virtual void Start() {
        status = Status.CLOSED;
        materialBeforeSwitchedOff = closedMaterial;
    }

    /// <summary>
    /// Toggles the door if the given element is not fire.
    /// </summary>
    /// <param name="element">The element that contacted this door panel.</param>
    public void ToggleDoor(Element element) {
        if (element.ElementType != Element.Type.FIRE) {
            ToggleDoor();
        }
    }

    /// <summary>
    /// Toggles the door.
    /// </summary>
    protected void ToggleDoor() {
        if (status == Status.JAMMED || status == Status.OFF || door.IsOpeningOrClosing()) {
            return;
        }

        if (door.IsOpen) {
            if (door.IsPlayerInDoorway) {
                StartCoroutine(Jam());
            } else {
                Close();
                door.CloseDoor();
            }
        } else {
            Open();
            door.OpenDoor();
        }
    }

    /// <summary>
    /// Opens the door.
    /// </summary>
    protected void Open() {
        status = Status.OPENED;
        mesh.material = openMaterial;
        audioSource.clip = activateSound;
        audioSource.Play();
    }

    /// <summary>
    /// Closes the door.
    /// </summary>
    protected void Close() {
        status = Status.CLOSED;
        mesh.material = closedMaterial;
        audioSource.clip = deactivateSound;
        audioSource.Play();
    }

    /// <summary>
    /// Jams the door, preventing it from opening or closing.
    /// </summary>
    /// <returns>IEnumerator; this is a coroutine.</returns>
    protected IEnumerator Jam() {
        Status originalStatus = status;

        status = Status.JAMMED;
        mesh.material = jamMaterial;
        audioSource.clip = jamSound;
        audioSource.Play();

        yield return new WaitForSeconds(jamDuration);
        
        mesh.material = openMaterial;
        status = originalStatus;
    }

    /// <summary>
    /// Switches the door panel off, preventing interaction.
    /// </summary>
    public void SwitchOff() {
        status = Status.OFF;
        materialBeforeSwitchedOff = mesh.material;
        mesh.material = offMaterial;
    }

    /// <summary>
    /// Switches the door panel on, allowing interaction.
    /// </summary>
    public void SwitchOn() {
        status = door.IsOpen ? Status.OPENED : Status.CLOSED;
        mesh.material = materialBeforeSwitchedOff;
    }
}
