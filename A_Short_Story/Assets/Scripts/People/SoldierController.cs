using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    [SerializeField] protected float movementSpeed; // The movementSpeed of this soldier
    [SerializeField] private GameObject soldierSpriteObject; // The GameObject that holds the sprite of the soldier
    [SerializeField] private GameObject selectedSprite; // The game object that shows wether a soldier is selected or not
    protected Animator soldierAnimator; // The Animator component of the soldier
    protected bool controlled = false; // If the soldier is controlled the player can take control, otherwise the person thinks freely
    protected bool selected = false; // A boolean that specifies wether the soldier is selected or not

    // Coroutine objects:
    protected Coroutine moveToCoroutine; // The object storing the MoveToCor coroutine

    protected void Start()
    {
        soldierAnimator = soldierSpriteObject.GetComponent<Animator>(); // Get the Animator component of the child sprite object
        selectedSprite.SetActive(false);
        EventManager.onSelect += OnSelect;
    }

    protected void OnDestroy()
    {
        EventManager.onSelect -= OnSelect;
        EventManager.onMove -= OnMove;
    }

    // WHEN PLAYER LEFT CLICKS ON THE SOLDIER, THE PLAYER CAN THEN TELL THE SOLDIER WHAT TO DO
    protected void OnMouseUp()
    {
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army)
        {
            selected = true; // Soldier becomes selected
            selectedSprite.SetActive(true);
            EventManager.RaiseOnSelected(); // Let other soldiers know that this one is selected
            EventManager.onSelect += OnSelect; // Start listening to when other game objects get selected
            EventManager.onMove += OnMove; // Start listening to when the Player wants to move this soldier

            print("The spirit talks to me!"); // DEBUG
        }
    }
    // WHEN THE PLAYER RIGHT CLICKS ON A SOLDIER, THE PLAYER CAN THEN CONTROL THAT SOLDIER
    protected void OnMouseOver()
    {
        // This is the best way I found to check for right clicking a soldier
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army)
        {
            if (Input.GetMouseButtonDown(1))
            {
                print("The spirit controls me!");
                GameManager.instance.SetPlayerMode(Constants.PlayerMode.Battle); // Player Mode is then set to Battle Mode
            }
        }
    }

    /// <summary>
    /// Moves the soldier to the specified position the Player wants
    /// </summary>
    /// <param name="targetPosition">The position the soldier is supposed to walk to</param>
    /// <returns></returns>
    protected IEnumerator MoveToCor(float targetX)
    {
        soldierAnimator.SetBool("isWalking", true); // Start The walking animation
        float direction;
        if (targetX - transform.position.x > 0)
        {
            direction = movementSpeed;
        } else
        {
            direction = -movementSpeed;
        }
        yield return null;
        while (Mathf.Abs(targetX - transform.position.x) > 0.05f)
        {
            transform.Translate(direction * Time.deltaTime, 0f, 0f);
            yield return null;
        }
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
        soldierAnimator.SetBool("isWalking", false); // End The walking animation
    }

    /// <summary>
    /// This function executes when the player tells a soldier to move to a specific postition
    /// </summary>
    protected void OnMove(float targetX)
    {
        if (moveToCoroutine != null) { StopCoroutine(moveToCoroutine); } // Make sure that no other instances of move coroutine are active

        moveToCoroutine = StartCoroutine(MoveToCor(targetX)); // Start the move to coroutine and store the object for future checks
    }

    /// <summary>
    /// This function runs when the player left-clicks/selects any object, which means this soldier will become unselected
    /// </summary>
    protected void OnSelect()
    {
        selected = false; // This soldier is not selected any more ;(
        EventManager.onSelect -= OnSelect; // Stop listening to the player selecting other objects since it doesn't matter
        EventManager.onMove -= OnMove; // Stop listening to the player wanting to move the soldier since the soldier is not selected anymore
    }
}
