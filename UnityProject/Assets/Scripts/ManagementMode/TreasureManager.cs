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

    public List<Treasure> Treasures
    {
        get
        {
            return GameMap.GetTreasures();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateNewTreasures(int pintTreasuresPerTier, int pintTierCount)
    {
        Treasure musTreasure;
        int intRetryCount = 0;
        // foreach tier
        for(int i = 0; i < GameManagerScript.MapTierCount; i++)
        {
            for (int j = 0; j < pintTreasuresPerTier; j++)
            {
                musTreasure = new Treasure(Treasure.GetRandomTreasureType());
                intRetryCount = 0;

                while (!GameMap.PlaceTreasure(musTreasure, GameMap.CalculateRandomPositionInTier(i)) && intRetryCount < 100)
                {
                    intRetryCount++;
                }

                // Hide treasure for higher tiers
                if(i > 0)
                {
                    musTreasure.MapGameObject.SetActive(false);
                }
            }
        }
    }

    public void LoadSavedTreasures(GameInfo.SavedTreasure[] parrSavedTreasures)
    {
        Treasure musTreasure;
        Vector3 uniCetreVec3 = new Vector3(0, 1.5f, 0);
        float fDistance;
        if(parrSavedTreasures != null)
        {
            foreach (GameInfo.SavedTreasure musSavedTreasure in parrSavedTreasures)
            {
                musTreasure = new Treasure(musSavedTreasure);
                GameMap.PlaceTreasure(
                    musTreasure,
                    new Vector3(musSavedTreasure.x, musSavedTreasure.y, musSavedTreasure.z),
                    true);
                fDistance = Vector3.Distance(musTreasure.ObjectPosition, uniCetreVec3);
                if(fDistance > (GameManagerScript.CurrentTier + 1) * (250/3))
                {
                    musTreasure.MapGameObject.SetActive(false);
                }
            }
        }
    }
}
