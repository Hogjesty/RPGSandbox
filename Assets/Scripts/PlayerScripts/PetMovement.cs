using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetMovement : MonoBehaviour {
    
    [SerializeField] private Transform master;
    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, master.position, 10 * Time.deltaTime);
    }
    
    private void OnTriggerEnter(Collider other) {
        Debug.Log("Game over");
    }
}