// File: SpriteSorterY.cs
using UnityEngine;

/// <summary>
/// Converts world Y (plus an optional feet offset) into a sorting order.
/// Use only when you actually need manual 2D stacking; prefer depth/Z when ZWrite is ON.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
[DisallowMultipleComponent]
public class SpriteSorter : MonoBehaviour
{
    [Tooltip("Sorting Order = Base - RoundToInt((Y + YOffset) * Multiplier)")]
    public int SortingOrderBase = 0;

    [Tooltip("Higher multiplier = stronger separation between objects.")]
    public int SortingOrderMultiplier = 100;

    [Tooltip("Use this if your sprite pivot isn't exactly at the feet.")]
    public float YOffset = 0f;

    Renderer _renderer;
    Transform _tf;

    void OnEnable()
    {
        _renderer = GetComponent<Renderer>();
        _tf = transform;
        UpdateOrder();
    }

    void LateUpdate() => UpdateOrder();

    void UpdateOrder()
    {
        if (_renderer == null) return;
        float y = _tf.position.y + YOffset;
        int order = SortingOrderBase - Mathf.RoundToInt(y * SortingOrderMultiplier);
        // Clamp to Unity's safe range
        order = Mathf.Clamp(order, short.MinValue, short.MaxValue);
        _renderer.sortingOrder = order;
    }
}
