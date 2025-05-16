using UnityEngine;

public class BirdFlyAcross : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    public void Init(Vector3 target, float _speed)
    {
        direction = (target - transform.position).normalized;
        speed = _speed;

        float distance = Vector3.Distance(transform.position, target);
        Destroy(gameObject, distance / speed + 2f); // Extra buffer time
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
