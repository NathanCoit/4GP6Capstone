using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    public enum TreasureType
    {
        Attack,
        Defense,
        Health
    }

    public TerrainMap GameMap;
    public GameManager GameManagerScript;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateNewTreasures(int pintTreasuresPerTier, float pfTierRadius)
    {
        Object TreasureObject = Resources.Load("Treasure_Chest");
        // foreach tier
        for(int i = 0; i < GameManagerScript.MapTierCount; i++)
        {
            for (int j = 0; j < pintTreasuresPerTier; j++)
            {
                
            }
        }
        // create pint number of treasures
    }
}
