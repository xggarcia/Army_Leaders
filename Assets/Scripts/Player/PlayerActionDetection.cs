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
    [SerializeField] private int baseDigLimit = 3;
    private int currentDigLimit;

    [Header("Spawned Object Settings")]
    [SerializeField] private List<GameObject> spawnPrefabs;
    [SerializeField] private float spawnRiseAmount = 2f;
    [SerializeField] private float spawnShrinkTime = 1.5f;
    [SerializeField] private float spawnRotationSpeed = 120f;

    [Header("Dig Precision Settings")]
    [SerializeField] private float digRange = 1.5f;
    [SerializeField] private float prefabScaleMultiplier = 2.0f;

    [Header("Combat")]
    [SerializeField] private Combat tankScript;
    [SerializeField] private string playerTeam = "Red";

    [Header("Rarity Visuals")]
    [SerializeField] private RarityManager rarityManager;
    [SerializeField] private GameObject commonParticles;
    [SerializeField] private GameObject rareParticles;
    [SerializeField] private GameObject epicParticles;
    [SerializeField] private GameObject legendaryParticles;

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

    private DigSpot lastDigSpot;

    void Start()
    {
        lastPosition = transform.position;
        currentDigLimit = baseDigLimit;
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
                        if (cubeScript != null)
                        {
                            cubeScript.CubeActivation();
                        }
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
                        lastDigSpot = spot;
                        diggingFeedback.TriggerDig();
                        diggingFeedback.ShowRarityColor(spot.rarity, rarityManager);
                        lastCompletedDigPosition = spot.transform.position;

                        currentDigLimit--;
                        if (currentDigLimit <= 0)
                        {
                            diggingFeedback.ClearDigVisuals();
                            FUNCTION();
                            digZoneManager.SpotCompleted(spot);
                            ResetPlayerDigLimit();
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

    private void ReducePlayerDigLimit()
    {
        baseDigLimit = Mathf.Max(1, baseDigLimit - 1);
        currentDigLimit = Mathf.Min(currentDigLimit, baseDigLimit);
        Debug.Log("[BASE DIG LIMIT REDUCED] New base: " + baseDigLimit);
    }

    private void ResetPlayerDigLimit()
    {
        currentDigLimit = baseDigLimit;
    }

    private GameObject FUNCTION()
    {
        if (lastDigSpot == null || spawnPrefabs == null || spawnPrefabs.Count == 0)
        {
            Debug.LogWarning("Missing dig spot or prefabs!");
            return null;
        }

        List<GameObject> filteredPrefabs = new List<GameObject>();
        foreach (var prefab in spawnPrefabs)
        {
            RarityTag tag = prefab.GetComponent<RarityTag>();
            if (tag != null && tag.rarity == lastDigSpot.rarity)
            {
                filteredPrefabs.Add(prefab);
            }
        }

        if (filteredPrefabs.Count == 0)
        {
            Debug.LogWarning("No prefabs found for rarity: " + lastDigSpot.rarity);
            return null;
        }

        GameObject selectedPrefab = filteredPrefabs[Random.Range(0, filteredPrefabs.Count)];
        GameObject instance = Instantiate(selectedPrefab, lastCompletedDigPosition, Quaternion.identity);
        if (instance.GetComponent<BombHandler>() != null)
        {
            // Let the normal prefab animate/disappear
            StartCoroutine(AnimateSpawnedObject(instance));

            // Spawn a second bomb that attaches to the player
            GameObject bombCopy = Instantiate(selectedPrefab, transform.position, Quaternion.identity);
            bombCopy.transform.localScale *= 30.0f; // ✅ increase size (you can tweak this factor)
            BombHandler bomb = bombCopy.GetComponent<BombHandler>();
            bomb.AttachToPlayer(this.gameObject);
            return instance;
        }

        instance.transform.localScale *= prefabScaleMultiplier;

        // 🔥 Add rarity-based particle effect
        GameObject particlePrefab = null;
        switch (lastDigSpot.rarity)
        {
            case Rarity.Common:
                particlePrefab = commonParticles; break;
            case Rarity.Rare:
                particlePrefab = rareParticles; break;
            case Rarity.Epic:
                particlePrefab = epicParticles; break;
            case Rarity.Legendary:
                particlePrefab = legendaryParticles; break;
        }

        if (particlePrefab != null)
        {
            GameObject effect = Instantiate(particlePrefab, instance.transform.position, Quaternion.identity, instance.transform);
            var ps = effect.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }

        if (tankScript != null)
        {
            tankScript.ImproveStats(selectedPrefab, playerTeam);

            if (selectedPrefab.name == tankScript.specialDigObject.name)
            {
                ReducePlayerDigLimit();
            }
            else if(selectedPrefab.name == tankScript.specialDigObject2.name)
            {
                 ReducePlayerDigLimit();
                 ReducePlayerDigLimit();
            }


        }
        // If the object is a bomb, attach it and skip animation

        // Not a bomb → animate and destroy
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
            if (obj == null) yield break; // ✅ safely exit if object is destroyed

            float t = elapsed / spawnShrinkTime;

            obj.transform.position = Vector3.Lerp(originalPos, targetPos, t);
            obj.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            obj.transform.Rotate(Vector3.up, spawnRotationSpeed * Time.deltaTime, Space.World);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (obj != null)
            Destroy(obj);
    }

    public void SpawnReward(GameObject prefab, Rarity rarity)
    {
        Vector3 spawnPosition = transform.position + Vector3.forward * 1.5f;  // in front of player
        GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity);
        instance.transform.localScale *= prefabScaleMultiplier;

        // Particles
        GameObject particlePrefab = null;
        switch (rarity)
        {
            case Rarity.Common: particlePrefab = commonParticles; break;
            case Rarity.Rare: particlePrefab = rareParticles; break;
            case Rarity.Epic: particlePrefab = epicParticles; break;
            case Rarity.Legendary: particlePrefab = legendaryParticles; break;
        }

        if (particlePrefab != null)
        {
            GameObject effect = Instantiate(particlePrefab, instance.transform.position, Quaternion.identity, instance.transform);
            var ps = effect.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }

        // Handle bombs specially
        if (instance.GetComponent<BombHandler>() != null)
        {
            StartCoroutine(AnimateSpawnedObject(instance)); // animate + disappear

            GameObject bombCopy = Instantiate(prefab, transform.position, Quaternion.identity);
            bombCopy.transform.localScale *= 30.0f;
            bombCopy.GetComponent<BombHandler>().AttachToPlayer(this.gameObject);
        }
        else
        {
            // Animate & destroy
            StartCoroutine(AnimateSpawnedObject(instance));
        }

        // Apply object effects
        if (tankScript != null)
        {
            tankScript.ImproveStats(prefab, playerTeam);

            if (prefab.name == tankScript.specialDigObject.name)
            {
                ReducePlayerDigLimit();
            }
            else if (prefab.name == tankScript.specialDigObject2.name)
            {
                ReducePlayerDigLimit();
                ReducePlayerDigLimit();
            }
        }
    }


}
