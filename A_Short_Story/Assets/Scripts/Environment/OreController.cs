using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreController : MonoBehaviour
{
    private Transform nearestSmithyTransform;
    private float minDistance = float.MaxValue;

    private void OnMouseDown()
    {
        if (GameManager.instance.GetPlayerMode() == Constants.PlayerMode.Army)
        {
            EventManager.RaiseOnOreClicked(transform, this); // Let selected soldiers know that they should come to me and mine diiiaammmmoooooonnnndddsss
            UpdateNearestSmithyTransform(); // Update what smithy is closest to me
        }
    }

    /// <summary>
    /// Call upon all active Smithies to figure out which one is closest to me
    /// </summary>
    private void UpdateNearestSmithyTransform()
    {
        EventManager.RaiseOnFindNearestSmithy(this);
    }

    /// <summary>
    /// This is the function that Smithies call to sign up in the nearest Smithy competition
    /// </summary>
    /// <param name="buildingTransform"></param>
    public void HandleNearestSmithyCandidate(Transform buildingTransform)
    {
        if (nearestSmithyTransform == null)
        {
            nearestSmithyTransform = buildingTransform;
            minDistance = Vector2.Distance(transform.position, nearestSmithyTransform.position);
            return;
        }

        float distance = Vector2.Distance(transform.position, buildingTransform.position); // Calculate the distance from me to the smithy
        
        if (distance < minDistance)
        {
            nearestSmithyTransform = buildingTransform;
            minDistance = distance;
        }

    }


    public Transform getNearestSmithyTransform()
    {
        return nearestSmithyTransform;
    }
}
