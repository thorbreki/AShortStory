using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierController : PersonController
{

    // DRAGGING
    [Header("Dragging")]
    [SerializeField] protected float dragDistanceThreshold; // How much the god has to drag me before I actually start to respond

    // ATTACK
    [Header("Attack")]
    [SerializeField] protected float attackDistance = 0.8f; // The distance I want to be from an enemy when I start attacking that enemy

    // CONNECTED GAME OBJECTS AND COMPONENTS
    [Header("Connected game objects and components")]
    [SerializeField] private GameObject selectedSprite; // The game object that shows wether a soldier is selected or not
    protected Rigidbody2D parentRigidbody; // The rigidbody component of the parent

    // Status booleans
    protected bool controlled = false; // If the soldier is controlled the player can take control, otherwise the person thinks freely
    protected bool selected = false;
    protected bool mouseDown = false; // A boolean that specifies wether the soldier was clicked on or not
    protected bool squareSelected = false; // Was the soldier selected with the select soldier square?


    // Coroutine objects:
    protected Coroutine dragCoroutine; // The object storing the DragCor coroutine
    protected Coroutine attackCoroutine; // The object storing the AttackCor coroutine

    protected string[] animationStatusArray = new string[3] // An array that contains all different booleans in the soldier's animator 
    {
        animationStatus.isWalking.ToString(),
        animationStatus.isAttacking.ToString(),
        animationStatus.isBuilding.ToString()
    };

    protected override void Start()
    {
        base.Start();

        parentRigidbody = transform.parent.GetComponent<Rigidbody2D>(); // Set the rigidbody component of the parent
        selectedSprite.SetActive(false); // Set the selected sprite inactive by default

        // setting stuff up
        parentRigidbody.gravityScale = gravityScale;

    }

    protected void OnDestroy()
    {
        StopListenToEvents();
    }

    protected void OnMouseDown()
    {
        print("ONMOUSEDOWN!");
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army)
        {
            mouseDown = true; // Specify that the soldier was indeed clicked on

            if (dragCoroutine != null) { StopCoroutine(dragCoroutine); }
            dragCoroutine = StartCoroutine(DragCor());
        }
    }

    // WHEN PLAYER LEFT CLICKS ON THE SOLDIER, THE PLAYER CAN THEN TELL THE SOLDIER WHAT TO DO
    protected void OnMouseUp()
    {
        print("ONMOUSEUP!");
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


    /// <summary>
    /// Run this function to execute all the stuff that are always required when the soldier gets selected
    /// </summary>
    protected virtual void WhenSelected()
    {
        StopListenToEvents();

        selectedSprite.SetActive(true);

        ListenToEvents();
    }


    protected IEnumerator AttackCor(Transform targetTransform)
    {
        print("AttackCor started!");
        focusedEnemyTransform = targetTransform; // Set the transform so other methods know what enemy they are supposed to interact with
        bool notDisturbTimeAlreadySet = false;

        // The loop runs until the enemy has been dealt with
        while (targetTransform != null)
        {
            // This code runs when I am too far away from the enemy to attack
            if (Vector2.Distance(transform.position, targetTransform.position) > attackDistance)
            {
                UpdateRotation(targetTransform.position.x);
                doNotDisturb = false; // I'm not idle when walking
                SetAnimationStatus(animationStatus.isWalking);
                MoveStep(targetTransform);
            }

            // This code runs when I am close enough to my target to attack
            else
            {
                if (!notDisturbTimeAlreadySet)
                {
                    latestDoNotDisturbTime = Time.time;
                    notDisturbTimeAlreadySet = true;
                }
                doNotDisturb = true; // I am not to be disturbed when battling a dangerous foe!
                SetAnimationStatus(animationStatus.isAttacking);
            }
            yield return null;
        }
        SetAnimationStatus(animationStatus.isIdle);
        focusedEnemyTransform = null; // I shouldn't focus on the enemy anymore since they're dead, hurray!
        doNotDisturb = true; // Idle yet again
        print("AttackCor finished!");
    }

    protected void MoveStep(Transform targetTransform)
    {
        // First find out if I want to go to the left or to the right
        float direction;
        if (targetTransform.position.x - transform.position.x > 0)
        {
            //parentTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
            direction = movementSpeed;
        } else
        {
            //parentTransform.rotation = Quaternion.Euler(0f, 180f, 0f);
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
        StopCoroutines(); // Stop all current coroutines so the god can toss me around as they please
        parentRigidbody.gravityScale = 0f; // No gravity while I'm being tossed around by the god
        doNotDisturb = false; // I am not idle while the god is tossing me around
        SetAnimationStatus(animationStatus.isIdle);

        Vector2 startingPosition = transform.position; // My position when the god starts tossing me around
        
        // The god will not drag me around unless he moves his force far enough from me, this loop will make sure of that
        while (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position) < dragDistanceThreshold)
        {
            yield return null;
        }

        Vector3 targetPosition = Vector3.zero;
        while (Input.GetMouseButton(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Calculate the targetPosition
            targetPosition.z = parentTransform.position.z; // Make sure that the targetPosition doesn't change the z-value
            parentTransform.position = Vector3.Lerp(parentTransform.position, targetPosition, 0.2f); // Move the parent's transform since the sprite child cannot be moved
            yield return null;
        }
        parentRigidbody.gravityScale = gravityScale;
        doNotDisturb = true; // I'm idle yet again!
        latestDoNotDisturbTime = Time.time;
        print("DragCor has ended!");
    }

    /// <summary>
    /// Sets the animator booleans to respective values according to the status that is given as parameter
    /// </summary>
    /// <param name="newStatus"></param>
    protected override void SetAnimationStatus(animationStatus newStatus)
    {
        int newStatusInt = (int)newStatus; // The new animation status integerized

        for (int i = 0; i < animationStatusArray.Length; i++)
        {
            if (i == newStatusInt)
            {
                personAnimator.SetBool(animationStatusArray[i], true);
            } else
            {
                personAnimator.SetBool(animationStatusArray[i], false);
            }
        }
    }

    /// <summary>
    /// This function executes when the player tells a soldier to move to a specific postition
    /// </summary>
    protected void OnMove(float targetX)
    {
        if (moveToCoroutine != null) { StopCoroutine(moveToCoroutine); } // Make sure that no other instances of move coroutine are active
        if (attackCoroutine != null) { StopCoroutine(attackCoroutine); } // I should definitely stop attacking and go to where the god wants me to

        moveToCoroutine = StartCoroutine(MoveToCor(targetX)); // Start the move to coroutine and store the object for future checks
    }

    /// <summary>
    /// This function runs when the player left-clicks/selects any object, which means this soldier will become unselected
    /// </summary>
    protected virtual void OnSelect()
    {
        if (!squareSelected)
        {
            StopListenToEvents();
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

    /// <summary>
    /// Damage the enemy I am focused on, this method runs when attack animation event occurs
    /// </summary>
    public virtual void DamageEnemy()
    {
        // DAMAGE ENEMY IN THE CHILD SCRIPT OVERRIDE METHODS!
    }

    /// <summary>
    /// This bad boy stops all soldier-specific coroutines so I can have a clear mind when the god tells me what to do
    /// </summary>
    protected override void StopCoroutines()
    {
        if (moveToCoroutine != null) { StopCoroutine(moveToCoroutine); }
        if (attackCoroutine != null) { StopCoroutine(attackCoroutine); }
        if (dragCoroutine != null) { StopCoroutine(dragCoroutine); }
    }

    /// <summary>
    /// Start listening to events that I should be listening to
    /// </summary>
    protected virtual void ListenToEvents()
    {
        EventManager.onSelect += OnSelect;
        EventManager.onMove += OnMove;
        EventManager.onAttack += OnAttack;
    }

    /// <summary>
    /// Stop listening to events that I can listen to
    /// </summary>
    protected virtual void StopListenToEvents()
    {
        EventManager.onSelect -= OnSelect;
        EventManager.onMove -= OnMove;
        EventManager.onAttack -= OnAttack;
    }
}
