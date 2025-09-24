using UnityEngine;

/// <summary>
/// Este script, al iniciar una escena, busca al jugador y lo mueve
/// al SpawnPoint correcto basándose en la información del GameManager.
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance == null) return;

        // Pedimos al GameManager el ID del punto de spawn.
        string spawnId = GameManager.Instance.GetAndClearNextSpawnPoint();

        // Si no hay un ID, significa que es la primera vez que se carga la escena. No hacemos nada.
        if (string.IsNullOrEmpty(spawnId)) return;

        // Buscamos el punto de spawn con el ID correspondiente.
        SpawnPoint destinationSpawnPoint = FindSpawnPointById(spawnId);

        if (destinationSpawnPoint != null)
        {
            // Buscamos al jugador y lo movemos a la posición del spawn point.
            var player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.transform.position = destinationSpawnPoint.transform.position;
                player.transform.rotation = destinationSpawnPoint.transform.rotation;
                Debug.Log($"Jugador movido al SpawnPoint: {spawnId}");
            }
            else
            {
                Debug.LogError("PlayerSpawner no pudo encontrar al Player en la escena.");
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró un SpawnPoint con el ID: '{spawnId}' en la escena actual.");
        }
    }

    private SpawnPoint FindSpawnPointById(string id)
    {
        SpawnPoint[] allSpawnPoints = FindObjectsOfType<SpawnPoint>();
        foreach (SpawnPoint sp in allSpawnPoints)
        {
            if (sp.spawnId == id)
            {
                return sp;
            }
        }
        return null;
    }
}