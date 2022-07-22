using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextRollout : MonoBehaviour {

    private TextMeshPro text;
    private AudioSource audioSource;

    public string Text { get => text.text; set => text.text = value; }

    [SerializeField] private float timeBetweenCharacters = 0.05f;
    [SerializeField] private float punctuationPauseTime = 0.5f;

    public bool IsRollingOut { get; private set; }

    private IEnumerator currentRollout;

    void Awake() {
        text = GetComponent<TextMeshPro>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    void Start() {
        IsRollingOut = false;
    }

    public void StartRollOut(string textToSet, bool interruptCurrRollout = false) {
        if (interruptCurrRollout) {
            StopRollOut();
        }

        currentRollout = RollOutText(textToSet);
        StartCoroutine(currentRollout);
    }

    public void StopRollOut() {
        if (IsRollingOut) {
            StopCoroutine(currentRollout);
            audioSource.Stop();
            IsRollingOut = false;
        }
    }

    private IEnumerator RollOutText(string textToSet) {
        yield return new WaitUntil(() => !IsRollingOut);

        IsRollingOut = true;
        text.text = "";
        audioSource.Play();

        string currText = "";
        bool isPausedForPunctuation = false;
        for (int i = 0; i < textToSet.Length; i++) {
            if (isPausedForPunctuation) {
                isPausedForPunctuation = false;
                audioSource.Play();
            }

            currText += textToSet[i];
            text.text = currText;

            if (i >= textToSet.Length - 1) {
                break;
            }

            if (IsPunctuationPause(textToSet[i])) {
                isPausedForPunctuation = true;
                audioSource.Pause();
                yield return new WaitForSeconds(punctuationPauseTime);
            } else {
                yield return new WaitForSeconds(timeBetweenCharacters);
            }
        }

        audioSource.Stop();
        IsRollingOut = false;
    }

    private bool IsPunctuationPause(char character) {
        return character == '.' || character == ','
            || character == '?' || character == ';'
            || character == ':' || character == '-';
    }
}
