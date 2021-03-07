using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    private Animator animator; // The animator that is attached to the player object's sprite

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Only execute the walk animation if the Player Mode = Battle Mode
        if (!GameManager.instance.isArmyMode)
        {
            HandleWalkAnimation();
        }
    }

    // HANDLES THE PLAYER'S WALKING ANIMATION, MANIPULATES THE isWalking BOOL IN THE ANIMATOR
    private void HandleWalkAnimation()
    {
        // only change the isWalking bool if it is false, not when it is true already
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            animator.SetBool("isWalking", true);
        else
            animator.SetBool("isWalking", false);
    }
}
