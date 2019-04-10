using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that handles the creation and interactions with treasure objects on the map.
/// Used to reward the player for exploring the map.
/// </summary>
public class TreasureManager : MonoBehaviour
{
    public enum TreasureType
    {
        Attack,
        Defense,
        Health
    }

    public TerrainMap GameMap;
    public GameInfo GameInfoObject;
    public GameManager GameManagerScript;
    public PlayerGodController PlayerGod;
    public InformationBoxDisplay InformationBoxController;

    /// <summary>
    /// Helper getter for all treasures on the map.
    /// </summary>
    public List<Treasure> Treasures
    {
        get
        {
            return GameMap.GetTreasures();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check the player god is colliding with a treasure, if so, unlock it
        if (GameManagerScript.CurrentMenuState == GameManager.MENUSTATE.God_Selected_State)
        {
            foreach (Treasure musTreasure in Treasures)
            {
                if (Vector3.Distance(musTreasure.ObjectPosition, PlayerGod.PlayerGod.transform.position) < 5)
                {
                    UnlockTreasure(musTreasure);
                    // Break as treasure is being deleted, breaking foreach loop
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Method to add the stats from a given unlocked treasure and
    /// provide feedback to the player.
    /// </summary>
    /// <param name="pmusTreasure"></param>
    private void UnlockTreasure(Treasure pmusTreasure)
    {
        switch (pmusTreasure.Type)
        {
            case Treasure.TreasureType.Attack:
                GameInfoObject.GodAttackMultiplier += 0.2f;
                break;
            case Treasure.TreasureType.Defense:
                GameInfoObject.GodDefenseMultiplier += 0.2f;
                break;
            case Treasure.TreasureType.Health:
                GameInfoObject.GodHealthMultiplier += 0.2f;
                break;
        }
        InformationBoxController.DisplayInformationBox(
pmusTreasure.Type + @" treasure found!
" + pmusTreasure.Type + " increased by 0.2!");
        GameManagerScript.MenuPanelController.EnterGodSelectedState(GameManagerScript.PlayerFaction, GameInfoObject);
        RemoveTreasure(pmusTreasure);
    }

    /// <summary>
    /// Method called once at the beginning of a new game by game manager to create new treasures
    /// Creates a set number of treasure in a given number of tiers.
    /// </summary>
    /// <param name="pintTreasuresPerTier"></param>
    /// <param name="pintTierCount"></param>
    public void CreateNewTreasures(int pintTreasuresPerTier, int pintTierCount)
    {
        Treasure musTreasure;
        int intRetryCount = 0;
        // foreach tier
        for (int i = 0; i < GameManagerScript.MapTierCount; i++)
        {
            for (int j = 0; j < pintTreasuresPerTier; j++)
            {
                musTreasure = new Treasure(Treasure.GetRandomTreasureType());
                intRetryCount = 0;

                // Attempt to place the treasure 100 times to avoid random position hitting other treasures
                while (!GameMap.PlaceTreasure(musTreasure, GameMap.CalculateRandomPositionInTier(i)) && intRetryCount < 100)
                {
                    intRetryCount++;
                }

                // Hide treasure for higher tiers
                if (i > 0)
                {
                    musTreasure.MapGameObject.SetActive(false);
                }
            }
        }
    }
    
    /// <summary>
    /// Method for loading treasures from a save state
    /// </summary>
    /// <param name="parrSavedTreasures"></param>
    public void LoadSavedTreasures(GameInfo.SavedTreasure[] parrSavedTreasures)
    {
        Treasure musTreasure;
        Vector3 uniCetreVec3 = new Vector3(0, 1.5f, 0);
        float fDistance;
        if (parrSavedTreasures != null)
        {
            foreach (GameInfo.SavedTreasure musSavedTreasure in parrSavedTreasures)
            {
                musTreasure = new Treasure(musSavedTreasure);
                GameMap.PlaceTreasure(
                    musTreasure,
                    new Vector3(musSavedTreasure.x, musSavedTreasure.y, musSavedTreasure.z),
                    true);
                fDistance = Vector3.Distance(musTreasure.ObjectPosition, uniCetreVec3);
                // check if treasure is out of current game tier radius
                // Assumes 3 tiers and map diameter of 500
                if (fDistance > (GameManagerScript.CurrentTier + 1) * (250 / 3))
                {
                    musTreasure.MapGameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Method for removing a treasure from the game once it has been unlocked
    /// </summary>
    /// <param name="pmusTreasure"></param>
    private void RemoveTreasure(Treasure pmusTreasure)
    {
        GameMap.RemoveTreasure(pmusTreasure);
        pmusTreasure.Destroy();
    }
}
