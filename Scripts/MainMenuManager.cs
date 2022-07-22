using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {

    [SerializeField] private FadePlayerView fade;

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(RunIntro());
    }

    private IEnumerator RunIntro() {
        yield return new WaitForSeconds(3f);
        fade.FadeIn();
    }
}
