using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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

    [Header("VFX Settings")]
    [SerializeField] private GameObject damageVFXPrefab; // ← Prefab del VFX que quieres instanciar
    [SerializeField] private float vfxLifetime = 2f; // ← Cuánto dura antes de destruirse

    // --- Rendering / Tint (_Tint via MPB) ---
    private static readonly int ID_Tint = Shader.PropertyToID("_Tint");
    private Renderer enemyRenderer;
    private MaterialPropertyBlock mpb;
    private Color originalTint = Color.white;

    private Rigidbody rb;
    private bool canMove = true;
    private bool isCollidingWithPlayer = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyRenderer = GetComponentInChildren<Renderer>();

        if (!playerTransform)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null) playerTransform = playerObject.transform;
            else Debug.LogError("Player GameObject with 'Player' tag not found in the scene.");
        }

        if (enemyRenderer != null)
        {
            mpb = new MaterialPropertyBlock();
            enemyRenderer.GetPropertyBlock(mpb);

            Color maybeTint = mpb.GetColor(ID_Tint);
            if (maybeTint == default) maybeTint = Color.white;
            originalTint = maybeTint;

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

                SetTint(Color.yellow);
                yield return new WaitForSeconds(0.5f);

                SetTint(Color.red);
                EnemyDoDamage();
                yield return new WaitForSeconds(0.2f);

                SetTint(originalTint);
                yield return new WaitForSeconds(0.5f);
            }
        }

        SetTint(originalTint);
        yield return new WaitForSeconds(0.3f);
        canMove = true;
    }

    public void EnemyTakeDamage(float damageAmount)
    {
        e_health -= damageAmount;
        Debug.Log("Enemy took " + damageAmount + " damage. Current health: " + e_health);

        // === NUEVO BLOQUE: reproducir VFX ===
        if (damageVFXPrefab != null)
        {
            GameObject vfxInstance = Instantiate(damageVFXPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);

            // Si tiene VisualEffect, darle Play()
            var vfx = vfxInstance.GetComponent<VisualEffect>();
            if (vfx != null)
            {
                vfx.Play();
                Debug.Log($"▶️ VisualEffect reproducido en {gameObject.name}");
            }
            else
            {
                Debug.LogWarning("⚠ El prefab no tiene componente VisualEffect.");
            }

            Destroy(vfxInstance, 3f); // opcional: destruir después
        }
        else
        {
            Debug.LogWarning("⚠ No hay prefab de daño asignado en el enemigo.");
        }

        // === Daño / feedback visual ===
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
            player.TakeDamage(10f);
        }
    }

    private void Die()
    {
        Debug.Log("Enemy has died.");

        if (dropPrefab != null)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void SetTint(Color c)
    {
        if (enemyRenderer == null) return;
        if (mpb == null) mpb = new MaterialPropertyBlock();

        enemyRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(ID_Tint, c);
        enemyRenderer.SetPropertyBlock(mpb);
    }
}

