using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child class for defining attributes on Debuff abilities
/// for diminishing enemies stats or applying status effects.
/// </summary>
public class DebuffAbility : Ability
{
    public DEBUFFTYPE DebuffType;
    public int DebuffAmount;
    public float DebuffScalingAmount;
    public bool SingleTarget;
    public int EffectDuration;

    /// <summary>
    /// Override constructor for loading debuff abilties
    /// </summary>
    /// <param name="pstrAbilityName"></param>
    public DebuffAbility(string pstrAbilityName) :
        base(pstrAbilityName)
    {
    }

    /// <summary>
    /// Load a debuff ability. Set the properties of the ability.
    /// Called from Debuff Ability constructor.
    /// See Ability.LoadAbility
    /// </summary>
    /// <param name="pstrAbilityName"></param>
    /// <returns></returns>
    protected override bool LoadAbility(string pstrAbilityName)
    {
        bool blnAbilityFound = true;
        AbiltyType = ABILITYTYPE.Debuff;
        switch (pstrAbilityName.ToLower().Replace(" ", string.Empty))
        {
            case "spreadspores":
                AbilityDescription = "Spread the great word of the mushroom god, through deadly posionous spores.";
                DebuffType = DEBUFFTYPE.Poison;
                DebuffAmount = 50;
                DebuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 20;
                break;
            case "forkflash":
                AbilityDescription = "Spread the great word of the fork god, through flashy forks.";
                DebuffType = DEBUFFTYPE.Blind;
                DebuffAmount = 50;
                DebuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 20;
                break;
            case "coloursplash":
                AbilityDescription = "Spread the great word of the paint god, through deadly paint.";
                DebuffType = DEBUFFTYPE.Slow;
                DebuffAmount = 50;
                DebuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 20;
                break;
            case "brokenankles":
                AbilityDescription = "Spread the great word of the shoe god, through deadly ankle.";
                DebuffType = DEBUFFTYPE.Slow;
                DebuffAmount = 50;
                DebuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 20;
                break;
            case "quack¿":
                AbilityDescription = "Spread the great word of the duck god, through deadly quacks.";
                DebuffType = DEBUFFTYPE.Burn;
                DebuffAmount = 50;
                DebuffScalingAmount = 0.5f;
                SingleTarget = true;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 20;
                break;
            case "analyze":
                AbilityDescription = "Spread the great word of the robot god, through deadly analyzing.";
                DebuffType = DEBUFFTYPE.DefenseReduction;
                DebuffAmount = 50;
                DebuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 20;
                break;
            case "outoftunesolo":
                AbilityDescription = "Spread the great word of the jazz god, through deadly terrible music.";
                DebuffType = DEBUFFTYPE.Paralyze;
                DebuffAmount = 50;
                DebuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                break;
            case "charm":
                AbilityDescription = "Spread the great word of the love god, through deadly love. Makes enemy attacks weaker.";
                DebuffType = DEBUFFTYPE.DamageReduction;
                DebuffAmount = 50;
                DebuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 20;
                break;
            case "armorbreak":
                AbilityDescription = "Spread the great word of the smith god, through deadly broken armor.";
                DebuffType = DEBUFFTYPE.DefenseReduction;
                DebuffAmount = 50;
                DebuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 20;
                break;
            case "burn":
                AbilityDescription = "Spread the great word of the fire god, through deadly fire.";
                DebuffType = DEBUFFTYPE.Burn;
                DebuffAmount = 50;
                DebuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 20;
                break;
            case "stun":
                AbilityDescription = "Spread the great word of the thunder god, through deadly thunder.";
                DebuffType = DEBUFFTYPE.Stun;
                DebuffAmount = 50;
                DebuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 20;
                break;
            case "root":
                AbilityDescription = "Spread the great word of the nature god, through deadly roots.";
                DebuffType = DEBUFFTYPE.Paralyze;
                DebuffAmount = 50;
                DebuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 20;
                break;
            default:
                blnAbilityFound = false;
                break;
        }
        return blnAbilityFound;
    }
}
