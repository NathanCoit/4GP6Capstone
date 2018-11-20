using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// The faction class holds information about each "Faction" currently in the game (i.e. the player, and enemies) 
/// </summary>
public class Faction
{
    public enum GodType
    {
        Mushroom,
        Shoe,
        Fork,
        Duck,
        Jazz,
        Hounds,
        Love,
        Fire,
        Water,
        Earth //etc.
    }
    public static List<GodType> TierOneGods = new List<GodType>
    {
        GodType.Mushroom,
        GodType.Shoe,
        GodType.Fork,
        GodType.Duck
    };

    public static List<GodType> TierTwoGods = new List<GodType>
    {
        GodType.Jazz,
        GodType.Hounds,
        GodType.Love
    };

    public static List<GodType> TierThreeGods = new List<GodType>
    {
        GodType.Fire,
        GodType.Water,
        GodType.Earth
    };

    public string GodName { get; private set; }
    public GodType Type { get; private set; }
    public int WorshipperCount = 0;
    public int MaterialCount = 0;
    public float Morale = 1.0f;
    public int TierRewardPoints = 1;
    public List<Ability> CurrentAbilites;
    // public List<Upgrade> CurrentUpgrades;
    public List<Building> OwnedBuildings = new List<Building>();
    public List<float[]> FactionArea; //Starting Radius, ending Radius, starting angle, ending angle
    public int FactionDifficulty = 0;
    public int GodTier = 0;

    public Faction(string pstrGodName, GodType penumGodType, int pintTier)
    {
        GodName = pstrGodName;
        CurrentAbilites = new List<Ability>();
        Type = penumGodType;
        GodTier = pintTier;
    }

    public void SetHidden(bool pblnHide = true)
    {
        if(pblnHide)
        {
            foreach(Building buildingToHide in OwnedBuildings)
            {
                buildingToHide.BuildingObject.SetActive(false);
            }
        }
        else
        {
            foreach(Building buildingToShow in OwnedBuildings)
            {
                buildingToShow.BuildingObject.SetActive(true);
            }
        }
    }
}

