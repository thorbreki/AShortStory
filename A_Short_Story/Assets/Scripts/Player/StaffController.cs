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
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Instantiate(projectile, staffLight.position, transform.rotation);
    }
}
