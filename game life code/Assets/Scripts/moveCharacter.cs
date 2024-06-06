using UnityEngine;

public class moveCharacter : MonoBehaviour
{
    [SerializeField] private byte movementSpeed;
    private float speedPref = 1.0f;
    [SerializeField] private int turnSpeed;
    private float turnSpeedPref = 1.0f;
    [SerializeField] private byte turnLimit;
    private Rigidbody _rb;

    private void Start() {
        _rb = GetComponent<Rigidbody>();
        speedPref = PlayerPrefs.GetFloat("speed");
        turnSpeedPref = PlayerPrefs.GetFloat("crs");
    }

    private void Update() {
        Move();
        Rotate();
    }

    public void Move() {
        transform.position += (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * movementSpeed * speedPref * Time.deltaTime;
    }

    public void Rotate() {
        float turnSpeedModifier = turnSpeed * turnSpeedPref * Time.deltaTime;
        float X = -Input.GetAxis("Mouse Y") * turnSpeedModifier;
        float Y = Input.GetAxis("Mouse X") * turnSpeedModifier;

        float X_current_rotation = transform.localEulerAngles.x;
        float X_rotate = X_current_rotation + X;
        if (X_rotate > turnLimit && X_rotate <= 180) {
            if (X_current_rotation < turnLimit) transform.Rotate(turnLimit - X_current_rotation, 0, 0, Space.Self);
        }
        else if (X_rotate < 360 - turnLimit && X_rotate > 180) {
            if (X_current_rotation > 360 - turnLimit) transform.Rotate(360 - turnLimit - X_current_rotation, 0, 0, Space.Self);
        }
        else transform.Rotate(X, 0, 0, Space.Self);
        transform.Rotate(0, Y, 0, Space.World);
    }
}