using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmithyController : BuildingController
{
    protected override void OnMouseDown()
    {
        if ((GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army) && !GameManager.instance.GetPlayerIsHoveringUI())
        {
            base.OnMouseDown();
            if (status == BuildingStatus.finished)
            {
                EventManager.RaiseOnSmithyClick(); // Raise the clicked-on-smithy event
            }
        }
    }
}
