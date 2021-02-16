using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 0f;

    private Vector2 targetDirection; // The target direction the projectile shot by the staff tries to travel in (mouse Position)
    private Rigidbody2D rigidBody; // the rigid body component of the projectile

    private void Start()
    {
        // Assigning all the necessary stuff
        rigidBody = GetComponent<Rigidbody2D>();

        // Doing some calculations
        targetDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        rigidBody.velocity = targetDirection.normalized * movementSpeed;
    }

    // THE PROJECTILE SHOULD DIE WHEN IT HAS BEEN ALIVE FOR LET'S SAY 10 SECONDS
}
