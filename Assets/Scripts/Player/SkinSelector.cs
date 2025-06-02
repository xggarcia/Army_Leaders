using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinSelector : MonoBehaviour
{
    public GameObject hatSkin;
    public GameObject shipSkin;
    public ParticleSystem skinParticles;
    // ✅ Particle effect prefab

    void Start()
    {
        SetSkin(false); // Start with the hat active
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Skin Is Hat"))
        {
            SetSkin(true);
        }
        else if (other.CompareTag("Skin Is Ship"))
        {
            SetSkin(false);
        }
    }

    void SetSkin(bool useHat)
    {
        if (useHat)
        {
            hatSkin.SetActive(true);
            shipSkin.SetActive(false);
        }
        else
        {
            hatSkin.SetActive(false);
            shipSkin.SetActive(true);
        }

        // ✅ Play skin change particles using pre-placed system
        if (skinParticles != null)
        {
            Vector3 offset = Vector3.up * 1.8f;
            skinParticles.transform.position = transform.position + offset;
            skinParticles.transform.rotation = Quaternion.LookRotation(Vector3.up);

            skinParticles.Play();
        }
    }


}
