using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarracksController : BuildingController
{
    protected override void OnMouseDown()
    {
        if ((GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army) && !GameManager.instance.GetPlayerIsHoveringUI())
        {
            base.OnMouseDown(); // Do all general stuff

            EventManager.RaiseOnBarrackClick(); // Raise the clicked-on-barrack event
        }
    }
}
