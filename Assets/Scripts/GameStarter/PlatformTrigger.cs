using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public string platformColor; // "Red" or "Blue"
    public GameStarter gameStarter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameStarter.PlayerEntered(platformColor, other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameStarter.PlayerExited(platformColor, other.gameObject);
        }
    }
}
