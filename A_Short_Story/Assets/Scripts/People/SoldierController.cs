using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    [SerializeField] protected float movementSpeed; // The movementSpeed of this soldier
    [SerializeField] protected float moveAwayDistance; // The distance I want to move from another soldier when we are invading each other's personal spaces
    [SerializeField] protected float height = 0.5f; // My height
    [SerializeField] private GameObject selectedSprite; // The game object that shows wether a soldier is selected or not
    [SerializeField] protected float gravityScale = 2f; // The gravityScale for this soldier
    [SerializeField] protected float attackDistance = 0.8f; // The distance I want to be from an enemy when I start attacking that enemy
    protected Animator soldierAnimator; // The Animator component of the soldier
    protected Rigidbody2D parentRigidbody; // The rigidbody component of the parent
    protected bool controlled = false; // If the soldier is controlled the player can take control, otherwise the person thinks freely
    protected bool selected = false;
    protected bool mouseDown = false; // A boolean that specifies wether the soldier was clicked on or not
    protected bool squareSelected = false; // Was the soldier selected with the select soldier square?
    protected bool isIdle = true; // Am I just standing still, minding my own business?
    protected Transform parentTransform; // The Parent Transform of the soldier sprite

    // A vector that I can use for whatever:
    protected Vector3 vector;

    // Coroutine objects:
    protected Coroutine moveToCoroutine; // The object storing the MoveToCor coroutine
    protected Coroutine dragCoroutine; // The object storing the DragCor coroutine
    protected Coroutine attackCoroutine; // The object storing the AttackCor coroutine

    protected void Start()
    {
        parentTransform = transform.parent; // Set the parentTransform
        soldierAnimator = gameObject.GetComponent<Animator>(); // Get the Animator component of the child sprite object
        parentRigidbody = transform.parent.GetComponent<Rigidbody2D>(); // Set the rigidbody component of the parent
        selectedSprite.SetActive(false); // Set the selected sprite inactive by default

        // setting stuff up
        parentRigidbody.gravityScale = gravityScale;

    }

    protected void OnDestroy()
    {
        EventManager.onSelect -= OnSelect;
        EventManager.onMove -= OnMove;
        EventManager.onAttack -= OnAttack;
    }

    protected void OnMouseDown()
    {
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army)
        {
            mouseDown = true; // Specify that the soldier was indeed clicked on

            // Drag the soldier around!
            if (dragCoroutine != null) { StopCoroutine(dragCoroutine); }
            dragCoroutine = StartCoroutine(DragCor());
        }
    }

    // WHEN PLAYER LEFT CLICKS ON THE SOLDIER, THE PLAYER CAN THEN TELL THE SOLDIER WHAT TO DO
    protected void OnMouseUp()
    {
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army && mouseDown)
        {

            if (!Input.GetKey(KeyCode.LeftShift)) { EventManager.RaiseOnSelected(); } // Let other soldiers know that this one is selected if the player did not hold shift

            WhenSelected(); // Execute all basic stuff when soldier gets selected

            mouseDown = false; // The soldier is no longer clicked
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // When the select soldier square hits me up
        if (collision.GetComponent<SelectSoldiersSquareController>() != null)
        {
            squareSelected = true; // Notify this soldier that the selection method was with the square select, so ignore next Selected event to not become unselected
            WhenSelected(); // Execute all basic stuff when soldier gets selected
        }
    }

    protected void OnTriggerStay2D(Collider2D collision)
    {
        //print("OnTriggerStay colliding with: " + collision.name);
        // Must make sure that I'm not in any other fellow soldier's personal space. It is very important for one's mental health!
        if (moveToCoroutine == null) // Only move from other soldier's if I'm idle
        {
            SoldierController otherSoldierController = collision.GetComponent<SoldierController>();
            if (otherSoldierController != null) // If the "thing" I'm colliding with is actually a soldier
            {
                if (!otherSoldierController.GetIsIdle()) { return; } // Don't do anything since the other soldier is already on a move and most definitely passing by

                if (transform.position.x <= collision.transform.position.x) // If I am to the left of the other fellow soldier
                {
                    moveToCoroutine = StartCoroutine(MoveToCor(transform.position.x - moveAwayDistance));
                } else // If I am to the right of the other fellow soldier
                {
                    moveToCoroutine = StartCoroutine(MoveToCor(transform.position.x + moveAwayDistance));
                }
            }
        }
    }

    /// <summary>
    /// Returns wether this soldier is idle or not
    /// </summary>
    /// <returns>A boolean</returns>
    public bool GetIsIdle()
    {
        return isIdle;
    }

    /// <summary>
    /// Run this function to execute all the stuff that are always required when the soldier gets selected
    /// </summary>
    protected void WhenSelected()
    {
        EventManager.onSelect -= OnSelect; // Stop listening to when other game objects get selected
        EventManager.onMove -= OnMove; // Stop listening to when the Player wants to move this soldier
        EventManager.onAttack -= OnAttack;

        selectedSprite.SetActive(true);
        EventManager.onSelect += OnSelect; // Start listening to when other game objects get selected
        EventManager.onMove += OnMove; // Start listening to when the Player wants to move this soldier
        EventManager.onAttack += OnAttack;
    }

    /// <summary>
    /// Moves the soldier to the specified position the Player wants
    /// </summary>
    /// <param name="targetPosition">The position the soldier is supposed to walk to</param>
    /// <returns></returns>
    protected IEnumerator MoveToCor(float targetX)
    {
        isIdle = false; // I'm not idle when walking
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
            parentTransform.Translate(direction * Time.deltaTime, 0f, 0f);

            yield return null;
        }
        parentTransform.position = new Vector3(targetX, parentTransform.position.y, parentTransform.position.z);
        soldierAnimator.SetBool("isWalking", false); // End The walking animation
        isIdle = true; // Idle yet again!
        moveToCoroutine = null; // Set the coroutine object for this coroutine to null since this coroutine has ended
    }

    protected IEnumerator AttackCor(Transform targetTransform)
    {
        print("AttackCor started!");
        isIdle = false; // I'm not idle when attacking

        // The loop runs until the enemy has been dealt with
        while (targetTransform != null)
        {
            // This code runs when I am too far away from the enemy to attack
            if (Vector2.Distance(transform.position, targetTransform.position) > attackDistance)
            {
                print(Vector2.Distance(transform.position, targetTransform.position));
                soldierAnimator.SetBool("isWalking", true); // Start the walking animation
                MoveStep(targetTransform);
            }

            // This code runs when I am close enough to my target to attack
            else
            {
                soldierAnimator.SetBool("isWalking", false);
            }
            yield return null;
        }
        

        soldierAnimator.SetBool("isWalking", false); // No longer need to move
        isIdle = true; // Idle yet again
        //attackCoroutine = null; // Since this coroutine is finished
        print("AttackCor finished!");
    }

    protected void MoveStep(Transform targetTransform)
    {
        // First find out if I want to go to the left or to the right
        float direction;
        if (targetTransform.position.x - transform.position.x > 0)
        {
            direction = movementSpeed;
        } else
        {
            direction = -movementSpeed;
        }

        // Then move a little closer to my prey
        parentTransform.Translate(direction * Time.deltaTime, 0f, 0f);
    }

    /// <summary>
    /// Smoothly Lerps the soldier to the mouse's position, so the player can move this soldier around however the tides turn
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DragCor()
    {
        if (moveToCoroutine != null) { StopCoroutine(moveToCoroutine); } // Since the god is dragging me around, I should definitely stop trying to go where I was to go
        parentRigidbody.gravityScale = 0f; // No gravity while I'm being tossed around by the god
        isIdle = false; // I am not idle while the god is tossing me around
        
        Vector3 targetPosition = Vector3.zero;
        while (!Input.GetMouseButtonUp(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Calculate the targetPosition
            targetPosition.z = parentTransform.position.z; // Make sure that the targetPosition doesn't change the z-value
            parentTransform.position = Vector3.Lerp(parentTransform.position, targetPosition, 0.2f); // Move the parent's transform since the sprite child cannot be moved
            yield return null;
        }
        parentRigidbody.gravityScale = gravityScale;
        isIdle = true; // I'm idle yet again!
        print("DragCor has ended!");
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
        if (!squareSelected)
        {
            EventManager.onSelect -= OnSelect; // Stop listening to the player selecting other objects since it doesn't matter
            EventManager.onMove -= OnMove; // Stop listening to the player wanting to move the soldier since the soldier is not selected anymore
            EventManager.onAttack -= OnAttack; // Stop listening to the player wanting to attack enemy soldiers since the soldier is unselected
            selectedSprite.SetActive(false); // Deactivate the selected sprite, since this soldier isn't selected any more
        } else
        {
            squareSelected = false;
        }
        
    }

    protected void OnAttack(Transform targetTransform)
    {
        if (moveToCoroutine != null) { StopCoroutine(moveToCoroutine); }
        if (attackCoroutine != null) { StopCoroutine(attackCoroutine); } // Stop attacking someone else and focus on my new target
        attackCoroutine = StartCoroutine(AttackCor(targetTransform));
    }
}
