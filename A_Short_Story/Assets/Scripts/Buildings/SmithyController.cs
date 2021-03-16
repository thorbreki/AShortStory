using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmithyController : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army)
        {
            print("THE PLAYER CLICKED ON THE SMITHY!");
        }
    }
}
