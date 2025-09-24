using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    public bool isDodging { get; private set; } // Propiedad para que PlayerController sepa si está esquivando.
    private bool canDodge = true;
    
    [SerializeField] private float dodgeSpeed = 20f;
    [SerializeField] private float dodgeTime = 0.2f;
    [SerializeField] private float dodgeCooldown = 1f;

    private Rigidbody rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        isDodging = false;
    }

    public void StartDodge(Vector2 playerMoveInput)
    {
        if (canDodge)
        {
            moveInput = playerMoveInput;
            StartCoroutine(PerformDodge());
        }
    }

    private IEnumerator PerformDodge()
    {
        isDodging = true; // Empieza el esquive
        canDodge = false; // No se puede esquivar de nuevo

        Vector3 dodgeDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        if (dodgeDirection == Vector3.zero)
        {
            dodgeDirection = transform.forward;
        }

        rb.velocity = dodgeDirection.normalized * dodgeSpeed;

        yield return new WaitForSeconds(dodgeTime);

        // Resetea la velocidad solo si el jugador no está intentando moverse
        if (moveInput == Vector2.zero)
        {
            rb.velocity = Vector3.zero;
        }

        isDodging = false; // Termina el esquive

        yield return new WaitForSeconds(dodgeCooldown);

        canDodge = true; // Puede esquivar de nuevo
    }
}
