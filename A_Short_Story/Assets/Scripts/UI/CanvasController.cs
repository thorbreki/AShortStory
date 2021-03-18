using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObject barracksMenu; // The menu that drops down when Player clicks on a Barrack

    private void Start()
    {
        barracksMenu.SetActive(false); // Make sure that the Barracks Menu is inactive when game starts

        EventManager.onBarrackClick += OnBarrackClick; // Listen to when Player clicks on the Barrack
    }

    // WHEN PLAYER CLICKS ON BARRACK THIS FUNCTION RUNS, ACTIVATES THE BARRACKS MENU
    private void OnBarrackClick(Vector3 barrackPosition)
    {
        barracksMenu.SetActive(true);
    }
}
