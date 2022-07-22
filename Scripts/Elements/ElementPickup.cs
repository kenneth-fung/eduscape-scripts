using UnityEngine;

/// <summary>
/// An Element for the player to pick up.
/// </summary>
public class ElementPickup : Element {

    protected AudioSource audioSource;

    [SerializeField] protected AudioClip pickUpSound;
    public AudioClip PickUpSound { get => pickUpSound; }

    [Tooltip("The held element corresponding to this pickup element.")]
    [SerializeField] protected ElementHeld correspondingHeldElement;
    public ElementHeld CorrespondingHeldElement { get => correspondingHeldElement; }

    [Space(10)]

    [SerializeField] protected ElementEvent elementPickedUpEvent;

    protected override void Awake() {
        base.Awake();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    protected override void OnEnable() {
        base.OnEnable();
        audioSource.Play();
    }

    /// <summary>
    /// Picks up the element, adding it to the player's arsenal.
    /// </summary>
    public void PickUp() {
        elementPickedUpEvent.Invoke(this);
        gameObject.SetActive(false);
    }

    protected void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Hand")) {
            PickUp();
        }
    }
}
