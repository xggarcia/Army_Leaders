using UnityEngine;

public class BirdFlapAnimation : MonoBehaviour
{
    public GameObject[] birdFrames;
    public float frameRate = 10f; // frames per second

    private int currentFrame = 0;
    private float timer = 0f;

    void Start()
    {
        ShowOnlyCurrentFrame();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            currentFrame = (currentFrame + 1) % birdFrames.Length;
            ShowOnlyCurrentFrame();
            timer = 0f;
        }
    }

    void ShowOnlyCurrentFrame()
    {
        for (int i = 0; i < birdFrames.Length; i++)
        {
            birdFrames[i].SetActive(i == currentFrame);
        }
    }
}
