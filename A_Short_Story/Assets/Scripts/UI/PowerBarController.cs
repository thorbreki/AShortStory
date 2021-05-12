using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBarController : MonoBehaviour
{
    private Vector3 scaleVector = Vector3.one; // A random scale vector that I can use for whatever


    private void Update()
    {
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Battle)
        {
            UpdatePowerBar(); // Update the power bar
        }
    }

    /// <summary>
    /// Updates the power bar scale value so it corresponds to the player's power value noramalized withing the maximum power value
    /// </summary>
    private void UpdatePowerBar()
    {
        scaleVector.x = GameManager.instance.GetPlayerCurrPower() / GameManager.instance.GetPlayerMaxPower();
        transform.localScale = scaleVector;
    }
}
