using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuckDoorHints : HintsController {

    [SerializeField] protected DisplayScreen[] hintScreens;

    protected override void Start() {
        base.Start();
        foreach (DisplayScreen screen in hintScreens) {
            screen.DeactivateScreen();
        }
    }

    /// <summary>
    /// Activates the stuck door's hints.
    /// </summary>
    protected override void ActivateHints() {
        base.ActivateHints();
        foreach (DisplayScreen screen in hintScreens) {
            screen.ActivateScreen();
            screen.SetText(hintText);
        }
    }

    /// <summary>
    /// Deactivates the stuck door's hints.
    /// </summary>
    public override void DeactivateHints() {
        base.DeactivateHints();
        foreach (DisplayScreen screen in hintScreens) {
            screen.DeactivateScreen();
        }
    }
}
