using UnityEngine;

public class test : MonoBehaviour {

    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform floor;
    [SerializeField] private Transform target;

    private void Update() {

        // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * rotationSpeed);

        // transform.rotation = Quaternion.RotateTowards(transform.rotation, floor.rotation,
        //     Time.deltaTime * rotationSpeed);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, target.position);
        
        print("Magnitude: " + (target.position - transform.position).magnitude);
        print("Normalize: " + (target.position - transform.position).normalized);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position,(target.position - transform.position).normalized* (target.position - transform.position).magnitude);
    }
}