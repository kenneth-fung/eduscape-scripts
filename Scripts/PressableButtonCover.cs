using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressableButtonCover : MonoBehaviour {

    private Animator anim;
    private AudioSource audioSource;

    [SerializeField] private string openCloseAnimParam = "isOpen";

    public bool IsOpen { get => anim.GetBool(openCloseAnimParam); }

    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    [SerializeField] private GameObject dummyButton;
    [SerializeField] private GameObject actualButton;

    void Awake() {
        anim = GetComponent<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    // Start is called before the first frame update
    void Start() {
        SetDummyActualButtons();
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Open() {
        anim.SetBool(openCloseAnimParam, true);
        SetDummyActualButtons();
    }

    public void PlayOpenSound() {
        audioSource.PlayOneShot(openSound);
    }

    public void Close() {
        anim.SetBool(openCloseAnimParam, false);
        SetDummyActualButtons();
    }

    public void PlayCloseSound() {
        audioSource.PlayOneShot(closeSound);
    }

    private void SetDummyActualButtons() {
        dummyButton.SetActive(!IsOpen);
        actualButton.SetActive(IsOpen);
    }
}
