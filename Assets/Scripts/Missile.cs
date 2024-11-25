using Unity.Collections;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public Rigidbody rb;
    public bool launched = false;
    public bool destroyed = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void LaunchMissile()
    {
        launched = true;
        rb.isKinematic = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!launched || destroyed || transform.IsChildOf(other.transform))
        {
            return;
        }

        rb.useGravity = true;
        GetComponentInChildren<Renderer>().enabled = false;
        transform.Translate(transform.forward * (rb.linearVelocity.magnitude * 1e-6F));
        
        Vector3 explosionPos = transform.position;

        foreach (var i in new int[]{1, 2, 3})
        {
            foreach (Collider hit in Physics.OverlapSphere(explosionPos, 2 * i))
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb == null)
                {
                    continue;
                }
            
                rb.AddExplosionForce(100 * i*i*i, explosionPos, 3 * i, 0);
            }
        }
        Destroy(gameObject, 0.2F);
    }
    
    private void FixedUpdate()
    {
        if (!launched || destroyed)
        {
            return;
        }

        if (rb.mass > 20)
        {
            rb.AddForce(transform.forward * 300f, ForceMode.Impulse);
            rb.mass -= 0.01F;
        }
    }
}