using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour {

    [SerializeField] private OVRInput.Controller controller;
    public OVRInput.Controller Controller { get => controller; }

    [SerializeField] private Vector3 defaultRotation;

    public Vector3 DefaultRotation { get => defaultRotation; }

    public Vector3 OffsetRotation { get; set; }

    void Start() {
        OffsetRotation = defaultRotation;
    }

    // Update is called once per frame
    void Update() {
        transform.localPosition = OVRInput.GetLocalControllerPosition(Controller);
        transform.localRotation = Quaternion.Euler(OffsetRotation + OVRInput.GetLocalControllerRotation(Controller).eulerAngles);
    }
}
