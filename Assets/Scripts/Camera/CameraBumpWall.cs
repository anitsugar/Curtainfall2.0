using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class CameraBumpWall : MonoBehaviour
{
    public static readonly HashSet<Collider> Registered = new HashSet<Collider>();
    Collider _col;

    void OnEnable()
    {
        _col = GetComponent<Collider>();
        if (_col && _col.enabled) Registered.Add(_col);
    }

    void OnDisable()
    {
        if (_col) Registered.Remove(_col);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        var c = GetComponent<Collider>();
        if (c && !c.isTrigger)
            Debug.LogWarning($"CameraBumpWall '{name}': set collider as Trigger to avoid gameplay collisions.");
    }
#endif
}
