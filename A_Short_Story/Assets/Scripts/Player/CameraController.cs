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

    // CAMERA VARIABLES
    [SerializeField] private float maximumMovementSpeed = 0f; // How fast the Camera can move when game is in Army Mode
    [SerializeField] private float mouseMoveThreshold = 0f; // How close to the edges of the screen does the cursor need to be in order for the camera to start move when game is in Army Mode

    private Camera cameraComponent; // The camera component of the camera
    private float screenWidth; // The width of the screen (in pixel coordinates)


    private void Start()
    {
        playerPosition = new Vector3(0f, 0f, transform.position.z); // Set the starting position of the camera
        cameraComponent = GetComponent<Camera>(); // Get the camera component of the camera
        screenWidth = Screen.width; // Set the screen width in the beginning to not have to do it again (might want to rethink that later on)
    }

    private void Update()
    {
        if (GameManager.instance.isArmyMode)
        {
            ArmyMode();
        } else
        {
            BattleMode();
        }
    }

    // MAKE THE CAMERA FOLLOW THE PLAYER'S POSITION (BATTLE MODE)
    private void BattleMode()
    {
        cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, battleModeFOV, zoomSpeed);
        playerPosition.x = playerTransform.position.x;
        playerPosition.y = playerTransform.position.y;
        transform.position = playerPosition;
    }

    // TODO: CHANGE THE MOUSE MOVEMENT OF THE CAMERA IN ARMY MODE BY SPLITTING THE SCREEN UP INTO N DIFFERENT SAME-SIZE PARTS AND USE THE WIDTH OF 1 OF THOSE PARTS TO DETERMINE IF THE MOUSE IS WITHIN THE THRESHOLD
    // THAT WAY NOT HAVING TO DEAL WITH DIFFERENT SCREEN SIZES, WILL ALWAYS BE RELATIVE TO HOW BIG THE SCREEN IS

    // MAKE CAMERA FUNCTION ACCORDINGLY WHEN PLAYER IS IN ARMY MODE (ARMY MODE)
    private void ArmyMode()
    {
        cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, armyModeFOV, zoomSpeed); // Zoom out

        if (Input.GetKey(KeyCode.A)) // When the player presses A the camera should pan to the left
        {
            transform.Translate(-maximumMovementSpeed * Time.deltaTime, 0f, 0f);
        } else if (Input.GetKey(KeyCode.D)) // When the player presses D the camera should pan to the right
        {
            transform.Translate(maximumMovementSpeed * Time.deltaTime, 0f, 0f);
        }

        if (Input.mousePosition.x < mouseMoveThreshold) // Checking if the mouse is close enough to the left edge of the screen
        {
            transform.Translate(-maximumMovementSpeed * CalculateMouseMoveSpeed(0f) * Time.deltaTime, 0f, 0f);
        } else if (Input.mousePosition.x > (Screen.width - mouseMoveThreshold))
        {
            transform.Translate(maximumMovementSpeed * CalculateMouseMoveSpeed(Screen.width) * Time.deltaTime, 0f, 0f);
        }
    }

    // RETURNS HOW MUCH OF THE MAXIMUM MOVEMENT SPEED THE CAMERA SHOULD BE MOVING ACCORDING TO THE POSITION OF THE MOUSE (CAN ASSUME THAT IT MOUSE POSITION IS ALWAYS WITHIN BOUNDS)
    private float CalculateMouseMoveSpeed(float targetPosition)
    {
        float thresholdPosition = Mathf.Abs(targetPosition - mouseMoveThreshold); // Find where the threshold point actually is on the screen
        return Mathf.Abs(thresholdPosition - Input.mousePosition.x) / mouseMoveThreshold;
    }
}
