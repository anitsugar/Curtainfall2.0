using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreRaycast : MonoBehaviour
{
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) => false;
}
