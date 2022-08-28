using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        turn.x = Camera.rotation.eulerAngles.y;
        turn.y = -Camera.rotation.eulerAngles.x;
        cameraPosition = Camera.localPosition;
    }

    public Transform Camera;
    public Vector3 cameraPosition;
    public Vector3 turn;
    public float altitude;

    public float forwardSensivity = 0.2f;
    public float rightSensivity = 0.2f;
    public float upSensivity = 50f;

    // Update is called once per frame
    void Update()
    {
        lockMouse();
        rotateCamera();
        moveCamera();
    }

    // Rotate camera around own axis if right mouse button is down
    void rotateCamera()
    {
        if (Input.GetAxis("Fire3") > 0)
        {
            Cursor.visible = false;
            turn.x += Input.GetAxis("Mouse X");
            turn.y += Input.GetAxis("Mouse Y");
            turn.y = Mathf.Clamp(turn.y, -90f, 90f);
            Camera.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
        }
        else
        {
            Cursor.visible = true;
        }
    }

    // Move camera relatively to camera rotation
    void moveCamera()
    {
        Vector3 forward = Camera.forward;
        forward.y = 0;
        Vector3 right = Camera.right;
        right.y = 0;

        Vector3 forwardRelativeInput = Input.GetAxis("Vertical") * forward * forwardSensivity;
        Vector3 rightRelativeInput = Input.GetAxis("Horizontal") * right * rightSensivity;
        float upInput = Input.GetAxis("Mouse ScrollWheel") * upSensivity;

        Vector3 cameraRelativeMovement = forwardRelativeInput + rightRelativeInput;
        Camera.Translate(cameraRelativeMovement, Space.World);
        Camera.localPosition += new Vector3(0,upInput,0);
    }

    // Lock mouse on screen pressing K
    void lockMouse()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }

        }
    }
}
