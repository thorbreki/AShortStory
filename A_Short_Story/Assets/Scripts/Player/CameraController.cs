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

    private Vector3 targetPosition; // The position the Camera is trying to get to, always tries to follow the player linearly

    // CAMERA VARIABLES
    [SerializeField] private float maximumMovementSpeed = 0f; // How fast the Camera can move when game is in Army Mode
    [SerializeField] private int numOfScreenParts = 3; // How many parts the screen should be partitioned, that way calculating the mouseMoveThreshold correctly no matter the screen size

    private Camera cameraComponent; // The camera component of the camera
    private float screenWidth; // The width of the screen (in pixel coordinates)
    private float mouseMoveThreshold = 0f; // How close to the edges of the screen does the cursor need to be in order for the camera to start move when game is in Army Mode

    // OTHER VARIABLES
    private float t; // For Lerping purposes
    private float startingNum; // The a parameter in the Lerp function
    private bool isFocusedOnBuilding; // If the camera is looking at a building and therefore can't move (Barracks, Smithy, ...)


    private void Start()
    {
        targetPosition = new Vector3(0f, 0f, transform.position.z); // Set the starting position of the camera
        cameraComponent = GetComponent<Camera>(); // Get the camera component of the camera
        screenWidth = Screen.width; // Set the screen width in the beginning to not have to do it again (might want to rethink that later on)
        mouseMoveThreshold = Screen.width / numOfScreenParts; // The threshold will be equal to the width of a single part of the screen
        isFocusedOnBuilding = false; // The camera is not looking at a building when game starts

        EventManager.onBarrackClick += OnBuildingClick; // When player clicks on a building, the Camera's OnBuildingClick method runs
    }

    void OnDestroy()
    {
        EventManager.onBarrackClick -= OnBuildingClick; // Remove event listener
    }


    private void Update()
    {
        if (GameManager.instance.isArmyMode)
        {
            ArmyMode();
        }
        else
        {
            cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, battleModeFOV, zoomSpeed); // Zoom in
            targetPosition.x = Mathf.Lerp(transform.position.x, playerTransform.position.x, 0.2f);
            transform.position = targetPosition;
        }
    }

    // MAKE CAMERA FUNCTION ACCORDINGLY WHEN PLAYER IS IN ARMY MODE (ARMY MODE)
    private void ArmyMode()
    {
        cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, armyModeFOV, zoomSpeed); // Zoom out

        if (isFocusedOnBuilding)
            return;

        // Check and handle if player presses both shift, and A or D
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A)) // When the player presses Shift and A the camera should pan to the left at full speed
        {
            transform.Translate(-maximumMovementSpeed * Time.deltaTime, 0f, 0f);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D)) // When the player presses D the camera should pan to the right
        {
            transform.Translate(maximumMovementSpeed * Time.deltaTime, 0f, 0f);
        }

        // Check and handle when player presses just A or D
        else if (Input.GetKey(KeyCode.A)) // When the player presses A the camera should pan to the left
        {
            transform.Translate((-maximumMovementSpeed / 2) * Time.deltaTime, 0f, 0f);
        }
        else if (Input.GetKey(KeyCode.D)) // When the player presses D the camera should pan to the right
        {
            transform.Translate((maximumMovementSpeed / 2) * Time.deltaTime, 0f, 0f);
        }

        // Check if the mouse cursor is enough to the left or right for the camera to move smoothly in the corresponding direction
        else if (Input.mousePosition.x < mouseMoveThreshold) // Checking if the mouse is close enough to the left edge of the screen
        {
            transform.Translate(-maximumMovementSpeed * CalculateMouseMoveSpeed(0f) * Time.deltaTime, 0f, 0f);
        }
        else if (Input.mousePosition.x > (Screen.width - mouseMoveThreshold))
        {
            transform.Translate(maximumMovementSpeed * CalculateMouseMoveSpeed(Screen.width) * Time.deltaTime, 0f, 0f);
        }

        // Reset parameters for Lerp in BattleMode
        t = 0; // Reset the t parameter in the Lerp function for the BattleMode method
        startingNum = transform.position.x;
    }

    // RETURNS HOW MUCH OF THE MAXIMUM MOVEMENT SPEED THE CAMERA SHOULD BE MOVING ACCORDING TO THE POSITION OF THE MOUSE (CAN ASSUME THAT IT MOUSE POSITION IS ALWAYS WITHIN BOUNDS)
    private float CalculateMouseMoveSpeed(float targetPosition)
    {
        float thresholdPosition = Mathf.Abs(targetPosition - mouseMoveThreshold); // Find where the threshold point actually is on the screen
        return Mathf.Abs(thresholdPosition - Input.mousePosition.x) / mouseMoveThreshold;
    }

    private void OnBuildingClick(Vector3 barrackPosition)
    {
        StartCoroutine(SmoothLerpPosition(barrackPosition));
        isFocusedOnBuilding = true;
    }

    // SMOOTHLY LERP THE CAMERA'S X POSITION TO TARGET X POSITION (STOPS WHEN CLOSE ENOUGH TO THE TARGET)
    private IEnumerator SmoothLerpPosition(Vector3 inputPosition)
    {
        targetPosition.x = inputPosition.x;
        while (Mathf.Abs(transform.position.x - targetPosition.x) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
            yield return null;
        }

        transform.position = targetPosition;
        print("SmoothLerpPosition is done!");
    }
}


// --------------------------------------------------------------------- SCRAPPED
// MAKE THE CAMERA FOLLOW THE PLAYER'S POSITION (BATTLE MODE)
//private void BattleMode()
//{
//    cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, battleModeFOV, zoomSpeed); // Lerp the camera's FOV to get a nice smooth zoom-in effect
//    // targetPosition.x = Lerp(startingNum, playerTransform.position.x, 1f);
//    targetPosition.x = Mathf.Lerp(transform.position.x, playerTransform.position.x, 0.1f);
//    targetPosition.y = playerTransform.position.y;
//    transform.position = targetPosition;
//}
