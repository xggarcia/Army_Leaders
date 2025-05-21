using System.Collections;
using UnityEngine;

public class CubeBehaviour : MonoBehaviour
{
    [Header("Disappearance Settings")]
    public float rotationSpeed = 720f;        // Degrees per second
    public float shrinkDuration = 1.0f;       // How long to shrink
    public float reappearDelay = 5.0f;        // How long before reappearing

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    public void CubeActivation()
    {
        StartCoroutine(AnimateCube());
    }

    private IEnumerator AnimateCube()
    {
        float elapsed = 0f;

        // Phase 1: Rotate and shrink
        while (elapsed < shrinkDuration)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            float t = elapsed / shrinkDuration;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final scale zero and hide
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);

        // Phase 2: Wait, then reappear
        yield return new WaitForSeconds(reappearDelay);

        // Reactivate and restore scale
        transform.localScale = initialScale;
        gameObject.SetActive(true);
    }
}
