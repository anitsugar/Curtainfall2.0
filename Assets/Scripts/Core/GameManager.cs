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

    // --- Datos de las zonas ---
    private DropZoneUI.RoomElement zone2Element = DropZoneUI.RoomElement.None;
    private DropZoneUI.RoomElement zone3Element = DropZoneUI.RoomElement.None;
    private int zoneNumber = 1; // Empieza en 1, incrementa al pasar por portal

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

    // -------------------------------
    // Lógica de zonas
    // -------------------------------
    public void SetZone2Element(DropZoneUI.RoomElement element)
    {
        zone2Element = element;
        Debug.Log($"Zona 2 guardada: {zone2Element}");
    }

    public void SetZone3Element(DropZoneUI.RoomElement element)
    {
        zone3Element = element;
        Debug.Log($"Zona 3 guardada: {zone3Element}");
    }
    

    public DropZoneUI.RoomElement GetZone2Element()
    {
        return zone2Element;
    }

    public DropZoneUI.RoomElement GetZone3Element()
    {
        return zone3Element;
    }

    public int GetCurrentZoneNumber()
    {
        return zoneNumber;
    }

    public void IncrementZoneNumber()
    {
        zoneNumber++;
        Debug.Log($"ZoneNumber incrementado: {zoneNumber}");
    }

    // -------------------------------
    // Funciones agregadas para compatibilidad
    // -------------------------------

    /// <summary>
    /// Guarda los valores de las zonas actuales.
    /// </summary>
    public void SaveZones(DropZoneUI.RoomElement zone2, DropZoneUI.RoomElement zone3)
    {
        zone2Element = zone2;
        zone3Element = zone3;
        Debug.Log($"SaveZones: Zona2={zone2Element}, Zona3={zone3Element}");
    }

    /// <summary>
    /// Asigna manualmente el número de zona actual.
    /// </summary>
    public void SetCurrentZoneNumber(int number)
    {
        zoneNumber = number;
        Debug.Log($"SetCurrentZoneNumber: {zoneNumber}");
    }
    
    
}