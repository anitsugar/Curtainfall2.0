using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>(); // Necesitamos la referencia del Canvas
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; // Guardamos el parent original
        canvasGroup.alpha = 0.6f; // Se vuelve semi-transparente
        canvasGroup.blocksRaycasts = false; // Para que los DropZones detecten el objeto
        transform.SetParent(canvas.transform); // Lo llevamos al frente mientras se arrastra
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; 
        canvasGroup.blocksRaycasts = true;

        // Si no se suelta en un DropZone válido, vuelve al parent original
        if (transform.parent == canvas.transform)
        {
            transform.SetParent(originalParent);
        }
    }
}
