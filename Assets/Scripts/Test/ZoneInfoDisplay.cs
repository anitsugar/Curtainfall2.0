using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class ZoneInfoDisplay : MonoBehaviour
{
    [Header("Referencia al texto")]
    [SerializeField] private TextMeshProUGUI percentageText;

    private void Update()
    {
        if (GameManager.Instance == null || percentageText == null) return;

        // Obtener el número de zona actual
        int currentZone = GameManager.Instance.GetCurrentZoneNumber();

        float light = 50f;
        float dark = 50f;

        // Obtener los porcentajes correspondientes según la zona
        switch (currentZone)
        {
            case 1:
                light = GameManager.Instance.GetZone1Light();
                dark  = GameManager.Instance.GetZone1Dark();
                break;
            case 2:
                light = GameManager.Instance.GetZone2Light();
                dark  = GameManager.Instance.GetZone2Dark();
                break;
            case 3:
                light = GameManager.Instance.GetZone3Light();
                dark  = GameManager.Instance.GetZone3Dark();
                break;
        }

        // Actualizar el texto
        percentageText.text = $"Dark: {dark:F0} / Light: {light:F0}";
    }
}

