using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToogleObjectOnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // El objeto a encender/apagar
    [SerializeField] private GameObject targetObject2;
    [SerializeField] private GameObject targetObject3;
    private bool playerInside = false;

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            // Cambia el estado del objeto
            targetObject.SetActive(!targetObject.activeSelf);
            targetObject2.SetActive(!targetObject2.activeSelf);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            targetObject3.SetActive(!targetObject3.activeSelf);
            Debug.Log("Jugador dentro del trigger. Presiona E para interactuar.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            targetObject3.SetActive(!targetObject3.activeSelf);
            Debug.Log("Jugador salió del trigger.");
        }
    }
}
