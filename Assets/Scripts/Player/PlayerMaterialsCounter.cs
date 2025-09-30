using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaterialsCounter : MonoBehaviour
{
    public int lightEssence;
    public int darkEssence;

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMaterials(this);
        }
    }
    

    public void IncreaseLightAmount(int amount)
    {
        lightEssence += amount;
        GameManager.Instance?.SaveMaterials(this);
    }

    public void IncreaseDarkAmount(int amount)
    {
        darkEssence += amount;
        GameManager.Instance?.SaveMaterials(this);
    }
}
