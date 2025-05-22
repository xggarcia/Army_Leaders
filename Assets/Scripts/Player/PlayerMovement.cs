using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2.0f;

    void Update()
    {
        Vector3 moveDir = Vector3.zero;

        // Horizontal movement (WASD)
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down
        moveDir += new Vector3(horizontal, 0f, vertical);

        // Vertical movement (Q down, E up)
        if (Input.GetKey(KeyCode.E))
            moveDir += Vector3.up;
        if (Input.GetKey(KeyCode.Q))
            moveDir += Vector3.down;

        // Normalize to prevent faster diagonal movement
        moveDir = moveDir.normalized;

        // Apply movement
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    // Optional: External position access
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }
}
