using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 centerOffset = new Vector3(60f, 280f, 0f);

    [SerializeField]
    private float cameraHeight = 20.0f;

    private Transform cameraTransform;

    private bool isFollowing;
    void LateUpdate()
    {
        if (cameraTransform == null && isFollowing)
        {
            FollowPlayer();
        }

        if (isFollowing)
        {
            positioningCamera();
        }
    }

    public void FollowPlayer()
    {
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        positioningCamera();
    }

    void positioningCamera()
    {
        Vector3 targetCenter = transform.position + centerOffset;
        float cameraTargetHeight = transform.position.y + cameraHeight;
        // set the position of the camera
        cameraTransform.position = targetCenter;
        // Set the height of the camera
        cameraTransform.position = new Vector3(cameraTransform.position.x, cameraTargetHeight, cameraTransform.position.z);
    }
}
