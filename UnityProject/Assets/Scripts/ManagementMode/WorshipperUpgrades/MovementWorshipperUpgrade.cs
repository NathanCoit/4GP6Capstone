using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class MovementWorshipperUpgrade : WorshipperUpgrade
{
    public int MovementBuff;

    public MovementWorshipperUpgrade(
        string pstrUpgradeName,
        string pstrFlavourText,
        int pintCost,
        int pintMovementBuff)
        : base(pstrUpgradeName, pstrFlavourText, pintCost)
    {
        MovementBuff = pintMovementBuff;
    }

    public override string UpgradeDescription()
    {
        string strDesc =
mstrFlavourText + @"
Cost: " + UpgradeCost + @"
Movement Buff: " + MovementBuff;
        return strDesc;
    }
}
