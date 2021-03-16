using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void OnBarrackClick(Vector3 barrackPosition);
    public static event OnBarrackClick onBarrackClick;

    public static void RaiseOnBarrackClick(Vector3 barrackPosition)
    {
        if (onBarrackClick != null)
        {
            onBarrackClick(barrackPosition);
        }
    }
}
