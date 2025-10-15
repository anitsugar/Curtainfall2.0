using System;
using UnityEngine;

/// <summary>
/// Script gen�rico para un portal que transporta al jugador a otra escena
/// y le indica a un PlayerSpawner d�nde debe aparecer.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    
    [Header("Escenas según el RoomElement de la Zona 1")]
    [SerializeField] private string lightSceneName = "LightRoom";
    [SerializeField] private string darkSceneName = "DarkRoom";

    [Tooltip("ID del SpawnPoint en la escena de destino")]
    [SerializeField] private string destinationSpawnPointId = "Spawn_Default";

    [Header("Referencias a las zonas")]
    public DropZoneUI zone1;
    public DropZoneUI zone2;
    public DropZoneUI zone3;
    
    //para el GameManager
    public int zoneNumber = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Validar referencias
        if (zone1 == null || zone2 == null || zone3 == null)
        {
            Debug.LogError("Una o más zonas no están asignadas en el portal.");
            return;
        }

        // Verificar si todas las zonas 1,2,3 tienen elemento asignado (no None)
        if (zone1.roomElement == DropZoneUI.RoomElement.None ||
            zone2.roomElement == DropZoneUI.RoomElement.None ||
            zone3.roomElement == DropZoneUI.RoomElement.None)
        {
            Debug.Log("❌ No todas las zonas están completas. El portal no se activará.");
            return;
        }

        // Determinar escena según Zona 1
        string sceneToLoadName = null;
        switch (zone1.roomElement)
        {
            case DropZoneUI.RoomElement.Light:
                sceneToLoadName = lightSceneName;
                break;
            case DropZoneUI.RoomElement.Dark:
                sceneToLoadName = darkSceneName;
                break;
        }

        if (string.IsNullOrEmpty(sceneToLoadName))
        {
            Debug.LogError("No se pudo determinar la escena a cargar según el RoomElement de la Zona 1.");
            return;
        }

        // Guardar Zonas 2 y 3 para la próxima sala
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveZones(zone2.roomElement, zone3.roomElement);
            GameManager.Instance.SetCurrentZoneNumber(zoneNumber);
            GameManager.Instance.SetNextSpawnPoint(destinationSpawnPointId);
            GameManager.Instance.LoadScene(sceneToLoadName);
        }
        else
        {
            Debug.LogError("❌ No se encontró GameManager en la escena.");
        }
        
        if (GameManager.Instance != null)
        {
            // Incrementa el número de zona
            GameManager.Instance.IncrementZoneNumber();

            // Luego carga la escena normalmente
            GameManager.Instance.SetNextSpawnPoint(destinationSpawnPointId);
            GameManager.Instance.LoadScene(sceneToLoadName);
        }
    }

    private void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }
    
    
}
