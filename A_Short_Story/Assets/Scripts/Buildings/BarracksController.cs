using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarracksController : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army)
        {
            GameManager.instance.SetPlayerMode(Constants.PlayerMode.BuildingInteraction); // Change the Player Mode to BuildingInteraction
            EventManager.RaiseOnBarrackClick(transform.position); // Raise the clicked-on-barrack event
        }
    }
}
