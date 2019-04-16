using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Inherits from Worshipper upgrade
/// Defines attack stat increase and upgrade flavour text
/// </summary>
public class AttackWorshipperUpgrade : WorshipperUpgrade
{
    public int DamageBuff;

    public AttackWorshipperUpgrade(
        string pstrUpgradeName,
        string pstrFlavourText,
        int pintCost,
        int pintDamageBuff)
        : base(pstrUpgradeName, pstrFlavourText, pintCost)
    {
        DamageBuff = pintDamageBuff;
        UpgradeType = UPGRADETYPE.Attack;
    }

    /// <summary>
    /// override with attack specific details
    /// </summary>
    /// <returns></returns>
    public override string UpgradeDescription()
    {
        string strDesc =
mstrFlavourText + @"
Cost: " + UpgradeCost + @"
Damage Buff: " + DamageBuff;
        return strDesc;
    }
}
