using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTarget : MonoBehaviour {

    private Rigidbody rb;

    [SerializeField] private float turnTimeMultiplier = 20f;
    [SerializeField] private float forwardSpeed;
    [SerializeField] private float rotateSpeed;

    [SerializeField] private Transform[] waypoints;
    private int currWaypoint = 0;

    public bool IsMoving { get; private set; }

    public int NumWaypointsRemaining { get => waypoints.Length - currWaypoint - 1; }

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        if (IsMoving) {
            Transform dest = waypoints[currWaypoint];
            transform.rotation = Quaternion.RotateTowards(transform.rotation, dest.rotation, rotateSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate() {
        if (IsMoving) {
            if (Vector3.Distance(transform.position, waypoints[currWaypoint].position) < 0.1f) {
                currWaypoint++;
                if (currWaypoint >= waypoints.Length) {
                    IsMoving = false;
                    return;
                }
            }

            Transform dest = waypoints[currWaypoint];
            Vector3 toDest = (dest.position - rb.position).normalized;
            Vector3 velocityDirection = Vector3.Slerp(transform.forward, toDest, Time.time / turnTimeMultiplier);
            rb.velocity = velocityDirection * rb.velocity.magnitude;
        }
    }

    public void StartMoving() {
        rb.velocity = transform.forward * forwardSpeed;
        IsMoving = true;
    }

    public void StopMoving() {
        rb.velocity = Vector3.zero;
        IsMoving = false;
    }
}
