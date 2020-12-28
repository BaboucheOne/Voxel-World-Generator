using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MouseLook mouseLook;

    [SerializeField]
    private float jumpForce = 10.0f;
    [SerializeField]
    private float forward_Force = 15.0f;
    [SerializeField]
    private float up_force = 10.0f;
    [SerializeField]
    private float gravity = 14.0f;

    public Vector3 velocity = new Vector3();
    private float lastRot = 0.0f;

    private Vector3 moveDirection = Vector3.zero;


    private void Awake()
    {
        mouseLook = GetComponent<MouseLook>();
    }

    private void Update()
    {
        velocity.x *= 0.9f;
        velocity.y *= 0.9f;
        velocity.z *= 0.9f;

        float xMove = Input.GetAxis("Horizontal") * forward_Force;
        float yMove = 0;
        float zMove = Input.GetAxis("Vertical") * forward_Force;

        if (Input.GetKey(KeyCode.Space))
        {
            yMove = up_force;
        } else if(Input.GetKey(KeyCode.LeftShift))
        {
            yMove = -up_force;
        }

        //AddForce(transform.TransformDirection(new Vector3(xMove, yMove, zMove)));

        //velocity += transform.TransformDirection(Vector3.right) * zMove;
        velocity += transform.right * xMove;
        velocity += transform.forward * zMove;
        velocity += Vector3.up * yMove;


        moveDirection = velocity;
        transform.Translate(moveDirection * Time.deltaTime);
        lastRot = mouseLook.rotX;
    }

    public void AddForce(Vector3 force)
    {
        velocity += force;
    }

    public void AddForce(float x, float y, float z)
    {
        velocity.x += x;
        velocity.y += y;
        velocity.z += z;
    }
}
