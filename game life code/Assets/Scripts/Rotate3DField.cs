using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    private float distance = 0.0f;
    private const float Speed = 6000.0f;
    
    private float x = 0.0f;
    private float y = 0.0f;

    private void OnEnable() {
        transform.localPosition = Vector3.one * 2;
        transform.LookAt(target.transform.position);
        Vector3 angles = this.transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        distance = Vector3.Distance(this.transform.position, target.transform.position);
    }

    private void Update() {
        if (Input.GetMouseButton(1)) {
            x += Input.GetAxis("Mouse X") * Speed  * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * Speed  * Time.deltaTime;
        }
        UpdateCamera();
    }

    private void UpdateCamera() {
        Quaternion rotation = Quaternion.Euler(y, x, 0);

        transform.position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
        transform.rotation = rotation;
    }
}