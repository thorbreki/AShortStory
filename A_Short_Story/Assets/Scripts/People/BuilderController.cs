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
    private float buildingDistance = 6f;

    private bool addedMyselfToBuilding = false; // Have I already added myself to the construction of a building?
    private BuildingController currBuildingController; // The building controller component of the building I am building

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
    protected void OnBuild(Transform buildingTransform, BuildingController buildingController)
    {
        StopCoroutines();
        buildCoroutine = StartCoroutine(BuildCor(buildingTransform, buildingController));
    }

    /// <summary>
    /// This coroutine gets called when the player has selected a building that he/she wants to build
    /// </summary>
    /// <returns></returns>
    protected IEnumerator BuildCor(Transform buildingTransform, BuildingController buildingController)
    {
        // TODO: START WORKING ON THIS, THE BUILDER SHOULD RUN TO THE BUILDING AND START THE BUILDING ANIMATION, AND ADD TO THE NUMBER OF BUILDERS IN THE BUILDINGCONTROLLER
        yield return null;
        print("BuildCor started!");
        currBuildingController = buildingController;
        bool alreadyAddedMyself = false; // If I have already let the building know that I am building it
        float targetXPosition = Random.Range(buildingTransform.position.x - buildingDistance, buildingTransform.position.x + buildingDistance);
        print("TARGET X POSITION: " + targetXPosition.ToString());

        // The loop runs until the building has been fully constructed
        while (!buildingController.GetBuildingIsConstructed())
        {
        //    // This code runs when I am too far away from the building to build
            if (Mathf.Abs(parentTransform.position.x - targetXPosition) > 0.1f)
            {
                UpdateRotation(targetXPosition);
                doNotDisturb = false; // I'm not idle when walking
                SetAnimationStatus(animationStatus.isWalking);
                MoveStep(buildingTransform);
            }

            // This code runs when I am close enough to my target to build
            else
            {
                if (!alreadyAddedMyself)
                {
                    SetAnimationStatus(animationStatus.isBuilding);
                    buildingController.AddBuilderCount(1); // Let the building know I am helping to construct it
                    alreadyAddedMyself = true;
                    addedMyselfToBuilding = true; // I have to remember that I am constructing a building
                }
            }
            yield return null;
        }
        SetAnimationStatus(animationStatus.isIdle);
        doNotDisturb = false; // The builder is always going to allow other people to be in their personal space, for now at least
        addedMyselfToBuilding = false;
        print("BuildCor finished!");
    }

    /// <summary>
    /// If I am currently building and suddenly should stop, this function has to be called to let the building know and other important stuff to happen
    /// </summary>
    protected void HandleStopBuilding()
    {
        if (addedMyselfToBuilding)
        {
            currBuildingController.AddBuilderCount(-1);
            currBuildingController = null;
            addedMyselfToBuilding = false;
        }
    }

    protected override void ListenToEvents()
    {
        base.ListenToEvents();
        EventManager.onConstructMe += OnBuild;
    }

    protected override void StopListenToEvents()
    {
        base.StopListenToEvents();
        EventManager.onConstructMe -= OnBuild;
    }

    protected override void StopCoroutines()
    {
        base.StopCoroutines();
        if (buildCoroutine != null) { StopCoroutine(buildCoroutine); }

        HandleStopBuilding();
    }

    //protected override void WhenSelected()
    //{
    //    base.WhenSelected();
    //}

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        // Dont do anything since builders are supposed to allow other people into their personal space, for now at least!
        //base.OnTriggerStay2D(collision);
    }

    /// <summary>
    /// This function runs when this builder is deselected
    /// </summary>
    //protected override void OnSelect()
    //{
    //    base.OnSelect();
    //}
}
