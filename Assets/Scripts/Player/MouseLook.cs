using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;

    public float rotY = 0.0f; // rotation around the up/y axis
    public float rotX = 0.0f; // rotation around the right/x axis

    private bool mouseLock = false;

    [SerializeField]
    private Camera _cam;

    private Quaternion localRotCamera;
    private Quaternion localRotation;
    private float camZrotation;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            mouseLock = !mouseLock;
            if (mouseLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }

            Cursor.visible = true;
        }

        RotateTransform();
    }

    public void SetCameraRotation(float x)
    {
        camZrotation = x;
    }

    private void RotateTransform()
    {
        if (Cursor.lockState == CursorLockMode.None) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        //rotY = Mathf.Clamp(rotY, -clampAngle, clampAngle);

        localRotCamera = Quaternion.Euler(rotX, rotY, camZrotation);
        localRotation = Quaternion.Euler(0.0f, rotY, 0.0f);
        transform.root.rotation = localRotation;
        transform.rotation = localRotCamera;
    }
}
