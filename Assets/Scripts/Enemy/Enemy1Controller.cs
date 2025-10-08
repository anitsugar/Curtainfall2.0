using System.Collections;
using System.Collections.Generic;
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

    [Header("Drop Settings")]
    [SerializeField] private GameObject dropPrefab;

    // --- Rendering / Tint (_Tint via MPB) ---
    private static readonly int ID_Tint = Shader.PropertyToID("_Tint");
    private Renderer enemyRenderer;
    private MaterialPropertyBlock mpb;
    private Color originalTint = Color.white; // default si el material no define otro

    private Rigidbody rb;

    private bool canMove = true;
    private bool isCollidingWithPlayer = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyRenderer = GetComponentInChildren<Renderer>();

        // Cachear Player por tag si no está asignado
        if (!playerTransform)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null) playerTransform = playerObject.transform;
            else Debug.LogError("Player GameObject with 'Player' tag not found in the scene.");
        }

        // Inicializar MPB y leer tint base (si existe en el material)
        if (enemyRenderer != null)
        {
            mpb = new MaterialPropertyBlock();
            enemyRenderer.GetPropertyBlock(mpb);

            // Intentar leer un Tint existente (si no hay, usar blanco)
            Color maybeTint = mpb.GetColor(ID_Tint);
            if (maybeTint == default) maybeTint = Color.white;
            originalTint = maybeTint;

            // Asegurar que arrancamos con el tint base
            SetTint(originalTint);
        }
    }

    void FixedUpdate()
    {
        if (canMove && playerTransform != null)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            directionToPlayer.y = 0f;
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
            player = collision.gameObject.GetComponent<PlayerController>();

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

                // Flash amarillo -> rojo -> original
                SetTint(Color.yellow);
                yield return new WaitForSeconds(0.5f);

                SetTint(Color.red);
                EnemyDoDamage();
                yield return new WaitForSeconds(0.2f);

                SetTint(originalTint);
                yield return new WaitForSeconds(0.5f);
            }
        }

        // Restaurar color por si salió del bucle sin último reset
        SetTint(originalTint);

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
            StopCoroutine(nameof(HandleDamageFeedback));
            StartCoroutine(nameof(HandleDamageFeedback));
        }
    }

    // Feedback de daño breve (azul)
    private IEnumerator HandleDamageFeedback()
    {
        SetTint(Color.blue);
        yield return new WaitForSeconds(0.2f);
        SetTint(originalTint);
    }

    public void EnemyDoDamage()
    {
        if (isCollidingWithPlayer && player != null)
        {
            player.TakeDamage(5f);
        }
    }

    private void Die()
    {
        Debug.Log("Enemy has died.");

        if (dropPrefab != null)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No dropPrefab asignado en el enemigo.");
        }

        Destroy(gameObject);
    }

    // --- Utilidades de Tint (_Tint via MPB) ---
    private void SetTint(Color c)
    {
        if (enemyRenderer == null) return;
        if (mpb == null) mpb = new MaterialPropertyBlock();

        enemyRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(ID_Tint, c);
        enemyRenderer.SetPropertyBlock(mpb);
    }
}
