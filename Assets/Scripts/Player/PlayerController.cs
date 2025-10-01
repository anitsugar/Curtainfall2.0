using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;


/// Controla el movimiento, las acciones y el estado del jugador.
/// Utiliza el nuevo Input System de Unity.

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float p_health = 30f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    
    private PlayerMaterialsCounter materialsCounter;
    
    [SerializeField] private GameObject pauseMenu;

    private Rigidbody rb;
    private Vector2 moveInput;
    private PlayerInputActions playerInputActions;

    // Referencia al script PlayerDodge
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

    private void FixedUpdate()
    {
        // Solo mueve al jugador si NO está esquivando
        if (!playerDodge.isDodging)
        {
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            rb.velocity = moveDirection * moveSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu.activeInHierarchy == false)
        {
            pauseMenu.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu.activeInHierarchy == true)
        {
            pauseMenu.SetActive(false);
        }
    }

    // Método que se llama cuando se presiona o suelta una tecla de movimiento
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Este método resetea el input cuando se sueltan las teclas
    public void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    // Este es el único método que necesita el script
    public void OnDodge(InputAction.CallbackContext context)
    {
        if (playerDodge != null)
        {
            playerDodge.StartDodge(moveInput);
        }
    }

    private void OnEnable()
    {
        if (playerInputActions != null)
            playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    public void TakeDamage(float damageAmount)
    {
        p_health -= damageAmount;
        Debug.Log("Player took " + damageAmount + " damage. Current health: " + p_health);

        if (p_health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");

        PlayerMaterialsCounter counter = GetComponent<PlayerMaterialsCounter>();
        Debug.Log("Counter encontrado? " + (counter != null));
        Debug.Log("GameManager existe? " + (GameManager.Instance != null));

        if (GameManager.Instance != null && counter != null)
        {
            GameManager.Instance.SaveMaterials(counter);
        }

        GameManager.Instance.LoadScene("SanctumTheater");
    }
}