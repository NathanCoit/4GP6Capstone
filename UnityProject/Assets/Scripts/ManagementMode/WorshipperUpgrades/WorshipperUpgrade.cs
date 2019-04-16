using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// Base class to define common attributes and methods shared among all worshipper upgrades
/// All worshipper rewards will inherit from this
/// </summary>
public class WorshipperUpgrade
{
    public enum UPGRADETYPE
    {
        Attack,
        Defense,
        Movement
    }
    // Attributes shared among all upgrade types
    public int UpgradeCost = 0;
    public string UpgradeName;
    public UPGRADETYPE UpgradeType;

    protected string mstrFlavourText;

    /// <summary>
    /// Base constructor inherited by all upgrades
    /// Sets common attributes for an ability
    /// </summary>
    /// <param name="pstrUpgradeName"></param>
    /// <param name="pstrFlavourText"></param>
    /// <param name="pintUpgradeCost"></param>
    public WorshipperUpgrade(string pstrUpgradeName, string pstrFlavourText, int pintUpgradeCost)
    {
        UpgradeName = pstrUpgradeName;
        mstrFlavourText = pstrFlavourText;
        UpgradeCost = pintUpgradeCost;
    }

    /// <summary>
    /// Base method for returning a formatted description of the upgrade.
    /// To be overidden in all child classes.
    /// </summary>
    /// <returns></returns>
    public virtual string UpgradeDescription()
    {
        string strAbilityDescription = "Base ability with no affect on gameplay.";
        return strAbilityDescription;
    }
}
