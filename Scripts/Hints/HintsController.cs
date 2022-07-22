using System.Collections;
using UnityEngine;

/// <summary>
/// This component controls the hints for a puzzle.
/// </summary>
public abstract class HintsController : MonoBehaviour {

    [SerializeField] protected string hintText;
    [SerializeField] protected float hintCountdownTime = 60f;
    [SerializeField] protected AudioSource[] hintAlertAudioSources;

    public bool IsPuzzleSolved { get; protected set; }
    public bool AreHintsProvided { get; protected set; }
    public bool IsCountingDown { get; protected set; }

    protected IEnumerator countdown;

    protected virtual void Start() {}

    /// <summary>
    /// Counts down to the moment when the hints should be provided.
    /// </summary>
    /// <returns>IEnumerator; this is a coroutine.</returns>
    protected IEnumerator CountDownToHintsActivation() {
        IsCountingDown = true;

        yield return new WaitForSeconds(hintCountdownTime);

        IsCountingDown = false;
        AreHintsProvided = true;

        ActivateHints();
    }

    /// <summary>
    /// Activates the hints.
    /// </summary>
    protected virtual void ActivateHints() {
        foreach (AudioSource source in hintAlertAudioSources) {
            source.Play();
        }
    }

    /// <summary>
    /// Deactivates the hints, indicating the puzzle has been or is about to be solved.
    /// </summary>
    public virtual void DeactivateHints() {
        IsPuzzleSolved = true;
        if (countdown != null) {
            StopCoroutine(countdown);
        }
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (!IsPuzzleSolved && !AreHintsProvided && !IsCountingDown && other.CompareTag("Player")) {
            countdown = CountDownToHintsActivation();
            StartCoroutine(countdown);
        }
    }
}
