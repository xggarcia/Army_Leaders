// ✅ FINALIZED BOMB: BombHandler.cs
using System.Collections;
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

    public void AttachToPlayer(GameObject player)
    {
        carrier = player;
        transform.position = player.transform.position + new Vector3(0, 1.5f, -0.5f);
        transform.SetParent(null); // optional, not parenting
        active = true;
    }

    private void Update()
    {
        if (active && carrier != null)
        {
            transform.position = carrier.transform.position + new Vector3(0, 1.5f, -0.5f);
        }
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
