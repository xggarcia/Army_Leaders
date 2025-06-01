using System.Collections;
using UnityEngine;

public class CubeBehaviour : MonoBehaviour
{
    [Header("Disappear/Respawn Settings")]
    public float rotationSpeed = 720f;
    public float shrinkDuration = 1.0f;
    public float cooldown = 5f;
    public float appearDuration = 1.0f;
    public float hideYOffset = -10f; // Distance below the floor to hide

    [Header("Idle Bounce Settings")]
    public float rotationSpeed_basic = 15f;
    public float bounceHeight_basic = 1.5f;
    public float bounceSpeed_basic = 3.5f;
    public RarityManager rarityManager;
    public GameObject[] epicObjects;
    public GameObject[] legendaryObjects;

    private Vector3 initialScale;
    private Vector3 startPosition;
    private bool onCooldown = false;

    private PlayerActionDetection playerDetector;


    void Start()
    {
        initialScale = transform.localScale;
        startPosition = transform.position;

    }


    void Update()
    {
        if (onCooldown)
            return;

        // Rotate and bounce
        transform.Rotate(Vector3.one * rotationSpeed_basic * Time.deltaTime);

        float newY = startPosition.y + Mathf.Sin(Time.time * bounceSpeed_basic) * bounceHeight_basic;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    public void CubeActivation(GameObject triggeringPlayer)
    {
        if (onCooldown || triggeringPlayer == null)
            return;

        PlayerActionDetection detector = triggeringPlayer.GetComponent<PlayerActionDetection>();
        if (detector == null) return;

        // 2/3 Epic, 1/3 Legendary
        Rarity rarity = (Random.value < 2f / 3f) ? Rarity.Epic : Rarity.Legendary;

        GameObject objToSpawn = (rarity == Rarity.Epic) ?
            epicObjects[Random.Range(0, epicObjects.Length)] :
            legendaryObjects[Random.Range(0, legendaryObjects.Length)];

        detector.SpawnReward(objToSpawn, rarity);

        StartCoroutine(AnimateCubeAndRespawn());
    }




    private IEnumerator AnimateCubeAndRespawn()
    {
        onCooldown = true;

        float elapsed = 0f;

        // Shrink and move down
        while (elapsed < shrinkDuration)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            float t = elapsed / shrinkDuration;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.one * 0.01f, t); // avoid 0
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Move it below the map and keep tiny
        transform.localScale = Vector3.one * 0.01f;
        transform.position = startPosition + Vector3.up * hideYOffset;

        // Wait
        yield return new WaitForSeconds(cooldown);

        // Move back to original position
        transform.position = startPosition;

        // Scale up smoothly
        elapsed = 0f;
        while (elapsed < appearDuration)
        {
            float t = elapsed / appearDuration;
            transform.localScale = Vector3.Lerp(Vector3.one * 0.01f, initialScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = initialScale;
        onCooldown = false;
    }
}
