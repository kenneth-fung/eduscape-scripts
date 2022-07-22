using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipEngine : MonoBehaviour {

    private Light[] lights;
    private AudioSource audioSource;

    [SerializeField] private MeshRenderer heatMesh;

    [SerializeField] private Material cooledMaterial;
    [SerializeField] private Material heatedMaterial;

    [SerializeField] private AudioClip heatSound;

    public bool IsHeated { get; private set; }

    [SerializeField] private UnityEvent heatEvent;

    void Awake() {
        lights = GetComponentsInChildren<Light>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    // Start is called before the first frame update
    void Start() {
        Cool();
    }

    public void Heat() {
        IsHeated = true;
        heatMesh.material = heatedMaterial;

        foreach (Light light in lights) {
            light.gameObject.SetActive(true);
        }

        audioSource.Play();
        audioSource.PlayOneShot(heatSound);

        heatEvent.Invoke();
    }

    public void Cool() {
        IsHeated = false;
        heatMesh.material = cooledMaterial;

        foreach (Light light in lights) {
            light.gameObject.SetActive(false);
        }

        audioSource.Stop();
        audioSource.PlayOneShot(heatSound);
    }
}
