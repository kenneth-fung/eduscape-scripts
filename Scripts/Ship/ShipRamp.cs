using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRamp : MonoBehaviour {

    private Animator anim;
    private AudioSource audioSource;

    [SerializeField] private BoxCollider closingCollider;

    [Header("Animation")]

    [SerializeField] private string animCloseParam = "isOpen";

    [Header("Audio")]

    [SerializeField] private AudioClip closingSound;
    [SerializeField] private AudioClip stopSound;

    private void Awake() {
        anim = GetComponent<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    // Start is called before the first frame update
    void Start() {
        closingCollider.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

    }

    public void CloseRamp() {
        anim.SetBool(animCloseParam, false);

        // prevent the player from teleporting out of the ship while the ramp is closing
        closingCollider.gameObject.SetActive(true);

        audioSource.Play();

        StartCoroutine(GameManager.Instance.WaitForConditionBeforeAction(
            () => anim.GetCurrentAnimatorStateInfo(0).IsName("Ramp Closed"),
            () => closingCollider.gameObject.SetActive(false)));
    }

    public void PlayRampStopSound() {
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.PlayOneShot(stopSound);
    }
}
