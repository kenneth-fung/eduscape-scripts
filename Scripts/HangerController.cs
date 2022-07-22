using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HangerController : MonoBehaviour {

    [SerializeField] private Light[] lightSetFirst;
    [SerializeField] private Light[] lightSetSecond;

    [SerializeField] private AudioSource[] firstSetAudioSource;
    [SerializeField] private AudioSource[] secondSetAudioSource;

    [SerializeField] private Animator[] wallAnims;
    [SerializeField] private string wallAnimParam = "isOpen";

    [SerializeField] private AudioSource[] wallOpenSoundSources;

    // Start is called before the first frame update
    void Start() {
        DeactivateSign();
    }

    public void ActivateSign() {
        StartCoroutine(ActivateSignDramatic());
    }

    private IEnumerator ActivateSignDramatic() {
        yield return new WaitForSeconds(0.5f);

        foreach (AudioSource source in firstSetAudioSource) {
            source.Play();
        }

        foreach (Light light in lightSetFirst) {
            light.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);

        foreach (AudioSource source in secondSetAudioSource) {
            source.Play();
        }

        foreach (Light light in lightSetSecond) {
            light.gameObject.SetActive(true);
        }
    }

    private void DeactivateSign() {
        foreach (Light light in lightSetFirst) {
            light.gameObject.SetActive(false);
        }

        foreach (Light light in lightSetSecond) {
            light.gameObject.SetActive(false);
        }
    }

    public void OpenWalls() {
        foreach (Animator wallAnim in wallAnims) {
            wallAnim.SetBool(wallAnimParam, true);
        }

        foreach (AudioSource source in wallOpenSoundSources) {
            source.Play();
        }
    }

    public void CloseWalls() {
        foreach (Animator wallAnim in wallAnims) {
            wallAnim.SetBool(wallAnimParam, false);
        }
    }
}
