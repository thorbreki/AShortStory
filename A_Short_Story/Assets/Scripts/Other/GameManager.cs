using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // THE INSTANCE
    public static GameManager instance; // The singleton GameManager instance

    // PHYSICS
    [SerializeField] private float gravity; // The strength of the gravity in the game

    // UI
    [SerializeField] private CanvasController canvasController; // The controller for the canvas
    [SerializeField] private GameObject builderMenuObject; // The builder menu object
    [SerializeField] private RectTransform builderMenuRectTransform; // The builder menu RectTransform

    // PRIVATE VARIABLES
    private Constants.PlayerMode playerMode; // This is the variable that chooses what Player Mode is ongoing
    private bool selectSoldierSquareShouldSpawn = false; // This variable lets the PlayerController know if the select soldier square should be spawned or not
    private bool onMoveShouldBeRaised = true; // This variable lets the PlayerController know if the OnMove event should be raised, resulting in soldiers moving to position
    private Vector3 buildingInteractionPosition; // This vector tells where the building the Player is interacting with is located
    private bool playerisHoveringUI; // This boolean tells whether or not the player is hovering over a UI element, if so the raycast should be blocked

    private void Awake()
    {
        instance = this;
        // Cursor.lockState = CursorLockMode.Confined;
        playerMode = Constants.PlayerMode.Army; // The game starts with the Player Mode set to Army!
    }

    private void Start()
    {
        EventManager.onBuildingClick += OnBuildingClick;
    }

    private void OnDestroy()
    {
        EventManager.onBuildingClick -= OnBuildingClick;
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

    /// <summary>
    /// A getter for the position of the building the Player is currently interacting with
    /// </summary>
    /// <returns>The interacted building position</returns>
    public Vector3 GetBuildingInteractionPosition()
    {
        return buildingInteractionPosition;
    }

    /// <summary>
    /// A getter for whether the player is hovering over a UI element
    /// </summary>
    /// <returns></returns>
    public bool GetPlayerIsHoveringUI()
    {
        return playerisHoveringUI;
    }

    // -------------------------------------------------------------------
    // S E T T E R S

    /// <summary>
    /// Sets the Player Mode of the game to the given new Player Mode and also makes sure that the game transitions between these modes correctly
    /// </summary>
    /// <param name="newPlayerMode">The new Player Mode that the game will be operating</param>
    public void SetPlayerMode(Constants.PlayerMode newPlayerMode)
    {
        playerMode = newPlayerMode; // Update the Player Mode of the game
        EventManager.RaiseOnPlayerModeChanged(newPlayerMode); // Let others know the player mode just changed

        // Make sure all transitions work correctly
        switch (newPlayerMode)
        {
            case Constants.PlayerMode.BuildingInteraction:
                EventManager.RaiseOnSelected(); // Make sure that all selected soldiers become unselected, since this game mode does not allow selection of soldiers
                break;
            case Constants.PlayerMode.Army:
                buildingInteractionPosition = Vector3.zero; // The player is not interacting with any buildings
                break;
            case Constants.PlayerMode.Battle:
                buildingInteractionPosition = Vector3.zero; // The player is not interacting with any buildings
                break;
            default:
                break;
        }
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

    /// <summary>
    /// Sets whether or not the player is hovering over a UI element
    /// </summary>
    /// <param name="newBool"></param>
    public void SetPlayerIsHoveringUI(bool newBool)
    {
        playerisHoveringUI = newBool;
    }

    // -------------------------------------------------------------------
    // E V E N T S

    private void OnBuildingClick(Vector3 buildingPosition)
    {
        buildingInteractionPosition = buildingPosition;
    }
}
