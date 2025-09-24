using UnityEngine;

/// <summary>
/// Un simple marcador para identificar un punto de aparici�n en la escena.
/// Solo contiene un ID para que el PlayerSpawner lo encuentre.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [Tooltip("El identificador �nico para este punto de aparici�n.")]
    public string spawnId;
}
