using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int movementSpeed = 0; // The speed of the Player's movement
    private Vector2 movementVector = new Vector2(0, 0); // The vector of the Player's movement
    private Rigidbody2D rb; // The Player's rigidbody component

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        // The Player can only move when the Player Mode = Battle Mode
        if (!GameManager.instance.isArmyMode)
        {
            HandleMoveVector();
        } else
        {
            if (movementVector.x != 0)
                movementVector.x = 0;
        }

        HandlePlayerMode();
    }

    private void FixedUpdate()
    {
        rb.velocity = movementVector; // Actually move the character
    }

    // TAKE INPUT FROM THE PLAYER AND CREATE THE VECTOR THAT WILL BE THE MOVEMENT OF THE PLAYER
    private void HandleMoveVector()
    {
        if (Input.GetKey(KeyCode.A))
        {
            movementVector.x = -movementSpeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movementVector.x = movementSpeed;
        }
        else
        {
            movementVector.x = 0;
        }
    }

    private void HandlePlayerMode()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameManager.instance.FlipArmyMode();
        }
    }
}
