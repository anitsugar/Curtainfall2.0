using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager Singleton que persiste entre escenas.
/// Gestiona el estado del juego, la carga de niveles y los datos de transici�n.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Almacenar� el ID del punto de spawn para la siguiente escena.
    private string nextSpawnPointId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// M�todo llamado por los portales para registrar a d�nde debe ir el jugador.
    /// </summary>
    public void SetNextSpawnPoint(string spawnId)
    {
        nextSpawnPointId = spawnId;
    }

    /// <summary>
    /// M�todo llamado por el PlayerSpawner de la nueva escena para mover al jugador.
    /// Devuelve el ID y lo limpia para evitar re�sos accidentales.
    /// </summary>
    public string GetAndClearNextSpawnPoint()
    {
        string id = nextSpawnPointId;
        nextSpawnPointId = null; // Limpiamos el ID para que no se vuelva a usar.
        return id;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log($"Cargando escena: {sceneName}");
    }
}