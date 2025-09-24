using UnityEngine;

/// <summary>
/// Un simple marcador para identificar un punto de aparición en la escena.
/// Solo contiene un ID para que el PlayerSpawner lo encuentre.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [Tooltip("El identificador único para este punto de aparición.")]
    public string spawnId;
}
