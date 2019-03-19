using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Super class for defining abilities
/// All other abilities inherit from this class. 
/// Each ability type will have different properties for use in combat mode.
/// </summary>
public class Ability{
    public string AbilityName;
    public string AbilityDescription;
    public ABILITYTYPE AbiltyType;
    public int Range = 0;
    public int FaithCost;

    public enum ABILITYTYPE
    {
        SingleTarget,
        MultiTarget,
        Buff,
        Debuff
        // Other in waiting room (alter terrain, do something custom, etc.)
    };

    public enum MultiTargetShape
    {
        Line,
        Square,
        Cone // "cone" shape in front of god
    }

    public enum BUFFTYPE
    {
        Healing,
        Damage,
        Defense,
        Shield,
        Speed
    }

    public enum DEBUFFTYPE
    {
        DamageReduction,
        DefenseReduction,
        Stun,
        Paralyze, // Or root?
        Burn,
        Poison,
        Slow,
        Blind
    }

    // Lists of each type of ability for loading purposes.
    public static List<string> SingleTargetAbilities = new List<string>
    {
        "throwmushroom",
        "paintslap",
        "throwfork",
        "kick",
        "quack",
        "electrocute",
        "batonslap",
        "blowakiss",
        "hammerslap",
        "fireball",
        "drown",
        "smite",
        "tornado"
    };
    public static List<string> MultiTargetAbilities = new List<string>
    {
        "mushroomlaser",
        "paintcannon",
        "forksweep",
        "legsweep",
        "quack!!",
        "eyelaser",
        "jazzhands",
        "giantheartslap",
        "dropanvil",
        "lavariver",
        "tsunami",
        "electricfield",
        "earthquake"
    };
    public static List<string> BuffAbilities = new List<string>
    {
        "eatmushroom",
        "warpaint",
        "eatspaghett",
        "yeezys",
        "quack?",
        "betterprogramming",
        "saxsolo",
        "slapass",
        "sharpenarms",
        "igniteweapons",
        "stayhydrated"
    };
    public static List<string> DebuffAbilities = new List<string>
    {
        "spreadspores",
        "forkflash",
        "coloursplash",
        "brokenankles",
        "quack¿",
        "analyze",
        "outoftunesolo",
        "charm",
        "armorbreak",
        "burn",
        "stun",
        "root"
    };

    /// <summary>
    /// Base constructor. Load ability and give it a name
    /// </summary>
    /// <param name="pstrAbilityName"></param>
    public Ability(string pstrAbilityName)
    {
        if (LoadAbility(pstrAbilityName))
        {
            AbilityName = pstrAbilityName;
        }
        else
        {
            throw new System.Exception(pstrAbilityName + " is not a defined ability.");
        }
    }

    /// <summary>
    /// Method for loading an ability.
    /// Override this for each type of ability
    /// </summary>
    /// <param name="pstrAbilityName">The name of the ability to load.</param>
    /// <returns></returns>
    protected virtual bool LoadAbility(string pstrAbilityName)
    {
        return false;
    }

    /// <summary>
    /// Method for loading ability
    /// Returns an ability loaded based on its type.
    /// </summary>
    /// <param name="pstrAbilityName"></param>
    /// <returns></returns>
    public static Ability LoadAbilityFromName(string pstrAbilityName)
    {
        Ability loadedAbility = null;
        string strFormattedAbilityName = pstrAbilityName.ToLower().Replace(" ", string.Empty);

        if (SingleTargetAbilities.Contains(strFormattedAbilityName))
        {
            loadedAbility = new SingleTargetAbility(pstrAbilityName);
        }
        else if (MultiTargetAbilities.Contains(strFormattedAbilityName))
        {
            loadedAbility = new MultiTargetAbility(pstrAbilityName);
        }
        else if (BuffAbilities.Contains(strFormattedAbilityName))
        {
            loadedAbility = new BuffAbility(pstrAbilityName);
        }
        else if (DebuffAbilities.Contains(strFormattedAbilityName))
        {
            loadedAbility = new DebuffAbility(pstrAbilityName);
        }
        return loadedAbility;
    }

    /// <summary>
    /// Virtual method for returning the description of an ability
    /// Override in each ability subclass to return an appropriate description of that ability type.
    /// Used to get a nicely formatted string containing numerical data about ability.
    /// </summary>
    /// <returns></returns>
    public virtual string GetAbilityDescription()
    {
        string strAbilityDescription = string.Empty;
        return strAbilityDescription;
    }
}
