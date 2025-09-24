using UnityEngine;

/// <summary>
/// Ajusta din�micamente el Sorting Order de un SpriteRenderer basado en la posici�n Y del objeto.
/// Esto asegura un correcto renderizado 2.5D incluso con una c�mara en perspectiva.
/// Requiere que el pivote del sprite est� en la base ("pies") del personaje/objeto.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSorter : MonoBehaviour
{
    // Multiplicador para dar m�s granularidad al sorting order.
    // Un valor m�s alto permite m�s precisi�n en el ordenamiento.
    [SerializeField] private int sortingOrderMultiplier = 100;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Usamos LateUpdate para asegurarnos de que el ordenamiento se aplique
    // DESPU�S de que todos los c�lculos de movimiento en Update() hayan ocurrido.
    private void LateUpdate()
    {
        // La magia est� aqu�:
        // 1. Tomamos la posici�n Y del objeto en el mundo.
        // 2. La multiplicamos por un n�mero negativo (para que un Y m�s alto resulte en un orden menor).
        // 3. Lo convertimos a un entero para asignarlo al sortingOrder.
        spriteRenderer.sortingOrder = (int)(transform.position.y * -sortingOrderMultiplier);
    }
}