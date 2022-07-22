using UnityEngine;

/// <summary>
/// ExteriorMusicTrigger is responsible for playing the exterior area music when the player enters it.
/// </summary>
public class ExteriorMusicTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            MusicManager.Instance.PlayExteriorMusic();
        }
    }
}
