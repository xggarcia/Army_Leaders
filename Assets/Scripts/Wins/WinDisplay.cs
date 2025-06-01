using TMPro;
using UnityEngine;

public class WinDisplay : MonoBehaviour
{
    public enum Team { Red, Blue }
    public Team team;

    public TextMeshProUGUI winText;

    void Start()
    {
        UpdateWinText();
    }

    void Update()
    {
        UpdateWinText();
    }

    void UpdateWinText()
    {
        if (winText == null || WinTracker.Instance == null) return;

        int wins = team == Team.Red ? WinTracker.Instance.redWins : WinTracker.Instance.blueWins;
        winText.text = wins.ToString();
    }
}
