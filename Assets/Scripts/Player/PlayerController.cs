using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using System;


/// Controla el movimiento, las acciones y el estado del jugador.
/// Utiliza el nuevo Input System de Unity.

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float p_health = 30f;
    private float maxHealth = 30f;

    public event Action<float> OnHealthChanged; // 👈 evento que notifica la vida actual

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    
    private PlayerMaterialsCounter materialsCounter;
    [SerializeField] private GameObject pauseMenu;

    private Rigidbody rb;
    private Vector2 moveInput;
    private PlayerInputActions playerInputActions;
    private PlayerDodge playerDodge;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerDodge = GetComponent<PlayerDodge>();
        
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMoveCanceled;
        playerInputActions.Player.Dodge.performed += OnDodge;
    }

    private void Start()
    {
        // Notificar el valor inicial de vida
        OnHealthChanged?.Invoke(p_health);
    }

    private void FixedUpdate()
    {
        if (!playerDodge.isDodging)
        {
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            rb.velocity = moveDirection * moveSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (playerDodge != null)
            playerDodge.StartDodge(moveInput);
    }

    private void OnEnable()
    {
        playerInputActions?.Player.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    public void TakeDamage(float damageAmount)
    {
        p_health -= damageAmount;
        p_health = Mathf.Clamp(p_health, 0, maxHealth);

        Debug.Log($"Player took {damageAmount} damage. Current health: {p_health}");
        StartCoroutine(FlashRed());

        OnHealthChanged?.Invoke(p_health); // 👈 Notificamos el cambio

        if (p_health <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        p_health = Mathf.Clamp(p_health + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(p_health); // 👈 Notificamos el cambio
    }

    private IEnumerator FlashRed()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        Color[] originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].color;
            renderers[i].color = Color.red;
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].color = originalColors[i];
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        PlayerMaterialsCounter counter = GetComponent<PlayerMaterialsCounter>();
        if (GameManager.Instance != null && counter != null)
            GameManager.Instance.SaveMaterials(counter);

        GameManager.Instance.LoadScene("SanctumTheater");
    }

    public float GetHealth() => p_health;
}