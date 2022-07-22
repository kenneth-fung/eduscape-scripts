using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureDoorPanel : DoorPanel {

    [Tooltip("Two consecutive molecules must hit within this time frame to maintain pressure.")]
    [SerializeField] private float hitWindow = 0.5f;

    private float lastMoleculeHitTime = 0f;

    [Tooltip("The amount of time pressure needs to stay on the panel to open the door.")]
    [SerializeField] private float pressureDurationNeeded = 3f;

    private float pressureOnStartTime = 0f;
    private bool isPressureOn = false;

    // we use this variable to prevent molecules from spamming the door panel even if the door shuts again
    private bool hasDoorOpened = false;

    [SerializeField] private TextRollout rationale;

    protected override void Start() {
        base.Start();
        StartCoroutine(WaitForMoleculesToOpenDoor());
    }

    private IEnumerator WaitForMoleculesToOpenDoor() {
        while (!door.IsOpen) {
            if (status == Status.OPENED && Time.time - lastMoleculeHitTime > hitWindow) {
                isPressureOn = false;
                Close();
            }

            if (isPressureOn & Time.time - pressureOnStartTime >= pressureDurationNeeded) {
                hasDoorOpened = true;
                door.OpenDoor();
                rationale.StartRollOut(Rationale.PressurePuzzle);
            }

            yield return null;
        }
    }

    private void HandleMoleculeHit() {
        if (!isPressureOn) {
            isPressureOn = true;
            pressureOnStartTime = Time.time;
            Open();
        }

        lastMoleculeHitTime = Time.time;

        audioSource.clip = activateSound;
        audioSource.Play();
    }

    private void OnCollisionEnter(Collision collision) {
        if (!hasDoorOpened && !door.IsOpen && collision.gameObject.GetComponent<Molecule>()) {
            HandleMoleculeHit();
        }
    }
}
