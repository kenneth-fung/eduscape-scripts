using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculesSfx : MonoBehaviour {

    private AudioSource[] audioSources;

    [SerializeField] private AudioClip moleculeBounceClip;

    [Tooltip("The delay between each bounce per source.")]
    [SerializeField] private float bounceDelayPerSource;

    [Tooltip("This value is multiplied to the bounce delay when the temperature increases.")]
    [SerializeField] private float delayDecreaseMultiplier;

    void Awake() {
        audioSources = GetComponentsInChildren<AudioSource>();
    }

    void Start() {
        StartCoroutine(PlayAllSources());
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void DecreaseDelayBetweenBounceSounds() {
        bounceDelayPerSource *= delayDecreaseMultiplier;
    }

    public void StopAllSources() {
        StopAllCoroutines();
        foreach (AudioSource source in audioSources) {
            source.Stop();
        }
    }

    private IEnumerator PlayAllSources() {
        float interSourceDelay = bounceDelayPerSource / audioSources.Length;
        foreach (AudioSource source in audioSources) {
            StartCoroutine(PlaySingleSourceFx(source));
            yield return new WaitForSeconds(interSourceDelay);
        }
    }

    private IEnumerator PlaySingleSourceFx(AudioSource source) {
        while (true) {
            source.PlayOneShot(moleculeBounceClip);
            yield return new WaitForSeconds(bounceDelayPerSource);
        }
    }
}
