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
    [SerializeField] private DigZoneManager digZoneManager;

    [Header("Dig Limit Settings")]
    [SerializeField] private int digLimit = 3;
    private int currentDigCount = 0;

    [Header("Spawned Object Settings")]
    [SerializeField] private List<GameObject> spawnPrefabs;
    [SerializeField] private float spawnRiseAmount = 2f;
    [SerializeField] private float spawnShrinkTime = 1.5f;
    [SerializeField] private float spawnRotationSpeed = 120f;

    [Header("Dig Precision Settings")]
    [SerializeField] private float digRange = 1.5f;
    [SerializeField] private float prefabScaleMultiplier = 2.0f;


    private Vector3 lastPosition;
    private Vector3 currentVelocity;
    private Vector3 lastCompletedDigPosition;

    private float lastDigTime = 0f;
    private bool canDig = true;
    private bool isDiggingDown = false;
    private float downwardMovementTime = 0f;
    private float timeSinceDirectionChange = 0f;
    private float digStartY = 0f;
    private float digEndY = 0f;
    private float minYDuringDig = 0f;

    private bool isGoingUp = false;
    private float cubeStartY = 0f;
    private float peakY = 0f;
    private float timeSinceUpward = 0f;

    void Start()
    {
        lastPosition = transform.position;
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

        if (!isGoingUp && verticalVelocity > 1.0f)
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

            if (verticalVelocity < -0.8f)
            {
                float heightGain = peakY - cubeStartY;

                if (heightGain >= 1.2f && timeSinceUpward <= 1.0f)
                {
                    if (cubeObject != null)
                    {
                        CubeBehaviour cubeScript = cubeObject.GetComponent<CubeBehaviour>();
                        if (cubeScript != null) cubeScript.CubeActivation();
                    }
                }

                isGoingUp = false;
            }

            if (timeSinceUpward > 1.0f) isGoingUp = false;
        }
    }

    private void DetectDiggingAction()
    {
        if (!canDig || digZoneManager == null) return;

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
                if (currentY < minYDuringDig) minYDuringDig = currentY;
            }
            else if (verticalVelocity > minDigVelocity)
            {
                digEndY = currentY;
                float digDepth = Mathf.Abs(digStartY - minYDuringDig);
                float digRise = Mathf.Abs(digEndY - minYDuringDig);
                float totalVerticalDistance = digDepth + digRise;

                if (downwardMovementTime > 0.05f && timeSinceDirectionChange < 0.5f && totalVerticalDistance >= minDigDistance)
                {
                    isDiggingDown = false;
                    lastDigTime = Time.time;
                    canDig = false;

                    if (digZoneManager.IsPlayerNearAnySpot(transform.position, out DigSpot spot, digRange))
                    {
                        diggingFeedback.TriggerDig();
                        spot.RegisterDig();

                        lastCompletedDigPosition = spot.transform.position;

                        currentDigCount++;
                        if (currentDigCount >= digLimit)
                        {
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
                if (timeSinceDirectionChange > 0.5f) isDiggingDown = false;
            }
        }
    }

    private GameObject FUNCTION()
    {
        if (spawnPrefabs == null || spawnPrefabs.Count == 0)
        {
            Debug.LogWarning("No prefabs assigned in spawnPrefabs list!");
            return null;
        }

        GameObject prefabToSpawn = spawnPrefabs[Random.Range(0, spawnPrefabs.Count)];
        GameObject instance = Instantiate(prefabToSpawn, lastCompletedDigPosition, Quaternion.identity);

        // Scale it up before animation starts (e.g., 2x)
        instance.transform.localScale *= prefabScaleMultiplier;

        StartCoroutine(AnimateSpawnedObject(instance));
        return instance;
    }

    private IEnumerator AnimateSpawnedObject(GameObject obj)
    {
        float elapsed = 0f;
        Vector3 originalPos = obj.transform.position;
        Vector3 targetPos = originalPos + Vector3.up * spawnRiseAmount;
        Vector3 originalScale = obj.transform.localScale;

        while (elapsed < spawnShrinkTime)
        {
            float t = elapsed / spawnShrinkTime;

            obj.transform.position = Vector3.Lerp(originalPos, targetPos, t);
            obj.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            obj.transform.Rotate(Vector3.up, spawnRotationSpeed * Time.deltaTime, Space.World);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(obj);
    }
}
