using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerShooting : MonoBehaviour
{
    private Camera mainCamera;

    [Header("Bullet Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;

    

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI bulletCountText;
    public GameObject lightBullet;
    public GameObject darkBullet;
    public GameObject reloadSymbol;

    [Header("Ammo Settings")]
    public int maxAmmo = 10;
    public int currentAmmo;
    public float reloadTime = 3f;
    private bool isReloading = false;

    private void Start()
    {
        mainCamera = Camera.main;

        lightBullet.SetActive(true);
        darkBullet.SetActive(false);

        currentAmmo = maxAmmo;
        UpdateAmmoUI("Start()");
    }

    private void Update()
    {
        if (isReloading) return;

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeBulletAttribute();
        }
        UpdateAmmoUI("Update()");
    }

    private void Shoot()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        // Disparo hacia donde apunta el mouse
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 targetDirection;

        if (Physics.Raycast(ray, out RaycastHit hit))
            targetDirection = (hit.point - firePoint.position).normalized;
        else
            targetDirection = firePoint.forward;

        // Crear el proyectil
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = targetDirection * projectileSpeed;

        // Cámara: retroceso (knockback)
        var fx = GetComponentInChildren<CameraFXController>();
        if (fx != null)
            fx.PlayShootKnockback(targetDirection); // 👉 le pasamos la dirección de disparo real

        currentAmmo--;

        if (currentAmmo <= 0)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        if (isReloading) yield break;

        isReloading = true;
        Debug.Log("Reloading...");

        
        bulletCountText.gameObject.SetActive(false);
        
        reloadSymbol.SetActive(true);
        
        yield return new WaitForSeconds(reloadTime);
        
        reloadSymbol.SetActive(false);
        bulletCountText.gameObject.SetActive(true);
        
        currentAmmo = maxAmmo;
        isReloading = false;
        
        
    }

    private void UpdateAmmoUI(string caller = "")
    {
        
        if (bulletCountText == null)
        {
            var found = FindObjectOfType<TextMeshProUGUI>();
            if (found != null)
            {
                bulletCountText = found;
            }
        }

        if (bulletCountText != null)
        {
            
            bulletCountText.text = $"{currentAmmo} / {maxAmmo}";
        }
        
    }

    private void ChangeBulletAttribute()
    {
        bool usingLight = lightBullet.activeInHierarchy;
        lightBullet.SetActive(!usingLight);
        darkBullet.SetActive(usingLight);

        currentAmmo = 0;
        StartCoroutine(Reload());
    }

    
}
