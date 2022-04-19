using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineMovement : MonoBehaviour {
    [SerializeField]
    private float frequency = 1f, amplitude = 5f;

    private Vector3 nullPosition;

    private void Start()
    {
        nullPosition = transform.position;
    }

    void FixedUpdate () {
        transform.position = nullPosition + new Vector3(amplitude * Mathf.Sin(Time.time * frequency), transform.position.y - nullPosition.y);
	}
}
