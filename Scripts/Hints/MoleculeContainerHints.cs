using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeContainerHints : HintsController {

    /*
     * There is nowhere to place display screens without affecting the design of the puzzle's layout or forcing the player to move/turn away.
     * Thus, we hijack the rationale boards to display the hint.
     */
    [SerializeField] protected TextRollout[] rationaleBoards;

    /// <summary>
    /// Activates the molecule container's hints.
    /// </summary>
    protected override void ActivateHints() {
        base.ActivateHints();
        foreach (TextRollout board in rationaleBoards) {
            board.StartRollOut(hintText);
        }
    }

    /// <summary>
    /// Deactivates the molecule container's hints.
    /// </summary>
    public override void DeactivateHints() {
        base.DeactivateHints();
        foreach (TextRollout board in rationaleBoards) {
            if (board.IsRollingOut) {
                board.StopRollOut();
                board.Text += "- Oh.";
            }
        }
    }
}
