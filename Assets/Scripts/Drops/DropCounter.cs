using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCounter : MonoBehaviour
{
    private PlayerMaterialsCounter playerMaterialsCounter;
    
    public void OnTriggerEnter(Collider other)
    {
        
        PlayerMaterialsCounter playerMaterialsCounter = other.GetComponent<PlayerMaterialsCounter>();

        if (playerMaterialsCounter == null) return; 

        
        if (CompareTag("LightEssence"))
        {
            int amount = Random.Range(5, 10);
            playerMaterialsCounter.IncreaseLightAmount(amount);
            Debug.Log("Light Essence x" + amount);
            Destroy(gameObject);
        }
        else if (CompareTag("DarkEssence"))
        {
            int amount = Random.Range(5, 10);
            playerMaterialsCounter.IncreaseDarkAmount(amount);
            Debug.Log("Dark Essence x" + amount);
            Destroy(gameObject);
        }
    }
     
}
