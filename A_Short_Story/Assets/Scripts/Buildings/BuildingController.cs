using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] protected Sprite transparentGreenSprite;
    [SerializeField] protected Sprite transparentRedSprite;
    [SerializeField] protected Sprite transparentYellowSprite;

    [Header("Components")]
    [SerializeField] protected SpriteRenderer spriteRenderer; // The sprite renderer component of thos building

    [Header("Colors")]
    [SerializeField] Color transparentGreen;
    [SerializeField] Color transparentRed;
    [SerializeField] Color transparentYellow;
    [SerializeField] Color constructedColor;

    // A vector that I can use for whatever
    private Vector3 vector = Vector3.zero;

    // The different statuses each building can have
    protected enum BuildingStatus
    {
        placing = 0, // The player is trying to find a good spot to place this building
        constructing = 1, // The player has found a good spot but has to construct this building
        finished = 2 // The building is finished and ready to be used

    }

    /// <summary>
    /// The status this building is currently in
    /// </summary>
    [SerializeField] protected BuildingStatus status;
    [SerializeField] protected bool setByGameMaker = false;

    protected void Start()
    {
        // If the game maker of this game has not explicitly set the status of this object, than you can do this
        if (!setByGameMaker)
        {
            status = BuildingStatus.placing; // The status first starts off being placing since the player has to place the building somewhere
            spriteRenderer.color = transparentGreen;
        }

        // Make sure that the vector always have the right y and z coordinates by settig them in the beginning
        vector.y = transform.position.y;
        vector.z = transform.position.z;
    }

    protected void Update()
    {
        // If the building is being placed by the player
        if (status == BuildingStatus.placing)
        {
            HandlePlacing();
        }
        // Other statuses dont need to perform any operations in the Update function, so yay!
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

        // SHOULD RAISE A CONSTRUCTME EVENT OR SOMETHING WITH THIS OBJECT'S TRANSFORM AND THIS SCRIPT AS WELL SO THE BUILDERS CAN ACCESS IT
    }


    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (status != BuildingStatus.placing) { return; } // Dont do anything if the player is not placing the building

        // If the building is not colliding with anything in the back layer, the sprite should be transparent green
        spriteRenderer.color = transparentRed;
        print("colliding with: " + collision.name);
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (status != BuildingStatus.placing) { return; } // Dont do anything if the player is not placing the building
        spriteRenderer.color = transparentGreen;
    }

    /// <summary>
    /// Perform all general procedures when a building is left-clicked. Should be called only when the Player Mode is Army Mode
    /// </summary>
    protected virtual void OnMouseDown()
    {
        if (status == BuildingStatus.placing)
        {
            ChangeToConstructing();
        }

        // When the building is constructed
        if (status == BuildingStatus.finished)
        {
            GameManager.instance.SetPlayerMode(Constants.PlayerMode.BuildingInteraction); // Change the Player Mode to BuildingInteraction
            EventManager.RaiseOnBuildingClick(transform.position); // Raise the clicked-on-building event
        }
    }
}
