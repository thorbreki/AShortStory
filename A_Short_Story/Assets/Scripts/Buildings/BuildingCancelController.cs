using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCancelController : MonoBehaviour
{
    [SerializeField] private BuildingController parentBuildingController; // The building controller component of the parent


    private void OnMouseDown()
    {
        print("THIS RUUUUNNNSS!");
        parentBuildingController.CancelBuilding();
    }
}
