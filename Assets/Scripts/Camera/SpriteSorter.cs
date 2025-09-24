using UnityEngine;

/// <summary>
/// Ajusta dinámicamente el Sorting Order de un SpriteRenderer basado en la posición Y del objeto.
/// Esto asegura un correcto renderizado 2.5D incluso con una cámara en perspectiva.
/// Requiere que el pivote del sprite esté en la base ("pies") del personaje/objeto.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSorter : MonoBehaviour
{
    // Multiplicador para dar más granularidad al sorting order.
    // Un valor más alto permite más precisión en el ordenamiento.
    [SerializeField] private int sortingOrderMultiplier = 100;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Usamos LateUpdate para asegurarnos de que el ordenamiento se aplique
    // DESPUÉS de que todos los cálculos de movimiento en Update() hayan ocurrido.
    private void LateUpdate()
    {
        // La magia está aquí:
        // 1. Tomamos la posición Y del objeto en el mundo.
        // 2. La multiplicamos por un número negativo (para que un Y más alto resulte en un orden menor).
        // 3. Lo convertimos a un entero para asignarlo al sortingOrder.
        spriteRenderer.sortingOrder = (int)(transform.position.y * -sortingOrderMultiplier);
    }
}