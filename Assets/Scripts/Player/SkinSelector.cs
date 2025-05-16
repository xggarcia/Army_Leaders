using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinSelector : MonoBehaviour
{

    public GameObject hatSkin;
    public GameObject shipSkin;


    // Start is called before the first frame update
    void Start()
    {
       SetSkin(false); // Start with the hat active

    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Skin Is Hat"))
        {
            SetSkin(true);
        }
        if (other.CompareTag("Skin Is Ship"))
        {
            SetSkin(false);
        }
    }

    void SetSkin(bool useHat)
    {
        if (useHat == true)
        {
            hatSkin.SetActive(true);
            shipSkin.SetActive(false);
        }
        else
        {
            hatSkin.SetActive(false);
            shipSkin.SetActive(true);
        }
    }
}
