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
    [System.Serializable]
    public class ZoneData
    {
        public DropZoneUI.RoomElement roomElement = DropZoneUI.RoomElement.None;
        public float LightPercentage = 50f;
        public float DarkPercentage = 50f;

        public ZoneData() { }

        public ZoneData(DropZoneUI.RoomElement element, float light, float dark)
        {
            roomElement = element;
            LightPercentage = light;
            DarkPercentage = dark;
        }
    }

    private ZoneData zone1 = new ZoneData();
    private ZoneData zone2 = new ZoneData();
    private ZoneData zone3 = new ZoneData();

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
    public void SetNextSpawnPoint(string spawnId) => nextSpawnPointId = spawnId;

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
    // Guardar zonas
    // -------------------------------
    public void SaveZone1(DropZoneUI.RoomElement element, float light, float dark)
    {
        zone1.roomElement = element;
        zone1.LightPercentage = light;
        zone1.DarkPercentage = dark;
        Debug.Log($"Zona 1 guardada -> Element={element}, Light={light}, Dark={dark}");
    }

    public void SaveZone2(DropZoneUI.RoomElement element, float light, float dark)
    {
        zone2.roomElement = element;
        zone2.LightPercentage = light;
        zone2.DarkPercentage = dark;
        Debug.Log($"Zona 2 guardada -> Element={element}, Light={light}, Dark={dark}");
    }

    public void SaveZone3(DropZoneUI.RoomElement element, float light, float dark)
    {
        zone3.roomElement = element;
        zone3.LightPercentage = light;
        zone3.DarkPercentage = dark;
        Debug.Log($"Zona 3 guardada -> Element={element}, Light={light}, Dark={dark}");
    }

    // -------------------------------
    // Getters de zonas
    // -------------------------------
    public ZoneData GetZone1Data() => zone1;
    public ZoneData GetZone2Data() => zone2;
    public ZoneData GetZone3Data() => zone3;

    public DropZoneUI.RoomElement GetZone1Element() => zone1.roomElement;
    public DropZoneUI.RoomElement GetZone2Element() => zone2.roomElement;
    public DropZoneUI.RoomElement GetZone3Element() => zone3.roomElement;

    // -------------------------------
    // Compatibilidad con antiguos métodos SaveZones()
    // -------------------------------
    public void SaveZones(DropZoneUI.RoomElement z2, DropZoneUI.RoomElement z3)
    {
        zone2.roomElement = z2;
        zone3.roomElement = z3;
        Debug.Log($"SaveZones: Zona2={zone2.roomElement}, Zona3={zone3.roomElement}");
    }

    // -------------------------------
    // Número de zona
    // -------------------------------
    public int GetCurrentZoneNumber() => zoneNumber;

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
}