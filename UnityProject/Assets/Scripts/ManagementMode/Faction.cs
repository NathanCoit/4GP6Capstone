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

    public Faction(GameInfo.SavedFaction savedFaction)
    {
        GodName = savedFaction.GodName;
        Type = savedFaction.Type;
        GodTier = savedFaction.GodTier;
        MaterialCount = savedFaction.MatieralCount;
        Morale = savedFaction.Morale > 0.2f ? savedFaction.Morale : 0.2f;
        WorshipperCount = savedFaction.WorshipperCount;
        FactionArea = new List<float[]>();
        foreach (GameInfo.SavedArea area in savedFaction.FactionArea)
        {
            FactionArea.Add(
                new float[]
                {
                    area.StartingRad,
                    area.EndingRad,
                    area.StartingAngle,
                    area.EndingAngle
                });
        }
        CurrentAbilites = new List<Ability>();
        foreach (string AbilityName in savedFaction.Abilities)
        {
            CurrentAbilites.Add(Ability.LoadAbilityFromName(AbilityName));
        }
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
        "Jimothy",
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
        "Ezdite",
        "Martavias"
    };

    public static List<Ability> GetGodAbilities(GodType penumGodType)
    {
        List<Ability> GodAbilities = new List<Ability>();
        switch(penumGodType)
        {
            case GodType.Ducks:
                GodAbilities.Add(Ability.LoadAbilityFromName("Quack"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Quack!!"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Quack?"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Quack¿"));
                break;
            case GodType.Earth:
                GodAbilities.Add(Ability.LoadAbilityFromName("Tornado"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Earthquake"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Root"));
                break;
            case GodType.Fire:
                GodAbilities.Add(Ability.LoadAbilityFromName("FireBall"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Lava River"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Ignite Weapons"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Burn"));
                break;
            case GodType.Forks:
                GodAbilities.Add(Ability.LoadAbilityFromName("Throw Fork"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Fork Sweep"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Eat Spaghett"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Fork Flash"));
                break;
            case GodType.Hounds:
                GodAbilities.Add(Ability.LoadAbilityFromName("Chihuahua"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Poodle"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Corgi"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Cat"));
                break;
            case GodType.Jazz:
                GodAbilities.Add(Ability.LoadAbilityFromName("Baton Slap"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Jazz Hands"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Sax Solo"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Out of Tune Solo"));
                break;
            case GodType.Love:
                GodAbilities.Add(Ability.LoadAbilityFromName("Blow A Kiss"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Giant Heart Slap"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Slap Ass"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Charm"));
                break;
            case GodType.Mushrooms:
                GodAbilities.Add(Ability.LoadAbilityFromName("Throw Mushroom"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Eat Mushroom"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Spread Spores"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Mushroom Laser"));
                break;
            case GodType.Shoes:
                GodAbilities.Add(Ability.LoadAbilityFromName("Kick"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Yeezys"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Leg Sweep"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Broken Ankles"));
                break;
            case GodType.Water:
                GodAbilities.Add(Ability.LoadAbilityFromName("Drown"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Tsunami"));
                GodAbilities.Add(Ability.LoadAbilityFromName("Stay Hydrated"));
                break;
        }
        return GodAbilities;
    }
}

