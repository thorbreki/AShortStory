using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMenuController : MonoBehaviour
{
    private float descentAmount = 181f; // How much the menu descends down from the starting point
    private Vector3 startingPosition; // The position where the menu started at
    private RectTransform rectTransform; // The RectTransform component of this gameObject

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startingPosition = rectTransform.anchoredPosition;
        StartCoroutine(SmoothLerpPosition(rectTransform, new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - descentAmount, rectTransform.position.z), 0.1f, 0.05f));
    }

    private IEnumerator SmoothLerpPosition(RectTransform inputTransform, Vector3 targetPosition, float speed, float threshold)
    {
        print("starting");
        while (Vector2.Distance(inputTransform.anchoredPosition, targetPosition) > threshold)
        {
            inputTransform.anchoredPosition = Vector3.Lerp(inputTransform.anchoredPosition, targetPosition, speed);
            yield return null;
        }
        inputTransform.anchoredPosition = targetPosition;
        Debug.Log("SmoothLerpPosition is done!");
    }
}
