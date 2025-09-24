using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaterialsCounter : MonoBehaviour
{
    public int lightEssence;
    public int darkEssence;


    public void IncreaseLightAmount(int amount)
    {
        lightEssence += amount;
    }
    public void IncreaseDarkAmount(int amount)
    {
        darkEssence += amount;
    }
}
