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
        Mushrooms,
        Shoes,
        Forks,
        Ducks,
        Jazz,
        Hounds,
        Love,
        Fire,
        Water,
        Earth //etc.
    }
    public static List<GodType> TierOneGods = new List<GodType>
    {
        GodType.Mushrooms,
        GodType.Shoes,
        GodType.Forks,
        GodType.Ducks
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

    public static List<string> GodNames = new List<string>
    {
        "Jim",
        "Jimithy",
        "Tim",
        "Joe",
        "Leon",
        "Leopold",
        "Rekemis",
        "Phoztarr",
        "Weneyar",
        "Izotz",
        "Qhogtyx",
        "Mekdohr",
        "Weton",
        "Thormus",
        "Xihaos",
        "Anir",
        "Phelton",
        "Phiysus",
        "Qhenos",
        "Phurarus",
        "Shanas",
        "Xarmjir",
        "Phelo",
        "Calisto",
        "Tatmes",
        "Retarr",
        "Xugldir",
        "Ryyja",
        "Butia",
        "Brixara",
        "Druanke",
        "Gaghena",
        "Ylsoi",
        "Byanke",
        "Ghutana",
        "Qhixrena",
        "Ynja",
        "Zoais",
        "Elaris",
        "Vavren",
        "Xumlir",
        "Nehas",
        "Xoaldin",
        "Qiserin",
        "Qhilos",
        "Sakesis",
        "Ezdite"
    };

    public static List<Ability> GetGodAbilities(GodType penumGodType)
    {
        List<Ability> GodAbilities = new List<Ability>();
        switch(penumGodType)
        {
            case GodType.Ducks:
                GodAbilities.Add(GameInfo.LoadAbility("Quack"));
                GodAbilities.Add(GameInfo.LoadAbility("Quack!!"));
                GodAbilities.Add(GameInfo.LoadAbility("Quack?"));
                GodAbilities.Add(GameInfo.LoadAbility("Quack¿"));
                break;
            case GodType.Earth:
                GodAbilities.Add(GameInfo.LoadAbility("Tornado"));
                GodAbilities.Add(GameInfo.LoadAbility("Earthquake"));
                GodAbilities.Add(GameInfo.LoadAbility("Root"));
                break;
            case GodType.Fire:
                GodAbilities.Add(GameInfo.LoadAbility("FireBall"));
                GodAbilities.Add(GameInfo.LoadAbility("Lava River"));
                GodAbilities.Add(GameInfo.LoadAbility("Ignite Weapons"));
                GodAbilities.Add(GameInfo.LoadAbility("Burn"));
                break;
            case GodType.Forks:
                GodAbilities.Add(GameInfo.LoadAbility("Throw Fork"));
                GodAbilities.Add(GameInfo.LoadAbility("Fork Sweep"));
                GodAbilities.Add(GameInfo.LoadAbility("Eat Spaghett"));
                GodAbilities.Add(GameInfo.LoadAbility("Fork Flash"));
                break;
            case GodType.Hounds:
                GodAbilities.Add(GameInfo.LoadAbility("Chihuahua"));
                GodAbilities.Add(GameInfo.LoadAbility("Poodle"));
                GodAbilities.Add(GameInfo.LoadAbility("Corgi"));
                GodAbilities.Add(GameInfo.LoadAbility("Cat"));
                break;
            case GodType.Jazz:
                GodAbilities.Add(GameInfo.LoadAbility("Baton Slap"));
                GodAbilities.Add(GameInfo.LoadAbility("Jazz Hands"));
                GodAbilities.Add(GameInfo.LoadAbility("Sax Solo"));
                GodAbilities.Add(GameInfo.LoadAbility("Out of Tune Solo"));
                break;
            case GodType.Love:
                GodAbilities.Add(GameInfo.LoadAbility("Blow A Kiss"));
                GodAbilities.Add(GameInfo.LoadAbility("Giant Heart Slap"));
                GodAbilities.Add(GameInfo.LoadAbility("Slap Ass"));
                GodAbilities.Add(GameInfo.LoadAbility("Charm"));
                break;
            case GodType.Mushrooms:
                GodAbilities.Add(GameInfo.LoadAbility("Throw Mushroom"));
                GodAbilities.Add(GameInfo.LoadAbility("Eat Mushroom"));
                GodAbilities.Add(GameInfo.LoadAbility("Spread Spores"));
                GodAbilities.Add(GameInfo.LoadAbility("Mushroom Laser"));
                break;
            case GodType.Shoes:
                GodAbilities.Add(GameInfo.LoadAbility("Kick"));
                GodAbilities.Add(GameInfo.LoadAbility("Yeezys"));
                GodAbilities.Add(GameInfo.LoadAbility("Leg Sweep"));
                GodAbilities.Add(GameInfo.LoadAbility("Broken Ankles"));
                break;
            case GodType.Water:
                GodAbilities.Add(GameInfo.LoadAbility("Drown"));
                GodAbilities.Add(GameInfo.LoadAbility("Tsunami"));
                GodAbilities.Add(GameInfo.LoadAbility("Stay Hydrated"));
                break;
        }
        return GodAbilities;
    }
}

