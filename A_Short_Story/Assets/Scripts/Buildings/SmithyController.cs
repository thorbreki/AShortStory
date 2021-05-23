using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmithyController : BuildingController
{
    protected override void Start()
    {
        base.Start();
        EventManager.onFindNearestSmithy += OnFindNearestSmithy;
    }

    protected void OnDestroy()
    {
        EventManager.onFindNearestSmithy -= OnFindNearestSmithy;
    }

    protected override void OnMouseDown()
    {
        print("The player wants to put the building there!");
        print(GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army);
        print(!GameManager.instance.GetPlayerIsHoveringUI());
        if ((GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army) && !GameManager.instance.GetPlayerIsHoveringUI())
        {
            base.OnMouseDown();
            if (status == BuildingStatus.finished)
            {
                EventManager.RaiseOnSmithyClick(); // Raise the clicked-on-smithy event
            }
        }
    }

    /// <summary>
    /// This function runs when a builder is trying to find the nearest Smithy, I give my Transform component to the builder but they handle the logic
    /// </summary>
    /// <param name="builderTransform"></param>
    /// <param name="builderController"></param>
    protected void OnFindNearestSmithy(OreController oreController)
    {
        oreController.HandleNearestSmithyCandidate(transform);
    }
}
