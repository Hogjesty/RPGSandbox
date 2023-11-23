using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    
    public event EventHandler OnColliderEnter;
    public event EventHandler OnColliderExit;

    private void OnCollisionEnter(Collision collision) {
        OnColliderEnter?.Invoke(this, EventArgs.Empty);
    }
    private void OnCollisionExit(Collision collision) {
        OnColliderExit?.Invoke(this, EventArgs.Empty);
    }
}
