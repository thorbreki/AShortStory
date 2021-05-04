using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverChecker : MonoBehaviour
{
    [SerializeField] private CanvasController canvasController; // I need to know if there is a menu that has dropped down and to do that I'll talk directly to the canvas


    private void Update()
    {
        // DONT DO ANY CHECKS IF THERE ARE NO MENUS ACTIVE
        if (!canvasController.GetIsThereAnActiveMenu()) { GameManager.instance.SetPlayerIsHoveringUI(false); return; }


        // It will turn true if hovering over any UI elements
        GameManager.instance.SetPlayerIsHoveringUI(EventSystem.current.IsPointerOverGameObject());
    }
}
