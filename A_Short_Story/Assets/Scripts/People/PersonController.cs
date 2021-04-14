using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonController : MonoBehaviour
{   
    /// <summary>
    /// The speed in which I move
    /// </summary>
    [SerializeField] protected float movementSpeed;

    /// <summary>
    /// The distance I will walk away from other soldiers if I am accidentally in their personal spaces
    /// </summary>
    [SerializeField] protected float moveAwayDistance;

    /// <summary>
    /// The scale of how much gravity affects me
    /// </summary>
    [SerializeField] protected float gravityScale = 2f;

    /// <summary>
    /// How tall I am
    /// </summary>
    [SerializeField] protected float height = 0.5f;

    /// <summary>
    /// My animator component
    /// </summary>
    protected Animator personAnimator; // The Animator component of the soldier

    /// <summary>
    /// The transform of the parent object that actually moves while this object has the animator
    /// </summary>
    protected Transform parentTransform;

    /// <summary>
    /// This variable lets others know if I want to have my own personal space or not
    /// </summary>
    protected bool doNotDisturb = true;

    /// <summary>
    /// The time signature of when I last started not wanting to be disturbed, used to determine who moves out of the way
    /// </summary>
    protected float latestDoNotDisturbTime = 0f;

    /// <summary>
    /// Determines whether or not I go to the left or to the right when walking away from another person
    /// </summary>
    protected int leftOrRight;

    /// <summary>
    /// A vector that I can use for whatever
    /// </summary>
    protected Vector3 vector;

    /// <summary>
    /// If I am focused and want to attack an enemy, its transform is stored here
    /// </summary>
    protected Transform focusedEnemyTransform;

    // Coroutine objects:
    /// <summary>
    /// The coroutine object associated with the MoveToCor
    /// </summary>
    protected Coroutine moveToCoroutine;


    protected enum animationStatus // The different options for the status of the soldier's animator that are also indices to the animationStatusArray
    {
        isWalking = 0,
        isAttacking = 1,
        isDead = 2,
        isIdle = 69
    }

    protected virtual void Start()
    {
        parentTransform = transform.parent; // Set the parentTransform
        personAnimator = GetComponent<Animator>(); // Set the animator component

        // Determine whether I will go to the left or to the right when moving from other people
        int randomInt = Random.Range(0, 2);
        if (randomInt == 0)
            leftOrRight = -1;
        else if (randomInt == 1)
            leftOrRight = 1;
    }


    /// <summary>
    /// Returns wether this soldier is idle or not
    /// </summary>
    /// <returns>A boolean</returns>
    public bool GetDoNotDisturb()
    {
        return doNotDisturb;
    }

    /// <summary>
    /// Get the time signature of when I last started to require my own personal space
    /// </summary>
    /// <returns></returns>
    public float GetLatestDoNotDisturbTime()
    {
        return latestDoNotDisturbTime;
    }

    /// <summary>
    /// Returns the Transform that I am currently trying to engage in chivalrous combat with
    /// </summary>
    /// <returns></returns>
    public Transform GetFocusedEnemyTransform()
    {
        return focusedEnemyTransform;
    }

    /// <summary>
    /// This is a function that currently only moves me from other soldiers if I am invading their personal space
    /// </summary>
    /// <param name="collision"></param>
    protected void OnTriggerStay2D(Collider2D collision)
    {
        // Must make sure that I'm not in any other fellow soldier's personal space. It is very important for one's mental health!
        if (doNotDisturb) // Only move from other soldier's if I don't want to be disturbed
        {
            PersonController otherSoldierController = collision.GetComponent<PersonController>();
            if (otherSoldierController != null) // If the "thing" I'm colliding with is actually a soldier
            {

                if (!otherSoldierController.GetDoNotDisturb()) { return; } // Don't do anything since the other soldier is already on a move and most definitely passing by

                // If the other soldier started asking for personal space after me, then I have higher priority, thus I wont do crap!
                if (otherSoldierController.GetLatestDoNotDisturbTime() > latestDoNotDisturbTime) { return; }

                // Stop trying to move and attack, I am obviously in the way of someone else
                //if (moveToCoroutine != null) { StopCoroutine(moveToCoroutine); }
                //if (attackCoroutine != null) { StopCoroutine(attackCoroutine); }
                StopCoroutines();


                Transform otherSoldierEnemy = otherSoldierController.GetFocusedEnemyTransform(); // The enemy the other soldier is focused on
                if (otherSoldierEnemy != null) // The other soldier is battling an enemy so I should go the opposite direction
                {
                    print("Oh damn, the other soldier is fighting an enemy!");
                    if (otherSoldierEnemy.position.x > otherSoldierController.transform.position.x) // If the enemy is to the right of the other soldier
                    {
                        print("The enemy is to the right of the other soldier!");
                        moveToCoroutine = StartCoroutine(MoveToCor(transform.position.x - moveAwayDistance));
                    }
                    else // The enemy is to the left of the other soldier
                    {
                        moveToCoroutine = StartCoroutine(MoveToCor(transform.position.x + moveAwayDistance));
                    }
                }
                else // The other soldier is just idle and doing nothing so I can choose which way to go
                {
                    print("Left Or Right: " + leftOrRight);
                    moveToCoroutine = StartCoroutine(MoveToCor(transform.position.x + moveAwayDistance * leftOrRight));
                }
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
        doNotDisturb = false; // I'm not idle when walking
        SetAnimationStatus(animationStatus.isWalking);
        UpdateRotation(targetX);

        yield return null;
        while (Mathf.Abs(targetX - transform.position.x) > 0.05f)
        {
            parentTransform.Translate(movementSpeed * Time.deltaTime, 0f, 0f);

            yield return null;
        }
        parentTransform.position = new Vector3(targetX, parentTransform.position.y, parentTransform.position.z);
        SetAnimationStatus(animationStatus.isIdle);
        doNotDisturb = true; // Idle yet again!
        latestDoNotDisturbTime = Time.time;
        moveToCoroutine = null; // Set the coroutine object for this coroutine to null since this coroutine has ended
    }

    /// <summary>
    /// Self-made overridable method that stops all coroutines
    /// </summary>
    protected virtual void StopCoroutines()
    {
        if (moveToCoroutine != null) { StopCoroutine(moveToCoroutine); moveToCoroutine = null; }
    }

    protected void UpdateRotation(float targetXPosition)
    {
        if (targetXPosition < parentTransform.position.x)
        {
            parentTransform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            parentTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    protected virtual void SetAnimationStatus(animationStatus newStatus)
    {
        // Just smile and wave. Smile and wave...
    }
}
