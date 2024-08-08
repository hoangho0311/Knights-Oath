using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarCanvas : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        // Lấy Transform của camera
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Quay canvas theo hướng camera
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);
    }
}
