using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingDirection : MonoBehaviour
{

    private Vector3 lastPosition;
    public float rotationSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 movementDirection = currentPosition - lastPosition;

        // Only rotate if actually moving
        if (movementDirection.sqrMagnitude > 0.0001f)
        {
            Vector3 flatDirection = new Vector3(movementDirection.x, 0f, movementDirection.z).normalized;
            if (flatDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        lastPosition = currentPosition;
    }
}
