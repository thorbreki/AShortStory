using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // THE INSTANCE
    public static GameManager instance; // The singleton GameManager instance

    // PHYSICS
    [Header("Physics")]
    [SerializeField] private float gravity; // The strength of the gravity in the game

    // UI
    [Header("UI")]
    [SerializeField] private GameObject builderMenuObject; // The builder menu object
    [SerializeField] private RectTransform builderMenuRectTransform; // The builder menu RectTransform
    [SerializeField] private CanvasController canvasController; // The controller for the canvas

    // PLAYER
    [Header("Player")]
    [SerializeField] private float playerMaxPower = 1f; // The maximum amount of power the player can have
    [SerializeField] private float playerAttackDamage; // The amount of damage the player can deal to enemies
    [SerializeField] private float playerPowerReduction = 0.2f; // The amount of power the player uses on a basic attack
    private int playerOreAmount = 0;

    // PRIVATE VARIABLES
    private Constants.PlayerMode playerMode; // This is the variable that chooses what Player Mode is ongoing
    private bool selectSoldierSquareShouldSpawn = false; // This variable lets the PlayerController know if the select soldier square should be spawned or not
    private bool onMoveShouldBeRaised = true; // This variable lets the PlayerController know if the OnMove event should be raised, resulting in soldiers moving to position
    private Vector3 buildingInteractionPosition; // This vector tells where the building the Player is interacting with is located
    private bool playerisHoveringUI; // This boolean tells whether or not the player is hovering over a UI element, if so the raycast should be blocked
    private float playerCurrPower = 1f; // The current level of power the player has

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

    /// <summary>
    /// Get how much the damage the player can deal to enemies
    /// </summary>
    /// <returns></returns>
    public float GetPlayerAttackDamage()
    {
        return playerAttackDamage;
    }

    /// <summary>
    /// Returns the amount of power the player has left
    /// </summary>
    /// <returns></returns>
    public float GetPlayerCurrPower()
    {
        return playerCurrPower;
    }

    /// <summary>
    /// Returns the amount of power the player loses when player attacks enemies
    /// </summary>
    /// <returns></returns>
    public float GetPlayerPowerReduction()
    {
        return playerPowerReduction;
    }

    /// <summary>
    /// Returns the maximum amount of power the player can have
    /// </summary>
    /// <returns></returns>
    public float GetPlayerMaxPower()
    {
        return playerMaxPower;
    }

    /// <summary>
    /// Returns the amount of ore the player has
    /// </summary>
    /// <returns></returns>
    public int GetPlayerOreAmount()
    {
        return playerOreAmount; 
    }

    /// <summary>
    /// Displays a new string on the bottom of the screen, intended to be a detail text, for instance to say if something is wrong
    /// </summary>
    /// <param name="newText">The string the text will display</param>
    /// <param name="duration">How long in seconds the text will be displayed</param>
    /// <param name="newColor">The color of the text which will be displayed</param>
    public void DisplayBottomText(string newText, float duration, Color newColor)
    {
        canvasController.DisplayBottomText(newText, duration, newColor);
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
                EventManager.RaiseOnSelected(); // All selected soldiers become unselected since the player is starting to attack
                buildingInteractionPosition = Vector3.zero; // The player is not interacting with any buildings
                break;
            case Constants.PlayerMode.Building:
                EventManager.RaiseOnSelected();
                buildingInteractionPosition = Vector3.zero;

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

    /// <summary>
    /// Sets the player's current power to the new amount
    /// </summary>
    /// <param name="newAmount"></param>
    public void SetPlayerPower(float newAmount)
    {
        playerCurrPower = newAmount;
    }

    /// <summary>
    /// Reduces the power of the player by the amount that is given.
    /// </summary>
    /// <param name="amount">The amount of power the player should lose</param>
    public void ReducePlayerPower(float amount)
    {
        playerCurrPower -= amount;
    }

    /// <summary>
    /// Increases the power of the player by the amount that is given.
    /// </summary>
    /// <param name="amount">The amount of power the player should gain</param>
    public void IncreasePlayerPower(float amount)
    {
        playerCurrPower += amount;
    }

    /// <summary>
    /// Add to the amount of ore the player has gathered
    /// </summary>
    /// <param name="amount">The amount that the current amount will be added by</param>
    public void changePlayerOreAmount(int amountToAddTo)
    {
        playerOreAmount += amountToAddTo;
        canvasController.UpdateOreAmountLabel();
    }

    // -------------------------------------------------------------------
    // E V E N T S

    private void OnBuildingClick(Vector3 buildingPosition)
    {
        buildingInteractionPosition = buildingPosition;
    }
}
