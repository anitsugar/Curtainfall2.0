using UnityEngine;

/// <summary>
/// Script gen�rico para un portal que transporta al jugador a otra escena
/// y le indica a un PlayerSpawner d�nde debe aparecer.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    [Tooltip("El nombre exacto de la escena a la que se va a cargar.")]
    [SerializeField] private string sceneToLoadName;

    [Tooltip("El ID del SpawnPoint en la escena de destino donde aparecer� el jugador.")]
    [SerializeField] private string destinationSpawnPointId;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null && !string.IsNullOrEmpty(sceneToLoadName))
            {
                // Antes de cargar la escena, le decimos al GameManager d�nde spawnear.
                GameManager.Instance.SetNextSpawnPoint(destinationSpawnPointId);
                GameManager.Instance.LoadScene(sceneToLoadName);
            }
            else
            {
                Debug.LogError("Nombre de escena no asignado en el portal o no se encontr� el GameManager.");
            }
        }
    }

    private void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }
}
