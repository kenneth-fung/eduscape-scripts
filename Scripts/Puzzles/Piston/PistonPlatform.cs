using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PistonPlatform : MonoBehaviour {

    [SerializeField] private float minLocalY;
    [SerializeField] private float maxLocalY;

    [Space(10)]


    [SerializeField] private AudioSource startStopAudioSource;
    [SerializeField] private AudioSource raiseAudioSource;

    [SerializeField] private AudioClip startSound;
    [SerializeField] private AudioClip raiseFadeInSound;
    [SerializeField] private AudioClip raiseLoopSound;
    [SerializeField] private AudioClip stopSound;

    [Tooltip("The length of time in the stop sound before the full impact occurs.")]
    [SerializeField] private float stopSoundPreemptLength;
    private bool hasStopSoundStarted = false;

    [Space(10)]

    [SerializeField] private UnityEvent reachTopEvent;
    [SerializeField] private UnityEvent reachBottomEvent;

    private float currSpeed = 0f;
    private bool isMoving = false;

    public bool HasReachedTop { get => transform.localPosition.y > maxLocalY; }
    public bool HasReachedBottom { get => transform.localPosition.y < minLocalY; }

    void Update() {
        if (isMoving) {
            Move(currSpeed);
            // if time to reach destination is less than or equals to stop sound preempt length
            if (!hasStopSoundStarted && Mathf.Abs(maxLocalY - transform.localPosition.y) / currSpeed <= stopSoundPreemptLength) {
                hasStopSoundStarted = true;
                startStopAudioSource.clip = stopSound;
                startStopAudioSource.Play();
            }
        }
    }

    public float GetMoveableDistance() {
        return maxLocalY - minLocalY;
    }

    public void StartMoving() {
        isMoving = true;
        startStopAudioSource.clip = startSound;
        startStopAudioSource.Play();
        StartCoroutine(StartPlayingRaiseSound());
    }

    public void StopMoving() {
        isMoving = false;
        raiseAudioSource.Stop();
    }

    public void ChangeSpeed(float speed) {
        if (!isMoving) {
            StartMoving();
        }

        currSpeed = speed;
    }

    private void Move(float speed) {
        if (HasReachedTop) {
            reachTopEvent.Invoke();
        } else if (HasReachedBottom) {
            reachBottomEvent.Invoke();
        }

        if (HasReachedTop || HasReachedBottom) {
            StopMoving();
            return;
        }

        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private IEnumerator StartPlayingRaiseSound() {
        raiseAudioSource.PlayOneShot(raiseFadeInSound);

        yield return new WaitForSeconds(raiseFadeInSound.length);

        raiseAudioSource.clip = raiseLoopSound;
        raiseAudioSource.loop = true;
        raiseAudioSource.Play();
    }
}
