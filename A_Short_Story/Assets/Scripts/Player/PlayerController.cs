using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject selectSoldiersSquareObject; // The square that the player can use to select many soldiers
    [SerializeField] private GameObject playerAttackEffect; // The effect that spawns when player damages enemy

    [Header("Variables")]
    [SerializeField] private int movementSpeed = 0; // The speed of the Player's movement
    [SerializeField] private float powerIncreaseSpeed = 0.05f; // The speed of how much power the player gains

    private Vector3 effectVector = Vector3.zero; // The vector that I use to spawn in effects nicely without using the new keyword too often


    private void Start()
    {
        // Listen to events
        EventManager.onEnemyDamagedByPlayer += OnEnemyDamagedByPlayer;

        // Set up all relevant variables
        effectVector.z = -1;
    }

    private void OnDestroy()
    {
        EventManager.onEnemyDamagedByPlayer -= OnEnemyDamagedByPlayer;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army)
        {
            HandleMoveSoldiers(); // Handle the player wanting to move soldiers to specific positions
            HandleSelectSoldiersSquareSpawn(); // Handle the player being able to select multiple soldiers at a time with the square select method
        } else if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Battle)
        {
            HandleChangePlayerMode(); // Handle the player being able to change the player mode of the game
        }

        HandlePowerUpdate(); // update the player's power value
        HandleChangePlayerMode(); // Handle the player wanting to change the player mode of the game
    }

    /// <summary>
    /// When the Player right-clicks on the screen, all selected soldiers move to that position (on the ground of course)
    /// </summary>
    private void HandleMoveSoldiers()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (GameManager.instance.GetOnMoveShouldBeRaised())
            {
                EventManager.RaiseOnMove(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
                print("Raised the move event!");
            } else
            {
                GameManager.instance.SetOnMoveShouldBeRaised(true); // Set it otherwise to true since that should be the default value
            }
        }
    }

    /// <summary>
    /// This function handles the livelyhood of the select soldier square by spawning it only when appropriate
    /// </summary>
    private void HandleSelectSoldiersSquareSpawn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GameManager.instance.GetSelectSoldierSquareShouldSpawn())
            {
                Vector3 squarePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                squarePosition.z = -1f;
                Instantiate(selectSoldiersSquareObject, squarePosition, Quaternion.identity);
            }
        }
    }


    /// <summary>
    /// This function handles changing between player modes when the player wants to
    /// </summary>
    private void HandleChangePlayerMode()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameManager.instance.SetPlayerMode(Constants.PlayerMode.Army);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameManager.instance.SetPlayerMode(Constants.PlayerMode.Battle);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameManager.instance.SetPlayerMode(Constants.PlayerMode.Building);
        }
    }


    /// <summary>
    /// This function handles the continuous growth of the player's power throughout the game
    /// </summary>
    private void HandlePowerUpdate()
    {
        if (GameManager.instance.GetPlayerCurrPower() == GameManager.instance.GetPlayerMaxPower()) { return; } // Don't update the power value if it is already full

        GameManager.instance.IncreasePlayerPower(powerIncreaseSpeed * Time.deltaTime); // Increase power level

        if (GameManager.instance.GetPlayerCurrPower() > GameManager.instance.GetPlayerMaxPower())
        {
            GameManager.instance.SetPlayerPower(GameManager.instance.GetPlayerMaxPower());
        }
    }


    /// <summary>
    /// When the player actually damages an enemy
    /// </summary>
    private void OnEnemyDamagedByPlayer()
    {
        GameManager.instance.ReducePlayerPower(GameManager.instance.GetPlayerPowerReduction()); // Reduce the power of the player since this was an attack from the player
        GameObject attackEffect = Instantiate(playerAttackEffect);
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = -1;

        attackEffect.transform.position = mousePosition;
    }
}
