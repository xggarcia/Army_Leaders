using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class SkinMovement : MonoBehaviour
{
    public Transform target; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = target.position;

        transform.position = new Vector3(targetPosition.x, currentPosition.y, targetPosition.z);

    }
}
