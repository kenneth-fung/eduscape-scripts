using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Molecule : MonoBehaviour {

    private Rigidbody rb;

    [SerializeField] private float startSpeed;
    private float currSpeed;

    [SerializeField] private float tempToSpeedRatio = 1.5f;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();

        rb.velocity = new Vector3(
            Random.value > 0.5 ? 1f : -1f,
            Random.value > 0.5 ? 1f : -1f,
            Random.value > 0.5 ? 1f : -1f).normalized * startSpeed;

        currSpeed = startSpeed;
    }

    void FixedUpdate() {
        if (rb.velocity.sqrMagnitude < currSpeed * currSpeed) {
            rb.velocity = rb.velocity.normalized * currSpeed;
        }
    }

    private void BounceOffObject(Collision collisionInfo) {
        Vector3 collisionNormal = collisionInfo.GetContact(0).normal;

        Vector3 crossVelocityNormal = Vector3.Cross(rb.velocity, collisionNormal);
        Vector3 tangent = Vector3.Cross(collisionNormal, crossVelocityNormal);

        Vector3 tangentComponent = Vector3.Project(rb.velocity, tangent);
        Vector3 normalComponent = Vector3.Project(rb.velocity, collisionNormal);

        rb.velocity = (tangentComponent - normalComponent).normalized * currSpeed;
    }

    public void ChangeSpeed(float originalTemp, float newTemp) {
        currSpeed *= newTemp / originalTemp * tempToSpeedRatio;
    }

    private void OnCollisionEnter(Collision collision) {
        BounceOffObject(collision);
    }
}
