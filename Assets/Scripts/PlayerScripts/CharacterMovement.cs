using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour {
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float gravitySpeed;
    
    [SerializeField] private Transform camera;
    [SerializeField] private Transform groundPoint;
    [SerializeField] private Image teleportImage;
    
    [SerializeField] private Transform hitPoint;
    [SerializeField] private GameObject damageNumber;
    
    private Rigidbody rigidbody;
    private Animator playerHitAnimator;
    
    private bool onGround;
    private bool canTeleport;
    
    private float verticalRotate;
    private float gravity = -10;

    private bool canHit = true;
    
    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        playerHitAnimator = GetComponent<Animator>();
        canTeleport = true;
    }

    private void Update() {
        Move();
        Hit();
    }

    private void Move() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        float moveAmount = speed * Time.deltaTime;
        float moveForward = (new Vector3(horizontal, 0, vertical).normalized * moveAmount).magnitude;

        bool isPlayerMoving = moveForward > 0;
        
        if (isPlayerMoving) {
            float rotationY = camera.rotation.eulerAngles.y + Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotationY, 0), rotationSpeed);
        }

        bool isPlayerOnGround = Physics.OverlapSphere(groundPoint.position, 0.3f)
            .Where(x => x.gameObject.CompareTag("Ground"))
            .ToList()
            .Count > 0;
        
        if (Input.GetKey(KeyCode.Space) && isPlayerOnGround) {
            gravity = 30;
        }

        gravity -= Time.deltaTime * gravitySpeed;
        gravity = Mathf.Clamp(gravity, -10, 100);
        
        rigidbody.velocity = transform.TransformDirection(0, gravity, moveForward);
    }

    private void Hit() {
        if (Input.GetKey(KeyCode.Mouse0) && canHit) {
            StartCoroutine(AnimateHit());
        }
    }

    private IEnumerator AnimateHit() {
        canHit = false;
        playerHitAnimator.SetTrigger("playerHit");
        yield return new WaitForSeconds(0.1f);
        List<Collider> colliders = Physics.OverlapSphere(hitPoint.position, 2f)
            .Where(x => x.gameObject.CompareTag("Enemy"))
            .ToList();

        List<GameObject> damageNumbers = new List<GameObject>();
        foreach (Collider collider in colliders) {
            Vector3 point = collider.gameObject.transform.position;
            GameObject damageNumberObj = Instantiate(damageNumber, new Vector3(point.x + 2, point.y, point.z + 2), Quaternion.identity);
            damageNumberObj.GetComponent<DamageNumber>()?.SetCameraPoint(camera.position);
            damageNumberObj.GetComponentInChildren<TextMeshProUGUI>().text = UnityEngine.Random.Range(10, 20).ToString();
            damageNumbers.Add(damageNumberObj);
        }
        canHit = true;
        yield return new WaitForSeconds(0.3f);
        damageNumbers.ForEach(Destroy);
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
    
    private void OnDrawGizmos() {
        // Gizmos.color = Color.red;
        // Vector3 dir = (transform.position - camera.position).normalized;
        // Vector3 playerRotationPoint = transform.position + dir * 20;
        //
        // Gizmos.DrawLine(camera.position, transform.position);
        // Gizmos.DrawLine(transform.position, new Vector3(playerRotationPoint.x, transform.position.y, playerRotationPoint.z));
    }
}