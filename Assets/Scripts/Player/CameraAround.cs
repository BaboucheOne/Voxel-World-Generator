using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAround : MonoBehaviour
{
    private void Update()
    {
        transform.RotateAround(Vector3.forward * 8, Vector3.up, 20 * Time.deltaTime);
    }
}
