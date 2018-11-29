using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability{
    public string AbilityName;
    public string AbilityDescription;
    public AbilityType Type;
    public int Range = 0;
    public int FaithCost;

    public enum AbilityType
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

    public enum BuffType
    {
        Healing,
        Damage,
        Defense,
        Shield,
        Speed
    }

    public enum DebuffType
    {
        DamageReduction,
        DefenseReduction,
        Stun,
        Paralyze, // Or root?
        Burn,
        Poison,
        Slow,
        Charm,
        Blind
    }

    public static List<string> SingleTargetAbilities = new List<string>
    {
        "throwmushroom",
        "paintslap",
        "throwfork",
        "kick",
        "quack",
        "chihuahua",
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
        "corgi",
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
        "poodle",
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
        "cat",
        "outoftunesolo",
        "charm",
        "armorbreak",
        "burn",
        "stun",
        "root"
    };

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

    protected virtual bool LoadAbility(string pstrAbilityName)
    {
        return false;
    }

    public static Ability LoadAbilityFromName(string pstrAbilityName)
    {
        Ability loadedAbility = null;
        string strFormattedAbilityName = pstrAbilityName.ToLower().Replace(" ", string.Empty);

        if (SingleTargetAbilities.Contains(strFormattedAbilityName))
        {
            loadedAbility = new SingleTargetAbility(strFormattedAbilityName);
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
}
