using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderController : SoldierController
{
    // CREATE THE BUILDER MENU WHICH THE PLAYER CAN USE TO SELECT WHAT BUILDINGS HE WANTS TO PLACE
    // WHEN THE PLAYER SELECTS A BUILDING, A NEW GAME OBJECT GETS SPAWNED WHICH IS A TRANSPARENT BUILDING
    // WHEN THE PLAYER CLICKS ON A POSITION TO PLACE THE BUILDING, A NEW YELLOW TRANSPARENT BUILDING APPEARS AND THEN THE BUILDCOROUTINE STARTS

    // FINISH THIS CONTROLLER SO IT WORKS CORRECTLY WITH ALL THE NORMAL SOLDIER METHODS
    // OVERRIDE AND ADD TO THIS SCRIPT ALL NECESSARY METHODS:
    // OVERRIDE ALL FUNCTIONS YOU NEED TO MAKE THIS BUILDER ALSO LISTEN TO ONBUILD EVENT
    // AFTER THAT, CREATE THE BUILDING MECHANISM

    [SerializeField] private float damage; // How much damage I do

    // Coroutine objects
    private Coroutine buildCoroutine; // For the BuildCor

    public override void DamageEnemy()
    {
        if (focusedEnemyTransform == null) { return; } // If the enemy transform doesnt exist anymore than I'm not doing crap!
        EnemyController enemyController = focusedEnemyTransform.GetComponent<EnemyController>();
        if (enemyController != null) // Paranoid check
        {
            enemyController.DamageMe(damage);
        }
    }

    /// <summary>
    /// When the builder recieves a build event
    /// </summary>
    protected void OnBuild(GameObject buildingObject)
    {
        StopCoroutines();
        buildCoroutine = StartCoroutine(BuildCor(buildingObject));
    }

    /// <summary>
    /// This coroutine gets called when the player has selected a building that he/she wants to build
    /// </summary>
    /// <returns></returns>
    protected IEnumerator BuildCor(GameObject buildingObject)
    {
        yield return null;
        //print("AttackCor started!");
        //focusedEnemyTransform = targetTransform; // Set the transform so other methods know what enemy they are supposed to interact with
        //bool notDisturbTimeAlreadySet = false;

        //// The loop runs until the enemy has been dealt with
        //while (targetTransform != null)
        //{
        //    // This code runs when I am too far away from the enemy to attack
        //    if (Vector2.Distance(transform.position, targetTransform.position) > attackDistance)
        //    {
        //        UpdateRotation(targetTransform.position.x);
        //        doNotDisturb = false; // I'm not idle when walking
        //        SetAnimationStatus(animationStatus.isWalking);
        //        MoveStep(targetTransform);
        //    }

        //    // This code runs when I am close enough to my target to attack
        //    else
        //    {
        //        if (!notDisturbTimeAlreadySet)
        //        {
        //            latestDoNotDisturbTime = Time.time;
        //            notDisturbTimeAlreadySet = true;
        //        }
        //        doNotDisturb = true; // I am not to be disturbed when battling a dangerous foe!
        //        SetAnimationStatus(animationStatus.isAttacking);
        //    }
        //    yield return null;
        //}
        //SetAnimationStatus(animationStatus.isIdle);
        //focusedEnemyTransform = null; // I shouldn't focus on the enemy anymore since they're dead, hurray!
        //doNotDisturb = true; // Idle yet again
        //print("AttackCor finished!");
    }

    protected override void ListenToEvents()
    {
        base.ListenToEvents();
        EventManager.onBuild += OnBuild;
    }

    protected override void StopListenToEvents()
    {
        base.StopListenToEvents();
        EventManager.onBuild -= OnBuild;
    }

    protected override void StopCoroutines()
    {
        base.StopCoroutines();
        if (buildCoroutine != null) { StopCoroutine(buildCoroutine); }
    }

    protected override void WhenSelected()
    {
        base.WhenSelected();
        //EventManager.RaiseOnBuilderSelected();
        GameManager.instance.IncreaseNumOfBuildersSelected();
    }

    /// <summary>
    /// This function runs when this builder is deselected
    /// </summary>
    protected override void OnSelect()
    {
        base.OnSelect();
        GameManager.instance.DecreaseNumOfBuildersSelected();
    }
}
