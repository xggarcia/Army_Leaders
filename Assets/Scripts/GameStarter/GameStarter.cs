using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameStarter : MonoBehaviour
{
    public GameObject bluePlatform;
    public GameObject redPlatform;

    public GameObject redPlayer;
    public GameObject bluePlayer;

    public TextMeshProUGUI countdownText;
    public string nextSceneName;

    private HashSet<GameObject> redPlatformPlayers = new HashSet<GameObject>();
    private HashSet<GameObject> bluePlatformPlayers = new HashSet<GameObject>();

    private Coroutine countdownCoroutine;

    [Header("Audio")]
    public AudioSource audioSource; // Assign in inspector
    public AudioClip countdown3;
    public AudioClip countdown2;
    public AudioClip countdown1;
    public AudioClip goClip;

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
            switch (i)
            {
                case 3:
                    countdownText.text = "3";
                    audioSource.PlayOneShot(countdown3);
                    break;
                case 2:
                    countdownText.text = "2";
                    audioSource.PlayOneShot(countdown2);
                    break;
                case 1:
                    countdownText.text = "1";
                    audioSource.PlayOneShot(countdown1);
                    break;
                case 0:
                    countdownText.text = "FIGHT!!";
                    audioSource.PlayOneShot(goClip);
                    break;
            }

            countdownText.color = new Color(1, 1, 1, 0);
            countdownText.transform.localScale = Vector3.one * 0.5f;

            countdownText.DOFade(1f, 0.5f);
            countdownText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(1f);

            if (!redPlatformPlayers.Contains(redPlayer) || !bluePlatformPlayers.Contains(bluePlayer))
            {
                countdownText.DOKill();
                countdownText.gameObject.SetActive(false);
                yield break;
            }
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
