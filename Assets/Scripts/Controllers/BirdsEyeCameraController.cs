using UnityEngine;
using System.Collections;

public class BirdsEyeCameraController : MonoBehaviour
{
    Camera attachedCamera;
    public float zoomSpeed = 10f;
    public float edgeScreenBuffer = 10f;
    public float edgeMovementSpeed = .5f;

    private Vector3 rotationVector;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;


    // Use this for initialization
    void Start ()
    {
        attachedCamera = GetComponent<Camera>();
        yaw = attachedCamera.transform.rotation.eulerAngles.x;
        pitch = attachedCamera.transform.rotation.eulerAngles.y;
        rotationVector = new Vector3();
    }
	
	// Update is called once per frame 
    void Update()
    {
        HandleRotation();
        HandleZoom();
        HandleMovement();
    }

    void HandleZoom()
    {
        attachedCamera.transform.position += attachedCamera.transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
    }

    void HandleMovement()
    {
        //If right mouse button is pressed don't move the camera
        if (Input.GetMouseButton(1))
        {
            return;
        }

        //Move camera left or right
        if (Input.mousePosition.x > Screen.width - edgeScreenBuffer || Input.GetAxis("Horizontal") > 0)
        {
            attachedCamera.transform.position += attachedCamera.transform.right * edgeMovementSpeed;
        }
        else if (Input.mousePosition.x < edgeScreenBuffer || Input.GetAxis("Horizontal") < 0)
        {
            attachedCamera.transform.position -= attachedCamera.transform.right * edgeMovementSpeed;
        }

        //Move camera forward or backward
        if (Input.mousePosition.y > Screen.height - edgeScreenBuffer || Input.GetAxis("Vertical") > 0)
        {
            attachedCamera.transform.position += Vector3.Cross(Vector3.down, attachedCamera.transform.right) * edgeMovementSpeed;
        }
        else if (Input.mousePosition.y < edgeScreenBuffer || Input.GetAxis("Vertical") < 0)
        {
            attachedCamera.transform.position -= Vector3.Cross(Vector3.down, attachedCamera.transform.right) * edgeMovementSpeed;
        }
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftControl))
        {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            rotationVector.x = pitch;
            rotationVector.y = yaw;
            rotationVector.z = 0.0f;

            attachedCamera.transform.eulerAngles = rotationVector;
        }
    }
}
