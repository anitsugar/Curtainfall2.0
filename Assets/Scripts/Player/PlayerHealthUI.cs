using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
     [Header("Hearts del jugador (de izquierda a derecha)")]
    [SerializeField] private Image[] hearts;

    [Header("Imágenes adicionales sincronizadas con los corazones")]
    [SerializeField] private Image[] extraShakeImages;

    [SerializeField] private PlayerController player;

    private int lastHeartsShown;
    private Vector3[] heartOriginalPositions;
    private Color[] originalColors;
    private Vector3[] extraOriginalPositions;

    private void Awake()
    {
        if (hearts == null) hearts = new Image[0];
        if (extraShakeImages == null) extraShakeImages = new Image[0];

        heartOriginalPositions = new Vector3[hearts.Length];
        originalColors = new Color[hearts.Length];
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                heartOriginalPositions[i] = hearts[i].rectTransform.localPosition;
                originalColors[i] = hearts[i].color;
            }
        }

        extraOriginalPositions = new Vector3[extraShakeImages.Length];
        for (int i = 0; i < extraShakeImages.Length; i++)
        {
            if (extraShakeImages[i] != null)
                extraOriginalPositions[i] = extraShakeImages[i].rectTransform.localPosition;
        }

        lastHeartsShown = hearts.Length;
    }

    private void OnEnable()
    {
        if (player != null)
        {
            player.OnHealthChanged += UpdateHearts;
            UpdateHearts(player.GetHealth());
        }
    }

    private void OnDisable()
    {
        if (player != null)
            player.OnHealthChanged -= UpdateHearts;
    }

    private void UpdateHearts(float currentHealth)
    {
        int heartsToShow = Mathf.Clamp(Mathf.CeilToInt(currentHealth / 10f), 0, hearts.Length);

        // Si perdió corazones
        if (heartsToShow < lastHeartsShown)
        {
            for (int i = heartsToShow; i < lastHeartsShown && i < hearts.Length; i++)
            {
                if (hearts[i] == null) continue;
                hearts[i].enabled = true;
                hearts[i].rectTransform.localPosition = heartOriginalPositions[i];
                hearts[i].color = originalColors[i];

                // Parpadea y sacude el corazón junto con su imagen extra (si existe)
                StartCoroutine(FlashAndShakeHeart(i));
            }
        }

        // Mostrar u ocultar corazones activos
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < heartsToShow)
                hearts[i].enabled = true;
            else if (i >= lastHeartsShown)
                hearts[i].enabled = false;
        }

        lastHeartsShown = heartsToShow;
    }

    private IEnumerator FlashAndShakeHeart(int index)
    {
        Image heart = hearts[index];
        if (heart == null) yield break;

        RectTransform heartRT = heart.rectTransform;
        Vector3 heartOriginalPos = heartOriginalPositions[index];
        Color originalColor = originalColors[index];

        Image extra = (index < extraShakeImages.Length) ? extraShakeImages[index] : null;
        RectTransform extraRT = extra ? extra.rectTransform : null;
        Vector3 extraOriginalPos = extra ? extraOriginalPositions[index] : Vector3.zero;

        float duration = 0.4f;
        float elapsed = 0f;
        float maxShake = 100f; // 💥 fuerza del temblor

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Parpadeo rojo del corazón
            float t = Mathf.PingPong(elapsed * 6f, 1f);
            heart.color = Color.Lerp(originalColor, Color.red, t);

            // Movimiento aleatorio con intensidad decreciente
            float shakeStrength = Mathf.Lerp(maxShake, 0f, elapsed / duration);
            Vector3 shakeOffset = (Vector3)Random.insideUnitCircle * shakeStrength;

            heartRT.localPosition = heartOriginalPos + shakeOffset;

            if (extraRT != null)
                extraRT.localPosition = extraOriginalPos + shakeOffset * 0.8f; // un poco más suave

            yield return null;
        }

        // Restaurar posiciones y color
        heartRT.localPosition = heartOriginalPos;
        heart.color = originalColor;
        heart.enabled = false;

        if (extraRT != null)
            extraRT.localPosition = extraOriginalPos;
    }
}
