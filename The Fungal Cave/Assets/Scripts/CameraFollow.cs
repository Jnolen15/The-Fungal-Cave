using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.1f;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    private float cameraHieght = 16f;
    public float currentLevel = 0f;
    public float playerLevel = 0f;

    public void LateUpdate()
    {
        if (target.gameObject.GetComponent<PlayerController>().startedDialogue)
        {
            if (transform.position.y > 8)
            {
                // Smooth camera follow
                Vector3 pos = transform.position + offset;
                Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothSpeed);
                smoothedPos.x = 0;
                transform.position = smoothedPos;
            }
        }
        else
        {
            // Jump king like camera movement
            playerLevel = (target.position.y - (target.position.y % 16)) / 16;

            // Move up
            if (target.position.y > ((cameraHieght * currentLevel)) && currentLevel != playerLevel)
            {
                Vector3 newPos = new Vector3(transform.position.x, transform.position.y + cameraHieght, transform.position.z);
                transform.position = newPos;
                currentLevel += 1;
            }

            // Move down
            if (target.position.y < ((cameraHieght * currentLevel)) && currentLevel != playerLevel)
            {
                Vector3 newPos = new Vector3(transform.position.x, transform.position.y - cameraHieght, transform.position.z);
                transform.position = newPos;
                currentLevel -= 1;
            }
        }        
    }
}
