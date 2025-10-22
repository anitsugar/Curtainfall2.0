using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    [Header("Projectile Settings")]
    public float damageAmount = 10f;  // 👈 Daño configurable
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy1Controller enemy = collision.gameObject.GetComponent<Enemy1Controller>();
            if (enemy != null)
            {
                Vector3 hitDirection = -collision.contacts[0].normal; // dirección del disparo
                enemy.EnemyTakeDamage(damageAmount, hitDirection);
            }

            Destroy(gameObject);
        }
    }
}
