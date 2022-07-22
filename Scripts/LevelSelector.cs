using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelSelector : MonoBehaviour {

    [SerializeField] private Level[] levels;
    private int selectedLevelIndex = 0;

    public Level SelectedLevel { get => levels[selectedLevelIndex]; }

    [SerializeField] private TextMeshProUGUI selectedLevelScreenText;

    [SerializeField] private float screenUpdateDuration;

    public bool IsUpdatingScreen { get; private set; }

    // Start is called before the first frame update
    void Start() {
        foreach (Level level in levels) {
            level.Image.color = new Color(level.Image.color.r, level.Image.color.g, level.Image.color.b, 0f);
        }

        StartCoroutine(SelectLevel(selectedLevelIndex));
        StartCoroutine(FadeInLevel());
    }

    // Update is called once per frame
    void Update() {

    }

    public void LoadSelectedLevel() {
        if (SelectedLevel.IsAvailable) {
            GameManager.Instance.LoadLevel(SelectedLevel);
        }
    }

    public void SelectNextLevel() {
        StartCoroutine(SelectLevel((selectedLevelIndex + 1) % levels.Length));
    }

    public void SelectPreviousLevel() {
        int newIndex = selectedLevelIndex - 1;
        if (newIndex < 0) {
            newIndex = levels.Length - 1;
        }

        StartCoroutine(SelectLevel(newIndex));
    }

    private IEnumerator SelectLevel(int levelIndex) {
        IsUpdatingScreen = true;
        StartCoroutine(FadeOutLevel());

        yield return new WaitUntil(() => selectedLevelScreenText.alpha == 0f);

        selectedLevelIndex = levelIndex;
        selectedLevelScreenText.text = SelectedLevel.LevelName;
        
        StartCoroutine(FadeInLevel());
        IsUpdatingScreen = false;
    }

    private IEnumerator FadeOutLevel() {
        if (selectedLevelScreenText.alpha == 0f) {
            yield break;
        }

        float imageR = SelectedLevel.Image.color.r;
        float imageG = SelectedLevel.Image.color.g;
        float imageB = SelectedLevel.Image.color.b;

        float elapsedTime = 0f;
        float fadeDuration = screenUpdateDuration / 2f;

        while (elapsedTime < fadeDuration) {
            selectedLevelScreenText.alpha = Mathf.Lerp(selectedLevelScreenText.alpha, 0f, elapsedTime);
            
            SelectedLevel.Image.color = new Color(imageR, imageG, imageB, Mathf.Lerp(SelectedLevel.Image.color.a, 0f, elapsedTime));

            elapsedTime += Time.deltaTime / fadeDuration;

            yield return null;
        }

        selectedLevelScreenText.alpha = 0f;
    }

    private IEnumerator FadeInLevel() {
        if (selectedLevelScreenText.alpha == 1f) {
            yield break;
        }

        float imageR = SelectedLevel.Image.color.r;
        float imageG = SelectedLevel.Image.color.g;
        float imageB = SelectedLevel.Image.color.b;

        float elapsedTime = 0f;
        float fadeDuration = screenUpdateDuration / 2f;

        while (elapsedTime < fadeDuration) {
            selectedLevelScreenText.alpha = Mathf.Lerp(selectedLevelScreenText.alpha, 1f, elapsedTime);

            SelectedLevel.Image.color = new Color(imageR, imageG, imageB, Mathf.Lerp(SelectedLevel.Image.color.a, 1f, elapsedTime));

            elapsedTime += Time.deltaTime / fadeDuration;

            yield return null;
        }

        selectedLevelScreenText.alpha = 1f;
    }
}
