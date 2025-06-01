using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class BaseHealthController : MonoBehaviour
{
    public int RedBaseHealth;
    public int BlueBaseHealth;
    public GameObject blue_base;
    public GameObject red_base;
    public GameObject explosion;

    public GameObject new_base; 

    public int MaxRedBaseHealth = 100;
    public int MaxBlueBaseHealth = 100;


    private bool exploded = false; 
    private bool base_spawned = false;
    private bool first_time = true;

    void Update()
    {
        if (RedBaseHealth <= 0 & first_time == true)
        {
            DestroyBaseAnimation(red_base);


            StartCoroutine(SpawnNewBase(red_base));

            WinTracker.Instance?.RegisterWin("Blue");
            first_time = false;

        }
        else if (BlueBaseHealth <= 0 & first_time == true)
        {
            DestroyBaseAnimation(blue_base);

            StartCoroutine(SpawnNewBase(blue_base));

            WinTracker.Instance?.RegisterWin("Red");

            first_time =false;

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

    public float GetHealthPercent(string team)
    {
        if (team == "red")
            return Mathf.Clamp01((float)RedBaseHealth / MaxRedBaseHealth);
        else if (team == "blue")
            return Mathf.Clamp01((float)BlueBaseHealth / MaxBlueBaseHealth);
        else
            return 0f;
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
        if (base_color == red_base & !base_spawned)
        {
            yield return new WaitForSeconds(3f);
            Destroy(red_base);
            Vector3 spawnPosition = new Vector3(-11f, 5.1f, 21.5f);
            Instantiate(new_base, spawnPosition, Quaternion.identity);
            base_spawned = true;

        }
        else if (base_color == blue_base & !base_spawned)
        {
            yield return new WaitForSeconds(3f);
            Destroy(blue_base);
            Vector3 spawnPosition = new Vector3(-11f, 5.1f, -21.5f);
            Instantiate(new_base, spawnPosition, Quaternion.identity);
            base_spawned = true; 

        }

        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("MenuScene");



    }
}
