using UnityEngine;

public class BirdMapSoundTrigger : MonoBehaviour
{
    public AudioSource birdSoundSource; // Assign your looping sound object
    private int birdCountInside = 0;

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Birds"))
        {
            Debug.Log("Bird entered map trigger.");

            birdCountInside++;
            if (!birdSoundSource.isPlaying)
            {
                birdSoundSource.Play();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Birds"))
        {
            birdCountInside--;
            if (birdCountInside <= 0)
            {
                birdCountInside = 0;
                birdSoundSource.Stop();
            }
        }
    }



}
