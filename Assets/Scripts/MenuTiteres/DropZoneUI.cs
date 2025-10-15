using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZoneUI : MonoBehaviour, IDropHandler
{
    public enum RoomElement
    {
        None,
        Light,
        Dark,
        SanctumTheater
    }

    [Header("Configuración de zona")]
    [Tooltip("Número de zona (0, 1, 2 o 3) para identificar el tipo de habitación.")]
    public int zoneIndex; // Usa este número para identificar cada zona

    [Header("Estado actual")]
    public RoomElement roomElement = RoomElement.None;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        // 🔹 Lo hace hijo de la zona
        dropped.transform.SetParent(transform);

        // 🔹 Centra el objeto dentro de la zona
        RectTransform droppedRect = dropped.GetComponent<RectTransform>();
        if (droppedRect != null)
        {
            droppedRect.anchoredPosition = Vector2.zero;
        }

        // 🔹 Detecta el tipo de objeto por su tag
        if (dropped.CompareTag("LightTheaterObject"))
        {
            roomElement = RoomElement.Light;
        }
        else if (dropped.CompareTag("DarkTheaterObject"))
        {
            roomElement = RoomElement.Dark;
        }
        else
        {
            roomElement = RoomElement.None;
        }

        Debug.Log($"🟦 Zona {zoneIndex} asignada con elemento: {roomElement}");
    }
}
