using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {
    
    public float speedX = 40f, speedY = 5f;

	// Use this for initialization
	void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        float changeX = Input.GetAxis("Mouse X");
        transform.Rotate(new Vector3 (0, speedX * changeX * Time.fixedDeltaTime), Space.World);

        float changeY = Input.GetAxis("Mouse Y");
        transform.Translate(new Vector3(0, 0, speedY * changeY * Time.fixedDeltaTime), Space.World);
    }
}
