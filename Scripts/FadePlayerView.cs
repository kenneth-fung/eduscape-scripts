using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FadePlayerView : MonoBehaviour {

    private static readonly Color BLACK = Color.black * -5;
    private static readonly Color NONE = Color.white;

    public enum FadeDirection { IN, OUT }

    private ColorParameter colorParameter = null;

    public bool IsFading { get; private set; }

    [SerializeField] private UnityEvent fadeInCompleteEvent;
    public UnityEvent FadeInCompleteEvent { get => fadeInCompleteEvent; }

    [SerializeField] private UnityEvent fadeOutCompleteEvent;
    public UnityEvent FadeOutCompleteEvent { get => fadeOutCompleteEvent; }

    void Awake() {
        ColorAdjustments colorAdjustments;
        if (GetComponent<Volume>().profile.TryGet<ColorAdjustments>(out colorAdjustments)) {
            colorParameter = colorAdjustments.colorFilter;
        } else {
            Debug.LogWarning("No color adjustments found in post-processing volume profile.");
        }
    }

    void Start() {
        colorParameter.value = BLACK;
    }

    public void FadeIn() {
        StartCoroutine(Fade(FadeDirection.IN));
    }

    public void FadeOut() {
        StartCoroutine(Fade(FadeDirection.OUT));
    }

    private IEnumerator Fade(FadeDirection fadeDirection, float timing = 3f) {
        IsFading = true;
        Color fromColor = fadeDirection == FadeDirection.IN ? BLACK : NONE;
        Color toColor = fadeDirection == FadeDirection.IN ? NONE : BLACK;

        colorParameter.value = fromColor;
        float elapsedTime = 0;
        while (elapsedTime < timing) {
            colorParameter.Interp(fromColor, toColor, elapsedTime / timing);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        colorParameter.value = toColor;
        IsFading = false;

        if (fadeDirection == FadeDirection.IN) {
            fadeInCompleteEvent.Invoke();
        } else if (fadeDirection == FadeDirection.OUT) {
            fadeOutCompleteEvent.Invoke();
        }
    }

    private void OnDestroy() {
        fadeInCompleteEvent.RemoveAllListeners();
        fadeOutCompleteEvent.RemoveAllListeners();
    }
}
