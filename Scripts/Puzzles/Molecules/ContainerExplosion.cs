using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ContainerExplosion : MonoBehaviour {

    private MoleculesContainer container;

    [SerializeField] private GameObject explosionPrefab;

    [Space(10)]

    [SerializeField] private AudioMixerGroup explosionSfxGroup;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip shatterSound;

    [Space(10)]

    [SerializeField] private GameObject switchShield;
    [SerializeField] private ExteriorGateSwitch gateSwitch;

    [Space(10)]

    [SerializeField] private MoleculeContainerHints hintsController;
    [SerializeField] private TextRollout[] rationales;

    private bool isExploding = false;

    void Awake() {
        container = GetComponent<MoleculesContainer>();
    }

    public void StartExplosion() {
        if (isExploding) {
            return;
        }

        isExploding = true;
        Invoke(nameof(Explode), 3f);
        hintsController.DeactivateHints();
    }

    public void Explode() {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        audioSource.outputAudioMixerGroup = explosionSfxGroup;
        audioSource.PlayOneShot(explosionSound);
        audioSource.PlayOneShot(shatterSound);

        switchShield.SetActive(false);
        gateSwitch.StartRaise();

        foreach (TextRollout rationale in rationales) {
            rationale.StartRollOut(Rationale.MoleculeContainer, true);
        }
        
        container.Destroy();
    }
}
