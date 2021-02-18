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

        // Shoot onwards!
        Move();

        // Start dying
        StartCoroutine(DieCountdown());
    }

    // MOVE TOWARDS THE CURSOR BUT WITH IMPERFECT ACCURACY
    private void Move()
    {
        targetDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        rigidBody.velocity = targetDirection.normalized * movementSpeed;
        float imperfection = Random.Range(0, 10);
        int positiveOrNegative = Random.Range(0, 2);
        if (positiveOrNegative == 0)
        {
            // transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -imperfection));
            rigidBody.SetRotation(Quaternion.Euler(new Vector3(0f, 0f, -imperfection)));
        }
        else
        {
            // transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, imperfection));
            rigidBody.SetRotation(Quaternion.Euler(new Vector3(0f, 0f, imperfection)));
        }
    }

    private IEnumerator DieCountdown()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
