using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int movementSpeed = 0;
    private Vector2 movementVector = new Vector2(0, 0);
    private Rigidbody2D rb;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
    }

    private void FixedUpdate()
    {
        rb.velocity = movementVector * Time.fixedDeltaTime;
    }

    private void Move()
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
}
