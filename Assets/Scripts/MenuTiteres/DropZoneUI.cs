using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZoneUI : MonoBehaviour, IDropHandler, IPointerDownHandler, IPointerUpHandler
{
   public enum RoomElement
    {
        None,
        Light,
        Dark,
        SanctumTheater
    }

    [Header("Configuración de zona")]
    [Tooltip("Número de zona (2 o 3) para identificar cada zona en GameManager.")]
    public int zoneIndex; // 2 o 3 según tu GameManager

    [Header("Estado actual")]
    public RoomElement roomElement = RoomElement.None;
    private GameObject currentPuppet;

    [Header("Porcentajes de afinidad")]
    [Range(0, 100)] public float LightPercentage = 50f;
    [Range(0, 100)] public float DarkPercentage = 50f;

    private bool isHeld = false;
    private Coroutine holdCoroutine;

    // ------------------------------------------------------
    // --- DROP DE PUPPET ---
    // ------------------------------------------------------
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        if (!dropped.CompareTag("TeatherPuppet"))
        {
            ResetZone();
            Debug.Log($"🟧 Zona {zoneIndex}: Objeto inválido.");
            return;
        }

        // Lo hace hijo de la zona
        dropped.transform.SetParent(transform);

        // Centrar en la zona
        RectTransform droppedRect = dropped.GetComponent<RectTransform>();
        if (droppedRect != null)
            droppedRect.anchoredPosition = Vector2.zero;

        currentPuppet = dropped;

        UpdateRoomElement();
        Debug.Log($"🟦 Zona {zoneIndex} asignada con puppet ({roomElement})");
    }

    private void Update()
    {
        if (currentPuppet != null && currentPuppet.transform.parent != transform)
        {
            ResetZone();
        }
    }

    public void ResetZone()
    {
        currentPuppet = null;
        roomElement = RoomElement.None;
        LightPercentage = 50f;
        DarkPercentage = 50f;

        SaveToGameManager();
        Debug.Log($"🔄 Zona {zoneIndex} reseteada (sin puppet).");
    }

    // ------------------------------------------------------
    // --- MANEJO DE PORCENTAJES ---
    // ------------------------------------------------------
    public void SetLightPercentage(float value)
    {
        LightPercentage = Mathf.Clamp(value, 0, 100);
        DarkPercentage = 100 - LightPercentage;
        UpdateRoomElement();
    }

    public void SetDarkPercentage(float value)
    {
        DarkPercentage = Mathf.Clamp(value, 0, 100);
        LightPercentage = 100 - DarkPercentage;
        UpdateRoomElement();
    }

    private void UpdateRoomElement()
    {
        if (currentPuppet == null)
        {
            roomElement = RoomElement.None;
        }
        else
        {
            if (Mathf.Approximately(LightPercentage, 50f) && Mathf.Approximately(DarkPercentage, 50f))
            {
                int random = Random.Range(1, 101);
                roomElement = (random % 2 == 0) ? RoomElement.Light : RoomElement.Dark;
            }
            else
            {
                roomElement = (DarkPercentage > LightPercentage) ? RoomElement.Dark : RoomElement.Light;
            }
        }

        SaveToGameManager();
        Debug.Log($"🌗 Zona {zoneIndex}: Light={LightPercentage:F1} / Dark={DarkPercentage:F1} => {roomElement}");
    }

    // ------------------------------------------------------
    // --- GUARDAR EN GAMEMANAGER ---
    // ------------------------------------------------------
    private void SaveToGameManager()
    {
        if (GameManager.Instance == null) return;

        if (zoneIndex == 2)
            GameManager.Instance.SaveZone2(roomElement, LightPercentage, DarkPercentage);
        else if (zoneIndex == 3)
            GameManager.Instance.SaveZone3(roomElement, LightPercentage, DarkPercentage);
    }

    // ------------------------------------------------------
    // --- INPUT PARA MANTENER APRETADO ---
    // ------------------------------------------------------
    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentPuppet == null)
        {
            Debug.Log($"⚠️ Zona {zoneIndex}: No hay puppet, no se pueden ajustar valores.");
            return;
        }

        if (ZoneUIManager.Instance == null)
        {
            Debug.LogError("❌ ZoneUIManager.Instance es NULL.");
            return;
        }

        if (ZoneUIManager.Instance.currentMode == ZoneUIManager.AdjustmentMode.None)
        {
            Debug.Log($"⚠️ Zona {zoneIndex}: No hay modo activo.");
            return;
        }

        isHeld = true;
        holdCoroutine = StartCoroutine(HoldIncrease());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isHeld) return;
        isHeld = false;

        if (holdCoroutine != null)
            StopCoroutine(holdCoroutine);

        ZoneUIManager.Instance.ClearMode();
    }

    private IEnumerator HoldIncrease()
    {
        while (isHeld)
        {
            if (ZoneUIManager.Instance == null) yield break;

            var mode = ZoneUIManager.Instance.currentMode;

            if (mode == ZoneUIManager.AdjustmentMode.Light)
                SetLightPercentage(LightPercentage + Time.deltaTime * 20f);
            else if (mode == ZoneUIManager.AdjustmentMode.Dark)
                SetDarkPercentage(DarkPercentage + Time.deltaTime * 20f);

            yield return null;
        }
    }
}
