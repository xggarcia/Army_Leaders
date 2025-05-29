using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public List<AudioClip> musicTracks;
    public AudioSource audioSource;

    private int currentTrackIndex;
    private bool isPlaying = false;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (!isPlaying && musicTracks.Count > 0)
        {
            currentTrackIndex = Random.Range(0, musicTracks.Count);
            PlayTrack(currentTrackIndex);
            isPlaying = true;
        }
    }

    void Update()
    {
        if (!audioSource.isPlaying && isPlaying)
        {
            currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Count;
            PlayTrack(currentTrackIndex);
        }
    }

    void PlayTrack(int index)
    {
        if (musicTracks[index] != null)
        {
            audioSource.clip = musicTracks[index];
            audioSource.Play();
        }
    }
}
