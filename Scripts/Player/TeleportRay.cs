using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportRay : MonoBehaviour {

    [Header("Curve")]

    [SerializeField] private int iterations;
    [SerializeField] private float velocity;
    [SerializeField] private float smoothness;
    [SerializeField] private LayerMask layerMask;
    private RaycastHit hit;

    [Header("Renderer")]

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Color validColor;
    [SerializeField] private Color invalidColor;
    [SerializeField] private Transform endMarkerValid;
    [SerializeField] private Transform endMarkerInvalid;
    private Vector3[] curve;
    private int currIterations;

    [Header("Teleporter")]

    [SerializeField] private Teleport teleport;

    public bool IsTeleportAllowed { get; private set; }
    private bool isTeleportBlocked = false;

    // Start is called before the first frame update
    void Start() {
        curve = new Vector3[iterations];
        IsTeleportAllowed = false;
        DeactivateRay();
        ActionBlocker.AddEnterCallbackToActionBlockers(BlockTeleport);
        ActionBlocker.AddExitCallbackToActionBlockers(UnblockTeleport);
    }

    private void BlockTeleport() {
        isTeleportBlocked = true;
    }

    private void UnblockTeleport() {
        isTeleportBlocked = false;
    }

    public void RenderRay(Vector3 startPos, Vector3 direction) {
        CalculateCurve(startPos, direction);
        DrawRay();
    }

    /// <summary>
    /// Calculates the positions of the Vector3[] curve of the teleport ray.
    /// </summary>
    /// <param name="startPos">The Vector3 start position of the ray.</param>
    /// <param name="direction">The initial Vector3 direction of the ray.</param>
    private void CalculateCurve(Vector3 startPos, Vector3 direction) {
        curve[0] = startPos;

        Ray ray = new Ray(startPos, direction / smoothness);

        // the maximum number of positions is determined by the `iterations` variable
        for (int i = 1; i < iterations; i++) {
            if (Physics.Raycast(ray, out hit, 1f, layerMask) && !hit.collider.isTrigger) {
                // hit a surface; use hit point as final position
                currIterations = i;
                curve[i] = hit.point;
                return;
            }

            // curve the ray downwards using Physics.gravity (curve rate modified based on number of iterations and velocity)
            ray = new Ray(ray.origin + ray.direction / smoothness, ray.direction + (Physics.gravity / iterations / velocity));
            curve[i] = ray.origin;
        }

        // ray did not hit any surface
        currIterations = iterations - 1;
    }

    /// <summary>
    /// Draws the teleport ray using the calculated positions in the Vector3[] curve.
    /// </summary>
    private void DrawRay() {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = currIterations + 1;
        lineRenderer.SetPositions(curve);

        IsTeleportAllowed = !isTeleportBlocked && hit.collider != null && hit.collider.CompareTag("Ground");

        endMarkerValid.gameObject.SetActive(IsTeleportAllowed);
        endMarkerInvalid.gameObject.SetActive(!IsTeleportAllowed);

        if (IsTeleportAllowed) {
            lineRenderer.startColor = validColor;
            lineRenderer.endColor = validColor;
            endMarkerValid.position = hit.point;
            endMarkerValid.rotation = Quaternion.FromToRotation(hit.transform.up, hit.normal);
        } else {
            lineRenderer.startColor = invalidColor;
            lineRenderer.endColor = invalidColor;
            if (hit.collider == null) {
                endMarkerInvalid.position = curve[currIterations];
                endMarkerInvalid.rotation = Quaternion.FromToRotation(transform.up, curve[currIterations]);
            } else {
                endMarkerInvalid.position = hit.point;
                endMarkerInvalid.rotation = Quaternion.FromToRotation(transform.up, hit.normal);
            }
        }
    }

    /// <summary>
    /// Stops drawing the teleport ray.
    /// </summary>
    public void DeactivateRay() {
        lineRenderer.enabled = false;
        endMarkerValid.gameObject.SetActive(false);
        endMarkerInvalid.gameObject.SetActive(false);
    }

    /// <summary>
    /// Teleports the attached Teleport object to the final position in the Vector3[] curve.
    /// </summary>
    public void Teleport() {
        if (IsTeleportAllowed) {
            teleport.TeleportTo(curve[currIterations]);
        }
    }

    private void OnDrawGizmos() {
        if (curve != null && currIterations > 0) {
            Gizmos.DrawWireSphere(curve[currIterations], 0.5f);
        }
    }
}
