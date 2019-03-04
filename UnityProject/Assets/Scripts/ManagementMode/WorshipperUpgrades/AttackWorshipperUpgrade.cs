﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class AttackWorshipperUpgrade : WorshipperUpgrade
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
    }

    public override string UpgradeDescription()
    {
        string strDesc =
mstrFlavourText + @"
Cost: " + UpgradeCost + @"
Damage Buff: " + DamageBuff;
        return strDesc;
    }
}
