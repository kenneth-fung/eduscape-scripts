using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandController : MonoBehaviour {

    /// <summary>
    /// If true, checks for left hand controller input. False otherwise.
    /// </summary>
    public bool IsCheckingForLeftHandInput {
        get => checkForInput != null;
        set {
            if (value) {
                checkForInput = CheckForLeftHandInput();
                StartCoroutine(checkForInput);
            } else if (IsCheckingForLeftHandInput) {
                StopCoroutine(checkForInput);
            }
        }
    }

    [SerializeField] private string teleportTriggerName;
    private bool isTeleportTriggerHeld = false;

    private TeleportRay teleportRay;

    private IEnumerator checkForInput;

    // Start is called before the first frame update
    void Start() {
        teleportRay = GetComponent<TeleportRay>();
    }

    /// <summary>
    /// Checks for and handles left hand controller input.
    /// </summary>
    /// <returns>IEnumerator; this is a coroutine.</returns>
    private IEnumerator CheckForLeftHandInput() {
        while (IsCheckingForLeftHandInput) {
            if (Input.GetAxis(teleportTriggerName) < 1) {
                teleportRay.DeactivateRay();
                if (isTeleportTriggerHeld) {
                    isTeleportTriggerHeld = false;
                    teleportRay.Teleport();
                }
            }

            if (Input.GetAxis(teleportTriggerName) == 1) {
                isTeleportTriggerHeld = true;
                teleportRay.RenderRay(transform.position, transform.forward);
            }

            yield return null;
        }
    }
}
