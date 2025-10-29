using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager Singleton que persiste entre escenas.
/// Gestiona el estado del juego, la carga de niveles y los datos de transici�n.
/// </summary>

public class GameManager : MonoBehaviour
{
   public static GameManager Instance { get; private set; }
   public delegate void ZoneChangedHandler(int newZoneIndex);
   public event ZoneChangedHandler OnZoneChanged;

    // --- Spawn points ---
    private string nextSpawnPointId;

    // --- Datos persistentes del jugador ---
    public int lightEssence { get; private set; }
    public int darkEssence { get; private set; }

    // --- Datos de las zonas ---
    private DropZoneUI.DungeonElement zone1Element = DropZoneUI.DungeonElement.None;
    private DropZoneUI.DungeonElement zone2Element = DropZoneUI.DungeonElement.None;
    private DropZoneUI.DungeonElement zone3Element = DropZoneUI.DungeonElement.None;

    private float zone1LightPercentage = 50f;
    private float zone1DarkPercentage = 50f;

    private float zone2LightPercentage = 50f;
    private float zone2DarkPercentage = 50f;

    private float zone3LightPercentage = 50f;
    private float zone3DarkPercentage = 50f;

    private int zoneNumber = 1; // Empieza en 1, incrementa al pasar por portal

    // ------------------------------------------------------
    // --- Inicialización del Singleton ---
    // ------------------------------------------------------
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

    // ------------------------------------------------------
    // --- Lógica de spawn ---
    // ------------------------------------------------------
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

    // ------------------------------------------------------
    // --- Lógica de recursos del jugador ---
    // ------------------------------------------------------
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

    // ------------------------------------------------------
    // --- Lógica de zonas ---
    // ------------------------------------------------------

    // Zona 1
    public void SaveZone1(DropZoneUI.DungeonElement element, float light, float dark)
    {
        zone1Element = element;
        zone1LightPercentage = light;
        zone1DarkPercentage = dark;
        Debug.Log($"💾 Zona 1 guardada: Element={element}, Light={light}, Dark={dark}");
    }

    // Zona 2
    public void SaveZone2(DropZoneUI.DungeonElement element, float light, float dark)
    {
        zone2Element = element;
        zone2LightPercentage = light;
        zone2DarkPercentage = dark;
        Debug.Log($"💾 Zona 2 guardada: Element={element}, Light={light}, Dark={dark}");
    }

    // Zona 3
    public void SaveZone3(DropZoneUI.DungeonElement element, float light, float dark)
    {
        zone3Element = element;
        zone3LightPercentage = light;
        zone3DarkPercentage = dark;
        Debug.Log($"💾 Zona 3 guardada: Element={element}, Light={light}, Dark={dark}");
    }

    // ------------------------------------------------------
    // --- Getters de RoomElement ---
    // ------------------------------------------------------
    public DropZoneUI.DungeonElement GetZone1Element() => zone1Element;
    public DropZoneUI.DungeonElement GetZone2Element() => zone2Element;
    public DropZoneUI.DungeonElement GetZone3Element() => zone3Element;

    // ------------------------------------------------------
    // --- Getters de Porcentajes ---
    // ------------------------------------------------------
    public float GetZone1Light() => zone1LightPercentage;
    public float GetZone1Dark() => zone1DarkPercentage;

    public float GetZone2Light() => zone2LightPercentage;
    public float GetZone2Dark() => zone2DarkPercentage;

    public float GetZone3Light() => zone3LightPercentage;
    public float GetZone3Dark() => zone3DarkPercentage;

    // ------------------------------------------------------
    // --- Número de zona actual ---
    // ------------------------------------------------------
    public int GetCurrentZoneNumber()
    {
        return zoneNumber;
    }

    public void IncrementZoneNumber()
    {
        zoneNumber++;
        Debug.Log($"ZoneNumber incrementado: {zoneNumber}");
    }

    public void SetCurrentZoneNumber(int number)
    {
        zoneNumber = number;
        Debug.Log($"SetCurrentZoneNumber: {zoneNumber}");
    }

    // ------------------------------------------------------
    // --- Compatibilidad (por si aún lo usa algún script viejo) ---
    // ------------------------------------------------------
    public void SaveZones(DropZoneUI.DungeonElement zone2, DropZoneUI.DungeonElement zone3)
    {
        zone2Element = zone2;
        zone3Element = zone3;
        Debug.Log($"SaveZones (compatibilidad): Zona2={zone2Element}, Zona3={zone3Element}");
    }
}