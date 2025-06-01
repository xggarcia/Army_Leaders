using UnityEngine;

public class WinTracker : MonoBehaviour
{
    public static WinTracker Instance;

    public int redWins = 0;
    public int blueWins = 0;

    private void Awake()
    {
        // Singleton pattern to persist across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterWin(string team)
    {
        if (team == "Red")
        {
            redWins++;
            Debug.Log($"Red wins! Total: {redWins}");
        }
        else if (team == "Blue")
        {
            blueWins++;
            Debug.Log($"Blue wins! Total: {blueWins}");
        }
        else
        {
            Debug.LogWarning("Unknown team: " + team);
        }
    }

    public void ResetWins()
    {
        redWins = 0;
        blueWins = 0;
    }
}
