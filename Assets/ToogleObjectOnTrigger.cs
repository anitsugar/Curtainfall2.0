using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToogleObjectOnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // El objeto a encender/apagar
    private bool playerInside = false;

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            // Cambia el estado del objeto
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Jugador dentro del trigger. Presiona E para interactuar.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("Jugador salió del trigger.");
        }
    }
}
