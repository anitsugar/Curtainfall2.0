using UnityEngine;

/// Clamp the Main Camera inside a rectangular room defined by 4 *3D* Colliders
/// (north/south/east/west). Works even if camera & vcam are parented under Player.
/// Put this on the **Main Camera** (with CinemachineBrain).
[DisallowMultipleComponent]
[DefaultExecutionOrder(10000)] // run after CinemachineBrain/LateUpdate
public class CameraWorldRectClamp : MonoBehaviour
{
    [Header("Room Walls (3D Colliders)")]
    public Collider northWall; // e.g., BoxCollider or MeshCollider (3D)
    public Collider southWall;
    public Collider eastWall;
    public Collider westWall;

    [Header("Tuning")]
    [Tooltip("Extra padding inside the computed frustum margin (world units).")]
    public float extraPadding = 0.25f;

    [Tooltip("0 = snap, 0.2–0.5 = soft clamp.")]
    [Range(0f, 2f)] public float damping = 0.3f;

    Camera _cam;
    bool _valid;
    Bounds _room;

    void Awake()
    {
        _cam = GetComponent<Camera>();
        RecomputeRoomBounds();
    }

    /// Call this when you change rooms or swap any wall references.
    public void RecomputeRoomBounds()
    {
        _valid = false;
        if (!northWall || !southWall || !eastWall || !westWall) return;
        if (!northWall.enabled || !southWall.enabled || !eastWall.enabled || !westWall.enabled) return;

        Bounds b = northWall.bounds;
        b.Encapsulate(southWall.bounds);
        b.Encapsulate(eastWall.bounds);
        b.Encapsulate(westWall.bounds);

        _room = b;
        _valid = true;
    }

    // Correct right before rendering, after Cinemachine writes the pose
    void OnPreCull()
    {
        if (!_valid || _cam == null) return;

        float minX = _room.min.x, maxX = _room.max.x;
        float minZ = _room.min.z, maxZ = _room.max.z;

        Vector3 desired = transform.position;

        // Set this to your floor height if different:
        float floorY = _room.center.y;
        float camHeight = Mathf.Abs(desired.y - floorY);

        // Frustum half-extents at that height
        float vHalf = Mathf.Tan(_cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * camHeight;
        float hHalf = vHalf * _cam.aspect;

        float marginX = hHalf + extraPadding;
        float marginZ = vHalf + extraPadding;

        float minCamX = minX + marginX;
        float maxCamX = maxX - marginX;
        float minCamZ = minZ + marginZ;
        float maxCamZ = maxZ - marginZ;

        // If margins exceed room, collapse to center
        if (minCamX > maxCamX) { float cx = (minX + maxX) * 0.5f; minCamX = maxCamX = cx; }
        if (minCamZ > maxCamZ) { float cz = (minZ + maxZ) * 0.5f; minCamZ = maxCamZ = cz; }

        Vector3 clamped = new Vector3(
            Mathf.Clamp(desired.x, minCamX, maxCamX),
            desired.y,
            Mathf.Clamp(desired.z, minCamZ, maxCamZ)
        );

        if ((clamped - desired).sqrMagnitude > 0f)
        {
            float k = 1f - Mathf.Exp(-Mathf.Max(0.0001f, damping) * Mathf.Max(0f, Time.deltaTime));
            transform.position = Vector3.Lerp(desired, clamped, k);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (_cam == null) _cam = GetComponent<Camera>();
        if (!northWall || !southWall || !eastWall || !westWall) return;

        Bounds b = northWall.bounds;
        b.Encapsulate(southWall.bounds);
        b.Encapsulate(eastWall.bounds);
        b.Encapsulate(westWall.bounds);

        float floorY = b.center.y;
        Vector3 pos = transform.position;
        float camHeight = Mathf.Abs(pos.y - floorY);

        float vHalf = Mathf.Tan((_cam ? _cam.fieldOfView : 40f) * 0.5f * Mathf.Deg2Rad) * camHeight;
        float hHalf = vHalf * (_cam ? _cam.aspect : 16f / 9f);

        float marginX = hHalf + extraPadding;
        float marginZ = vHalf + extraPadding;

        float minCamX = b.min.x + marginX;
        float maxCamX = b.max.x - marginX;
        float minCamZ = b.min.z + marginZ;
        float maxCamZ = b.max.z - marginZ;

        Gizmos.color = new Color(0.1f, 0.8f, 1f, 0.35f);
        Vector3 a = new Vector3(minCamX, floorY, minCamZ);
        Vector3 d = new Vector3(minCamX, floorY, maxCamZ);
        Vector3 c = new Vector3(maxCamX, floorY, maxCamZ);
        Vector3 b0 = new Vector3(maxCamX, floorY, minCamZ);
        Gizmos.DrawLine(a, b0); Gizmos.DrawLine(b0, c); Gizmos.DrawLine(c, d); Gizmos.DrawLine(d, a);
    }
#endif
}
