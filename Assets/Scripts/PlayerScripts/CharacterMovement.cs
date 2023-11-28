using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour {
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform camera;
    [SerializeField] private float sensitivityY;
    [SerializeField] private float sensitivityX;
    [SerializeField] private Image teleportImage;
    
    private Rigidbody rigidbody;
    private bool onGround;
    private bool canTeleport;
    

    private float verticalRotate;
    private float cameraZoom = 30;
    
    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        canTeleport = true;
    }

    private void Update() {
        Move();
    }

    private void Move() {
        float normalizedSpeed = speed * Time.deltaTime;
        float horizontal = Input.GetAxis("Horizontal") * normalizedSpeed;
        float vertical = Input.GetAxis("Vertical") * normalizedSpeed;

        bool isPlayerMoving = horizontal != 0 || vertical != 0;
        
        if (isPlayerMoving) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, camera.rotation.eulerAngles.y, 0), rotationSpeed);
        }
        
        float gravity = onGround ? 0 : -10;
        if (Input.GetKey(KeyCode.Space) && onGround) {
            gravity = 10;
        }

        rigidbody.velocity = transform.TransformDirection(new Vector3(horizontal, gravity, vertical));
    }
    
    private void OnDrawGizmos() {
        // Gizmos.color = Color.red;
        // Vector3 dir = (transform.position - camera.position).normalized;
        // Vector3 playerRotationPoint = transform.position + dir * 20;
        //
        // Gizmos.DrawLine(camera.position, transform.position);
        // Gizmos.DrawLine(transform.position, new Vector3(playerRotationPoint.x, transform.position.y, playerRotationPoint.z));
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Portal") && canTeleport) {
            canTeleport = false;
            StartCoroutine(AnimateTeleportation(5f, 10f, other));
        }
    }

    private IEnumerator AnimateTeleportation(float coolDown, float duration, Collider other) {
        float halfDuration = duration / 2;

        float tempSpeed = speed;
        speed = 0f;

        teleportImage.enabled = true;
        WaitForSeconds waitForSeconds = new WaitForSeconds(halfDuration / 100);
        Color tempColor = teleportImage.color;
        tempColor.a = 0;
        for (int i = 0; i < 100; i++) {
            tempColor.a += 0.01f;
            teleportImage.color = tempColor;
            yield return waitForSeconds;
        }
        transform.position = other.gameObject.GetComponent<Portal>().destinationPortal.position;
        speed = tempSpeed;
        for (int i = 100; i > 0; i--) {
            tempColor.a -= 0.01f;
            teleportImage.color = tempColor;
            yield return waitForSeconds;
        }
        teleportImage.enabled = false;
        yield return new WaitForSeconds(coolDown);
        canTeleport = true;
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("ENTER");
        if (collision.gameObject.CompareTag("Ground")) {
            onGround = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        Debug.Log("EXIT");
        if (other.gameObject.CompareTag("Ground")) {
            onGround = false;
        }
    }
}