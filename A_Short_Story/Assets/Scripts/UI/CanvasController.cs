using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject barracksMenuObject; // The menu that drops down when Player clicks on a Barrack
    [SerializeField] private GameObject smithyMenuObject; // The menu that drops down when Player clicks on a Smithy
    [SerializeField] private GameObject builderMenuObject; // The menu that drops down when Player selects a builder

    [Header("RectTransforms")]
    [SerializeField] private RectTransform barracksMenuRectTransform; // The Rect Transform component of the barracksMenu object
    [SerializeField] private RectTransform smithyMenuRectTransform; // The Rect Transform component of the smithyMenu object
    [SerializeField] private RectTransform builderMenuRectTransform; // The Rect Transform component of the builderMenu object

    private float descentAmount = 210f; // How much the menu descends down from the starting point
    private Vector2 startingPosition; // The starting position of all the menus
    private Vector2 biggerMenuDescendPosition; // The descend position for the bigger menus
    private Vector2 smallerMenuDescendPosition; // The descend position for the smaller menus, like the menu for the builder

    private string nameOfActiveMenu = "yomama"; // The name of the menu that is currently active now

    private Coroutine smoothLerpPosition; // The coroutine object for the SmoothLerpPosition coroutine

    private bool isThereAnActiveMenu = false;

    // Coroutine objects
    private Coroutine descendMenuCoroutine; // The coroutine object for the descend menu coroutine
    private Coroutine ascendMenuCoroutine; // The coroutine object for the ascend menu coroutine

    private void Start()
    {
        barracksMenuObject.SetActive(false); // Make sure that the Barracks Menu is inactive when game starts
        startingPosition = barracksMenuRectTransform.anchoredPosition;
        biggerMenuDescendPosition = new Vector2(startingPosition.x, startingPosition.y - descentAmount);
        smallerMenuDescendPosition = biggerMenuDescendPosition; // FOR NOW

        EventManager.onBarrackClick += OnBarrackClick; // Listen to when Player clicks on the Barrack
        EventManager.onSmithyClick += OnSmithyClick; // Listen to when Player clicks on the Smithy
    }

    // WHEN PLAYER CLICKS ON BARRACK THIS FUNCTION RUNS, ACTIVATES THE BARRACKS MENU
    private void OnBarrackClick()
    {
        DropDownMenu(barracksMenuObject, barracksMenuRectTransform);
        //barracksMenuObject.SetActive(true);
        //StartCoroutine(DescendMenu(barracksMenuRectTransform,
        //    new Vector3(barracksMenuRectTransform.anchoredPosition.x,
        //    barracksMenuRectTransform.anchoredPosition.y - descentAmount,
        //    barracksMenuRectTransform.position.z), 0.1f, 0.05f));
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

        ascendMenuCoroutine = StartCoroutine(AscendMenuCoroutine(theMenu, theRectTransform, startingPosition, 0.1f, 0.05f));
        GameManager.instance.SetPlayerMode(Constants.PlayerMode.Army);
    }

    private void OnDestroy()
    {
        EventManager.onBarrackClick -= OnBarrackClick;
        EventManager.onSmithyClick -= OnSmithyClick;
    }

    // ------------------ GETTERS
    public bool GetIsThereAnActiveMenu()
    {
        return isThereAnActiveMenu;
    }
}
