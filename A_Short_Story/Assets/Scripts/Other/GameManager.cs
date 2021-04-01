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
    private bool selectSoldierSquareShouldSpawn = false; // This variable lets the PlayerController know if the select soldier square should be spawned or not
    private bool onMoveShouldBeRaised = true; // This variable lets the PlayerController know if the OnMove event should be raised, resulting in soldiers moving to position

    private void Awake()
    {
        instance = this;
        // Cursor.lockState = CursorLockMode.Confined;
        playerMode = Constants.PlayerMode.Army; // The game starts with the Player Mode set to Army!
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

    /// <summary>
    /// A getter for whether the onMove event should be raised
    /// </summary>
    /// <returns></returns>
    public bool GetOnMoveShouldBeRaised()
    {
        return onMoveShouldBeRaised;
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

    /// <summary>
    /// Sets whether or not the player should raise onMove event to let selected soldiers know they should move to a specific position
    /// </summary>
    /// <param name="newBool">The new value of onMoveShouldBeRaised</param>
    public void SetOnMoveShouldBeRaised(bool newBool)
    {
        onMoveShouldBeRaised = newBool;
    }
}
