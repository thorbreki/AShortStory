using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    /// <summary>
    /// Perform all general procedures when a building is left-clicked. Should be called only when the Player Mode is Army Mode
    /// </summary>
    protected virtual void OnMouseDown()
    {
        GameManager.instance.SetPlayerMode(Constants.PlayerMode.BuildingInteraction); // Change the Player Mode to BuildingInteraction
        EventManager.RaiseOnBuildingClick(transform.position); // Raise the clicked-on-building event
    }
}
