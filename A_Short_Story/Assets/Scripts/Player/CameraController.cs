using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // PLAYER VARIABLES
    [SerializeField] private Transform playerTransform; // The Transform component of the Player
    [SerializeField] private float battleModeFOV = 5f; // The camera's Field of View when player is in Battle Mode
    [SerializeField] private float armyModeFOV = 5f; // The camera's Field of View when player is in Army Mode
    [SerializeField] private float zoomSpeed = 0f; // The t in the lerp when zoom levels of camera changes when switching between Player Modes (Army or Battle Modes)

    private Vector3 playerPosition; // The position of the Player
    [SerializeField] private bool isArmyMode = false; // Is the Player managing his army

    // CAMERA VARIABLES
    private Camera cameraComponent; // The camera component of the camera

    private void Start()
    {
        playerPosition = new Vector3(0f, 0f, transform.position.z); // Set the starting position of the camera
        cameraComponent = GetComponent<Camera>(); // Get the camera component of the camera
    }

    private void Update()
    {
        if (isArmyMode)
        {
            ArmyMode();
        } else
        {
        FollowPlayer();
        }
    }

    // MAKE THE CAMERA FOLLOW THE PLAYER'S POSITION (BATTLE MODE)
    private void FollowPlayer()
    {
        cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, battleModeFOV, zoomSpeed);
        playerPosition.x = playerTransform.position.x;
        playerPosition.y = playerTransform.position.y;
        transform.position = playerPosition;
    }

    // MAKE CAMERA FUNCTION ACCORDINGLY WHEN PLAYER IS IN ARMY MODE (ARMY MODE)
    private void ArmyMode()
    {
        cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, armyModeFOV, zoomSpeed);
    }

    // SETS THE isArmyMode VARIABLE TO THE GIVEN BOOLEAN PARAMETER 
    public void SetArmyMode(bool inputBool)
    {
        isArmyMode = inputBool;
    }
}
