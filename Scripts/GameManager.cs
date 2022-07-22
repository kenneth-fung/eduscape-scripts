using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }

    [SerializeField] private FadePlayerView fade;

    void Awake() {
        // singleton
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Waits for a given condition to be true before executing a given action.
    /// </summary>
    /// <param name="condition">The predicate condition that should eventually be true.</param>
    /// <param name="action">The zero argument UnityAction to execute.</param>
    /// <returns>IEnumerator; this is a coroutine.</returns>
    public IEnumerator WaitForConditionBeforeAction(System.Func<bool> condition, UnityAction action) {
        yield return new WaitUntil(condition);
        action();
    }

    public void LoadLevel(Level level) {
        UnityAction load = () => {
            fade.FadeOutCompleteEvent.AddListener(() => SceneManager.LoadScene(level.SceneName));
            fade.FadeOut();
        };

        if (fade.IsFading) {
            StartCoroutine(WaitForConditionBeforeAction(() => !fade.IsFading, load));
        } else {
            load();
        }
    }

    public void ReturnToMainMenu() {
        UnityAction returnToMainMenu = () => {
            AudioManager.Instance.MuteAllAudio(true);
            fade.FadeOutCompleteEvent.AddListener(() => SceneManager.LoadScene("Main Menu"));
            fade.FadeOut();
        };

        if (fade.IsFading) {
            StartCoroutine(WaitForConditionBeforeAction(() => !fade.IsFading, returnToMainMenu));
        } else {
            returnToMainMenu();
        }
    }

    public void QuitGame() {
        UnityAction quit = () => {
            fade.FadeOutCompleteEvent.AddListener(() => {
                if (Application.isEditor) {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                } else {
                    Application.Quit();
                }
            });

            AudioManager.Instance.MuteAllAudio(true);
            fade.FadeOut();
        };

        if (fade.IsFading) {
            StartCoroutine(WaitForConditionBeforeAction(() => !fade.IsFading, quit));
        } else {
            quit();
        }
    }
}
