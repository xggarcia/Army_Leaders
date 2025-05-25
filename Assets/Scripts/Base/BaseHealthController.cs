using UnityEngine;

public class BaseHealthController : MonoBehaviour
{
    public int RedBaseHealth;
    public int BlueBaseHealth;
    public GameObject blue_base;
    public GameObject red_base;

    void Update()
    {
        if (RedBaseHealth <= 0)
        {
            Destroy(red_base);
            Debug.Log("Blue team has won! Game paused.");
            Time.timeScale = 0f;
        }
        else if (BlueBaseHealth <= 0)
        {
            Destroy(blue_base);
            Debug.Log("Red team has won! Game paused.");
            Time.timeScale = 0f;
        }
    }

    public void AddHealth(int health, string team)
    {
        if (team == "blue")
        {
            BlueBaseHealth += health;
        }
        else if (team == "red")
        {
            RedBaseHealth += health;
        }
    }

    public void RemoveHealth(int health, string team)
    {
        if (team == "blue")
        {
            BlueBaseHealth -= health;
        }
        else if (team == "red")
        {
            RedBaseHealth -= health;
        }
    }
}
