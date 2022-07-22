using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SciFiLight : MonoBehaviour {

    private MeshRenderer mesh;
    [SerializeField] private int lightMaterialIndex = 1;
    [SerializeField] private Material unlitMaterial;
    [SerializeField] private Material litMaterial;
    [SerializeField] private Material dangerMaterial;

    private Light pointLight;
    [SerializeField] private Color defaultLightColor;
    [SerializeField] private Color dangerLightColor;

    private AudioSource audioSource;

    void Awake() {
        mesh = GetComponent<MeshRenderer>();
        pointLight = GetComponentInChildren<Light>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    public void TurnOn() {
        Material[] materials = mesh.materials;
        materials[lightMaterialIndex] = litMaterial;
        mesh.materials = materials;
        pointLight.gameObject.SetActive(true);
        pointLight.color = defaultLightColor;

        if (audioSource != null) {
            audioSource.Play();
        }
    }

    public void TurnOff() {
        Material[] materials = mesh.materials;
        materials[lightMaterialIndex] = unlitMaterial;
        mesh.materials = materials;
        pointLight.gameObject.SetActive(false);
    }

    public void TurnOnDanger() {
        Material[] materials = mesh.materials;
        materials[lightMaterialIndex] = litMaterial;
        mesh.materials = materials;
        pointLight.gameObject.SetActive(true);
        pointLight.color = dangerLightColor;
    }
}
