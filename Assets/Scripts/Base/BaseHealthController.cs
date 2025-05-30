using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;

public class BaseHealthController : MonoBehaviour
{
    public int RedBaseHealth;
    public int BlueBaseHealth;
    public GameObject blue_base;
    public GameObject red_base;
    public GameObject explosion;

    public GameObject new_base; 

    private bool exploded = false; 

    void Update()
    {
        if (RedBaseHealth <= 0)
        {
            DestroyBaseAnimation(red_base);
            

            SpawnNewBase(red_base);

            Debug.Log("Blue team has won! Game paused.");

        }
        else if (BlueBaseHealth <= 0)
        {
            DestroyBaseAnimation(blue_base);

            SpawnNewBase(blue_base);
            Debug.Log("Red team has won! Game paused.");
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
    private void DestroyBaseAnimation(GameObject base_color)
    {
        if (base_color == red_base && !exploded)
        {
            Vector3 spawnPosition = new Vector3(-15f, 5.1f, 21.5f);
            for (int i = 0; i < 5; i++)
            {
                GameObject fx = CFX_SpawnSystem.GetNextObject(explosion);
                fx.transform.position = spawnPosition;
                fx.transform.rotation = Quaternion.identity;
                fx.SetActive(true);
            }
            exploded = true;
        }
        else if (base_color == blue_base && !exploded)
        {
            Vector3 spawnPosition = new Vector3(-15f, 5.1f, -21.5f);
            for (int i = 0; i < 5; i++)
            {
                GameObject fx = CFX_SpawnSystem.GetNextObject(explosion);
                fx.transform.position = spawnPosition;
                fx.transform.rotation = Quaternion.identity;
                fx.SetActive(true);
            }
            exploded = true; 
        }
    }

    private IEnumerator SpawnNewBase(GameObject base_color)
    {
        if (base_color == red_base)
        {
            yield return new WaitForSeconds(3f);
            Destroy(red_base);
            Vector3 spawnPosition = new Vector3(-15f, 5.1f, 21.5f);
            Instantiate(new_base, spawnPosition, Quaternion.identity);

        }
        else if (base_color == blue_base)
        {
            yield return new WaitForSeconds(3f);
            Destroy(blue_base);
            Vector3 spawnPosition = new Vector3(-15f, 5.1f, -21.5f);
            Instantiate(new_base, spawnPosition, Quaternion.identity);


        }

    }
}
