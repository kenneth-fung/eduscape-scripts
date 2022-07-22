using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTutorial : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (TutorialManager.Instance.CurrTutorialStage == TutorialManager.TutorialStage.TRANSFERRING
            && other.CompareTag("Player")) {
            TutorialManager.Instance.CompleteTeleportTutorial();
        }
    }
}
