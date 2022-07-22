using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressableButton : MonoBehaviour {

    private Rigidbody rb;

    [SerializeField] private float pressLength;
    [SerializeField] private float releaseSpeed;
    private Vector3 startLocalPos;
    private float startHeight;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pressClip;
    [SerializeField] private AudioClip releaseClip;

    [SerializeField] private UnityEvent pressedEvent;
    [SerializeField] private UnityEvent releasedEvent;

    public int CollidersTouching { get; private set; }

    public bool IsPressed { get; private set; }
    private bool isPressing = false;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        startLocalPos = transform.localPosition;
        startHeight = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update() {
        float distanceFromStartPos = Mathf.Abs(transform.localPosition.y - startHeight);

        if (isPressing) {
            if (distanceFromStartPos >= pressLength) {
                transform.localPosition = new Vector3(transform.localPosition.x, startHeight - pressLength, transform.localPosition.z);
                rb.velocity = Vector3.zero;
                if (!IsPressed) {
                    IsPressed = true;
                    audioSource.PlayOneShot(pressClip);
                    pressedEvent.Invoke();
                }
            }
        }

        if (!isPressing) {
            if (distanceFromStartPos > 0.01f) {
                float direction = transform.localPosition.y < startHeight ? 1 : -1;
                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    transform.localPosition.y + direction * releaseSpeed * Time.deltaTime,
                    transform.localPosition.z);
            } else {
                transform.localPosition = new Vector3(transform.localPosition.x, startHeight, transform.localPosition.z);
                rb.velocity = Vector3.zero;
            }

            if (IsPressed && distanceFromStartPos < pressLength / 2f) {
                // we consider the button released if it's more than half way up
                IsPressed = false;
                audioSource.PlayOneShot(releaseClip);
                releasedEvent.Invoke();
            }
        }

        // lock x and z values of local position and clamp local y value
        transform.localPosition = new Vector3(
            startLocalPos.x,
            Mathf.Clamp(transform.localPosition.y, startHeight - pressLength, startHeight),
            startLocalPos.z);
    }

    public void IncrementCollidersTouching() {
        if (CollidersTouching == 0) {
            isPressing = true;
        }

        CollidersTouching++;
    }

    public void DecrementCollidersTouching() {
        CollidersTouching--;
        if (CollidersTouching == 0) {
            isPressing = false;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            // this is the player's body, and we don't want them body-slamming the button unintentionally
            return;
        }

        IncrementCollidersTouching();
    }

    private void OnCollisionExit(Collision collision) {
        DecrementCollidersTouching();
    }
}
