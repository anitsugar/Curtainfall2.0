using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro; 

/// <summary>
/// Nueva versión de DropZoneUI (v2).
/// - Tiene 3 modos exclusivos: PuppetScript, PuppetTeather, MasterTeather (Inspector).
/// - MasterTeather => controla 3 hijos DropZoneUI (debe haber 3) y determina PortalUnlocked, nombres y permisos.
/// - PuppetScript acepta tag "PuppetScript".
/// - PuppetTeather acepta tag "TeatherPuppet".
/// - Light/Dark en Master se manejan en pasos de 50 y no se resetean al quitar objetos.
/// - No interactúa con GameManager (por ahora).
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class DropZoneUI : MonoBehaviour, IDropHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum DungeonElement { None, Light, Dark, SanctumTheater }
    
    [Header("Master TMP Display")]
    public TextMeshProUGUI lightDarkText;

    [Header("ObjectToAccept")]
    public bool PuppetScript = false;
    public bool PuppetTeather = false;
    public bool MasterTeather = true; // por defecto true (asegura uno activo)

    [Header("Zone Identification (optional)")]
    public int zoneIndex = 0; // libre uso (no obligatorio)

    [Header("Stored state")]
    public GameObject currentObject; // objeto que fue dropeado (si aplica)
    public DungeonElement dungeonElement = DungeonElement.None;

    [Header("Master percentages (only used if MasterTeather==true)")]
    [Tooltip("Valores en pasos de 0/50/100. Se mantienen al quitar objetos.")]
    [Range(0, 100)] public int LightPercentage = 0;
    [Range(0, 100)] public int DarkPercentage = 0;

    [Header("Master UI / control")]
    [Tooltip("Si este Master tiene 3 hijos DropZoneUI y todos tienen objetos, PortalUnlocked = true.")]
    public bool PortalUnlocked = false;

    // Lista de nombres: [ScriptName, MiniBossName, BigBossName]
    public List<string> NamesList = new List<string>() { "", "", "" };

    [Header("Sprites for PuppetScript display (used by MasterTeather)")]
    public Sprite script1Sprite;
    public Sprite script2Sprite;
    public Sprite script3Sprite;

    [Header("Sprites to swap for PuppetTeather objects")]
    public Sprite lightSprite; // when Light 100
    public Sprite darkSprite;  // when Dark 100

    // Internals
    private RectTransform _rect;
    private Sprite _originalSprite;
    private Image _autoImageForScript;
    private bool _isPointerHeld = false;
    private Coroutine _holdCoroutine;

    // ----------------------------------------------------------------------
    // Validation: asegurar que solo un boolean sea true
    // ----------------------------------------------------------------------
    private void OnValidate()
    {
        bool[] all = { PuppetScript, PuppetTeather, MasterTeather };
        int trueCount = (PuppetScript ? 1 : 0) + (PuppetTeather ? 1 : 0) + (MasterTeather ? 1 : 0);

        if (trueCount == 0)
        {
            MasterTeather = true;
        }
        else if (trueCount > 1)
        {
            if (PuppetScript)
            {
                PuppetTeather = false;
                MasterTeather = false;
            }
            else if (PuppetTeather)
            {
                PuppetScript = false;
                MasterTeather = false;
            }
            else if (MasterTeather)
            {
                PuppetScript = false;
                PuppetTeather = false;
            }
        }

        if (MasterTeather)
        {
            LightPercentage = Snap50(LightPercentage);
            DarkPercentage = 100 - LightPercentage;
        }
    }

    private int Snap50(int val)
    {
        if (val <= 25) return 0;
        if (val <= 75) return 50;
        return 100;
    }

    private void EnforceLightDarkInvariant()
    {
        LightPercentage = Mathf.Clamp(LightPercentage, 0, 100);
        DarkPercentage = 100 - LightPercentage;
    }

    // ----------------------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------------------
    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        if (NamesList == null || NamesList.Count < 3)
            NamesList = new List<string>() { "", "", "" };
    }

    private void Start()
    {
        if (MasterTeather)
        {
            LightPercentage = Snap50(LightPercentage);
            DarkPercentage = 100 - LightPercentage;
        }
    }

    private void Update()
    {
        if (MasterTeather)
        {
            MasterLogic_RunChecks();
        }

        // 🔹 Si este slot no es master y ya no tiene objeto hijo, limpiar
        if (!MasterTeather && currentObject != null)
        {
            if (currentObject.transform.parent != transform)
            {
                // El objeto fue removido manualmente o movido a otro lugar
                RemoveCurrentObject();
            }
        }
        // Actualizar sprite PuppetTeather si tiene objeto
        if (currentObject != null && PuppetTeather)
            UpdatePuppetTeatherSprite();
    }

    // ----------------------------------------------------------------------
    // Master logic
    // ----------------------------------------------------------------------
    private void MasterLogic_RunChecks()
    {
        DropZoneUI[] childZones = GetComponentsInChildren<DropZoneUI>(true);
        List<DropZoneUI> directChildren = new List<DropZoneUI>();

        foreach (var z in childZones)
        {
            if (z == this) continue;
            if (IsDescendantOf(z.transform, transform))
                directChildren.Add(z);
        }

        bool allHaveObjects = true;
        foreach (var child in directChildren)
        {
            if (child.currentObject == null)
            {
                allHaveObjects = false;
                break;
            }
        }

        PortalUnlocked = directChildren.Count >= 3 && allHaveObjects;

        string scriptName = "";
        List<string> teatherNames = new List<string>();

        foreach (var child in directChildren)
        {
            if (child.PuppetScript && child.currentObject != null)
                scriptName = child.currentObject.name;

            if (child.PuppetTeather && child.currentObject != null)
                teatherNames.Add(child.currentObject.name);
        }

        NamesList[0] = scriptName;
        NamesList[1] = teatherNames.Count > 0 ? teatherNames[0] : "";
        NamesList[2] = teatherNames.Count > 1 ? teatherNames[1] : "";
    }

    private bool IsDescendantOf(Transform child, Transform parent)
    {
        if (child == null || parent == null) return false;
        Transform t = child.parent;
        while (t != null)
        {
            if (t == parent) return true;
            t = t.parent;
        }
        return false;
    }

    // ----------------------------------------------------------------------
    // Drop handling
    // ----------------------------------------------------------------------
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        if (MasterTeather)
        {
            Debug.Log($"MasterTeather ({name}) does not accept drops.");
            return;
        }

        if (PuppetScript)
        {
            if (!dropped.CompareTag("PuppetScript"))
            {
                Debug.Log("Drop rechazado: PuppetScript zone espera tag 'PuppetScript'.");
                return;
            }
        }
        else if (PuppetTeather)
        {
            if (!dropped.CompareTag("TeatherPuppet"))
            {
                Debug.Log("Drop rechazado: PuppetTeather zone espera tag 'TeatherPuppet'.");
                return;
            }
        }

        dropped.transform.SetParent(transform);
        RectTransform rt = dropped.GetComponent<RectTransform>();

        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
        }
        else
        {
            dropped.transform.localPosition = Vector3.zero;
            dropped.transform.localRotation = Quaternion.identity;
        }

        currentObject = dropped;

        if (PuppetScript)
            HandlePuppetScriptPlaced();
        else if (PuppetTeather)
            HandlePuppetTeatherPlaced();
        
        
    }

    // ----------------------------------------------------------------------
    // PuppetScript specific
    // ----------------------------------------------------------------------
    private void HandlePuppetScriptPlaced()
    {
        if (currentObject == null) return;
        string nm = currentObject.name;

        DropZoneUI master = FindMaster();
        if (master == null)
        {
            Debug.LogWarning("No MasterTeather encontrado para PuppetScript zone.");
            return;
        }

        Sprite spriteToShow = null;
        if (nm.Contains("Script 1")) spriteToShow = master.script1Sprite;
        else if (nm.Contains("Script 2")) spriteToShow = master.script2Sprite;
        else if (nm.Contains("Script 3")) spriteToShow = master.script3Sprite;

        if (spriteToShow == null)
        {
            Debug.Log($"PuppetScript '{nm}' no coincide con Script 1/2/3 o sprite no asignado.");
            return;
        }

        master.CreateOrUpdateScriptImage(spriteToShow, this);
    }

    // ----------------------------------------------------------------------
    // PuppetTeather specific
    // ----------------------------------------------------------------------
    private void HandlePuppetTeatherPlaced()
    {
        if (currentObject == null) return;

        // Guardar el sprite original si aún no está guardado
        Image img = currentObject.GetComponent<Image>();
        if (img == null) img = currentObject.GetComponentInChildren<Image>();
        if (img != null && _originalSprite == null)
        {
            _originalSprite = img.sprite;
        }

        // Actualizar el sprite según Light/Dark
        UpdatePuppetTeatherSprite();
    }

    // ----------------------------------------------------------------------
    // Helpers
    // ----------------------------------------------------------------------
    private DropZoneUI FindMaster()
    {
        Transform t = transform;
        while (t != null)
        {
            DropZoneUI dz = t.GetComponent<DropZoneUI>();
            if (dz != null && dz.MasterTeather)
                return dz;
            t = t.parent;
        }
        return null;
    }

    public void CreateOrUpdateScriptImage(Sprite sprite, DropZoneUI childZone)
    {
        if (!MasterTeather)
        {
            Debug.LogWarning("CreateOrUpdateScriptImage llamado en objeto no master.");
            return;
        }

        Transform existing = transform.Find("__ScriptDisplay");
        RectTransform masterRect = GetComponent<RectTransform>();

        if (existing == null)
        {
            GameObject g = new GameObject("__ScriptDisplay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            g.transform.SetParent(transform, false);
            g.transform.SetSiblingIndex(0); // 🔹 lo movemos atrás en el canvas

            existing = g.transform;
            _autoImageForScript = g.GetComponent<Image>();
            _autoImageForScript.raycastTarget = false;
        }
        else
        {
            _autoImageForScript = existing.GetComponent<Image>();
            if (_autoImageForScript == null)
            {
                _autoImageForScript = existing.gameObject.AddComponent<Image>();
                _autoImageForScript.raycastTarget = false;
            }

            existing.SetSiblingIndex(0); // 🔹 asegurar que siga atrás
        }

        _autoImageForScript.sprite = sprite;
        _autoImageForScript.preserveAspect = true;

        RectTransform rt = existing.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        if (masterRect != null)
            rt.sizeDelta = masterRect.rect.size;
    }

    // ----------------------------------------------------------------------
    // Pointer input
    // ----------------------------------------------------------------------
    public void OnPointerDown(PointerEventData eventData)
    {
        if (MasterTeather)
            return;

        _isPointerHeld = true;
        _holdCoroutine = StartCoroutine(HoldRoutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_holdCoroutine != null)
            StopCoroutine(_holdCoroutine);

        _isPointerHeld = false;
    }

    private IEnumerator HoldRoutine()
    {
        while (_isPointerHeld)
            yield return null;
    }

    // ----------------------------------------------------------------------
    // Public API
    // ----------------------------------------------------------------------
    public void RemoveCurrentObject()
    {
        if (currentObject == null) return;

        // Si es PuppetScript, elimina la imagen del Master
        if (PuppetScript)
        {
            DropZoneUI master = FindMaster();
            if (master != null)
            {
                Transform existing = master.transform.Find("__ScriptDisplay");
                if (existing != null)
                {
                    Destroy(existing.gameObject);
                }
            }
        }

        // Solo borra la referencia de la zona, no el objeto draggable
        currentObject = null;
    }

    public void SetMasterLight50()
    {
        if (!MasterTeather) return;
        LightPercentage = 50;
        DarkPercentage = 50;
    }

    public void SetMasterLight100()
    {
        if (!MasterTeather) return;
        LightPercentage = 100;
        DarkPercentage = 0;
    }

    public void SetMasterLight0()
    {
        if (!MasterTeather) return;
        LightPercentage = 0;
        DarkPercentage = 100;
    }
    public void AdjustLightDark(int lightDelta, int darkDelta)
    {
        if (!MasterTeather) return;

        // Ajuste balanceado: si sube luz, baja oscuridad y viceversa
        if (lightDelta != 0)
        {
            LightPercentage = Mathf.Clamp(LightPercentage + lightDelta, 0, 100);
            DarkPercentage  = Mathf.Clamp(100 - LightPercentage, 0, 100);
        }

        if (darkDelta != 0)
        {
            DarkPercentage  = Mathf.Clamp(DarkPercentage + darkDelta, 0, 100);
            LightPercentage = Mathf.Clamp(100 - DarkPercentage, 0, 100);
        }

        // Actualizar TMP si está asignado
        if (lightDarkText != null)
            lightDarkText.text = $"{LightPercentage} / {DarkPercentage}";
    }
    public void UpdatePuppetTeatherSprite()
    {
        if (!PuppetTeather || currentObject == null) return;

        Image img = currentObject.GetComponent<Image>();
        if (img == null) img = currentObject.GetComponentInChildren<Image>();
        if (img == null) return;

        DropZoneUI master = FindMaster();
        if (master == null) return;

        int light = master.LightPercentage;
        int dark  = master.DarkPercentage;

        if (light == 100 && dark == 0)
            img.sprite = master.lightSprite;
        else if (dark == 100 && light == 0)
            img.sprite = master.darkSprite;
        else
            img.sprite = _originalSprite;
    }
    public void UpdateTMP()
    {
        if (lightDarkText != null)
            lightDarkText.text = $"{LightPercentage} / {DarkPercentage}";
    }
}
