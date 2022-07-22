using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipController : MonoBehaviour {

    private ShipRamp ramp;

    public bool HasLaunched { get; private set; }

    [Header("Main Engine")]

    [SerializeField] private ShipEngine mainEngine;
    [SerializeField] private AudioSource mainEngineAudioSource;
    [SerializeField] private AudioClip defaultHeatedSound;
    [SerializeField] private AudioClip intenseHeatedSound;

    [Header("Engines")]

    [SerializeField] private ShipEngine[] engines;

    [Header("Cockpit Door")]

    [SerializeField] private Door cockpitDoor;

    [Header("Main Display")]

    [SerializeField] private DisplayScreen mainDisplay;

    [SerializeField] private Camera rearCamera;
    [SerializeField] private GameObject rearCameraScreen;
    [SerializeField] private AudioSource rearCameraScreenAudioSource;
    [SerializeField] private AudioClip rearCameraActivateSound;

    [Header("Summary and Button Stations")]

    [SerializeField] private GameObject buttonsBlocker;
    [SerializeField] private RationaleSummary summary;
    [SerializeField] private DisplayScreen[] gameOptionsScreens;
    [SerializeField] private string[] gameOptionsText;
    [SerializeField] private PressableButtonCover[] gameOptionButtonCovers;

    [Header("Launch")]

    [SerializeField] private AudioSource launchAudioSource;
    [SerializeField] private AudioClip startupBuild;
    [SerializeField] private AudioClip startupLoop;
    [SerializeField] private AudioClip startupFail;
    [SerializeField] private AudioClip startupSuccess;

    [Space(10)]

    [SerializeField] private UnityEvent launchEvent;
    [SerializeField] private UnityEvent launchFailEvent;

    public bool IsAttemptingLaunch { get; private set; }

    public bool HasAttemptedLaunch { get; private set; }

    [Header("Flight")]

    [SerializeField] private float riseSpeed;
    [SerializeField] private float riseHeight;

    [SerializeField] private float acceleration;
    [SerializeField] private float maxForwardSpeed;
    private float currSpeed = 0f;

    [SerializeField] private ShipTarget shipTarget;

    private bool isRising = false;
    private bool isChasingTarget = false;

    private float distanceMaintainedFromTarget = 0f;

    [SerializeField] private ActionBlocker[] flightActionBlockers;

    void Awake() {
        ramp = GetComponentInChildren<ShipRamp>();
    }

    // Start is called before the first frame update
    void Start() {
        HasLaunched = false;

        rearCamera.gameObject.SetActive(false);
        rearCameraScreen.SetActive(false);

        foreach (DisplayScreen screen in gameOptionsScreens) {
            screen.DeactivateScreen();
        }

        foreach (ActionBlocker actionBlocker in flightActionBlockers) {
            actionBlocker.Deactivate();
        }

        mainDisplay.SetText("LAUNCH");
    }

    // Update is called once per frame
    void Update() {
        if (isChasingTarget) {
            ChaseTarget();
        }
    }

    public void AffirmLaunchReadiness() {
        if (HasAttemptedLaunch && !IsAttemptingLaunch && !HasLaunched && System.Array.TrueForAll(engines, engine => engine.IsHeated)) {
            mainDisplay.SetText("Excellent. The engines have been heated!");
        }
    }

    public void PrimeLaunch() {
        if (!HasLaunched && !IsAttemptingLaunch) {
            StartCoroutine(AttemptLaunch());
        }
    }

    private IEnumerator AttemptLaunch() {
        IsAttemptingLaunch = true;

        cockpitDoor.CloseDoor();
        mainDisplay.SetText("Priming Launch...", true, true);
        launchAudioSource.clip = startupBuild;
        launchAudioSource.Play();

        yield return new WaitForSeconds(startupBuild.length);

        foreach (ShipEngine engine in engines) {
            if (!engine.IsHeated) {
                engine.Heat();
            }
        }

        launchAudioSource.clip = startupLoop;
        launchAudioSource.loop = true;
        launchAudioSource.Play();

        mainDisplay.SetText("THREE", false);
        yield return new WaitForSeconds(1f);
        mainDisplay.SetText("TWO", false);
        yield return new WaitForSeconds(1f);
        mainDisplay.SetText("ONE", false);
        yield return new WaitForSeconds(1f);

        /*
         * If the generators are heating and both state changers have been set to metal,
         * the launch should succeed even if the fuel hasn't reached the specific segments connected to the engines.
         * In such a case, as an allowance to the player, we wait for all segments to heat before attempting to launch.
         * Otherwise, the attempted launch would fail and the player would have to press the button again,
         * which would be strange and slightly annoying.
         */
        if (ConductionPuzzle.Instance.WillShipLaunchAttemptSucced()) {
            yield return new WaitUntil(() => ConductionPuzzle.Instance.AreLeftSegmentsHeated && ConductionPuzzle.Instance.AreRightSegmentsHeated);

            mainDisplay.SetText("LIFTOFF", false);

            launchAudioSource.loop = false;
            launchAudioSource.clip = startupSuccess;
            launchAudioSource.Play();

            Launch();

            yield return new WaitForSeconds(3f);
            mainDisplay.SetText("");
            yield break;
        }

        foreach (ShipEngine engine in engines) {
            engine.Cool();
        }

        HasAttemptedLaunch = true;
        mainDisplay.SetText("Launch Failed");

        launchAudioSource.loop = false;
        launchAudioSource.clip = startupFail;
        launchAudioSource.Play();

        yield return new WaitForSeconds(5f);
        mainDisplay.SetText("It's no good. We need to heat the engines from outside the ship.");
        launchFailEvent.Invoke();

        yield return new WaitUntil(() => !mainDisplay.IsRollingOut);
        cockpitDoor.OpenDoor();
        IsAttemptingLaunch = false;
    }

    private void Launch() {
        HasLaunched = true;
        IsAttemptingLaunch = false;
        mainEngine.Heat();
        ramp.CloseRamp();
        buttonsBlocker.SetActive(false);

        LevelManager.Instance.IsProjectileNetDestroyerEnabled = false;
        foreach (ActionBlocker actionBlocker in flightActionBlockers) {
            actionBlocker.Activate();
        }

        LevelManager.Instance.Player.transform.parent = transform;
        StartCoroutine(RunFlightPath());

        launchEvent.Invoke();
    }

    private IEnumerator RunFlightPath() {
        StartCoroutine(Rise(riseHeight));
        
        yield return new WaitUntil(() => !isRising);
        
        shipTarget.transform.position = new Vector3(
            shipTarget.transform.position.x,
            transform.position.y,
            shipTarget.transform.position.z);

        distanceMaintainedFromTarget = Vector3.Distance(transform.position, shipTarget.transform.position);

        yield return new WaitForSeconds(2f);

        mainDisplay.SetText("We have incoming...");

        yield return new WaitForSeconds(3f);

        mainDisplay.SetText("HOLD ON!", false, true);

        shipTarget.StartMoving();
        isChasingTarget = true;

        mainEngineAudioSource.clip = intenseHeatedSound;
        mainEngineAudioSource.Play();
        MusicManager.Instance.PlayShipEvadeMusic();

        yield return new WaitForSeconds(14.38f);

        mainDisplay.SetText("", false, true);
        rearCameraScreen.SetActive(true);
        rearCamera.gameObject.SetActive(true);
        rearCameraScreenAudioSource.PlayOneShot(rearCameraActivateSound);
    }

    private IEnumerator Rise(float targetHeight) {
        isRising = true;

        mainEngineAudioSource.clip = intenseHeatedSound;
        mainEngineAudioSource.Play();

        MusicManager.Instance.PlayShipRiseMusic();

        float startHeight = transform.position.y;
        while (transform.position.y - startHeight < targetHeight) {
            transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);
            yield return null;
        }

        mainEngineAudioSource.clip = defaultHeatedSound;
        mainEngineAudioSource.Play();

        isRising = false;
    }

    private void ChaseTarget() {
        if (!shipTarget.IsMoving) {
            isChasingTarget = false;
            return;
        }

        // look at target
        Vector3 relativePos = transform.position - shipTarget.transform.position;
        transform.rotation = Quaternion.LookRotation(relativePos, shipTarget.transform.up);

        if (Vector3.Distance(transform.position, shipTarget.transform.position) > distanceMaintainedFromTarget) {
            // accelerate
            currSpeed += acceleration * Time.deltaTime;
        } else {
            // decelerate
            currSpeed -= acceleration * Time.deltaTime;
        }

        currSpeed = Mathf.Clamp(currSpeed, 0f, maxForwardSpeed);

        transform.position = Vector3.MoveTowards(transform.position, shipTarget.transform.position, currSpeed * Time.deltaTime);
    }

    public void UnlockSummaryAndButtons() {
        foreach (PressableButtonCover cover in gameOptionButtonCovers) {
            cover.Open();
        }

        summary.Activate();

        if (gameOptionsScreens.Length != gameOptionsText.Length) {
            Debug.LogError("Game Options Screens length not equal to Game Options Text length.");
            return;
        }

        for (int i = 0; i < gameOptionsScreens.Length; i++) {
            gameOptionsScreens[i].ActivateScreen();
            gameOptionsScreens[i].SetText(gameOptionsText[i]);
        }
    }

    public IEnumerator WindDownAfterExplosion() {
        mainEngineAudioSource.clip = defaultHeatedSound;
        mainEngineAudioSource.Play();
        yield return new WaitForSeconds(6f);

        rearCamera.gameObject.SetActive(false);
        rearCameraScreen.SetActive(false);
        mainDisplay.SetText("Whew.");
        yield return new WaitUntil(() => !mainDisplay.IsRollingOut);
        yield return new WaitForSeconds(2f);

        mainDisplay.SetText("That was close!");
        yield return new WaitUntil(() => !mainDisplay.IsRollingOut);
        yield return new WaitForSeconds(4f);

        MusicManager.Instance.PlayVictoryMusic();
        mainDisplay.SetText("You can head to the back of the ship to finish up.");
        cockpitDoor.OpenDoor();

        yield return new WaitForSeconds(5.66f);

        mainDisplay.SetText("Well done ^U^");
        yield return new WaitForSeconds(1.25f);

        UnlockSummaryAndButtons();
        yield return new WaitUntil(() => shipTarget.NumWaypointsRemaining == 0);

        shipTarget.StopMoving();
    }
}
