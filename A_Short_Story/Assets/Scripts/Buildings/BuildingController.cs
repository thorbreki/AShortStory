using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{

    [Header("Game Objects")]
    [SerializeField] protected GameObject cancelBuildingButtonObject;

    [Header("Sprites")]
    [SerializeField] protected Sprite transparentGreenSprite;
    [SerializeField] protected Sprite transparentRedSprite;
    [SerializeField] protected Sprite transparentYellowSprite;

    [Header("Components")]
    [SerializeField] protected SpriteRenderer spriteRenderer; // The sprite renderer component of thos building
    [SerializeField] protected Transform healthBarTransform; // the healthbar game object
    [SerializeField] protected GameObject healthBarObject; // The parent health bar object

    [Header("Colors")]
    [SerializeField] Color transparentGreen;
    [SerializeField] Color transparentRed;
    [SerializeField] Color transparentYellow;
    [SerializeField] Color constructedColor;

    [Header("Attributes")]
    [SerializeField] protected float maxHealth; // The max health of this building
    [SerializeField] protected float constructionSpeed; // The speed of which how fast this building is constructed
    [SerializeField] protected int maxOreNeeded; // The amount of ores the player needs to construct me

    [Header("UI")]
    [SerializeField] protected CanvasController canvasController; // The canvas controller component

    protected bool collidingWithOtherObjects = false; // To let the building know if it is colliding with objects so it can't be built
    protected float health = 0f;
    protected int numOfBuildersWorkingOnMe = 0; // The number of builders that are working on me, defines how fast the health goes up

    // Vectors that I can use for whatever
    protected Vector3 vector = Vector3.zero;
    protected Vector3 scaleVector = Vector3.zero;

    // The different statuses each building can have
    protected enum BuildingStatus
    {
        placing = 0, // The player is trying to find a good spot to place this building
        constructing = 1, // The player has found a good spot but has to construct this building
        finished = 2, // The building is finished and ready to be used
        damaged = 3 // The building works but not as well as when it is finished

    }

    /// <summary>
    /// The status this building is currently in
    /// </summary>
    [SerializeField] protected BuildingStatus status;
    [SerializeField] protected bool setByGameMaker = false;

    protected virtual void Start()
    {
        // If the game maker of this game has not explicitly set the status of this object, than you can do this
        if (!setByGameMaker)
        {
            status = BuildingStatus.placing; // The status first starts off being placing since the player has to place the building somewhere
            spriteRenderer.color = transparentGreen;
            cancelBuildingButtonObject.SetActive(true); // When the building is already ready, then the button shouldn't be active
            EventManager.onPlacingNewBuilding += OnPlacingNewBuilding;

        } else
        {
            health = maxHealth;
            scaleVector.y = 1;
            scaleVector.z = 1;
            scaleVector.x = 1;
            healthBarTransform.localScale = scaleVector;
            // Set health bar to invinsible
            healthBarObject.SetActive(false);
            cancelBuildingButtonObject.SetActive(false); // The button should be active when it spawns
        }

        EventManager.onPlayerModeChanged += OnPlayerModeChanged; // Got to know when to disable and enable the cancel button

        // Make sure that the vector always have the right y and z coordinates by settig them in the beginning
        vector.y = transform.position.y;
        vector.z = transform.position.z;

    }

    protected virtual void OnDestroy()
    {
        EventManager.onPlayerModeChanged -= OnPlayerModeChanged;
        EventManager.onPlacingNewBuilding -= OnPlacingNewBuilding;
    }

    protected void Update()
    {
        // If the building is being placed by the player
        if (status == BuildingStatus.placing)
        {
            HandlePlacing();

            // The building will be transparent red always, if the player does not have enough ores
            if (GameManager.instance.GetPlayerOreAmount() < maxOreNeeded)
            {
                spriteRenderer.color = transparentRed;
            }
        }
        
        if (status == BuildingStatus.constructing)
        {
            HandleHealth(); // Keep updating the health
        }
    }


    /// <summary>
    /// Adds to the current builders working on this building according to the parameter
    /// </summary>
    /// <param name="valueToAdd">The amount of builders that should be added to this buidling</param>
    public void AddBuilderCount(int valueToAdd)
    {
        numOfBuildersWorkingOnMe += valueToAdd;
    }

    public bool GetBuildingIsConstructed()
    {
        return health >= maxHealth;
    }

    /// <summary>
    /// When the player clicks on the Cancel button before constructing the building
    /// </summary>
    public void CancelBuilding()
    {
        if (status == BuildingStatus.constructing)
        {
            GameManager.instance.changePlayerOreAmount(maxOreNeeded);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Handles the health this building has by adding to it the amount of builders working on it to the construction speed
    /// </summary>
    protected void HandleHealth()
    {
        // When the health of this building becomes the maximum health, the building is finally constructed!
        if (health >= maxHealth)
        {
            health = maxHealth;
            status = BuildingStatus.finished;
            spriteRenderer.color = constructedColor;
            // Set the health bar to invinsible
            healthBarObject.SetActive(false);
            cancelBuildingButtonObject.SetActive(false); // The player cannot cancel a building that is already constructed
            return;
        }

        // Update the health of this building according to the amount of builders working on it
        health += numOfBuildersWorkingOnMe * constructionSpeed * Time.deltaTime;
        scaleVector.y = 1;
        scaleVector.z = 1;
        scaleVector.x = health / maxHealth;
        healthBarTransform.localScale = scaleVector;
    }

    /// <summary>
    /// This function handles the functionality of this building when it is in placing mode
    /// </summary>
    protected virtual void HandlePlacing()
    {
        vector.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        transform.position = vector;
    }

    /// <summary>
    /// This method handles everything when the player selects the position for this building
    /// </summary>
    protected void ChangeToConstructing()
    {
        status = BuildingStatus.constructing;
        spriteRenderer.color = transparentYellow;
        GameManager.instance.changePlayerOreAmount(-maxOreNeeded);

        EventManager.RaiseOnConstructMe(transform, this);
    }


    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (status != BuildingStatus.placing) { return; } // Dont do anything if the player is not placing the building
        collidingWithOtherObjects = true;
        spriteRenderer.color = transparentRed;
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (status != BuildingStatus.placing) { return; } // Dont do anything if the player is not placing the building
        // If the building is not colliding with anything in the back layer, the sprite should be transparent green
        spriteRenderer.color = transparentGreen;
        collidingWithOtherObjects = false;
    }

    /// <summary>
    /// Perform all general procedures when a building is left-clicked. Should be called only when the Player Mode is Army Mode
    /// </summary>
    protected virtual void OnMouseDown()
    {
        // When the player tries to place the building
        if (status == BuildingStatus.placing)
        {
            if (collidingWithOtherObjects) // the player cannot place the building there
            {
                GameManager.instance.DisplayBottomText("There are other objects in the way", 5f, transparentRed);
                return;
            } 
            else if (GameManager.instance.GetPlayerOreAmount() < maxOreNeeded )
            {
                GameManager.instance.DisplayBottomText("Not enough ores", 5f, transparentRed);
                return;
            }
            ChangeToConstructing();
        }

        // When the player sends builders to construct the building
        if (status == BuildingStatus.constructing)
        {
            EventManager.RaiseOnConstructMe(transform, this);
        }

        // When the building is constructed
        if (status == BuildingStatus.finished)
        {
            GameManager.instance.SetPlayerMode(Constants.PlayerMode.BuildingInteraction); // Change the Player Mode to BuildingInteraction
            EventManager.RaiseOnBuildingClick(transform.position); // Raise the clicked-on-building event
        }
    }

    protected void OnPlayerModeChanged(Constants.PlayerMode newPlayerMode)
    {
        if (status == BuildingStatus.finished) { return; } // Dont do anything if the building is already finished

        if (newPlayerMode == Constants.PlayerMode.Building)
        {
            if (cancelBuildingButtonObject == null) { return; } // just making sure the cancel button exists

            cancelBuildingButtonObject.SetActive(true);
        }
        else
        {
            if (cancelBuildingButtonObject != null) { cancelBuildingButtonObject.SetActive(false); } // Disable the cancel button since player can only see it in Build mode

            if (status == BuildingStatus.placing) { Destroy(gameObject); } // The player cannot place down a building if player is not in Building mode

            EventManager.onPlacingNewBuilding -= OnPlacingNewBuilding;
        }
    }
    
    /// <summary>
    /// This function runs when the player wants to place a new building
    /// </summary>
    protected void OnPlacingNewBuilding()
    {
        if (gameObject == null) { return; }
        if (status == BuildingStatus.placing) { Destroy(gameObject); } // Die if the player chooses to place a different building
    }

}
