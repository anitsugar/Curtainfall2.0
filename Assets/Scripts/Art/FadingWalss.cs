using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingWalss : MonoBehaviour
{
    [Header("Player & Layer")]
    public Transform player;
    public LayerMask wallLayer;

    [Header("Fade Settings")]
    [Range(0f, 1f)] public float transparentAlpha = 0.3f;
    public float fadeSpeed = 5f;

    [Header("Materials")]
    public Material wallOpaque;       // Surface Type: Opaque
    public Material wallTransparent;  // Surface Type: Transparent

    // Lista de paredes actualmente transparentes
    private Dictionary<Renderer, float> fadingWalls = new Dictionary<Renderer, float>();

    void Update()
    {
        // Lanzar raycast desde cámara hacia player
        RaycastHit[] hits;
        Vector3 dir = player.position - transform.position;
        hits = Physics.RaycastAll(transform.position, dir, dir.magnitude, wallLayer);

        HashSet<Renderer> hitRenderers = new HashSet<Renderer>();

        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend == null) continue;

            hitRenderers.Add(rend);

            // Si la pared no está ya en la lista, agregarla y cambiar material
            if (!fadingWalls.ContainsKey(rend))
            {
                rend.material = new Material(wallTransparent);
                fadingWalls.Add(rend, rend.material.color.a);
            }

            // Fade hacia transparente
            float targetAlpha = transparentAlpha;
            Color color = rend.material.color;
            color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * fadeSpeed);
            rend.material.color = color;
        }

        // Restaurar paredes que ya no están entre cámara y player
        List<Renderer> toRemove = new List<Renderer>();
        foreach (var pair in fadingWalls)
        {
            Renderer rend = pair.Key;
            if (!hitRenderers.Contains(rend))
            {
                Color color = rend.material.color;
                color.a = Mathf.Lerp(color.a, 1f, Time.deltaTime * fadeSpeed);
                rend.material.color = color;

                // Si el alpha está casi 1, devolver material opaco y remover de lista
                if (Mathf.Abs(color.a - 1f) < 0.01f)
                {
                    rend.material = wallOpaque;
                    toRemove.Add(rend);
                }
            }
        }

        // Limpiar lista
        foreach (var rend in toRemove)
            fadingWalls.Remove(rend);
    }
}
