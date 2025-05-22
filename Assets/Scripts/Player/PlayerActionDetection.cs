using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionDetection : MonoBehaviour
{
    [Header("Digging Detection Settings")]
    [SerializeField] private float minDigVelocity = 0.8f;
    [SerializeField] private float cooldownTime = 1.0f;
    [SerializeField] private float minDigDistance = 0.3f;
    [SerializeField] private DiggingFeedback diggingFeedback;
    [SerializeField] private Transform digOrigin;

    [SerializeField] private GameObject cubeObject;

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

    // Digging counter
    private int currentDigCount = 0;
    [SerializeField] private int digLimit = 3;

    // Cube detection state
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
        if (!canDig && Time.time > lastDigTime + cooldownTime)
        {
            canDig = true;
        }
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

        float upwardVelocityThreshold = 1.0f;
        float downwardVelocityThreshold = -0.8f;
        float minHeightGain = 1.2f;
        float maxComboTime = 1.0f;

        if (!isGoingUp && verticalVelocity > upwardVelocityThreshold)
        {
            isGoingUp = true;
            cubeStartY = currentY;
            peakY = currentY;
            timeSinceUpward = 0f;
        }
        else if (isGoingUp)
        {
            if (currentY > peakY) peakY = currentY;
            timeSinceUpward += Time.deltaTime;

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

                isGoingUp = false;
                isComingDown = false;
            }

            if (timeSinceUpward > maxComboTime)
            {
                isGoingUp = false;
            }
        }
    }

    private void DetectDiggingAction()
    {
        if (!canDig) return;

        Vector3 currentPosition = transform.position;
        currentVelocity = (currentPosition - lastPosition) / Time.deltaTime;
        lastPosition = currentPosition;

        float verticalVelocity = currentVelocity.y;
        float currentY = currentPosition.y;

        if (!isDiggingDown && verticalVelocity < -minDigVelocity)
        {
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
                downwardMovementTime += Time.deltaTime;
                if (currentY < minYDuringDig)
                    minYDuringDig = currentY;
            }
            else if (verticalVelocity > minDigVelocity)
            {
                digEndY = currentY;
                float digDepth = Mathf.Abs(digStartY - minYDuringDig);
                float digRise = Mathf.Abs(digEndY - minYDuringDig);
                float totalVerticalDistance = digDepth + digRise;

                if (downwardMovementTime > 0.05f && timeSinceDirectionChange < 0.5f && totalVerticalDistance >= minDigDistance)
                {
                    Debug.Log("Player DUG into the ground!");
                    isDiggingDown = false;
                    lastDigTime = Time.time;
                    canDig = false;

                    if (diggingFeedback != null)
                    {
                        diggingFeedback.TriggerDig();

                        currentDigCount++;

                        if (currentDigCount >= digLimit)
                        {
                            Debug.Log("Dig limit reached. Calling FUNCTION()...");
                            diggingFeedback.ClearDigVisuals();
                            FUNCTION();
                            currentDigCount = 0;
                        }
                    }
                }
                else
                {
                    isDiggingDown = false;
                }
            }
            else
            {
                timeSinceDirectionChange += Time.deltaTime;
                if (timeSinceDirectionChange > 0.5f)
                {
                    isDiggingDown = false;
                }
            }
        }
    }

    private void FUNCTION()
    {
        Debug.Log("FUNCTION() triggered! 🎯 You dug enough times.");
        // Put your custom logic here
    }
}
