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

    public void SetLightMode()
    {
        currentMode = AdjustmentMode.Light;
        Debug.Log("🌕 Modo selección: Light");
    }

    public void SetDarkMode()
    {
        currentMode = AdjustmentMode.Dark;
        Debug.Log("🌑 Modo selección: Dark");
    }

    public void ClearMode()
    {
        currentMode = AdjustmentMode.None;
    }
}
