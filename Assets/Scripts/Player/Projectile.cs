using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    public float lifeTime = 3f;

    void Start()
    {
        
        Destroy(gameObject, lifeTime);
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy1Controller enemy1Controller = collision.gameObject.GetComponent<Enemy1Controller>();
            enemy1Controller.EnemyTakeDamage(5f);

        }
        
        Destroy(gameObject);
    }

}
