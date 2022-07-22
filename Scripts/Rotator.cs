using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    public enum RotationDirection { CLOCKWISE, ANTICLOCKWISE, FORWARD, BACK }

    [Tooltip("The pivot point about which to rotate. If none is given, it is set to the object's transform.")]
    [SerializeField] private Transform pivot;
    
    [SerializeField] private float speed;

    [SerializeField] private RotationDirection rotationAxis = RotationDirection.ANTICLOCKWISE;

    void Start() {
        if (pivot == null) {
            pivot = transform;
        }
    }

    // Update is called once per frame
    void Update() {
        Vector3 axis = Vector3.up;
        switch (rotationAxis) {
            case RotationDirection.CLOCKWISE:
                axis = transform.parent == null ? Vector3.down : -transform.parent.up;
                break;
            case RotationDirection.ANTICLOCKWISE:
                axis = transform.parent == null ? Vector3.up : transform.parent.up;
                break;
            case RotationDirection.FORWARD:
                axis = transform.parent == null ? Vector3.back : -transform.parent.forward;
                break;
            case RotationDirection.BACK:
                axis = transform.parent == null ? Vector3.forward : transform.parent.forward;
                break;
        }

        transform.RotateAround(pivot.position, axis, speed * Time.deltaTime);
    }
}
