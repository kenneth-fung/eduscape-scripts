using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionBlocker : MonoBehaviour {

    public static List<ActionBlocker> actionBlockers = new List<ActionBlocker>();

    private bool isHandInsideBlocker = false;

    [SerializeField] private UnityEvent handEnterActionBlockerEvent;
    public UnityEvent HandEnterActionBlockerEvent { get => handEnterActionBlockerEvent; }

    [SerializeField] private UnityEvent handExitActionBlockerEvent;
    public UnityEvent HandExitActionBlockerEvent { get => handExitActionBlockerEvent; }

    void Awake() {
        actionBlockers.Add(this);
    }

    public static void AddEnterCallbackToActionBlockers(UnityAction callback) {
        foreach (ActionBlocker actionBlocker in actionBlockers) {
            actionBlocker.handEnterActionBlockerEvent.AddListener(callback);
        }
    }

    public static void AddExitCallbackToActionBlockers(UnityAction callback) {
        foreach (ActionBlocker actionBlocker in actionBlockers) {
            actionBlocker.handExitActionBlockerEvent.AddListener(callback);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Hand")) {
            isHandInsideBlocker = true;
            handEnterActionBlockerEvent.Invoke();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Hand")) {
            isHandInsideBlocker = false;
            handExitActionBlockerEvent.Invoke();
        }
    }

    public void Activate() {
        gameObject.SetActive(true);
    }

    public void Deactivate() {
        // handle case where player's hands are inside the action blocker during deactivation
        if (isHandInsideBlocker) {
            isHandInsideBlocker = false;
            handExitActionBlockerEvent.Invoke();
        }

        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        handEnterActionBlockerEvent.RemoveAllListeners();
        handExitActionBlockerEvent.RemoveAllListeners();
    }
}
