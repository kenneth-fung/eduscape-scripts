using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour {

    [SerializeField] private float scaleSpeed = 1f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2f;

    [SerializeField] private AudioSource audioSource;
    
    public bool IsScalingUp { get; private set; }
    public bool IsScalingDown { get; private set; }
    public bool HasReachedMinSize { get => transform.localScale.x <= minScale; }
    public bool HasReachedMaxSize { get => transform.localScale.x >= maxScale; }

    private bool isUpButtonHeld = false;
    public bool IsUpButtonHeld {
        set {
            isUpButtonHeld = value;
            if (!value && isDownButtonHeld) {
                StartScalingDown();
            } else if (value && isDownButtonHeld) {
                StopScalingDown();
            }
        }
    }

    private bool isDownButtonHeld = false;
    public bool IsDownButtonHeld {
        set {
            isDownButtonHeld = value;
            if (!value && isUpButtonHeld) {
                StartScalingUp();
            } else if (value && isUpButtonHeld) {
                StopScalingUp();
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (IsScalingUp && !IsScalingDown) {
            if (HasReachedMaxSize) {
                IsScalingUp = false;
            } else {
                transform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
            }
        } else if (!IsScalingUp && IsScalingDown) {
            if (HasReachedMinSize) {
                IsScalingDown = false;
            } else {
                transform.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;
            }
        }

        if (!IsScalingUp && !IsScalingDown && audioSource.isPlaying) {
            audioSource.Stop();
        }
    }

    public void StartScalingUp() {
        if (HasReachedMaxSize || isDownButtonHeld) {
            return;
        }

        IsScalingUp = true;
        audioSource.Play();
    }

    public void StopScalingUp() {
        IsScalingUp = false;
        audioSource.Stop();
    }

    public void StartScalingDown() {
        if (HasReachedMinSize || isUpButtonHeld) {
            return;
        }

        IsScalingDown = true;
        audioSource.Play();
    }

    public void StopScalingDown() {
        IsScalingDown = false;
        audioSource.Stop();
    }
}
