using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineBuilding : Building {
    public int Miners = 0;
    public int MinerCap = 0;

    public MineBuilding(BUILDING_TYPE penumBuildingType, Faction pFactionOwner)
        :base (penumBuildingType, pFactionOwner)
    {
        Miners = 0;
        MinerCap = 100;
    }

    public MineBuilding(GameInfo.SavedBuilding pobjSavedBuilding, Faction pobjFactionOwner)
        :base(pobjSavedBuilding, pobjFactionOwner)
    {
        Miners = pobjSavedBuilding.Miners;
        MinerCap = (int)Mathf.Pow(10, UpgradeLevel + 1);
    }

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

    public override bool UpgradeBuilding(bool outline = true, bool pblnNoCost = false)
    {
        bool blnReturn = base.UpgradeBuilding(outline, pblnNoCost);
        if (blnReturn)
        {
            MinerCap = (int)Mathf.Pow(10, UpgradeLevel+1);
        }
        return blnReturn;
    }
}
