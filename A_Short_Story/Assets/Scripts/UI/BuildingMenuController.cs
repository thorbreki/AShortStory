using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMenuController : MonoBehaviour
{
    //[SerializeField] private GameObject swordsmanObject; // The Swordsman prefab
    //[SerializeField] private GameObject archerObject; // The Archer prefab
    //[SerializeField] private GameObject soilderTrainerObject; // The Soldier Trainer prefab
    //[SerializeField] private GameObject builderObject; // The Builder prefab
    //[SerializeField] private GameObject builderTrainerObject; // The Builder Trainer prefab
    private RectTransform rectTransform;
    private float ascendAmount = 210f;
    //private Vector3 focusedBarrackPosition; // The position of the Barrack that was just clicked on

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void StopBuildingInteraction()
    {
        StartCoroutine(AscendMenuCoroutine(rectTransform, new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + ascendAmount, transform.position.z), 0.1f, 0.05f));
        GameManager.instance.SetPlayerMode(Constants.PlayerMode.Army);
    }

    /// <summary>
    /// Spawn a person of any type into the game
    /// </summary>
    /// <param name="newPersonObject"></param>
    public void SpawnPerson(GameObject newPersonObject)
    {
        GameObject newPerson = Instantiate(newPersonObject);
        Vector3 buildingPosition = GameManager.instance.GetBuildingInteractionPosition();
        print("The x position of this building: " + buildingPosition.x);
        newPerson.transform.position = new Vector3(buildingPosition.x, buildingPosition.y, Constants.soldierZPosition);
    }
        
    // THIS COROUTINE MAKES THE MENU GO UP, OVER THE CANVAS AND SET IT AS INACTIVE
    private IEnumerator AscendMenuCoroutine(RectTransform inputTransform, Vector3 targetPosition, float speed, float threshold)
    {
        print("starting");
        while (Vector2.Distance(inputTransform.anchoredPosition, targetPosition) > threshold)
        {
            inputTransform.anchoredPosition = Vector3.Lerp(inputTransform.anchoredPosition, targetPosition, speed);
            yield return null;
        }
        inputTransform.anchoredPosition = targetPosition;
        Debug.Log("AscendMenu is done!");
        gameObject.SetActive(false);
    }
}
