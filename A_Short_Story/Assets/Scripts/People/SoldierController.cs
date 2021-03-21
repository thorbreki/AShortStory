using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    protected bool controlled = false; // If the soldier is controlled the player can take control, otherwise the person thinks freely
    protected bool selected = false; // If the soldier is selected, then the player can tell this particular soldier what to do


    // WHEN PLAYER LEFT CLICKS ON THE SOLDIER, THE PLAYER CAN THEN TELL THE SOLDIER WHAT TO DO
    private void OnMouseUp()
    {
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army)
        {
            selected = true;
            print("The spirit talks to me!");
        }
    }
    // WHEN THE PLAYER RIGHT CLICKS ON A SOLDIER, THE PLAYER CAN THEN CONTROL THAT SOLDIER
    private void OnMouseOver()
    {
        // This is the best way I found to check for right clicking a soldier
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army)
        {
            if (Input.GetMouseButtonDown(1))
            {
                print("The spirit controls me!");
                // Player Mode is then set to Battle Mode
                GameManager.instance.SetPlayerMode(Constants.PlayerMode.Battle);
            }
        }
    }
}
