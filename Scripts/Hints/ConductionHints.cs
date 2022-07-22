using System.Collections;
using UnityEngine;
using TMPro;

public class ConductionHints : HintsController {

    [SerializeField] protected Door hangarDoor;

    [SerializeField] protected DisplayScreen[] hintScreens;
    [SerializeField] protected float fontSize;
    [SerializeField] protected float originalFontSize;

    protected IEnumerator[] hintsDisplaying;

    protected override void Start() {
        base.Start();
        hintsDisplaying = new IEnumerator[hintScreens.Length];
    }

    /// <summary>
    /// Activates the conduction hints.
    /// </summary>
    protected override void ActivateHints() {
        base.ActivateHints();
        for (int i = 0; i < hintScreens.Length; i++) {
            hintsDisplaying[i] = DisplayHint(hintScreens[i]);
            StartCoroutine(hintsDisplaying[i]);
        }
    }

    /// <summary>
    /// Displays the hint on the given display screen.
    /// </summary>
    /// <param name="screen">The screen on which to display the hints.</param>
    /// <returns>IEnumerator; this is a coroutine.</returns>
    protected IEnumerator DisplayHint(DisplayScreen screen) {
        TextMeshPro screenText = screen.GetComponentInChildren<TextMeshPro>();

        string originalText = screenText.text;
        screen.SetText("", false);

        screenText.fontSize = fontSize;

        screen.SetText(hintText);
        yield return new WaitUntil(() => !screen.IsRollingOut);

        yield return new WaitForSeconds(5f);

        screen.SetText("", false);

        screenText.fontSize = originalFontSize;
        screen.SetText(originalText);
    }

    /// <summary>
    /// Deactivates the conduction hints.
    /// </summary>
    public override void DeactivateHints() {
        base.DeactivateHints();
        for (int i = 0; i < hintsDisplaying.Length; i++) {
            if (hintsDisplaying[i] != null) {
                hintScreens[i].SetText("", false, true);
                hintScreens[i].GetComponentInChildren<TextMeshPro>().fontSize = originalFontSize;
                StopCoroutine(hintsDisplaying[i]);
            }
        }
    }

    protected override void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);
        if (hangarDoor.IsOpen && other.CompareTag("Player")) {
            hangarDoor.CloseDoor();
        }
    }
}
