using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    [SerializeField] private float verticalSensitivity;
    [SerializeField] private float horizontalSensitivity;
    [SerializeField] private Transform playerTransform;
    
    private float cameraDistance;

    private float axisY;
    private float axisX;

    private void Start() {
        axisX = 0;
        axisY = 0;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        axisX += Input.GetAxis("Mouse X") * Time.deltaTime * verticalSensitivity;
        axisY -= Input.GetAxis("Mouse Y") * Time.deltaTime * horizontalSensitivity;
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel") * -15;
        
        axisY = Mathf.Clamp(axisY, -85f, 85f);
        cameraDistance = Mathf.Clamp(cameraDistance + mouseWheel, 1, 60);
        
        transform.rotation = Quaternion.Euler(axisY, axisX, 0f);
        transform.position = playerTransform.position - (transform.rotation * Vector3.forward * cameraDistance);
    }
}
