using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A child class of building. Contains separate functionality and attributes needed for mines.
/// </summary>
public class MineBuilding : Building {
    public int Miners = 0;
    public int MinerCap = 0;

    private const int mcintInitialMinerCap = 100;

    public MineBuilding(BUILDING_TYPE penumBuildingType, Faction pmusOwningFaction)
        :base (penumBuildingType, pmusOwningFaction)
    {
        Miners = 0;
        MinerCap = mcintInitialMinerCap;
    }

    /// <summary>
    /// Method to load a mine from a saved game state
    /// </summary>
    /// <param name="pmusSavedBuilding"></param>
    /// <param name="pmusOwningFaction"></param>
    public MineBuilding(GameInfo.SavedBuilding pmusSavedBuilding, Faction pmusOwningFaction)
        :base(pmusSavedBuilding, pmusOwningFaction)
    {
        Miners = pmusSavedBuilding.Miners;
        MinerCap = (int)Mathf.Pow(mcintMaterialCost, UpgradeLevel + 1);
    }

    /// <summary>
    /// Method for converting worshippers into miners
    /// Allows the scaling of resource generation to depend on worshipper growth
    /// and force player to balance worshippers and resources
    /// </summary>
    /// <param name="pintNumWorshippers"></param>
    /// <returns></returns>
    public bool BuyMiners(int pintNumWorshippers)
    {
        bool blnSuccessful = false;
        if(OwningFaction.WorshipperCount >= pintNumWorshippers && Miners + pintNumWorshippers <= MinerCap)
        {
            Miners += pintNumWorshippers;
            OwningFaction.WorshipperCount -= pintNumWorshippers;
            blnSuccessful = true;
        }
        return blnSuccessful;
    }

    /// <summary>
    /// Overriden method to upgrade building
    /// Allows same upgrade functionality with extension for minercap calc
    /// </summary>
    /// <param name="pblnOutline"></param>
    /// <param name="pblnNoCost"></param>
    /// <returns></returns>
    public override bool UpgradeBuilding(bool pblnOutline = true, bool pblnNoCost = false)
    {
        bool blnReturn = base.UpgradeBuilding(pblnOutline, pblnNoCost);
        if (blnReturn)
        {
            MinerCap = (int)Mathf.Pow(mcintMaterialCost, UpgradeLevel+1);
        }
        return blnReturn;
    }
}
