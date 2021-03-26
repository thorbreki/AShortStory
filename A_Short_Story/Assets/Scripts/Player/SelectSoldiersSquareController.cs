using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSoldiersSquareController : MonoBehaviour
{
    private Vector2 cursorPosition; // The world position of the cursor
    private Vector2 targetScale = Vector2.zero; // The scale the square should be

    // Update is called once per frame
    private void Update()
    {
        ResizeToCursor(); // Resize baby

        if (Input.GetMouseButtonUp(0))
        {
            Die();
        }
    }

    private void ResizeToCursor()
    {
        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get the world coordinates of the cursor
        targetScale.x = cursorPosition.x - transform.position.x; // The x-scale of the square should be the diff between self's x pos and cursor's x pos
        targetScale.y = cursorPosition.y - transform.position.y; // The y-scale of the square should be the diff between self's y pos and cursor's y pos
        transform.localScale = targetScale; // Set the scale
    }

    private void Die()
    {
        GameManager.instance.SetSelectSoldierSquareShouldSpawn(false);
        Destroy(gameObject);
    }
}
