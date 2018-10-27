using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// The faction class holds information about each "Faction" currently in the game (i.e. the player, and enemies) 
/// </summary>
public class Faction
{
    public string GodName { get; private set; }
    public int WorshipperCount = 0;
    public int MaterialCount = 0;
    public float Morale = 1.0f;
    public int TierRewardPoints = 1;
    public List<Ability> CurrentAbilites;
    // public List<Upgrade> CurrentUpgrades;
    public List<Building> OwnedBuildings = new List<Building>();

    public Faction(string pstrGodName)
    {
        GodName = pstrGodName;
        CurrentAbilites = new List<Ability>();
    }
}

