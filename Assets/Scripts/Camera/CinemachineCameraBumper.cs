using UnityEngine;
using Cinemachine;

/// Bumps the camera away from ONLY colliders that have CameraBumpWall.
/// Runs at Finalize stage and applies PositionCorrection (additive).
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineCameraBumper : CinemachineExtension
{
    [Header("Bump Settings")]
    [Tooltip("How close the camera can get to a wall before being pushed (world units).")]
    public float cameraRadius = 0.6f;

    [Tooltip("Extra clearance from walls to avoid flicker.")]
    public float skin = 0.02f;

    [Tooltip("0 = snap, 0.2–0.5 = soft bump.")]
    [Range(0f, 2f)] public float damping = 0.3f;

    [Header("Debug")]
    public bool debugLogs = false;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        // Run as late as possible so we override anything else
        if (stage != CinemachineCore.Stage.Finalize) return;

        if (CameraBumpWall.Registered.Count == 0)
        {
            if (debugLogs) Debug.Log("[Bumper] No registered walls found.");
            return;
        }

        Vector3 desired = state.FinalPosition; // final planned position
        float keep = Mathf.Max(0.0001f, cameraRadius + skin);
        float keepSqr = keep * keep;

        Vector3 totalOffset = Vector3.zero;
        int pushes = 0;

        foreach (var c in CameraBumpWall.Registered)
        {
            if (!c || !c.enabled) continue;

            // Closest point on wall to camera
            Vector3 closest = c.ClosestPoint(desired);
            Vector3 toCenter = desired - closest;
            float distSqr = toCenter.sqrMagnitude;

            if (distSqr < keepSqr && distSqr > 1e-8f)
            {
                float dist = Mathf.Sqrt(distSqr);
                Vector3 n = toCenter / Mathf.Max(1e-6f, dist);
                float push = keep - dist;
                totalOffset += n * push;
                pushes++;
            }
        }

        if (pushes > 0 && totalOffset.sqrMagnitude > 0f)
        {
            float k = 1f - Mathf.Exp(-Mathf.Max(0.0001f, damping) * Mathf.Max(0f, deltaTime));
            Vector3 correction = Vector3.Lerp(Vector3.zero, totalOffset, k);

            // Apply additively (Cinemachine-friendly way)
            state.PositionCorrection += correction;

            if (debugLogs)
                Debug.Log($"[Bumper] pushes={pushes}  offset={totalOffset:F3}  applied={correction:F3}");
        }
        else if (debugLogs)
        {
            Debug.Log("[Bumper] No push required this frame.");
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var vcam = GetComponent<CinemachineVirtualCamera>();
        if (!vcam) return;
        var pos = vcam.State.FinalPosition;
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.2f);
        Gizmos.DrawWireSphere(pos, cameraRadius + skin);
    }
#endif
}
