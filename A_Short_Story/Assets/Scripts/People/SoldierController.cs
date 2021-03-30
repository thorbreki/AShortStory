using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    [SerializeField] protected float movementSpeed; // The movementSpeed of this soldier
    [SerializeField] protected float moveAwayDistance; // The distance I want to move from another soldier when we are invading each other's personal spaces
    [SerializeField] protected float height = 0.5f; // My height
    [SerializeField] private GameObject selectedSprite; // The game object that shows wether a soldier is selected or not
    [SerializeField] private LayerMask gravityLayerMask; // The layermask for the gravity raycast
    protected Animator soldierAnimator; // The Animator component of the soldier
    protected bool controlled = false; // If the soldier is controlled the player can take control, otherwise the person thinks freely
    protected bool selected = false;
    protected bool mouseDown = false; // A boolean that specifies wether the soldier was clicked on or not
    protected bool squareSelected = false; // Was the soldier selected with the select soldier square?
    protected Transform parentTransform; // The Parent Transform of the soldier sprite
    protected float currGravityStrength; // The current strength of gravity I am experiencing

    // A vector that I can use for whatever:
    protected Vector3 vector;

    // Coroutine objects:
    protected Coroutine moveToCoroutine; // The object storing the MoveToCor coroutine
    protected Coroutine dragCoroutine; // The object storing the DragCor coroutine
    protected Coroutine gravitySlerpCoroutine; // The object storing the GravitySlerp coroutine

    protected void Start()
    {
        parentTransform = transform.parent;
        soldierAnimator = gameObject.GetComponent<Animator>(); // Get the Animator component of the child sprite object
        selectedSprite.SetActive(false);
    }

    protected void Update()
    {
        HandleGravity();
    }

    protected void HandleGravity()
    {
        RaycastHit2D hitinfo = Physics2D.Raycast(parentTransform.position, Vector2.down, 100f, gravityLayerMask);
        if (!hitinfo) { return; } // Not hitting anything, so I'm not going to do anything
        if (parentTransform.position.y - hitinfo.point.y <= 0.5) // When I'm grounded
        {
            vector.x = parentTransform.position.x;
            vector.z = parentTransform.position.z;
            vector.y = hitinfo.point.y + height; // Thats the current height of the soldier, maybe will have to change it later on
            currGravityStrength = 0f;
            return;
        } 

        // Since I am not grounded, its time for me to fall down
        if (gravitySlerpCoroutine == null) { gravitySlerpCoroutine = StartCoroutine(GravitySlerpCor(hitinfo)); }
        

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

            print("The spirit talks to me!"); // DEBUG
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
        print("OnTriggerStay colliding with: " + collision.name);
        // Must make sure that I'm not in any other fellow soldier's personal space. It is very important for one's mental health!
        if (moveToCoroutine == null) // Only move from other soldier's if I'm idle
        {
            SoldierController otherSoldierController = collision.GetComponent<SoldierController>();
            if (otherSoldierController != null) // If the "thing" I'm colliding with is actually a soldier
            {
                if (!otherSoldierController.isIdle()) { return; } // Don't do anything since the other soldier is already on a move and most definitely passing by

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
    public bool isIdle()
    {
        return moveToCoroutine == null;
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
        moveToCoroutine = null; // Set the coroutine object for this coroutine to null since this coroutine has ended
    }

    /// <summary>
    /// Smoothly Lerps the soldier to the mouse's position, so the player can move this soldier around however the tides turn
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DragCor()
    {
        Vector3 targetPosition = Vector3.zero;
        while (!Input.GetMouseButtonUp(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Calculate the targetPosition
            targetPosition.z = parentTransform.position.z; // Make sure that the targetPosition doesn't change the z-value
            parentTransform.position = Vector3.Lerp(parentTransform.position, targetPosition, 0.2f); // Move the parent's transform since the sprite child cannot be moved
            print("targetPosition: " + targetPosition);
            yield return null;
        }
        print("DragCor has ended!");
    }

    protected IEnumerator GravitySlerpCor(RaycastHit2D hitInfo)
    {
        print("GravitySlerpCor started!");
        Vector3 targetPosition = new Vector3(parentTransform.position.x, hitInfo.point.y + (height / 2), parentTransform.position.z); // Find the target position, which is straight down but should be half the height up
        Vector3 startingPosition = parentTransform.position; // The position of me when I start falling down
        float t = 0f; // The t in the Lerping function
        float distance = startingPosition.y - targetPosition.y; // The distance I will have to travel downwards to the ground
        while (t < 0.99f)
        {
            parentTransform.position = Vector3.Lerp(startingPosition, targetPosition, t);
            t += Time.deltaTime * GameManager.instance.GetGravity() / distance; // t increases with deltaTime times the gravity of the game, but I divide by distance so the speed of falling down is always the same no matter the distance
            yield return null;
        }

        // When I have fallen down, position me correctly and set the coroutine object to null to allow me to fall once again
        parentTransform.position = targetPosition;
        gravitySlerpCoroutine = null;
        print("GravitySlerpCor has ended!");
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
            print("yo this runs for some reason!");
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

    }
}
