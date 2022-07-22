using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileNetDestroyer : MonoBehaviour {
    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<ElementProjectile>()) {
            Destroy(other.gameObject);
        }
    }
}
