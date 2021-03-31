using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            EventManager.RaiseOnAttack(transform); // Let selected soldiers that the god wants them to attack me!
        }
    }
}
