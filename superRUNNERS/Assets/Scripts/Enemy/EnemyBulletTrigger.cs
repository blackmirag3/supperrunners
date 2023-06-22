using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletTrigger : MonoBehaviour
{
    [SerializeField]
    private string playerTag = "Player";
    [SerializeField]
    private float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            IDamageable player = other.GetComponent<IDamageable>();
            if (player != null)
            {
                player.Damage(damage);
            }
            Destroy(transform.parent.gameObject);
            //Debug.Log("Player hit detected");
        }
            
    }
}
