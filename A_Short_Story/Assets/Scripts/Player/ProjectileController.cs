using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 0f;
    public float upgradeableSpeed = 1f;

    private Vector2 targetVector; // The target direction the projectile shot by the staff tries to travel in (mouse Position)
    private Vector2 imperfectionVector; // The vector that will be perpendicular to the target vector in order to create the inaccuracy factor
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
        targetVector = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position); // The target vector points towards the mouse position
        imperfectionVector.x = targetVector.normalized.y; // The imperfectionVector is the targetVector rotated clockwise by 90° by saying x = y
        imperfectionVector.y = -targetVector.normalized.x; // and y = -x

        float imperfection = Random.Range(0f, 2f); // How much the projectile will stray away from the mouse position
        int positiveOrNegative = Random.Range(0, 2); // 0 = projectile will stray counter-clockwise, 1 = projectile will stray clockwise

        if (positiveOrNegative == 0)
        {
            rigidBody.velocity = (targetVector.normalized * movementSpeed * upgradeableSpeed) + (imperfectionVector.normalized * imperfection * upgradeableSpeed);
        } else
        {
            rigidBody.velocity = (targetVector.normalized * movementSpeed * upgradeableSpeed) - (imperfectionVector.normalized * imperfection * upgradeableSpeed);
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
