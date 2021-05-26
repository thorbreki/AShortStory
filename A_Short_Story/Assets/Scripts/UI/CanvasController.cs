using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject barracksMenuObject; // The menu that drops down when Player clicks on a Barrack
    [SerializeField] private GameObject smithyMenuObject; // The menu that drops down when Player clicks on a Smithy
    [SerializeField] private GameObject builderMenuObject; // The menu that drops down when Player selects a builder
    [SerializeField] private GameObject builderMenuDescriptionText; // The description text in the builder menu
    [SerializeField] private GameObject powerBarObject; // The player's power bar UI object
    [SerializeField] private GameObject oreAmountLabelObject; // The ore label object
    [SerializeField] private GameObject bottomTextObject; // The text in the bottom of the screen, used for detailing stuff

    [Header("RectTransforms")]
    [SerializeField] private RectTransform barracksMenuRectTransform; // The Rect Transform component of the barracksMenu object
    [SerializeField] private RectTransform smithyMenuRectTransform; // The Rect Transform component of the smithyMenu object
    [SerializeField] private RectTransform builderMenuRectTransform; // The Rect Transform component of the builderMenu object

    [Header("UI components")]
    [SerializeField] private TextMeshProUGUI oreAmountLabelText; // The text mesh pro component of the ore amount label
    [SerializeField] private TextMeshProUGUI bottomLabelText; // The text mesh pro ugui component of the bottom description label

    [Header("Values")]
    [SerializeField] private float upHoverThreshold; // How close does the mouse pointer have to be to the height of the screen so the builder menu goes down
    [SerializeField] private float downHoverThreshold; // How far away does the mouse pointer have to be to the height of the screen so the builder menu goes up again

    private float descentAmount = 210f; // How much the menu descends down from the starting point
    private Vector2 startingPosition; // The starting position of all the menus
    private Vector2 biggerMenuDescendPosition; // The descend position for the bigger menus
    private Vector2 builderMenuDescendPosition; // The descend position for the smaller menus, like the menu for the builder
    private Vector2 builderMenuActivePosition; // The position of the builder menu when its active but not being hovered over

    private string nameOfActiveMenu = "yomama"; // The name of the menu that is currently active now

    private Coroutine smoothLerpPosition; // The coroutine object for the SmoothLerpPosition coroutine

    private bool isThereAnActiveMenu = false;

    // Coroutine objects
    private Coroutine descendMenuCoroutine; // The coroutine object for the descend menu coroutine
    private Coroutine moveBuilderMenuCoroutine; // The coroutine object for the move builder menu coroutine
    private Coroutine ascendMenuCoroutine; // The coroutine object for the ascend menu coroutine
    private Coroutine displayBottomTextCoroutine; // The coroutine object for displaying the bottom text for a limited time

    private void Start()
    {
        barracksMenuObject.SetActive(false); // Make sure that the Barracks Menu is inactive when game starts
        startingPosition = barracksMenuRectTransform.anchoredPosition;
        biggerMenuDescendPosition = new Vector2(startingPosition.x, startingPosition.y - descentAmount);
        builderMenuActivePosition = new Vector2(startingPosition.x, 245);
        builderMenuDescendPosition = new Vector2(startingPosition.x, 175);

        MoveBuilderMenu(startingPosition); // The game always begins in Army Mode for now so this works
        powerBarObject.SetActive(false); // The player's power bar should not be seen when commanding armies

        UpdateOreAmountLabel(); // Update the ore amount label so it is 0
        DisableBottomText(); // Make sure that the bottom text is disabled
        EventManager.onBarrackClick += OnBarrackClick; // Listen to when Player clicks on the Barrack
        EventManager.onSmithyClick += OnSmithyClick; // Listen to when Player clicks on the Smithy
        EventManager.onPlayerModeChanged += OnPlayerModeChanged; // Listen to when the player mode of the game changes
    }

    private void Update()
    {
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Building)
        {
            HandleHoveringBuilderMenu();
        }
    }


    /// <summary>
    /// Update the player's ore amount label so it changes when the ore amount changes
    /// </summary>
    public void UpdateOreAmountLabel()
    {
        oreAmountLabelText.text = "Ore: " + GameManager.instance.GetPlayerOreAmount().ToString();
    }

    /// <summary>
    /// Set the bottom text inactive, so it does not render
    /// </summary>
    public void DisableBottomText()
    {
        bottomTextObject.SetActive(false);
    }

    /// <summary>
    /// Display the given string on the bottom of the screen in the color of choice. It will be there forerver so remember to set it inactive later
    /// </summary>
    /// <param name="newText"></param>
    /// <param name="newColor"></param>
    public void DisplayBottomText(string newText, Color newColor)
    {
        bottomLabelText.text = newText;
        bottomLabelText.color = newColor;
        bottomTextObject.SetActive(true);
    }

    /// <summary>
    /// Displays a new string on the bottom of the screen, intended to be a detail text, for instance to say if something is wrong
    /// </summary>
    /// <param name="newText">The string the text will display</param>
    /// <param name="duration">How long in seconds the text will be displayed</param>
    /// <param name="newColor">The color of the text which will be displayed</param>
    public void DisplayBottomText(string newText, float duration, Color newColor)
    {
        if (displayBottomTextCoroutine != null) { StopCoroutine(displayBottomTextCoroutine); }
        displayBottomTextCoroutine = StartCoroutine(displayBottomTextCor(newText, duration, newColor));
    }


    private IEnumerator displayBottomTextCor(string newText, float duration, Color newColor)
    {
        bottomLabelText.text = newText;
        bottomLabelText.color = newColor;
        bottomTextObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        bottomTextObject.SetActive(false);
    }

    // WHEN PLAYER CLICKS ON BARRACK THIS FUNCTION RUNS, ACTIVATES THE BARRACKS MENU
    private void OnBarrackClick()
    {
        DropDownMenu(barracksMenuObject, barracksMenuRectTransform);
    }

    private void OnSmithyClick()
    {
        DropDownMenu(smithyMenuObject, smithyMenuRectTransform);
    }

    /// <summary>
    /// A general method which activates and drops down a specified building menu
    /// </summary>
    /// <param name="theMenu"></param>
    /// <param name="menuRectTransform"></param>
    public void DropDownMenu(GameObject theMenu, RectTransform menuRectTransform)
    {
        isThereAnActiveMenu = true; // Let other scripts know that there is an active menu

        // Only stop coroutines if the menu that is going down was also the last menu that went down as well
        if (nameOfActiveMenu == theMenu.name)
        {
            // Make sure all other coroutines are done!
            if (descendMenuCoroutine != null) { StopCoroutine(descendMenuCoroutine); }
            if (ascendMenuCoroutine != null) { StopCoroutine(ascendMenuCoroutine); }
        } 


        theMenu.SetActive(true);
        descendMenuCoroutine = StartCoroutine(DescendMenu(menuRectTransform, biggerMenuDescendPosition, 0.1f, 0.05f));

        nameOfActiveMenu = theMenu.name;
    }

    private void MoveBuilderMenu(Vector2 newPosition)
    {
        //builderMenuObject.SetActive(true); // Just in case
        if (moveBuilderMenuCoroutine != null) { StopCoroutine(moveBuilderMenuCoroutine); }
        moveBuilderMenuCoroutine = StartCoroutine(MoveBuilderMenuCor(newPosition, 0.2f, 0.05f));
    }

    /// <summary>
    /// This function runs when the player mode of the game changes
    /// </summary>
    /// <param name="newPlayerMode">The new player mode of the game</param>
    private void OnPlayerModeChanged(Constants.PlayerMode newPlayerMode)
    {
        // When the new player mode is army mode, the builder menu should drop down to its active position
        if (newPlayerMode == Constants.PlayerMode.Building)
        {
            MoveBuilderMenu(builderMenuActivePosition); // Smoothly move the builder menu to its active positioning
            powerBarObject.SetActive(false); // The player's power bar should not be seen when commanding armies
        }

        else if (newPlayerMode == Constants.PlayerMode.Battle)
        {
            powerBarObject.SetActive(true); // The player's power bar should be seen when in battle mode
            MoveBuilderMenu(startingPosition); // The player cannot build when in battle mode
        }

        else
        {
            MoveBuilderMenu(startingPosition); // Smoothly move the builder menu to its start position
            powerBarObject.SetActive(false); // power bar should not be active in other player modes
        }
    }

    private void HandleHoveringBuilderMenu()
    {
        // When the player is close enough with the cursor to the top of the screen, the menu falls down
        if (Mathf.Abs(Input.mousePosition.y - Screen.height) <= downHoverThreshold)
        {
            if (moveBuilderMenuCoroutine != null) { return; } // Dont start the coroutine if it is already running
            if (Mathf.Abs(builderMenuRectTransform.anchoredPosition.y - builderMenuDescendPosition.y) < 0.05f) { return; } // The builder menu is already there

            moveBuilderMenuCoroutine = StartCoroutine(MoveBuilderMenuCor(builderMenuDescendPosition, 0.1f, 0.05f));
            builderMenuDescriptionText.SetActive(false);
            isThereAnActiveMenu = true;

        }
        // When the player is far enough with the cursor from the top of the screen, the menu goes back up
        else if(Mathf.Abs(Input.mousePosition.y - Screen.height) >= upHoverThreshold)
        {
            if (moveBuilderMenuCoroutine != null) { return; }
            if (Mathf.Abs(builderMenuRectTransform.anchoredPosition.y - builderMenuActivePosition.y) < 0.05f) { return; } // The builder menu is already there

            moveBuilderMenuCoroutine = StartCoroutine(MoveBuilderMenuCor(builderMenuActivePosition, 0.1f, 0.05f));
            builderMenuDescriptionText.SetActive(true);
            isThereAnActiveMenu = false;
        }
    }

    private IEnumerator DescendMenu(RectTransform inputTransform, Vector2 targetPosition, float speed, float threshold)
    {
        print("starting");
        while (Vector2.Distance(inputTransform.anchoredPosition, targetPosition) > threshold)
        {
            inputTransform.anchoredPosition = Vector3.Lerp(inputTransform.anchoredPosition, targetPosition, speed);
            yield return null;
        }
        inputTransform.anchoredPosition = targetPosition;
        Debug.Log("DescendMenu is done!");
    }

    // THIS COROUTINE MAKES THE MENU GO UP, OVER THE CANVAS AND SET IT AS INACTIVE
    private IEnumerator AscendMenuCoroutine(GameObject menuObject, RectTransform inputTransform, Vector3 targetPosition, float speed, float threshold)
    {
        while (Vector2.Distance(inputTransform.anchoredPosition, targetPosition) > threshold)
        {
            inputTransform.anchoredPosition = Vector3.Lerp(inputTransform.anchoredPosition, targetPosition, speed);
            yield return null;
        }
        inputTransform.anchoredPosition = targetPosition;
        menuObject.SetActive(false);
        ascendMenuCoroutine = null; // Just to let other functions know that this coroutine is 100% finished
        print("AscendMenu done!");
    }

    private IEnumerator MoveBuilderMenuCor(Vector2 newPosition, float speed, float threshold)
    {
        print("starting");
        while (Vector2.Distance(builderMenuRectTransform.anchoredPosition, newPosition) > threshold)
        {
            builderMenuRectTransform.anchoredPosition = Vector3.Lerp(builderMenuRectTransform.anchoredPosition, newPosition, speed);
            yield return null;
        }
        builderMenuRectTransform.anchoredPosition = newPosition;
        Debug.Log("MoveBuilderMenu is done!");
        moveBuilderMenuCoroutine = null; // To make sure that the coroutine object is null when this coroutine finishes
    }

    /// <summary>
    /// This method should be called when player presses the Cancel button
    /// </summary>
    public void StopBuildingInteraction(GameObject theMenu)
    {
        isThereAnActiveMenu = false; // Let other scripts know that the active menu is going back up again

        RectTransform theRectTransform = theMenu.GetComponent<RectTransform>();
        // Only stop coroutines if the menu that is going down was also the last menu that went down as well
        if (nameOfActiveMenu == theMenu.name)
        {
            // Make sure all other coroutines are done!
            if (descendMenuCoroutine != null) { StopCoroutine(descendMenuCoroutine); }
            if (ascendMenuCoroutine != null) { StopCoroutine(ascendMenuCoroutine); }
        }

        // If the ascending menu is the builder menu, we must make sure that the move builder menu coroutine is stopped if it is already in 
        //if (moveBuilderMenuCoroutine != null) { StopCoroutine(moveBuilderMenuCoroutine); }

        ascendMenuCoroutine = StartCoroutine(AscendMenuCoroutine(theMenu, theRectTransform, startingPosition, 0.1f, 0.05f));
        GameManager.instance.SetPlayerMode(Constants.PlayerMode.Army);
    }

    private void OnDestroy()
    {
        EventManager.onBarrackClick -= OnBarrackClick;
        EventManager.onSmithyClick -= OnSmithyClick;
        EventManager.onPlayerModeChanged -= OnPlayerModeChanged;
    }

    // ------------------ GETTERS
    public bool GetIsThereAnActiveMenu()
    {
        return isThereAnActiveMenu;
    }
}
