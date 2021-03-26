using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private int movementSpeed = 0; // The speed of the Player's movement
    [SerializeField] private GameObject selectSoldiersSquareObject; // The square that the player can use to select many soldiers
    private Vector2 movementVector = new Vector2(0, 0); // The vector of the Player's movement

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army)
        {
            HandleMoveSoldiers(); // Handle the player wanting to move soldiers to specific positions
            HandleSelectSoldiersSquareSpawn();
        }
        //// The Player can only move when the Player Mode = Battle Mode
        //else if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Battle)
        //{
        //    HandleMoveVector();
        //    transform.Translate(movementVector * Time.deltaTime);
        //}
    }

    /// <summary>
    /// When the Player right-clicks on the screen, all selected soldiers move to that position (on the ground of course)
    /// </summary>
    private void HandleMoveSoldiers()
    {
        if (Input.GetMouseButtonDown(1))
        {
            EventManager.RaiseOnMove(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
            print("Raised the move event!");
        }
    }

    /// <summary>
    /// This function handles the livelyhood of the select soldier square by spawning it only when appropriate
    /// </summary>
    private void HandleSelectSoldiersSquareSpawn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            print("PLAYERCONTROLLER!");
            if (GameManager.instance.GetSelectSoldierSquareShouldSpawn())
            {
                Vector3 squarePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                squarePosition.z = -1f;
                Instantiate(selectSoldiersSquareObject, squarePosition, Quaternion.identity);
            }
        }
    }

    // TAKE INPUT FROM THE PLAYER AND CREATE THE VECTOR THAT WILL BE THE MOVEMENT OF THE PLAYER
    //private void HandleMoveVector()
    //{
    //    if (Input.GetKey(KeyCode.A))
    //    {
    //        movementVector.x = -movementSpeed;
    //    }
    //    else if (Input.GetKey(KeyCode.D))
    //    {
    //        movementVector.x = movementSpeed;
    //    }
    //    else
    //    {
    //        movementVector.x = 0;
    //    }
    //}


    // ------------------------------------------------------------------- SCRAPPED
    //private void HandlePlayerMode()
    //{
    //    if (Input.GetKeyDown(KeyCode.Tab))
    //    {
    //        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Battle)
    //        {
    //            GameManager.instance.SetPlayerMode(Constants.PlayerMode.Army);
    //            movementVector.x = 0;
    //        }
    //        else
    //            GameManager.instance.SetPlayerMode(Constants.PlayerMode.Battle);
    //    }
    //}
}
