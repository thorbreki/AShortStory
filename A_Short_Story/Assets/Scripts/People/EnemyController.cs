using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float health; // My health points

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GameManager.instance.SetOnMoveShouldBeRaised(false); // The player should not raise the move event when soldier is attacking!
            EventManager.RaiseOnAttack(transform); // Let selected soldiers that the god wants them to attack me!
        }
    }

    /// <summary>
    /// Get this enemy's current health points
    /// </summary>
    /// <returns></returns>
    public float GetHealth()
    {
        return health;
    }

    public void DamageMe (float damage)
    {
        health -= damage;
        HandleDeath(); // Check if I am dead and act appropriately if I am
    }

    /// <summary>
    /// This function checks whether I am dead and acts appropriately when I am
    /// </summary>
    private void HandleDeath()
    {
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
