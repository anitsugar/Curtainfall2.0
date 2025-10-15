using System.Collections;
using UnityEngine;
using Cinemachine;

[DisallowMultipleComponent]
public class CameraFXController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Virtual camera that follows the player (the one on the Player prefab).")]
    public CinemachineVirtualCamera vcam;

    [Header("Damage Shake")]
    [Tooltip("Strength of the camera shake when taking damage.")]
    public float dmgAmplitude = 1.2f;   // how strong
    [Tooltip("Frequency of the camera shake when taking damage.")]
    public float dmgFrequency = 2.0f;   // how jittery
    [Tooltip("Duration of the camera shake when taking damage.")]
    public float dmgDuration = 0.18f;   // how long

    [Header("Shoot Knockback")]
    [Tooltip("Max offset push (in local camera space units). 0.25–0.6 feels good.")]
    public float recoilStrength = 0.35f;
    [Tooltip("Seconds to return to rest after recoil.")]
    public float recoilReturnTime = 0.15f;
    [Tooltip("Smaller delay before starting return (helps feel snappy).")]
    public float recoilHold = 0.03f;

    // --- Internal references ---
    CinemachineBasicMultiChannelPerlin _perlin;
    CinemachineCameraOffset _camOffset;
    float _baseAmp, _baseFreq;
    Vector3 _baseOffset;
    Coroutine _shakeCo, _recoilCo;

    void Awake()
    {
        if (!vcam)
            vcam = GetComponentInChildren<CinemachineVirtualCamera>(true);

        // Noise (for shake)
        _perlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (_perlin == null)
            _perlin = vcam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        _baseAmp = _perlin.m_AmplitudeGain;
        _baseFreq = _perlin.m_FrequencyGain;

        // Camera offset (for recoil)
        _camOffset = vcam.GetComponent<CinemachineCameraOffset>();
        if (_camOffset == null)
            _camOffset = vcam.VirtualCameraGameObject.AddComponent<CinemachineCameraOffset>();

        _baseOffset = _camOffset.m_Offset;
    }

    // ---- PUBLIC API ----

    /// <summary>Call when the player takes damage.</summary>
    public void PlayDamageShake()
    {
        if (_shakeCo != null)
            StopCoroutine(_shakeCo);

        _shakeCo = StartCoroutine(ShakeRoutine());
    }

    /// <summary>
    /// Call when the player shoots. Pass world-space shot direction
    /// (from player towards cursor/target).
    /// </summary>
    public void PlayShootKnockback(Vector3 shotDirWorld)
    {
        if (_recoilCo != null)
            StopCoroutine(_recoilCo);

        _recoilCo = StartCoroutine(RecoilRoutine(shotDirWorld));
    }

    // ---- COROUTINES ----

    IEnumerator ShakeRoutine()
    {
        // Start strong, decay back to base values
        _perlin.m_AmplitudeGain = dmgAmplitude;
        _perlin.m_FrequencyGain = dmgFrequency;

        float t = 0f;
        while (t < dmgDuration)
        {
            t += Time.deltaTime;
            float k = 1f - (t / dmgDuration);
            _perlin.m_AmplitudeGain = Mathf.Lerp(_baseAmp, dmgAmplitude, k);
            _perlin.m_FrequencyGain = Mathf.Lerp(_baseFreq, dmgFrequency, k);
            yield return null;
        }

        _perlin.m_AmplitudeGain = _baseAmp;
        _perlin.m_FrequencyGain = _baseFreq;
        _shakeCo = null;
    }

    IEnumerator RecoilRoutine(Vector3 shotDirWorld)
    {
        // Push opposite to shot direction (local to camera)
        var cam = vcam.VirtualCameraGameObject.transform;
        Vector3 local = cam.InverseTransformDirection(-shotDirWorld);
        local.y = 0f;
        if (local.sqrMagnitude > 0.0001f)
            local.Normalize();

        Vector3 targetOffset = _baseOffset + local * recoilStrength;

        // Snap to target quickly
        _camOffset.m_Offset = targetOffset;
        yield return new WaitForSeconds(recoilHold);

        // Ease back to base
        float t = 0f;
        while (t < recoilReturnTime)
        {
            t += Time.deltaTime;
            float k = t / recoilReturnTime;
            _camOffset.m_Offset = Vector3.Lerp(targetOffset, _baseOffset, k);
            yield return null;
        }

        _camOffset.m_Offset = _baseOffset;
        _recoilCo = null;
    }
}
