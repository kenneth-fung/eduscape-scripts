using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour {

    [SerializeField] private float minLocalY;
    [SerializeField] private float maxLocalY;
    private bool isMoving = false;

    [SerializeField] private float tempToSpeedRatio = 1.5f;
    private float currSpeed = 0f;

    [SerializeField] private PistonPlatform platform;
    private float pistonToPlatformSpeedRatio = 1f;

    // Start is called before the first frame update
    void Start() {
        pistonToPlatformSpeedRatio = CalculatePistonToPlatformSpeedRatio();
    }

    // Update is called once per frame
    void Update() {
        if (isMoving) {
            Move();
        }
    }

    private float CalculatePistonToPlatformSpeedRatio() {
        return platform.GetMoveableDistance() / (maxLocalY - minLocalY);
    }

    private bool IsWithinMovementLimits() {
        return transform.localPosition.y >= minLocalY && transform.localPosition.y <= maxLocalY;
    }

    public void ChangeSpeed(float originalTemp, float newTemp) {
        if (!IsWithinMovementLimits()) {
            return;
        }

        if (!isMoving) {
            StartMoving();
        }

        currSpeed += newTemp / originalTemp * tempToSpeedRatio;
    }

    private void StartMoving() {
        isMoving = true;
    }

    private void StopMoving() {
        isMoving = false;
    }

    private void Move() {
        if (!IsWithinMovementLimits()) {
            StopMoving();
            return;
        }

        transform.Translate(Vector3.down * currSpeed * Time.deltaTime);
        platform.ChangeSpeed(currSpeed * pistonToPlatformSpeedRatio);
    }
}
