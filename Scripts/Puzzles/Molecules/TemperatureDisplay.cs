using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TemperatureDisplay : MonoBehaviour {

    private TextMeshPro text;
    private AudioSource audioSource;

    [SerializeField] private TMP_FontAsset defaultFontAsset;
    [SerializeField] private TMP_FontAsset dangerFontAsset;

    [Space(10)]

    [SerializeField] private AudioClip tempIncreaseClip;
    [SerializeField] private AudioClip maxTempClip;
    [SerializeField] private float maxTempSoundLoopCount;
    [SerializeField] private bool doPlayMaxTempSound = true;

    void Awake() {
        text = GetComponentInChildren<TextMeshPro>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void Start() {
        audioSource.clip = tempIncreaseClip;
    }

    public void UpdateDisplay(float newTemp, bool isTempAboveMax) {
        text.text = newTemp.ToString("0.0") + " °C";

        if (isTempAboveMax) {
            SwitchFontAsset(dangerFontAsset);
        }

        if (isTempAboveMax && doPlayMaxTempSound) {
            StartCoroutine(PlayMaxTempSound());
        } else {
            audioSource.PlayOneShot(tempIncreaseClip);
        }
    }

    private void SwitchFontAsset(TMP_FontAsset asset) {
        text.font = asset;
    }

    private IEnumerator PlayMaxTempSound() {
        audioSource.clip = maxTempClip;

        for (int i = 0; i < maxTempSoundLoopCount; i++) {
            audioSource.Play();
            yield return new WaitUntil(() => !audioSource.isPlaying);
        }
    }
}
