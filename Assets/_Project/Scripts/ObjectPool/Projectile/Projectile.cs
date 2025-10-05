using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        // reset della velocità ogni volta che il proiettile viene preso dalla pool
        if (rb != null)
            rb.velocity = Vector3.zero;
    }

    public void LaunchStraight(Vector3 velocity)
    {
        rb.useGravity = false;
        rb.velocity = velocity;
    }

    void OnTriggerEnter(Collider other)
    {
        LifeController life = other.GetComponent<LifeController>();
        if (life != null)
        {
            life.TakeDamage(damage);
        }
        gameObject.SetActive(false);
    }
}
