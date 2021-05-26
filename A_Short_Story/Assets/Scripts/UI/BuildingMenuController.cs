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

    /// <summary>
    /// Spawn in a new unconstructed building of any type in the game so the player can choose where the building should be placed
    /// </summary>
    /// <param name="newBuildingObject"></param>
    public void SpawnUnconstructedBuilding(GameObject newBuildingObject)
    {
        
        GameObject newBuilding = Instantiate(newBuildingObject);
        newBuilding.transform.position = new Vector3(Camera.main.transform.position.x, newBuilding.transform.position.y, newBuilding.transform.position.z);
        EventManager.RaiseOnPlacingNewBuilding(); // Let other objects know that the player wants to place a new building
    }
}
