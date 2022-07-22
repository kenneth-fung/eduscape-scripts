using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuckDoorPanel : MonoBehaviour {

    private Rigidbody rb;
    private ElementTarget elementTarget;

    [SerializeField] private TextRollout rationale;

    private bool hasBeenHitByElement = false;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        elementTarget = GetComponent<ElementTarget>();
    }

    public void HitByElement(Element element) {
        if (!hasBeenHitByElement && element.ElementType != Element.Type.FIRE) {
            hasBeenHitByElement = true;
            rb.isKinematic = false;
            elementTarget.enabled = false;
            rationale.StartRollOut(Rationale.StuckDoor);
        }
    }
}
