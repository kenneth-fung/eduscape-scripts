using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayScreen : MonoBehaviour {

    protected Animator anim;
    protected TextRollout text;

    public bool IsRollingOut { get => text.IsRollingOut; }

    [Header("Screen")]

    [SerializeField] protected MeshRenderer screenMesh;
    [SerializeField] protected Material screenMaterial;
    [SerializeField] protected Material deactivatedMaterial;
    [SerializeField] protected Material dangerMaterial;
    [SerializeField] protected float warningDuration;
    [SerializeField] protected float warningPulseDuration;

    [Space(10)]

    [SerializeField] protected AudioSource screenSoundSource;
    [SerializeField] protected AudioClip activateSound;
    [SerializeField] protected AudioClip deactivateSound;

    public bool IsPulsingWarningScreen { get; protected set; }

    [Header("Stowing")]

    [SerializeField] protected bool isStowedAtStart = true;
    [SerializeField] protected string stowAnimParam;

    [Space(10)]

    [SerializeField] protected AudioSource flipSoundSource;
    [SerializeField] protected AudioClip flipWhooshSound;
    [SerializeField] protected AudioClip flipEndSound;

    public bool IsStowed {
        get {
            AnimatorStateInfo animStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            return animStateInfo.IsName("Stowed") || animStateInfo.IsName("Unstow");
        }
    }

    protected virtual void Awake() {
        anim = GetComponent<Animator>();
        text = GetComponentInChildren<TextRollout>();
    }

    // Start is called before the first frame update
    protected virtual void Start() {
        IsPulsingWarningScreen = false;
        if (isStowedAtStart) {
            Stow();
        } else {
            Unstow();
        }
    }

    public void Stow() {
        anim.SetBool(stowAnimParam, true);
    }

    public void Unstow() {
        anim.SetBool(stowAnimParam, false);
    }

    public void PlayFlipWhooshSound() {
        flipSoundSource.PlayOneShot(flipWhooshSound);
    }

    public void PlayFlipEndSound() {
        flipSoundSource.PlayOneShot(flipEndSound);
    }

    public void ActivateScreen() {
        screenMesh.material = screenMaterial;
        text.gameObject.SetActive(true);
        screenSoundSource.PlayOneShot(activateSound);
    }

    public void SetText(string textToSet, bool rollOut = true, bool interruptCurrRollout = false) {
        if (interruptCurrRollout) {
            text.StopRollOut();
        }

        if (rollOut) {
            text.StartRollOut(textToSet, interruptCurrRollout);
        } else {
            text.Text = textToSet;
        }
    }

    public void DisplayWarning() {
        StartCoroutine(PulseWarningScreen());
    }

    protected IEnumerator PulseWarningScreen() {
        IsPulsingWarningScreen = true;

        text.Text = "WARNING";

        bool isRed = false;
        int numPulses = Mathf.FloorToInt(warningDuration / warningPulseDuration);
        for (int i = 0; i < numPulses; i++) {
            screenMesh.material = isRed ? null : dangerMaterial;
            text.gameObject.SetActive(!isRed);
            isRed = !isRed;
            yield return new WaitForSeconds(warningPulseDuration);
        }

        screenMesh.material = dangerMaterial;
        text.gameObject.SetActive(true);
        IsPulsingWarningScreen = false;
    }

    public void DeactivateScreen() {
        text.gameObject.SetActive(false);
        screenMesh.material = deactivatedMaterial;
        screenSoundSource.PlayOneShot(deactivateSound);
    }
}
