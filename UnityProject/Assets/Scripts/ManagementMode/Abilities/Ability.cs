﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability{
    public string AbilityName;
    public string AbilityDescription;
    public AbilityType Type;

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
        Square
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
        Slow
    }

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
}