using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SegmentChangeGlow : MonoBehaviour {

    private MeshRenderer mesh;
    private Light glowLight;

    [SerializeField] private float glowSpeed;
    private float maxGlowIntensity;

    public Material GlowMaterial { get => mesh.material; set => mesh.material = value; }
    public Color GlowColor { get => glowLight.color; set => glowLight.color = value; }

    void Awake() {
        mesh = GetComponent<MeshRenderer>();
        glowLight = GetComponentInChildren<Light>();
    }

    // Start is called before the first frame update
    void Start() {
        Color color = mesh.material.color;
        color = new Color(color.r, color.g, color.b, 0f);
        mesh.material.color = color;

        maxGlowIntensity = glowLight.intensity;
        glowLight.intensity = 0f;
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void ActivateMaxGlow() {
        Color color = mesh.material.color;
        color = new Color(color.r, color.g, color.b, 1f);
        mesh.material.color = color;

        glowLight.intensity = maxGlowIntensity;
    }

    private void StopGlow() {
        Color color = mesh.material.color;
        color = new Color(color.r, color.g, color.b, 0f);
        mesh.material.color = color;

        glowLight.intensity = 0f;
    }

    public IEnumerator BrieflyGlow(float duration, UnityAction maxGlowAction = null, UnityAction postStopAction = null) {
        StopGlow();

        float upDownDurations = duration / 2;

        while (mesh.material.color.a < 0.99f) {
            Color color = mesh.material.color;
            color = new Color(color.r, color.g, color.b, color.a + glowSpeed * upDownDurations * Time.deltaTime);
            mesh.material.color = color;

            glowLight.intensity += glowSpeed * upDownDurations * maxGlowIntensity * Time.deltaTime;
            yield return null;
        }

        ActivateMaxGlow();
        maxGlowAction?.Invoke();

        while (mesh.material.color.a > 0.01f) {
            Color color = mesh.material.color;
            color = new Color(color.r, color.g, color.b, color.a - glowSpeed * upDownDurations * Time.deltaTime);
            mesh.material.color = color;

            glowLight.intensity -= glowSpeed * upDownDurations * maxGlowIntensity * Time.deltaTime;
            yield return null;
        }

        StopGlow();
        postStopAction?.Invoke();
    }
}
