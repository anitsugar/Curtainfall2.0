using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // Referencia a la cámara principal de la escena
    private Camera mainCamera;

    // Prefab del proyectil que se va a disparar
    public GameObject projectilePrefab;

    // Punto desde donde se originará el disparo (ej. la punta del arma)
    public Transform firePoint;

    // Velocidad a la que se moverá el proyectil
    public float projectileSpeed = 20f;

    void Start()
    {
        // Al iniciar, obtenemos la referencia a la cámara principal
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Detecta si se ha presionado el botón izquierdo del mouse
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Creamos un rayo desde la cámara hacia la posición del mouse en la pantalla
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Lanzamos el rayo y comprobamos si ha chocado con algo
        if (Physics.Raycast(ray, out hit))
        {
            // Calculamos la dirección desde el punto de disparo (firePoint) hacia el punto de impacto del rayo
            Vector3 targetDirection = hit.point - firePoint.position;
            
            // Nos aseguramos de que el disparo sea en el plano horizontal (Y=0)
            // Esto es clave para el estilo top-down 3D
            targetDirection.y = 0;

            // Creamos una instancia del proyectil en la posición y rotación del firePoint
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // Obtenemos el componente Rigidbody del proyectil y le aplicamos una fuerza
            // Usamos .normalized para obtener un vector de magnitud 1 (solo la dirección)
            // y lo multiplicamos por la velocidad deseada.
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = targetDirection.normalized * projectileSpeed;
            }

            
        }
    }
}
