using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerElements : MonoBehaviour {

    [SerializeField] private AudioSource audioSource;

    /// <summary>
    /// The elements the player currently has.
    /// </summary>
    private List<ElementHeld> elements;

    [Tooltip("Held elements added here will be given to the player at game start.")]
    [SerializeField] private ElementHeld[] startingElements;

    private int activeElementIndex = -1;

    /// <summary>
    /// Gets and sets the active held element based on the <c>activeElementIndex</c>.
    /// </summary>
    public ElementHeld ActiveElement {
        get => activeElementIndex == -1 ? null : elements[activeElementIndex];
        set {
            if (ActiveElement != null) {
                ActiveElement.gameObject.SetActive(false);
            }

            activeElementIndex = value == null ? -1 : elements.IndexOf(value);

            if (ActiveElement != null) {
                ActiveElement.gameObject.SetActive(true);
                switchToElementEvent.Invoke();
            } else {
                switchFromElementsEvent.Invoke();
            }
        }
    }

    [Space(10)]

    [SerializeField] private UnityEvent switchToElementEvent;
    public UnityEvent SwitchToElementEvent { get => switchToElementEvent; }

    [Space(10)]

    [SerializeField] private UnityEvent switchFromElementsEvent;
    public UnityEvent SwitchFromElementEvent { get => switchFromElementsEvent; }

    [Space(10)]

    [SerializeField] private float shootForce;

    [Space(10)]

    [SerializeField] private UnityEvent shootElementEvent;
    public UnityEvent ShootElementEvent { get => shootElementEvent; }

    void Awake() {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    // Start is called before the first frame update
    void Start() {
        elements = new List<ElementHeld>(startingElements);
    }

    public void AddElement(ElementPickup element) {
        ElementHeld correspondingHeldElement = element.CorrespondingHeldElement;
        if (elements.Contains(correspondingHeldElement)) {
            return;
        }

        elements.Add(correspondingHeldElement);
        ActiveElement = correspondingHeldElement;
        audioSource.PlayOneShot(element.PickUpSound);
    }

    public void RemoveElement(ElementHeld element) {
        if (ActiveElement == element) {
            ActiveElement = null;
        }

        elements.Remove(element);
    }

    public void CycleActiveElement() {
        ActiveElement = activeElementIndex == elements.Count - 1
            ? null
            : elements[activeElementIndex + 1];

        if (ActiveElement == null) {
            return;
        }

        audioSource.PlayOneShot(ActiveElement.SwitchSound);
    }

    public IEnumerator ShootActiveElement() {
        ElementHeld shotElement = ActiveElement;
        ActiveElement.gameObject.SetActive(false);

        audioSource.PlayOneShot(shotElement.ShootSound);
        ActiveElement.Shoot(transform.forward, shootForce);
        shootElementEvent.Invoke();

        yield return new WaitForSeconds(0.5f);
        
        if (ActiveElement == shotElement) {
            ActiveElement.gameObject.SetActive(true);
        }
    }

    private void OnDestroy() {
        switchToElementEvent.RemoveAllListeners();
        switchFromElementsEvent.RemoveAllListeners();
        shootElementEvent.RemoveAllListeners();
    }
}
