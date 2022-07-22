using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RationaleSummary : MonoBehaviour {

    private bool isActivated = false;

    private TextRollout summary;

    private string[] rationaleTexts = new string[] {
        Rationale.Conduction,
        Rationale.StuckDoor,
        Rationale.MoleculeContainer,
        Rationale.PistonPuzzle,
        Rationale.PressurePuzzle
    };

    private int currTextIndex;

    private void Awake() {
        summary = GetComponentInChildren<TextRollout>();
    }

    // Start is called before the first frame update
    void Start() {
        currTextIndex = 0;
    }

    public void Activate() {
        isActivated = true;
        summary.StartRollOut(rationaleTexts[currTextIndex]);
    }

    public void DisplayPrevious() {
        if (!isActivated) {
            return;
        }

        currTextIndex = currTextIndex == 0 ? rationaleTexts.Length - 1 : currTextIndex - 1;
        summary.StartRollOut(rationaleTexts[currTextIndex], true);
    }

    public void DisplayNext() {
        if (!isActivated) {
            return;
        }

        currTextIndex = currTextIndex == rationaleTexts.Length - 1 ? 0 : currTextIndex + 1;
        summary.StartRollOut(rationaleTexts[currTextIndex], true);
    }
}
