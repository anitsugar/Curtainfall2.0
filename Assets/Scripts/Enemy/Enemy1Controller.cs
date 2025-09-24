using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Enemy1Controller : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform playerTransform;

    [SerializeField] private PlayerController player;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Health Settings")]
    [SerializeField] private float e_health = 15f;


    private Rigidbody rb;
    private Renderer enemyRenderer;

    private bool canMove = true;
    private Color originalColor;
    private bool isCollidingWithPlayer = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyRenderer = GetComponentInChildren<Renderer>();
        
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }

        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player GameObject with 'Player' tag not found in the scene.");
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            directionToPlayer.y = 0;
            rb.velocity = directionToPlayer * moveSpeed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            player = playerController;
            StopAllCoroutines(); 
            canMove = false;
            StartCoroutine(DamageSequence());
        }
    }


    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
        }
    }

    private IEnumerator DamageSequence()
    {
        if (enemyRenderer != null)
        {
            
            while (isCollidingWithPlayer)
            {
                
                yield return new WaitForSeconds(0.2f);
                
                
                if (!isCollidingWithPlayer) break;

                
                enemyRenderer.material.color = Color.yellow;
                yield return new WaitForSeconds(0.5f);
                
                
                enemyRenderer.material.color = Color.red;
                EnemyDoDamage();
                yield return new WaitForSeconds(0.2f);
                
                enemyRenderer.material.color = originalColor;
                yield return new WaitForSeconds(0.5f);
            }
        }

        
        
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor;
        }

        
        yield return new WaitForSeconds(0.3f);

        
        canMove = true;
    }

    public void EnemyTakeDamage(float damageAmount)
    {
        e_health -= damageAmount;
        
        Debug.Log("Enemy took " + damageAmount + " damage. Current health: " + e_health);
        
        if (e_health <= 0)
        {
            Die();
        }
        else
        {
            // Detenemos cualquier corutina de feedback de daño anterior
            // y comenzamos una nueva.
            StopCoroutine("HandleDamageFeedback");
            StartCoroutine("HandleDamageFeedback");
        }
    }
    
    // Nueva corutina para manejar el feedback visual del daño
    private IEnumerator HandleDamageFeedback()
    {
        // Cambia el color a azul
        enemyRenderer.material.color = Color.blue;
        
        // Espera 0.2 segundos
        yield return new WaitForSeconds(0.2f);
        
        // Vuelve al color original
        enemyRenderer.material.color = originalColor;
    }

    public void EnemyDoDamage()
    {
        if (isCollidingWithPlayer)
        {
            player.TakeDamage(5f);
        }
    }

    private void Die()
    {
        Debug.Log("Enemy has died.");
        Destroy(gameObject);
    } 
}
