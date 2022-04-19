using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 2f; //movement speed variable
    Rigidbody2D rb2D;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
         // get player input and store as floats
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 inputVector = new Vector2(horizontalInput, verticalInput); // store player movement input as a vector
        //inputVector = Vector2.ClampMagnitude(inputVector, 1); // preventing diagonal movement from being faster than horizontal/vertical movement

        Vector2 playerMovement = inputVector * movementSpeed; // applying movement speed to player's input

        rb2D.MovePosition(rb2D.position + playerMovement); // move the player
    }

}
