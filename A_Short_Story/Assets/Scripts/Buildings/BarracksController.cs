using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarracksController : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.instance.isArmyMode)
        {
            EventManager.RaiseOnBarrackClick(transform.position);
        }
    }
}
