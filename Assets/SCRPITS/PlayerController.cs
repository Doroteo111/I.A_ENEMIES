using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed = 20f;
    private float turnSpeed = 60f;

    private float horizontalInput, verticalInput; //movment direction

    private void Update()
    {
        //Movment with w,a,s,d keys
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");


        transform.Translate(Vector3.forward * speed * Time.deltaTime * verticalInput);
        transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime * horizontalInput);

    }
}
