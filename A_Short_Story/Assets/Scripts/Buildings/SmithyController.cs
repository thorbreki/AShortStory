using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmithyController : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.instance.isArmyMode)
        {
            print("THE PLAYER CLICKED ON THE SMITHY!");
        }
    }
}
