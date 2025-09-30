using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
     private Camera mainCamera;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;

    [Header("Ammo Settings")]
    public int maxAmmo = 10;
    public int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;

    void Start()
    {
        mainCamera = Camera.main;
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        // Si ya está recargando, no dejar disparar
        if (isReloading) return;

        // Disparo normal
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        // Recarga manual con R
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        if (currentAmmo <= 0)
            return; // no dispara si está vacío (ya debería estar recargando automático)

        // Calcular dirección hacia el mouse
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 targetDirection;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetDirection = hit.point - firePoint.position;
            targetDirection.y = 0;
        }
        else
        {
            targetDirection = firePoint.forward;
        }

        // Instanciar proyectil
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = targetDirection.normalized * projectileSpeed;
        }

        currentAmmo--;
        Debug.Log("Balas restantes: " + currentAmmo);

        // 👇 apenas llega a 0, inicia recarga automática
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        if (isReloading) yield break; // evita doble recarga

        isReloading = true;
        Debug.Log("Recargando...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

        Debug.Log("Recarga completa. Munición: " + currentAmmo);
    }
}
