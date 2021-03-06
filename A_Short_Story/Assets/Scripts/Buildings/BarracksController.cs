using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarracksController : BuildingController
{
    protected override void OnMouseDown()
    {
        // Nothing happens if the player is not in Army Mode or the player is hovering over a UI element
        if ((GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Building) && !GameManager.instance.GetPlayerIsHoveringUI())
        {
            base.OnMouseDown(); // Do all general stuff
            
            if (status == BuildingStatus.finished)
            {
                EventManager.RaiseOnBarrackClick(); // Raise the clicked-on-barrack event
            }
        }
    }
}
