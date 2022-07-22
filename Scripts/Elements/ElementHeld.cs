using UnityEngine;

/// <summary>
/// An Element held in the player's hand.
/// </summary>
public class ElementHeld : Element {

    protected AudioSource audioSource;

    [SerializeField] private AudioClip switchSound;
    public AudioClip SwitchSound { get => switchSound; }

    [SerializeField] private AudioClip shootSound;
    public AudioClip ShootSound { get => shootSound; }

    [Tooltip("The projectile to instantiate when the element is shot.")]
    [SerializeField] protected ElementProjectile projectilePrefab;

    protected override void Awake() {
        base.Awake();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    protected override void OnEnable() {
        base.OnEnable();
        audioSource.Play();
    }

    /// <summary>
    /// Shoots the held element in the given direction with the given force.
    /// </summary>
    /// <param name="direction">The direction to shoot in.</param>
    /// <param name="force">The force with which to shoot.</param>
    public void Shoot(Vector3 direction, float force) {
        ElementProjectile projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        projectile.GetComponent<Rigidbody>().velocity = direction * force;
    }
}
