using UnityEngine;

public class PlyController : MonoBehaviour
{
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private float gravity;
    [SerializeField]
    private bool fly = false;

    private Vector3 move = Vector3.zero;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) fly = !fly;

        float x = Input.GetAxis("Vertical") * speed;
        float z = Input.GetAxis("Horizontal") * speed;


        if (!characterController.isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift)) x *= runSpeed;
            if(!fly) move.y -= gravity * Time.deltaTime;
        } else
        {
            if(!fly) move.y = 0f;
            if (Input.GetKey(KeyCode.Space)) move.y = jumpSpeed;
        }

        move += transform.forward * x + transform.right * z;

        characterController.Move(move * Time.deltaTime);

        move.x = 0;
        move.z = 0;
        if (fly) move.y = 0;
    }
}
