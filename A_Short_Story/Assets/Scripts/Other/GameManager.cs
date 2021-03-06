using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // THE INSTANCE
    public static GameManager instance; // The singleton GameManager instance

    // PUBLIC VARIABLES
    public bool isArmyMode = false; // This is the variable that chooses what Player Mode is ongoing: false = Battle Mode, true = Army Mode

    private void Awake()
    {
        instance = this;
    }


    // -------------------------------------------------------------------
    // S E T T E R S

    // FLIPS THE ARMY MODE OF THE MAIN CAMERA'S CAMERACONTROLLER COMPONENT
    public void FlipArmyMode()
    {
        isArmyMode = !isArmyMode;
    }
}
