using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // THE INSTANCE
    public static GameManager instance; // The singleton GameManager instance

    // PUBLIC VARIABLES
    [SerializeField] private float gravity; // The strength of the gravity in the game
    private Constants.PlayerMode playerMode; // This is the variable that chooses what Player Mode is ongoing
    private bool selectSoldierSquareShouldSpawn; // This variable lets the PlayerController know if the select soldier square should be spawned or not

    private void Awake()
    {
        instance = this;
        // Cursor.lockState = CursorLockMode.Confined;
        playerMode = Constants.PlayerMode.Army; // The game starts with the Player Mode set to Army!
        selectSoldierSquareShouldSpawn = false; // The Select Soldier Square should not be able to spawn right away when game starts
    }

    // -------------------------------------------------------------------
    // G E T T E R S

    /// <summary>
    /// Returns the strength of the gravity of the game
    /// </summary>
    /// <returns>gravity</returns>
    public float GetGravity()
    {
        return gravity;
    }
    
    /// <summary>
    /// A getter for the game's current Player Mode
    /// </summary>
    /// <returns>The game's current Player Mode</returns>
    public Constants.PlayerMode GetPlayerMode()
    {
        return playerMode;
    }

    /// <summary>
    /// A getter for the spawnability of the Select Soldier Square
    /// </summary>
    /// <returns>The spawnability of the Select Soldier Square</returns>
    public bool GetSelectSoldierSquareShouldSpawn()
    {
        return selectSoldierSquareShouldSpawn;
    }

    // -------------------------------------------------------------------
    // S E T T E R S

    // FLIPS THE ARMY MODE OF THE MAIN CAMERA'S CAMERACONTROLLER COMPONENT
    public void SetPlayerMode(Constants.PlayerMode newPlayerMode)
    {
        playerMode = newPlayerMode;
    }

    /// <summary>
    /// Sets the spawnability of the Select Soldier Square to the input boolean. You have to specify if it is available, since it always goes back to false after spawning.
    /// </summary>
    /// <param name="newBool">The new spawnability of the Select Soldier Square</param>
    public void SetSelectSoldierSquareShouldSpawn(bool newBool)
    {
        selectSoldierSquareShouldSpawn = newBool;
    }
}
