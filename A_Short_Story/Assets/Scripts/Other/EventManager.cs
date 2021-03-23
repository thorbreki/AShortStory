using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void OnBarrackClick(Vector3 barrackPosition);
    public static event OnBarrackClick onBarrackClick; // When a barrack is clicked on by the Player

    public delegate void OnSelect();
    public static event OnSelect onSelect; // When the Player selectes/left-clicks any object in the scene

    public delegate void OnMove(float targetX);
    public static event OnMove onMove;

    /// <summary>This event should be raised when the Player clicks on a Barrack</summary>
    public static void RaiseOnBarrackClick(Vector3 barrackPosition)
    {
        if (onBarrackClick != null)
        {
            onBarrackClick(barrackPosition);
        }
    }

    /// <summary>This event should be raised when the Player selects anything whatsoever</summary>
    public static void RaiseOnSelected()
    {
        if (onSelect != null)
        {
            onSelect();
        }
    }

    public static void RaiseOnMove(float targetX)
    {
        if (onMove != null)
        {
            onMove(targetX);
        }
    }
}
