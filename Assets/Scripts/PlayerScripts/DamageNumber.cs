using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumber : MonoBehaviour {

    private Vector3 cameraPoint = Vector3.zero;
    private void Update() {
        transform.LookAt(cameraPoint);
    }

    public void SetCameraPoint(Vector3 cameraPoint) {
        this.cameraPoint = cameraPoint;
    }
}