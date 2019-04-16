using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Class containing information for a treasure object on the map.
/// Used to promote exploration by the player throughout the map by rewarding the player with god stats.
/// </summary>
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

    /// <summary>
    /// Helper method to load a treasure from a save state
    /// </summary>
    /// <param name="pmusSavedTreasure"></param>
    public Treasure(GameInfo.SavedTreasure pmusSavedTreasure)
    {
        CreateTreasureObject();
        Type = pmusSavedTreasure.Type;
    }

    /// <summary>
    /// Load the treasure prefab and create a treasure game object
    /// </summary>
    private void CreateTreasureObject()
    {
        UnityEngine.Object uniTreasureObject = Resources.Load("Treasure_Chest");
        if (uniTreasureObject != null)
        {
            MapGameObject = (GameObject)GameObject.Instantiate(uniTreasureObject);
        }
    }

    /// <summary>
    /// Helper method for destroying the chest game object
    /// </summary>
    public void Destroy()
    {
        GameObject.Destroy(MapGameObject);
    }

    /// <summary>
    /// Helper method for generating a random treasure type.
    /// Used to help initial treasure generation
    /// </summary>
    /// <returns></returns>
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
