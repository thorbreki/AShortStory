using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private Vector3 playerPosition;

    private void Start()
    {
        playerPosition = new Vector3(0f, 0f, transform.position.z);
    }

    private void Update()
    {
        FollowPlayer();
    }

    // MAKE THE CAMERA FOLLOW THE PLAYER'S POSITION
    private void FollowPlayer()
    {
        playerPosition.x = playerTransform.position.x;
        playerPosition.y = playerTransform.position.y;
        transform.position = playerPosition;
    }
}
