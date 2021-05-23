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

    [Header("Builder Attributes")]
    [SerializeField] private float damage; // How much damage I do
    [SerializeField] private float mineSpeed = 0.1f; // How fast I mine
    [SerializeField] private int maxOreCapacity = 6; // The maximum number of ores I can carry

    [Header("Builder Objects")]
    [SerializeField] private GameObject oreObject; // The ore I hold when my current ore amount has reached full capacity

    // PROTECTED AND PRIVATE VARIABLES
    protected float currOreAmount = 0f; // How much ore I have, from 0 to 1
    protected Transform nearestSmithyTransform; // The nearest Smithy's transform component

    private float buildingDistance = 6f;


    private bool addedMyselfToBuilding = false; // Have I already added myself to the construction of a building?
    private BuildingController currBuildingController; // The building controller component of the building I am building

    // Coroutine objects
    private Coroutine buildCoroutine; // For the BuildCor
    private Coroutine mineCoroutine; // For the MineCor


    protected override void Start()
    {
        base.Start();
        oreObject.SetActive(false);
    }

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
                MoveStep(targetXPosition);
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
    /// When the builder recieves a mine event, so they starts mining
    /// </summary>
    /// <param name="oreTransform"></param>
    /// <param name="oreController"></param>
    protected void OnMine(Transform oreTransform, OreController oreController)
    {
        StopCoroutines();
        mineCoroutine = StartCoroutine(MineCor(oreTransform, oreController));
    }


    protected IEnumerator MineCor(Transform oreTransform, OreController oreController)
    {
        print("MineCor started!");
        float targetOreXPosition = Random.Range(oreTransform.position.x - buildingDistance, oreTransform.position.x + buildingDistance); // Uses BuildCor code I know...
        print("X position of the ore: " + oreTransform.position.x);
        print("The target ore X position: " + targetOreXPosition);
        float targetXPosition = 0f; // The target X position I will try to go to

        doNotDisturb = false; // The builder is always going to allow other people to be in their personal space, for now at least

        // This loops FOREVER! for now at least, while ores have infinite capacity
        while (true)
        {
            // Figure out the targetXPosition
            if (currOreAmount < maxOreCapacity)
            {
                targetXPosition = targetOreXPosition;
            }
            else
            {
                targetXPosition = oreController.getNearestSmithyTransform().position.x;
            }

            // This code runs when I am too far away from either the ore or the Smithy
            if (Mathf.Abs(parentTransform.position.x - targetXPosition) > 0.1f)
            {
                UpdateRotation(targetXPosition);
                SetAnimationStatus(animationStatus.isWalking);
                MoveStep(targetXPosition);
            }

            // This code runs when I am close enough to the ore to mine
            else if (Mathf.Abs(parentTransform.position.x - targetOreXPosition) < 0.1f)
            {
                SetAnimationStatus(animationStatus.isBuilding);
                currOreAmount += mineSpeed * Time.deltaTime;

                // Set the ore object to visible if I have enough ore
                if (currOreAmount >= maxOreCapacity) { oreObject.SetActive(true); }
            }

            // When I am where the nearest Smithy is, so now I let go of the ore and increase the player's resources
            else if (Mathf.Abs(parentTransform.position.x - oreController.getNearestSmithyTransform().position.x) < 0.1f)
            {
                SetAnimationStatus(animationStatus.isIdle);
                currOreAmount = 0;
                GameManager.instance.changePlayerOreAmount(maxOreCapacity);
                oreObject.SetActive(false);
            }
            yield return null;
        }
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
        EventManager.onOreClicked += OnMine;
    }

    protected override void StopListenToEvents()
    {
        base.StopListenToEvents();
        EventManager.onConstructMe -= OnBuild;
        EventManager.onOreClicked -= OnMine;
    }

    protected override void StopCoroutines()
    {
        base.StopCoroutines();
        SetAnimationStatus(animationStatus.isIdle); // Just in case, at least mine coroutine needs it
        if (buildCoroutine != null) { StopCoroutine(buildCoroutine); }
        if (mineCoroutine != null) { StopCoroutine(mineCoroutine); }

        HandleStopBuilding(); // Let the building that I am working on know that I have stopped working on it
    }
}
