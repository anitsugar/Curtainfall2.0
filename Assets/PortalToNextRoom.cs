using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Collider))]
public class PortalToNextRoom : MonoBehaviour
{
    [Header("Escenas a cargar")]
    [SerializeField] private string lightRoomScene = "LightRoom1";
    [SerializeField] private string darkRoomScene = "DarkRoom1";
    [SerializeField] private string sanctumScene = "SanctumTheater";

    [Tooltip("ID del SpawnPoint al llegar")]
    [SerializeField] private string destinationSpawnPointId = "Spawn_Default";

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("No se encontró GameManager en la escena.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance == null) return;

        int zoneNumber = GameManager.Instance.GetCurrentZoneNumber();
        string sceneToLoad = "";

        if (zoneNumber == 1)
        {
            // Hereda elemento de Zona2
            var element = GameManager.Instance.GetZone2Element();
            sceneToLoad = element == DropZoneUI.RoomElement.Light ? lightRoomScene :
                          element == DropZoneUI.RoomElement.Dark ? darkRoomScene : "";
        }
        else if (zoneNumber == 2)
        {
            // Hereda elemento de Zona3
            var element = GameManager.Instance.GetZone3Element();
            sceneToLoad = element == DropZoneUI.RoomElement.Light ? lightRoomScene :
                          element == DropZoneUI.RoomElement.Dark ? darkRoomScene : "";
        }
        else if (zoneNumber >= 3)
        {
            // Zona 3 -> directamente Sanctum
            sceneToLoad = sanctumScene;
        }

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("Portal no tiene un RoomElement válido, no se teletransporta.");
            return;
        }

        // Guardar el spawn point y cargar la escena
        GameManager.Instance.SetNextSpawnPoint(destinationSpawnPointId);
        GameManager.Instance.LoadScene(sceneToLoad);

        // Incrementar zoneNumber
        GameManager.Instance.IncrementZoneNumber();
    }

    private void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }
}
