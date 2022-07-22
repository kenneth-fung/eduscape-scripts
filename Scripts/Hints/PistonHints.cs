using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PistonHints : HintsController {

    /*
     * There is nowhere to place display screens without obstructing the view or forcing the player to turn away.
     * Thus, we hijack the tempeerature display to display the hint.
     */
    [SerializeField] private TextMeshPro temperatureText;
    [SerializeField] private AudioClip temperatureDisplaySound;

    protected IEnumerator hintDisplaying;

    /// <summary>
    /// Activates the piston's hints.
    /// </summary>
    protected override void ActivateHints() {
        base.ActivateHints();
        hintDisplaying = DisplayHint();
        StartCoroutine(hintDisplaying);
    }

    /// <summary>
    /// Displays the piston's hints on the temperature display.
    /// </summary>
    /// <returns>IEnumerator; this is a coroutine.</returns>
    protected IEnumerator DisplayHint() {
        string originalText = temperatureText.text;

        (string text, float duration)[] hintText = new (string text, float duration)[] {
            ("HEYO", 3),
            ("IT'S EDU", 2),
            ("MAYBE", 1),
            ("THEY", 1),
            ("NEED", 1),
            ("MORE", 1),
            ("ENERGY", 3),
            ("HMMM", 3)
        };

        foreach ((string text, float duration) in hintText) {
            temperatureText.text = text;
            foreach (AudioSource source in hintAlertAudioSources) {
                source.PlayOneShot(temperatureDisplaySound);
            }

            yield return new WaitForSeconds(duration);
        }

        temperatureText.text = originalText;
        foreach (AudioSource source in hintAlertAudioSources) {
            source.PlayOneShot(temperatureDisplaySound);
        }
    }

    /// <summary>
    /// Deactivates the molecule container's hints.
    /// </summary>
    public override void DeactivateHints() {
        base.DeactivateHints();
        if (hintDisplaying != null) {
            StopCoroutine(hintDisplaying);
        }
    }
}
