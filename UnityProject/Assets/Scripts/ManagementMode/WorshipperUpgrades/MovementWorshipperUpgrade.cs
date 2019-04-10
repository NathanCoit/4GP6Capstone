using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Inherits from Worshipper upgrade. Defines attack movement increase and upgrade flavour text
/// </summary>
public class MovementWorshipperUpgrade : WorshipperUpgrade
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
        UpgradeType = UPGRADETYPE.Movement;
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
