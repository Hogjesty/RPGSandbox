using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour {
    [SerializeField] private float speed;
    [SerializeField] private Transform camera;
    [SerializeField] private float sensitivityY;
    [SerializeField] private float sensitivityX;
    [SerializeField] private Image teleportImage;
    
    private Rigidbody rigidbody;
    private CameraScript cameraScript;
    private bool onGround;
    private bool canTeleport;
    

    private float verticalRotate;
    private float cameraZoom = 30;
    
    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        cameraScript = camera.gameObject.GetComponent<CameraScript>();
        canTeleport = true;
    }

    private void Start() {
        cameraScript.OnColliderEnter += (sender, args) => {
            Debug.Log("CAMERA ENTER");
        };
    }

    private void Update() {
        Move();
        CameraHandler();
    }

    private void Move() {
        
        float normalizedSpeed = speed * Time.deltaTime;
        float horizontal = Input.GetAxis("Horizontal") * normalizedSpeed;
        float vertical = Input.GetAxis("Vertical") * normalizedSpeed;
        
        float gravity = onGround ? 0 : -10;
        if (Input.GetKey(KeyCode.Space) && onGround) {
            gravity = 10;
        }

        rigidbody.velocity = transform.TransformDirection(new Vector3(horizontal, gravity, vertical));
    }

    private void CameraHandler() {
        float axisX = Input.GetAxis("Mouse X");
        float axisY = Input.GetAxis("Mouse Y");

        float mouseWheel = Input.GetAxis("Mouse ScrollWheel") * -15;
        cameraZoom = Mathf.Clamp(cameraZoom + mouseWheel, 20, 60);

        camera.Translate(Vector3.up * (-axisY * sensitivityY));
        camera.Translate(Vector3.right * (-axisX * sensitivityX));
        
        Vector3 pointY = new Vector3(transform.position.x, camera.position.y, transform.position.z);
        float distance = Vector3.Distance(pointY, camera.position);
        if (distance < 15) {
            camera.Translate(Vector3.up * (axisY * sensitivityY));
        }

        Vector3 dir = (transform.position - camera.position).normalized;
        camera.position = transform.position - dir * cameraZoom;

        camera.LookAt(transform);
        Vector3 inFrontOfPlayerPoint = transform.position + dir * 20;
        Vector3 playerRotationPoint = new Vector3(inFrontOfPlayerPoint.x, transform.position.y, inFrontOfPlayerPoint.z);
        transform.LookAt(playerRotationPoint);
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