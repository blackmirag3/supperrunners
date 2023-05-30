using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour, IHoldable
{

    public GunFire gun;
    public Rigidbody rb;
    public BoxCollider col;
    public Collider damageCol;
    public Rigidbody damageRb;

    public float throwForwardForce, throwUpForce;

    public bool equipped;
    private float despawnTime = 2f;
    private bool isThrown = false;

    void Start()
    {
        gun = GetComponent<GunFire>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();

        isThrown = false;
        damageCol.enabled = false;
        damageRb.isKinematic = true;

        if (equipped)
        {
            gun.enabled = true;
            rb.isKinematic = true;
            col.isTrigger = true;
        }
        else if (!equipped)
        {
            gun.enabled = false;
            rb.isKinematic = false;
            col.isTrigger = false;
        }
    }

    public void Pickup(Transform hand)
    {
        equipped = true;

        // Disable forces acting on gun and BoxCollider a trigger
        rb.isKinematic = true;
        col.isTrigger = true;

        // Pickup by making gun a child of hand
        transform.SetParent(hand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        // Enable gun shooting
        gun.enabled = true;
    }

    public void Throw(Vector3 dir, Vector3 velocity)
    {
        equipped = false;

        transform.SetParent(null);
        
        // Enable damage hitboxes
        damageRb.isKinematic = false;
        damageCol.enabled = true;

        // Throw gun
        damageRb.velocity = velocity;
        damageRb.AddForce(dir.normalized * throwForwardForce, ForceMode.Impulse);
        damageRb.AddForce(transform.up * throwUpForce, ForceMode.Impulse);

        float random = Random.Range(-1f, 1f);
        damageRb.AddTorque(new Vector3(random, random, random));

        gun.enabled = false;

        isThrown = true;
        Destroy(gameObject, despawnTime);
    }
 
    private void OnTriggerEnter(Collider other)
    {
        if (isThrown && other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Disabling collider");
            DisableCol();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isThrown && collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Disabling collider with collision");
            DisableCol();
        }   
    }

    // Prevent multiple collision hits on enemy but its not working
    private void DisableCol()
    {
        damageCol.enabled = false;
    }
}
