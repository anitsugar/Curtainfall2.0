using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCounter : MonoBehaviour
{
    private PlayerMaterialsCounter playerMaterialsCounter;
    
    public void OnTriggerEnter()
    {
        if (gameObject.CompareTag("LightEssence"))
        {
            int amount = Random.Range(5, 10);
            playerMaterialsCounter.IncreaseLightAmount(amount);
            Debug.Log("LightEssence x" +amount);
            Destroy(gameObject);
        }
        if (gameObject.CompareTag("DarkEssence"))
        {
            int amount = Random.Range(5, 10);
            playerMaterialsCounter.IncreaseDarkAmount(amount);
            Debug.Log("DarkEssence x" +amount);
            Destroy(gameObject);
        }
    }
     
}
