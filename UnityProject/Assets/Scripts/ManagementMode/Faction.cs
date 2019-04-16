using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// The faction class holds information about a faction.
/// Factions are representations of a player or enemy character including
/// their territories, owned buildings and stats.
/// </summary>
public class Faction
{
    public enum GodType
    {
        Mushrooms,
        Shoes,
        Forks,
        Ducks,
        Smiths,
        Robots,
        Love,
        Fire,
        Water,
        Lightning //etc.
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
        GodType.Smiths,
        GodType.Robots,
        GodType.Love
    };

    public static List<GodType> TierThreeGods = new List<GodType>
    {
        GodType.Fire,
        GodType.Water,
        GodType.Lightning
    };

    public string GodName { get; private set; }
    public GodType Type { get; private set; }
    public int WorshipperCount = 0;
    public int MaterialCount = 0;
    public float Morale = 1.0f;
    public int TierRewardPoints = 1;
    public List<Ability> CurrentAbilites;
    public List<WorshipperUpgrade> CurrentUpgrades;
    public List<Building> OwnedBuildings = new List<Building>();
    public List<float[]> FactionArea; //Starting Radius, ending Radius, starting angle, ending angle
    public int FactionDifficulty = 0;
    public int GodTier = 0;

    /// <summary>
    /// Contructor for creating a new god
    /// </summary>
    /// <param name="pstrGodName"></param>
    /// <param name="penumGodType"></param>
    /// <param name="pintTier"></param>
    public Faction(string pstrGodName, GodType penumGodType, int pintTier)
    {
        GodName = pstrGodName;
        CurrentAbilites = new List<Ability>();
        CurrentUpgrades = new List<WorshipperUpgrade>();
        Type = penumGodType;
        GodTier = pintTier;
    }

    /// <summary>
    /// Constructor for loading an already created god from a save state
    /// </summary>
    /// <param name="savedFaction"></param>
    public Faction(GameInfo.SavedFaction savedFaction)
    {
        GodName = savedFaction.GodName;
        Type = savedFaction.Type;
        GodTier = savedFaction.GodTier;
        MaterialCount = savedFaction.MatieralCount;
        Morale = savedFaction.Morale > 0.2f ? savedFaction.Morale : 0.2f;
        WorshipperCount = savedFaction.WorshipperCount;
        FactionArea = new List<float[]>();
        foreach (GameInfo.SavedArea musSavedArea in savedFaction.FactionArea)
        {
            FactionArea.Add(
                new float[]
                {
                    musSavedArea.StartingRad,
                    musSavedArea.EndingRad,
                    musSavedArea.StartingAngle,
                    musSavedArea.EndingAngle
                });
        }
        CurrentAbilites = new List<Ability>();
        CurrentUpgrades = new List<WorshipperUpgrade>();
        foreach (string strAbilityName in savedFaction.Abilities)
        {
            CurrentAbilites.Add(Ability.LoadAbilityFromName(strAbilityName));
        }
    }

    /// <summary>
    /// Helper method to hid all buildings owned by this faction.
    /// Used to hide higher tier gods.
    /// </summary>
    /// <param name="pblnHide"></param>
    public void SetHidden(bool pblnHide = true)
    {
        if (pblnHide)
        {
            foreach (Building musBuildingToHide in OwnedBuildings)
            {
                musBuildingToHide.MapGameObject.SetActive(false);
            }
        }
        else
        {
            foreach (Building musBuildingToShow in OwnedBuildings)
            {
                musBuildingToShow.MapGameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// List of god names randomly selected from on new game.
    /// </summary>
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

    /// <summary>
    /// Helper method to get the predefined abilities based on the given god type
    /// Used to load the abilities of a god on new game.
    /// </summary>
    /// <param name="penumGodType"></param>
    /// <returns></returns>
    public static List<Ability> GetGodAbilities(GodType penumGodType)
    {
        List<Ability> arrGodAbilities = new List<Ability>();
        switch (penumGodType)
        {
            case GodType.Ducks:
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Quack"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Quack!!"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Quack?"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Quack¿"));
                break;
            case GodType.Lightning:
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Smite"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Electric Field"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Stun"));
                break;
            case GodType.Fire:
                arrGodAbilities.Add(Ability.LoadAbilityFromName("FireBall"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Lava River"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Ignite Weapons"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Burn"));
                break;
            case GodType.Forks:
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Throw Fork"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Fork Sweep"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Eat Spaghett"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Fork Flash"));
                break;
            case GodType.Robots:
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Electrocute"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Eye Laser"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Analyze"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Better Programming"));
                break;
            case GodType.Smiths:
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Hammer Slap"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Drop Anvil"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Sharpen Arms"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Armor Break"));
                break;
            case GodType.Love:
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Blow A Kiss"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Giant Heart Slap"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Slap Ass"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Charm"));
                break;
            case GodType.Mushrooms:
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Throw Mushroom"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Eat Mushroom"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Spread Spores"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Mushroom Laser"));
                break;
            case GodType.Shoes:
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Kick"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Yeezys"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Leg Sweep"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Broken Ankles"));
                break;
            case GodType.Water:
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Drown"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Tsunami"));
                arrGodAbilities.Add(Ability.LoadAbilityFromName("Stay Hydrated"));
                break;
        }
        return arrGodAbilities;
    }
}

