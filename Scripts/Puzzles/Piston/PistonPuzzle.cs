using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonPuzzle : MonoBehaviour {

    [SerializeField] private TextRollout[] rationales;

    [SerializeField] private BoxCollider teleportBlocker;
    [SerializeField] private BoxCollider[] raisedGroundColliders;

    public void ActivateRationales() {
        foreach (TextRollout rationale in rationales) {
            rationale.StartRollOut(Rationale.PistonPuzzle, true);
        }
    }

    public void DeactivateTeleportBlocker() {
        teleportBlocker.gameObject.SetActive(false);
    }

    public void ActivateRaisedGroundColliders() {
        foreach (BoxCollider collider in raisedGroundColliders) {
            collider.gameObject.layer = LayerMask.NameToLayer("Ground");
            collider.gameObject.tag = "Ground";
        }
    }
}
