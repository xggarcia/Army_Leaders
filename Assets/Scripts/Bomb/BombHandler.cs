// ✅ FINALIZED BOMB: BombHandler.cs
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BombHandler : MonoBehaviour
{
    public enum BombType { Epic, Legendary }

    public BombType bombType;
    public float epicDisableTime = 3f;
    public float legendaryDisableTime = 5f;

    private GameObject carrier;
    private bool active = false;

    public GameObject explosion;
    public AudioClip explosionSound; // assign this in the Inspector


    public void AttachToPlayer(GameObject player)
    {
        carrier = player;
        transform.position = player.transform.position;
        transform.SetParent(null); // optional, not parenting
        StartCoroutine(BombTimer(this));

        active = true;
    }

    private void Update()
    {
        if (active && carrier != null)
        {
            transform.position = new Vector3 (carrier.transform.position.x, 9, carrier.transform.position.z);
        }
    }


    private IEnumerator BombTimer(BombHandler obj)
    {
        float totalDuration = 4f;
        float elapsed = 0f;
        float blinkInterval = 1f; // starts slow, gets faster



        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning("No Renderer found on bomb.");
            yield break;
        }
        yield return new WaitForSeconds(3f);

        while (elapsed < totalDuration)
        {
            if (obj == null || obj.gameObject == null) yield break;

            // Toggle visibility
            rend.enabled = !rend.enabled;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;

            // Accelerate blinking
            blinkInterval = Mathf.Max(0.05f, blinkInterval * 0.75f);
        }

        // Ensure it's visible before destroying
        rend.enabled = true;

        Destroy(obj.gameObject);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (!active || other.gameObject == carrier) return;
        if (!other.CompareTag("Player")) return;

        PlayerMovement target = other.GetComponent<PlayerMovement>();
        if (target != null)
        {
            float disableTime = (bombType == BombType.Epic) ? epicDisableTime : legendaryDisableTime;
            target.DisableMovement(disableTime);
            SpawnBomb(target);

            Debug.Log($"Bomb hit {other.name}, frozen for {disableTime}s");
            Destroy(gameObject);
        }
    }
    private void SpawnBomb(PlayerMovement target)
    {
        GameObject fx = CFX_SpawnSystem.GetNextObject(explosion);
        fx.transform.position = target.transform.position;
        fx.transform.rotation = Quaternion.identity;
        fx.SetActive(true);

    }



}
