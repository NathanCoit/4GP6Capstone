using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Treasure : MapObject
{
    public enum TreasureType
    {
        Attack,
        Defense,
        Health
    }

    public TreasureType Type;

    public Treasure(TreasureType penumType)
    {
        CreateTreasureObject();
        Type = penumType;
    }

    public Treasure(GameInfo.SavedTreasure pmusSavedTreasure)
    {
        CreateTreasureObject();
        Type = pmusSavedTreasure.Type;
    }

    private void CreateTreasureObject()
    {
        UnityEngine.Object uniTreasureObject = Resources.Load("Treasure_Chest");
        if(uniTreasureObject != null)
        {
            MapGameObject = (GameObject)GameObject.Instantiate(uniTreasureObject);
        }
    }

    public void Destroy()
    {
        GameObject.Destroy(MapGameObject);
    }

    public static TreasureType GetRandomTreasureType()
    {
        switch ((int)(UnityEngine.Random.value * 99 / 33))
        {
            case 0:
                return TreasureType.Attack;
            case 1:
                return TreasureType.Defense;
            case 2:
                return TreasureType.Health;
            default:
                return TreasureType.Health;
        }
    }
}
