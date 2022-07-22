using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ExteriorGate controls the behaviour of the gate leading to the exterior area of the station.
/// </summary>
public class ExteriorGate : MonoBehaviour {

    private Animator anim;
    private AudioSource audioSource;

    [Header("Animation")]

    [SerializeField] private string openBooleanParameter;
    [SerializeField] private string openingAnimationTag;
    [SerializeField] private string closingAnimationTag;

    [Header("Audio")]

    [Tooltip("The sound that should play when the gate starts moving.")]
    [SerializeField] private AudioClip startSound;
    [Tooltip("The creaking sound that should play when the gate is about to stop moving.")]
    [SerializeField] private AudioClip creakSound;
    [Tooltip("The impact sound that should play when the gate comes to a halt.")]
    [SerializeField] private AudioClip impactSound;
    [Tooltip("The reverb sound that should play after the impact of the gate stopping.")]
    [SerializeField] private AudioClip impactReverbSound;

    [Header("Action Blockers")]

    [Tooltip("The action blocker in the negative direction of the x axis.")]
    [SerializeField] private ActionBlocker actionBlockerLeft;
    [Tooltip("The action blocker in the positive direction of the x axis.")]
    [SerializeField] private ActionBlocker actionBlockerRight;

    public bool IsOpen { get => anim.GetBool(openBooleanParameter); }

    [Space(10)]

    [SerializeField] private UnityEvent raisedEvent;

    // Start is called before the first frame update
    void Start() {
        anim = GetComponent<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        ActivateActionBlocker();
    }

    /// <summary>
    /// Activates the Action Blocker on the side of the gate opposite from where the player is.
    /// </summary>
    private void ActivateActionBlocker() {
        // get the player's position in the local space of the gate
        Vector3 localPos = transform.InverseTransformPoint(LevelManager.Instance.PlayerBody.transform.position);

        // use the x axis because the gate is not rotated about the y axis (forward axis is the z axis)
        if (localPos.x < 0f) {
            actionBlockerLeft.Deactivate();
            actionBlockerRight.Activate();
        } else {
            actionBlockerLeft.Activate();
            actionBlockerRight.Deactivate();
        }
    }

    /// <summary>
    /// Opens the gate.
    /// </summary>
    public void Open() {
        actionBlockerLeft.Deactivate();
        actionBlockerRight.Deactivate();
        anim.SetBool(openBooleanParameter, true);
    }

    /// <summary>
    /// Closes the gate.
    /// </summary>
    public void Close() {
        ActivateActionBlocker();
        anim.SetBool(openBooleanParameter, false);
    }

    /// <summary>
    /// Returns true if the gate is opening or closing. False otherwise.
    /// </summary>
    public bool IsOpeningOrClosing() {
        // use the state of the gate in the animator
        AnimatorStateInfo animStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return animStateInfo.IsTag(openingAnimationTag) || animStateInfo.IsTag(closingAnimationTag);
    }

    /// <summary>
    /// Toggles the open/close status of the gate.
    /// </summary>
    public void Toggle() {
        if (IsOpeningOrClosing()) {
            return;
        }

        if (IsOpen) {
            Close();
        } else {
            Open();
        }
    }

    public void PlayStartSound() {
        audioSource.PlayOneShot(startSound);
    }

    public void PlayCreakSouund() {
        audioSource.PlayOneShot(creakSound);
    }

    public void PlayImpactSound() {
        audioSource.PlayOneShot(impactSound);
    }

    public void PlayImpactReverbSound() {
        audioSource.PlayOneShot(impactReverbSound);
    }

    public void NotifyRaisedEvent() {
        raisedEvent.Invoke();
    }
}
