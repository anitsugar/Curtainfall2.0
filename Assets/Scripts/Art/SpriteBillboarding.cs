using UnityEngine;

/// <summary>
/// This script makes the GameObject it's attached to always face the main camera.
/// It's perfect for 2D sprites in a 3D world, creating a "billboard" effect.
/// This is a common technique for games with a 2.5D perspective, like "Cult of the Lamb".
/// </summary>
public class SpriteBillboarding : MonoBehaviour
{
    private Camera mainCamera;

    /// <summary>
    /// If true, the sprite will only rotate on the Y-axis.
    /// This prevents it from tilting up or down with the camera, which is often a desired effect.
    /// </summary>
    [Tooltip("Locks rotation to the Y-axis only.")]
    public bool lockYAxis = true;

    void Start()
    {
        // Cache the main camera reference for better performance.
        // Camera.main can be slow if called repeatedly in Update().
        mainCamera = Camera.main;
    }

    // Using LateUpdate ensures that the billboarding calculation happens
    // after the camera has completed its movement for the frame.
    // This prevents visual jittering.
    void LateUpdate()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("Billboard script can't find the main camera. Please ensure you have a camera tagged as 'MainCamera'.");
            return;
        }

        if (lockYAxis)
        {
            // Y-Axis locked billboarding:
            // 1. Get the direction from the sprite to the camera.
            // 2. Set the Y component to be the same as the sprite's Y to prevent tilting.
            // 3. Create a rotation that looks in that direction.
            Vector3 targetPosition = mainCamera.transform.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
        }
        else
        {
            // Standard Billboarding:
            // Make the sprite's rotation the same as the camera's.
            // This ensures it's perfectly parallel to the camera's view plane.
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}