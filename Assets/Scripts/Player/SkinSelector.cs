using System.Collections;
using UnityEngine;

public class SkinSelector : MonoBehaviour
{
    public GameObject hatSkin;
    public GameObject shipSkin;
    public GameObject skinEffectPrefab;

    private bool isWearingHat = false; // tracks current state

    void Start()
    {
        SetSkin(false); // Start with the hat active
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Skin Is Hat") && !isWearingHat)
        {
            SetSkin(true);
        }
        else if (other.CompareTag("Skin Is Ship") && isWearingHat)
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

        isWearingHat = useHat; // update current state

        // ✅ Spawn and animate particle effect only if skin changes
        if (skinEffectPrefab != null)
        {
            GameObject fx = Instantiate(skinEffectPrefab, transform.position + Vector3.up * 1.8f, Quaternion.identity);
            fx.transform.SetParent(transform); // follow player
            StartCoroutine(ScaleAndDestroy(fx));
        }
    }

    private IEnumerator ScaleAndDestroy(GameObject fx)
    {
        float duration = 0.74f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 2.5f;
        Vector3 endScale = Vector3.one * 0.1f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            fx.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fx.transform.localScale = endScale;
        Destroy(fx);
    }
}
