using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager Singleton que persiste entre escenas.
/// Gestiona el estado del juego, la carga de niveles y los datos de transici�n.
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // --- Spawn points ---
    private string nextSpawnPointId;

    // --- Datos persistentes del jugador ---
    public int lightEssence { get; private set; }
    public int darkEssence { get; private set; }

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
            return;
        }
        
        if (GameManager.Instance == null)
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/GameManager"));
        }
    }

    // -------------------------------
    // Lógica de spawn
    // -------------------------------
    public void SetNextSpawnPoint(string spawnId)
    {
        nextSpawnPointId = spawnId;
    }

    public string GetAndClearNextSpawnPoint()
    {
        string id = nextSpawnPointId;
        nextSpawnPointId = null; 
        return id;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log($"Cargando escena: {sceneName}");
    }

    // -------------------------------
    // Lógica de recursos del jugador
    // -------------------------------
    public void SaveMaterials(PlayerMaterialsCounter counter)
    {
        lightEssence = counter.lightEssence;
        darkEssence = counter.darkEssence;
    }

    public void LoadMaterials(PlayerMaterialsCounter counter)
    {
        counter.lightEssence = lightEssence;
        counter.darkEssence = darkEssence;
    }
}