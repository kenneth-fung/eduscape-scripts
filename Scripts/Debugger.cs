using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour {

    private static Debugger _instance;
    public static Debugger Instance { get => _instance; }

    [SerializeField] private bool enableKeyboardShortcuts = true;

    private RightHandController rightHandController;
    private HeatGenerator[] generators;

    [SerializeField] private KeyCode cycleElement = KeyCode.E;
    [SerializeField] private KeyCode shootElement = KeyCode.R;
    [SerializeField] private KeyCode heatGenerators = KeyCode.C;

    void Awake() {
        // singleton
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }

    void Start() {
        if (enableKeyboardShortcuts) {
            rightHandController = FindObjectOfType<RightHandController>();
            generators = FindObjectsOfType<HeatGenerator>();

            // enabling debugger in play mode will have no effect
            StartCoroutine(HandleKeyboardInput());
        }
    }

    private IEnumerator HandleKeyboardInput() {
        while (enableKeyboardShortcuts) {
            if (Input.GetKeyDown(cycleElement)) {
                rightHandController.HandleCycle();
            }

            if (Input.GetKeyDown(shootElement)) {
                rightHandController.HandleShootElement();
            }

            if (Input.GetKeyDown(heatGenerators)) {
                foreach (HeatGenerator generator in generators) {
                    generator.StartHeating();
                }
            }

            yield return null;
        }
    }
}
