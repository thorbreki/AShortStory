using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static IEnumerator SmoothLerpPosition(Transform inputTransform, Vector3 targetPosition, float speed, float threshold)
    {
        while (Vector2.Distance(inputTransform.position, targetPosition) > threshold)
        {
            inputTransform.position = Vector3.Lerp(inputTransform.position, targetPosition, speed);
            yield return null;
        }
        inputTransform.position = targetPosition;
        Debug.Log("SmoothLerpPosition is done!");
    }
}
