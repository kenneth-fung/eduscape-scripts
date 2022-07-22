using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    private static TutorialManager _instance;
    public static TutorialManager Instance { get => _instance; }

    public enum TutorialStage { WAKEUP, WARNING, TRANSFERRING, CYCLE, SHOOT, DOOR_PANEL, COMPLETE }
    public TutorialStage CurrTutorialStage { get; private set; }

    [SerializeField] private LeftHandController leftHandController;
    [SerializeField] private RightHandController rightHandController;

    [Header("Bedroom Lights")]

    [SerializeField] private SciFiLight[] bedroomLights;

    [Header("Bedroom Screens")]

    private static readonly string wakeUpText = "Good day!";
    private static readonly string greetingText = "It's EDU, your station AI ^u^";
    private static readonly string interruptedText = "The time now is:";
    private static readonly string warningText = "WARNING";
    private static readonly string stationFailureText = "STATION ON COLLISION COURSE";
    private static readonly string transferText = "We need to leave!";

    [SerializeField] private DisplayScreen[] bedroomScreens;

    [Header("Transfer")]

    [SerializeField] private AudioSource alarmAudioSource;
    [SerializeField] private AudioSource distantAlarmAudioSource;

    [SerializeField] private PressableButtonCover transferButtonCover;

    [SerializeField] private Door podDoor;
    [SerializeField] private SciFiLight corridorLight;
    [SerializeField] private Door stationDoor;

    [Header("Elements")]

    private static readonly string cycleText = "Summon matter by tapping the Right Hand Middle Grip.";
    private static readonly string shootText = "Shoot with the Right Hand Index Trigger.";
    private static readonly string elementsCompleteText = "Great!";

    [SerializeField] private PlayerElements playerElements;
    [SerializeField] private DisplayScreen elementScreen;

    [Header("Door Panel")]

    private static readonly string doorPanelExplanationText = "Looks like the door panel is busted.";
    private static readonly string doorPanelInstructionText = "You'll have to shoot it to get out.";
    private static readonly string completeTutorialText = "Keep going!";

    [SerializeField] private DoorPanel startDoorPanel;
    [SerializeField] private DisplayScreen doorPanelInstructionScreen;

    [SerializeField] private Door startDoor;

    void Awake() {
        // singleton
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }

    private void SetDockingScreensTexts(string text, bool rollOut = true) {
        foreach (DisplayScreen screen in bedroomScreens) {
            screen.SetText(text, rollOut);
        }
    }

    public void ResetTutorial() {
        CurrTutorialStage = TutorialStage.WAKEUP;

        leftHandController.IsCheckingForLeftHandInput = false;
        rightHandController.IsCheckingForRightHandInput = false;

        podDoor.CloseDoor();
        
        corridorLight.TurnOff();
        stationDoor.CloseDoor();
        
        elementScreen.Stow();

        startDoor.CloseDoor();
        startDoorPanel.SwitchOff();
        doorPanelInstructionScreen.DeactivateScreen();

        foreach (DisplayScreen screen in bedroomScreens) {
            screen.DeactivateScreen();
        }

        foreach (SciFiLight light in bedroomLights) {
            light.TurnOff();
        }
    }

    public void StartTutorial() {
        if (CurrTutorialStage != TutorialStage.WAKEUP && CurrTutorialStage != TutorialStage.COMPLETE) {
            return;
        }

        StartCoroutine(RunTutorial());
    }

    private IEnumerator RunTutorial() {
        MusicManager.Instance.PlayIntroMusic(5f);

        foreach (SciFiLight light in bedroomLights) {
            light.TurnOn();
        }

        yield return new WaitForSeconds(MusicManager.Instance.UseSongForIntro ? 4f : 3.39f);

        foreach (DisplayScreen screen in bedroomScreens) {
            screen.ActivateScreen();
        }

        SetDockingScreensTexts(wakeUpText);

        yield return new WaitForSeconds(MusicManager.Instance.UseSongForIntro ? 3f : 4.625f);

        SetDockingScreensTexts(greetingText);

        yield return new WaitForSeconds(MusicManager.Instance.UseSongForIntro ? 5f : 5.496f);

        SetDockingScreensTexts(interruptedText);

        yield return new WaitForSeconds(MusicManager.Instance.UseSongForIntro ? 5f : 4.689f);

        MusicManager.Instance.StopIntroMusic();

        StartCoroutine(Warning());
    }

    private IEnumerator Warning() {
        CurrTutorialStage = TutorialStage.WARNING;

        SetDockingScreensTexts(warningText, false);
        foreach (DisplayScreen screen in bedroomScreens) {
            screen.DisplayWarning();
        }

        foreach (SciFiLight light in bedroomLights) {
            light.TurnOnDanger();
        }

        alarmAudioSource.Play();
        distantAlarmAudioSource.Play();

        yield return new WaitUntil(() => System.Array.TrueForAll(bedroomScreens, screen => !screen.IsPulsingWarningScreen));

        alarmAudioSource.loop = false;
        alarmAudioSource.Play();
        SetDockingScreensTexts(stationFailureText, false);

        yield return new WaitForSeconds(5f);

        SetDockingScreensTexts(transferText);

        yield return new WaitUntil(() => System.Array.TrueForAll(bedroomScreens, screen => !screen.IsRollingOut));

        BeginTransfer();
    }

    public void BeginTransfer() {
        if (CurrTutorialStage == TutorialStage.WARNING) {
            CurrTutorialStage = TutorialStage.TRANSFERRING;
            distantAlarmAudioSource.loop = false;
            podDoor.OpenDoor();
            leftHandController.IsCheckingForLeftHandInput = true;
            MusicManager.Instance.PlayStationMusic();
        }
    }

    public void CompleteTeleportTutorial() {
        corridorLight.TurnOn();
        stationDoor.OpenDoor();
    }

    public IEnumerator TeachElementCycling() {
        CurrTutorialStage = TutorialStage.CYCLE;
        rightHandController.IsCheckingForRightHandInput = true;
        elementScreen.Unstow();

        yield return new WaitUntil(() => !elementScreen.IsStowed);
        yield return new WaitForSeconds(0.5f);

        elementScreen.ActivateScreen();
        elementScreen.SetText(cycleText);
        playerElements.SwitchToElementEvent.AddListener(CompleteElementCycling);
    }

    private void CompleteElementCycling() {
        StartCoroutine(TeachElementShooting());
    }

    private IEnumerator TeachElementShooting() {
        playerElements.SwitchToElementEvent.RemoveListener(CompleteElementCycling);

        yield return new WaitUntil(() => System.Array.TrueForAll(bedroomScreens, screen => !screen.IsRollingOut));
        
        CurrTutorialStage = TutorialStage.SHOOT;
        elementScreen.SetText(shootText);
        playerElements.ShootElementEvent.AddListener(CompleteElementsTutorial);
    }

    private void CompleteElementsTutorial() {
        StartCoroutine(DisplayDoorPanelInstructions());
    }

    private IEnumerator DisplayDoorPanelInstructions() {
        playerElements.ShootElementEvent.RemoveListener(CompleteElementsTutorial);
        elementScreen.SetText(elementsCompleteText);

        yield return new WaitUntil(() => System.Array.TrueForAll(bedroomScreens, screen => !screen.IsRollingOut));
        yield return new WaitForSeconds(3f);

        elementScreen.Stow();

        CurrTutorialStage = TutorialStage.DOOR_PANEL;
        doorPanelInstructionScreen.ActivateScreen();
        doorPanelInstructionScreen.SetText(doorPanelExplanationText);

        yield return new WaitUntil(() => System.Array.TrueForAll(bedroomScreens, screen => !screen.IsRollingOut));
        yield return new WaitForSeconds(8f);

        doorPanelInstructionScreen.SetText(doorPanelInstructionText);
        startDoorPanel.SwitchOn();
        startDoor.OpenEvent.AddListener(CompleteTutorial);
    }

    public void CompleteTutorial() {
        StartCoroutine(DisplayFinalInstruction());
    }

    private IEnumerator DisplayFinalInstruction() {
        startDoor.OpenEvent.RemoveListener(CompleteTutorial);

        yield return new WaitUntil(() => System.Array.TrueForAll(bedroomScreens, screen => !screen.IsRollingOut));

        CurrTutorialStage = TutorialStage.COMPLETE;
        doorPanelInstructionScreen.SetText(completeTutorialText);
    }
}
