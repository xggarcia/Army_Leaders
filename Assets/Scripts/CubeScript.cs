using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    public float rotationSpeed = 15f;
    public float bounceHeight = 1.5f;
    public float bounceSpeed = 3.5f;

    private Vector3 startPosition;

    void Start()
    {
        // Save the starting position so we bounce around it
        startPosition = transform.position;
    }

    void Update()
    {
        // Rotate the cube
        transform.Rotate(new Vector3(rotationSpeed, rotationSpeed, rotationSpeed) * Time.deltaTime);

        // Apply bouncing effect
        float newY = startPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
