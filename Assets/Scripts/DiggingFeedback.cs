using UnityEngine;

public class DiggingFeedback : MonoBehaviour
{
    [Header("Prefabs y Assets")]
    public ParticleSystem digParticles;
    public AudioClip digSound;
    public GameObject sandDarkDecalPrefab;
    public GameObject sandPilePrefab;

    [Header("Puntos de referencia")]
    public Transform playerTransform;
    public Transform digOrigin;

    private GameObject decalInstance;
    private GameObject pileInstance;

    private float decalScale = 10f;
    private float pileScale = 0.5f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void TriggerDig()
    {
        // 🟫 1. Altura más alta para que no se trague el suelo
        float decalY = 2.5f;
        float pileY = 3;

        Vector3 groundPos = new Vector3(playerTransform.position.x, decalY, playerTransform.position.z);
        Vector3 pilePos = new Vector3(digOrigin.position.x, pileY, digOrigin.position.z);

        // 🌀 Decal (sprite oscuro)
        if (sandDarkDecalPrefab)
        {
            if (decalInstance == null)
            {
                decalInstance = Instantiate(sandDarkDecalPrefab, groundPos, Quaternion.Euler(90, 0, 0));
                decalInstance.transform.localScale = Vector3.one * decalScale;
            }
            else
            {
                decalScale += 2f;
                decalInstance.transform.localScale = Vector3.one * decalScale;
            }
        }

        // ⛰️ Sand pile
        if (sandPilePrefab)
        {
            if (pileInstance == null)
            {
                pileInstance = Instantiate(sandPilePrefab, pilePos, Quaternion.identity);
                pileInstance.transform.localScale = Vector3.one * pileScale;
            }
            else
            {
                pileScale += 0.1f;
                pileInstance.transform.localScale = Vector3.one * pileScale;
            }
        }

        // 🌪️ Partículas (posición y visibilidad)
        if (digParticles)
        {
            Vector3 dirToPile = (digOrigin.position - playerTransform.position).normalized;
            Vector3 particlePos = playerTransform.position - dirToPile * 0.5f + Vector3.up * 1.2f;

            digParticles.transform.position = particlePos;
            digParticles.transform.rotation = Quaternion.LookRotation(Vector3.up);
            digParticles.Clear();
            digParticles.Play();

            Debug.DrawRay(particlePos, Vector3.up * 0.5f, Color.cyan, 1.5f);
        }

        // 🔊 Sonido
        if (digSound)
        {
            audioSource.PlayOneShot(digSound);
        }
    }
}
