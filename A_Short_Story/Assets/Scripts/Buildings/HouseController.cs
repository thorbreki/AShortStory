using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseController : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private GameObject parentHealthBarObject; // To make the health bar appear and dissapear

    [Header("Components")]
    [SerializeField] private Transform healthBarTransform; // To scale the health bar

    [Header("Attributes")]
    [SerializeField] private int maxHealth;

    private int health;

    private Vector3 scaleVector;

    private void Start()
    {
        health = maxHealth;
        scaleVector = Vector3.one;
        parentHealthBarObject.SetActive(false);
    }


    /// <summary>
    /// Damage me with the given amount
    /// </summary>
    /// <param name="amount"></param>
    public void DamageMe(int amount)
    {
        health = Mathf.Max(0, health -amount); // Decrease my health

        // Scale the healthBar
        scaleVector.x = health / maxHealth;
        healthBarTransform.localScale = scaleVector;

        // Handle dying
        if (health <= 0)
        {
            Destroy(gameObject); // Die
        }
    }
}
