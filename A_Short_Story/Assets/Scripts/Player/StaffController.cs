using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffController : MonoBehaviour
{
    [SerializeField] private GameObject projectile; // The projectile game object
    [SerializeField] private Transform staffLight; // The position of the light and energy source of the staff
    

    // Update is called once per frame
    private void Update()
    {
        if (!GameManager.instance.isArmyMode)
        {
            HandleShooting();
        }
    }


    // WHEN PLAYER PRESSES THE LEFT MOUSE BUTTON, THEN SPAWN A PROJECTILE
    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(projectile, staffLight.position, transform.rotation);
        }
    }
}
