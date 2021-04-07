using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObject barracksMenuObject; // The menu that drops down when Player clicks on a Barrack
    private RectTransform barracksMenuRectTransform; // The Rect Transform component of the barracksMenu object

    private float descentAmount = 210f; // How much the menu descends down from the starting point

    private Coroutine smoothLerpPosition; // The coroutine object for the SmoothLerpPosition coroutine

    private void Start()
    {
        barracksMenuObject.SetActive(false); // Make sure that the Barracks Menu is inactive when game starts
        barracksMenuRectTransform = barracksMenuObject.GetComponent<RectTransform>();

        EventManager.onBarrackClick += OnBarrackClick; // Listen to when Player clicks on the Barrack
    }

    // WHEN PLAYER CLICKS ON BARRACK THIS FUNCTION RUNS, ACTIVATES THE BARRACKS MENU
    private void OnBarrackClick(Vector3 barrackPosition)
    {
        barracksMenuObject.SetActive(true);
        barracksMenuObject.GetComponent<BuildingMenuController>().SetFocusedBarrackPosition(barrackPosition);
        StartCoroutine(DescendMenu(barracksMenuRectTransform,
            new Vector3(barracksMenuRectTransform.anchoredPosition.x,
            barracksMenuRectTransform.anchoredPosition.y - descentAmount,
            barracksMenuRectTransform.position.z), 0.1f, 0.05f));
    }

    private IEnumerator DescendMenu(RectTransform inputTransform, Vector3 targetPosition, float speed, float threshold)
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

    private void OnDestroy()
    {
        EventManager.onBarrackClick -= OnBarrackClick;
    }
}
