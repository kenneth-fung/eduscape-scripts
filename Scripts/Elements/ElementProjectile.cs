using UnityEngine;

/// <summary>
/// An Element shot from the player's hand.
/// </summary>
public class ElementProjectile : Element {

    protected AudioSource audioSource;

    protected override void Awake() {
        base.Awake();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    protected override void Start() {
        base.Start();
        audioSource.Play();
    }

    protected void OnCollisionEnter(Collision collision) {
        // prevent the player from shooting themselves
        if (!collision.gameObject.CompareTag("Hand") && !collision.gameObject.CompareTag("Player")) {
            // check if the collided object is an element target
            ElementTarget elementTarget = collision.gameObject.GetComponent<ElementTarget>();
            if (elementTarget != null) {
                elementTarget.GetHitByElementProjectile(this);
            }

            Destroy(gameObject);
        }
    }
}
