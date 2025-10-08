using UnityEngine;

/// <summary>
/// This script makes the GameObject it's attached to always face the main camera.
/// It's perfect for 2D sprites in a 3D world, creating a "billboard" effect.
/// This is a common technique for games with a 2.5D perspective, like "Cult of the Lamb".
/// </summary>
public class SpriteBillboarding : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField] bool freezeXZAxis = true;
    private void Update()
    {
        if (freezeXZAxis)
        {
            transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}