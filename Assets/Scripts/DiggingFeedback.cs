using UnityEngine;

public class DiggingFeedback : MonoBehaviour
{
    [Header("Feedback Prefabs & Assets")]
    public ParticleSystem digParticles;
    public AudioClip digSound;
    public GameObject sandDarkDecalPrefab;
    public GameObject sandPilePrefab;

    [Header("Settings")]
    public Transform groundParent; // where decals/sandpiles are placed
    public Vector3 decalOffset = Vector3.down * 0.01f;
    public float pileOffsetDistance = 0.5f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void TriggerDig(Vector3 position, Vector3 direction)
    {
        // 1. Darken sand texture with decal
        if (sandDarkDecalPrefab)
        {
            Instantiate(sandDarkDecalPrefab, position + decalOffset, Quaternion.Euler(90, 0, 0), groundParent);
        }

        // 2. Emit dig particles
        if (digParticles)
        {
            digParticles.transform.position = position;
            digParticles.Play();
        }

        // 3. Play dig sound
        if (digSound)
        {
            audioSource.PlayOneShot(digSound);
        }

        // 4. Optionally instantiate or grow sand pile
        if (sandPilePrefab)
        {
            Vector3 pilePos = position + direction.normalized * pileOffsetDistance;
            Instantiate(sandPilePrefab, pilePos, Quaternion.identity, groundParent);
        }
    }
}
