using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectScript : MonoBehaviour
{
    private void OnMouseUp()
    {
        EventManager.RaiseOnSelected(); // Let the world know that the background was selected and therefore everything else should be deselected
    }
}
