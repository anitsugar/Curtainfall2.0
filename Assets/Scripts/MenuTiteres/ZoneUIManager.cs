using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneUIManager : MonoBehaviour
{
    public static ZoneUIManager Instance;

    public enum AdjustmentMode { None, Light, Dark }
    public AdjustmentMode currentMode = AdjustmentMode.None;

    private void Awake()
    {
        Instance = this;
    }

    // Ejemplo: busca al MasterTeather en la escena
    private DropZoneUI FindMaster()
    {
        DropZoneUI[] masters = GameObject.FindObjectsOfType<DropZoneUI>();
        foreach (var dz in masters)
        {
            if (dz.MasterTeather) return dz;
        }
        return null;
    }

    public void SetLightMode()
    {
        currentMode = AdjustmentMode.Light;
        DropZoneUI master = FindMaster();
        if (master == null) return;

        // Sumar 50 a Light, restar 50 a Dark (mantener límites 0-100)
        int delta = 50;
        master.LightPercentage = Mathf.Min(master.LightPercentage + delta, 100);
        master.DarkPercentage  = Mathf.Max(master.DarkPercentage - delta, 0);
        master.UpdateTMP();

        // Actualiza sprites de PuppetTeather
        UpdateAllPuppetTeatherSprites(master);
    }

    public void SetDarkMode()
    {
        currentMode = AdjustmentMode.Dark;
        DropZoneUI master = FindMaster();
        if (master == null) return;

        int delta = 50;
        master.DarkPercentage  = Mathf.Min(master.DarkPercentage + delta, 100);
        master.LightPercentage = Mathf.Max(master.LightPercentage - delta, 0);
        master.UpdateTMP();

        UpdateAllPuppetTeatherSprites(master);
    }

    private void UpdateAllPuppetTeatherSprites(DropZoneUI master)
    {
        DropZoneUI[] childZones = master.GetComponentsInChildren<DropZoneUI>();
        foreach (var child in childZones)
        {
            if (child.PuppetTeather)
                child.UpdatePuppetTeatherSprite(); // nuevo método que tenés que crear en DropZoneUI
        }
    }
    
}