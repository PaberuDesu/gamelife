using UnityEngine;

public class moveCharacter : MonoBehaviour
{
    public byte movementSpeed = 60;
    public byte turnSpeed = 100;
    public byte turnLimit = 60;
    Rigidbody _rb;

    private void Start() {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        Move();
        Rotate();
    }

    public void Move()
    {
        Vector3 movement = (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * movementSpeed * Time.deltaTime;

        transform.position += movement;
    }

    public void Rotate()
    {
        float X = Input.GetAxis("Mouse Y") * turnSpeed * Time.deltaTime;
        float Y = Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime;

        float X_current_rotation = transform.localEulerAngles.x;
        float X_rotate = X_current_rotation + X;
        if (X_rotate > turnLimit && X_rotate <= 180)
        {
            if (X_current_rotation < turnLimit)
                transform.Rotate(turnLimit - X_current_rotation, 0, 0, Space.Self);
        }
        else if (X_rotate < 360 - turnLimit && X_rotate > 180)
        {
            if (X_current_rotation > 360 - turnLimit)
                transform.Rotate(360 - turnLimit - X_current_rotation, 0, 0, Space.Self);
        }
        else
            transform.Rotate(X, 0, 0, Space.Self);
        transform.Rotate(0, Y, 0, Space.World);
    }
}