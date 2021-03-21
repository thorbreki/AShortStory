using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // THE INSTANCE
    public static GameManager instance; // The singleton GameManager instance

    // PUBLIC VARIABLES
    private Constants.PlayerMode playerMode; // This is the variable that chooses what Player Mode is ongoing: false = Battle Mode, true = Army Mode

    private void Awake()
    {
        instance = this;
        // Cursor.lockState = CursorLockMode.Confined;
        playerMode = Constants.PlayerMode.Army;
    }

    // -------------------------------------------------------------------
    // G E T T E R S
    public Constants.PlayerMode GetPlayerMode()
    {
        return playerMode;
    }

    // -------------------------------------------------------------------
    // S E T T E R S

    // FLIPS THE ARMY MODE OF THE MAIN CAMERA'S CAMERACONTROLLER COMPONENT
    public void SetPlayerMode(Constants.PlayerMode newPlayerMode)
    {
        playerMode = newPlayerMode;
    }
}
