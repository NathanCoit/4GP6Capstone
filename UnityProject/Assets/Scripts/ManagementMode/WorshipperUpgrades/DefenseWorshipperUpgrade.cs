using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DefenseWorshipperUpgrade : WorshipperUpgrade
{
    public int DefenseBuff;

    public DefenseWorshipperUpgrade(
        string pstrUpgradeName,
        string pstrFlavourText,
        int pintCost,
        int pintDefenseBuff)
        : base(pstrUpgradeName, pstrFlavourText, pintCost)
    {
        DefenseBuff = pintDefenseBuff;
        UpgradeType = UPGRADETYPE.Defense;
    }

    public override string UpgradeDescription()
    {
        string strDesc =
mstrFlavourText + @"
Cost: " + UpgradeCost + @"
Defense Buff: " + DefenseBuff;
        return strDesc;
    }
}
