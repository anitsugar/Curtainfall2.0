using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboarding : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main; // obtiene la cámara principal
    }

    private void LateUpdate()
    {
        if (cam == null) return;

        // hace que el sprite mire hacia la cámara
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
