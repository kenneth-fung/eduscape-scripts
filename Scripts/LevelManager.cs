using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    private static LevelManager _instance;
    public static LevelManager Instance { get => _instance; }

    [SerializeField] private GameObject player;
    public GameObject Player { get => player; }

    [SerializeField] private GameObject playerHead;
    public GameObject PlayerHead { get => playerHead; }

    [SerializeField] private GameObject playerBody;
    public GameObject PlayerBody { get => playerBody; }

    [Space(10)]

    [SerializeField] private FadePlayerView fade;

    [SerializeField] private ProjectileNetDestroyer projectileNetDestroyer;

    [Space(10)]

    [SerializeField] private ShipController ship;

    public bool IsProjectileNetDestroyerEnabled {
        get => projectileNetDestroyer.isActiveAndEnabled;
        set => projectileNetDestroyer.gameObject.SetActive(value);
    }

    void Awake() {
        // singleton
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel() {
        AudioManager.Instance.MuteAllAudio();

        yield return new WaitForSeconds(1f);

        TutorialManager.Instance.ResetTutorial();
        fade.FadeIn();
        AudioManager.Instance.UnmuteAllAudio(true);
    }

    public void CompleteLevel() {
        StartCoroutine(ship.WindDownAfterExplosion());
    }

    public void RestartLevel() {
        UnityAction restart = () => {
            AudioManager.Instance.MuteAllAudio(true);
            fade.FadeOutCompleteEvent.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
            fade.FadeOut();
        };

        if (fade.IsFading) {
            StartCoroutine(GameManager.Instance.WaitForConditionBeforeAction(() => !fade.IsFading, restart));
        } else {
            restart();
        }
    }
}
