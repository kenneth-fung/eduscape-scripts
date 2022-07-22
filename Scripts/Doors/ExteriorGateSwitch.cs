using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ExteriorGateSwitch controls the switch that controls ExteriorGate.
/// </summary>
public class ExteriorGateSwitch : MonoBehaviour {

    [Tooltip("The specific GameObject to use to raise the switch into position.")]
    [SerializeField] private GameObject objectToRaise;
    [Tooltip("The local Y value to raise the object to raise to.")]
    [SerializeField] private float raiseToLocalYValue;
    [SerializeField] private float preRaisePauseLength = 1.5f;
    [SerializeField] private float raiseSpeed = 1f;

    [Space(10)]

    [Tooltip("Because we cannot move the actual switch, we move the dummy switch, then swap it out for the actual switch once in position.")]
    [SerializeField] private GameObject dummySwitch;
    [SerializeField] private GameObject actualSwitch;

    [Space(10)]

    [SerializeField] private AudioSource raiseAudioSource;
    [SerializeField] private AudioClip raiseClip;
    [SerializeField] private AudioClip stopClip;

    [Space(10)]

    [SerializeField] private UnityEvent raisedEvent;

    /// <summary>
    /// Starts the raising of the exterior gate switch into position.
    /// </summary>
    public void StartRaise() {
        StartCoroutine(Raise());
    }

    /// <summary>
    /// Raises the exterior gate switch into position for the player to interact with.
    /// </summary>
    /// <returns>IEnumerator; this is a coroutine.</returns>
    private IEnumerator Raise() {
        yield return new WaitForSeconds(preRaisePauseLength);

        raiseAudioSource.clip = raiseClip;
        raiseAudioSource.Play();

        while (objectToRaise.transform.localPosition.y < raiseToLocalYValue) {
            objectToRaise.transform.Translate(new Vector3(0, raiseSpeed * Time.deltaTime, 0));
            yield return null;
        }

        raiseAudioSource.clip = stopClip;
        raiseAudioSource.Play();

        actualSwitch.SetActive(true);
        dummySwitch.SetActive(false);

        raisedEvent.Invoke();
    }
}
