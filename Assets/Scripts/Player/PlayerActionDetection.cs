using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionDetection : MonoBehaviour
{
    [Header("Digging Detection Settings")]
    [SerializeField] private float minDigVelocity = 0.8f;     // Minimum velocity to register as digging
    [SerializeField] private float cooldownTime = 1.0f;       // Cooldown between dig detections
    [SerializeField] private float minDigDistance = 0.3f;     // Minimum vertical distance to count as a dig
    [SerializeField] private DiggingFeedback diggingFeedback;
    [SerializeField] private Transform digOrigin;

    // Movement tracking
    private Vector3 lastPosition;
    private Vector3 currentVelocity;
    
    // Digging state
    private float lastDigTime = 0f;
    private bool canDig = true;
    private bool isDiggingDown = false;
    private float downwardMovementTime = 0f;
    private float timeSinceDirectionChange = 0f;
    private float digStartY = 0f;
    private float digEndY = 0f;
    private float minYDuringDig = 0f;
    private CubeBehaviour cubescript;
    [SerializeField] private GameObject cubeObject;



    // Detect if the player is performing a digging action
    // --- Cube jump detection state ---
    private bool isGoingUp = false;
    private bool isComingDown = false;
    private float cubeStartY = 0f;
    private float peakY = 0f;
    private float lastCubeActionTime = 0f;
    private float timeSinceUpward = 0f;


    void Start()
    {
        lastPosition = transform.position;
        Debug.Log("PlayerActionDetection initialized. Ready to detect digging actions.");
    }

    void Update()
    {
        // Handle cooldown timer
        if (!canDig && Time.time > lastDigTime + cooldownTime)
        {
            canDig = true;
        }
        
        // Track movement and detect digging
      }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("SandZone"))
        {
            DetectDiggingAction();
        }
        if (other.CompareTag("CubeZone"))
        {
            DetectCubeAction(); 

        }
    }


    private void DetectCubeAction()
    {
        Vector3 currentPosition = transform.position;
        currentVelocity = (currentPosition - lastPosition) / Time.deltaTime;
        lastPosition = currentPosition;

        float verticalVelocity = currentVelocity.y;
        float currentY = currentPosition.y;

        float upwardVelocityThreshold = 1.0f;   // Min speed going up
        float downwardVelocityThreshold = -0.8f; // Min speed going down
        float minHeightGain = 1.2f;             // Required height delta
        float maxComboTime = 1.0f;              // Max time between up/down

        if (!isGoingUp && verticalVelocity > upwardVelocityThreshold)
        {
            // Started moving up fast
            isGoingUp = true;
            cubeStartY = currentY;
            peakY = currentY;
            timeSinceUpward = 0f;
            // Debug.Log("Started upward motion");
        }
        else if (isGoingUp)
        {
            // Track peak height
            if (currentY > peakY)
                peakY = currentY;

            timeSinceUpward += Time.deltaTime;

            // Now going down?
            if (verticalVelocity < downwardVelocityThreshold)
            {
                float heightGain = peakY - cubeStartY;

                if (heightGain >= minHeightGain && timeSinceUpward <= maxComboTime)
                {
                    Debug.Log("Player completed cube jump!");


                    if (cubeObject != null)
                    {
                        CubeBehaviour cubeScript = cubeObject.GetComponent<CubeBehaviour>();
                        if (cubeScript != null)
                        {
                            cubeScript.CubeActivation();
                        }
                        else
                        {
                            Debug.LogWarning("CubeBehaviour not found on assigned cubeObject!");
                        }
                    }


                }

                // Reset
                isGoingUp = false;
                isComingDown = false;
            }

            // Reset if too slow
            if (timeSinceUpward > maxComboTime)
            {
                isGoingUp = false;
            }
        }
    }


    private void DetectDiggingAction()
    {
        if (!canDig) return;
        
        // Calculate current velocity
        Vector3 currentPosition = transform.position;
        currentVelocity = (currentPosition - lastPosition) / Time.deltaTime;
        lastPosition = currentPosition;
        
        float verticalVelocity = currentVelocity.y;
        float currentY = currentPosition.y;
        
        // Track digging state - looking for down-then-up pattern
        if (!isDiggingDown && verticalVelocity < -minDigVelocity)
        {
            // Started moving down quickly - first part of dig
            isDiggingDown = true;
            downwardMovementTime = 0f;
            timeSinceDirectionChange = 0f;
            digStartY = currentY;
            minYDuringDig = currentY;
        }
        else if (isDiggingDown)
        {
            if (verticalVelocity < 0)
            {
                // Still moving down
                downwardMovementTime += Time.deltaTime;
                if (currentY < minYDuringDig)
                    minYDuringDig = currentY;
            }
            else if (verticalVelocity > minDigVelocity)
            {
                // Direction changed to upward - check if we have a complete dig motion
                digEndY = currentY;
                float digDepth = Mathf.Abs(digStartY - minYDuringDig);
                float digRise = Mathf.Abs(digEndY - minYDuringDig);
                float totalVerticalDistance = digDepth + digRise;
                if (downwardMovementTime > 0.05f && timeSinceDirectionChange < 0.5f && totalVerticalDistance >= minDigDistance)
                {
                    Debug.Log($"Player DUG into the ground!");
                    isDiggingDown = false;
                    lastDigTime = Time.time;
                    canDig = false;

                    if (diggingFeedback != null)
                    {
                        Debug.Log("Calling TriggerDig()");
                        diggingFeedback.TriggerDig();

                    }
                }
                else
                {
                    // Not enough distance, reset
                    isDiggingDown = false;
                }
            }
            else
            {
                // Tracking time since downward motion stopped
                timeSinceDirectionChange += Time.deltaTime;
                
                // If too much time has passed, reset the digging state
                if (timeSinceDirectionChange > 0.5f)
                {
                    isDiggingDown = false;
                }
            }
        }
    }
}
