using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // if using TextMeshPro
using DG.Tweening; // Add this at the top


public class GameStarter : MonoBehaviour
{
    public GameObject bluePlatform;
    public GameObject redPlatform;

    public GameObject redPlayer;
    public GameObject bluePlayer;

    public TextMeshProUGUI countdownText; // Assign in Canvas (center of screen)
    public string nextSceneName;

    private HashSet<GameObject> redPlatformPlayers = new HashSet<GameObject>();
    private HashSet<GameObject> bluePlatformPlayers = new HashSet<GameObject>();

    private Coroutine countdownCoroutine;

    void Start()
    {
        redPlatform.GetComponent<PlatformTrigger>().gameStarter = this;
        redPlatform.GetComponent<PlatformTrigger>().platformColor = "Red";

        bluePlatform.GetComponent<PlatformTrigger>().gameStarter = this;
        bluePlatform.GetComponent<PlatformTrigger>().platformColor = "Blue";

        countdownText.gameObject.SetActive(false);
    }

    public void PlayerEntered(string platform, GameObject player)
    {
        if (platform == "Red") redPlatformPlayers.Add(player);
        else if (platform == "Blue") bluePlatformPlayers.Add(player);

        CheckIfReady();
    }

    public void PlayerExited(string platform, GameObject player)
    {
        if (platform == "Red") redPlatformPlayers.Remove(player);
        else if (platform == "Blue") bluePlatformPlayers.Remove(player);

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
            countdownText.gameObject.SetActive(false);
        }
    }

    void CheckIfReady()
    {
        if (redPlatformPlayers.Contains(redPlayer) && bluePlatformPlayers.Contains(bluePlayer))
        {
            if (countdownCoroutine == null)
            {
                countdownCoroutine = StartCoroutine(StartCountdown());
            }
        }
    }

    IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i >= 0; i--)
        {
            if (i == 0)
            {
                countdownText.text = "GO!!";
            }
            else
            {
                countdownText.text = i.ToString();
            }
            countdownText.color = new Color(1, 1, 1, 0); // transparent
            countdownText.transform.localScale = Vector3.one * 0.5f;

            // Animate scale and fade
            countdownText.DOFade(1f, 0.5f);
            countdownText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(1f);

            // Cancel if anyone leaves
            if (!redPlatformPlayers.Contains(redPlayer) || !bluePlatformPlayers.Contains(bluePlayer))
            {
                countdownText.DOKill(); // stop DOTween animations
                countdownText.gameObject.SetActive(false);
                yield break;
            }
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
