using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rocket : MonoBehaviour
{
    [Header("Flight trail and explosion setup:")]
    public float speed = 40f;
    public float explosionRadius = 6f;
    public float explosionForce = 800f;
    public float damage = 100f;

    [Header("Effects:")]
    public GameObject explosionParticles;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, 10f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(explosionParticles != null)
            Instantiate(explosionParticles, transform.position, Quaternion.identity);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider hit in colliders)
        {
            IDamageable target = hit.GetComponentInParent<IDamageable>();
            target?.TakeDamage(damage);
            Rigidbody hitRb = hit.GetComponent<Rigidbody>();
            
            if (hitRb != null)
                hitRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            if (hit.transform.CompareTag("Destroyable"))
                Destroy(hit.transform.gameObject);
        }

        Destroy(gameObject);
    }
}
