using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single target ability class containing properties for single target abilites
/// Single target abilities may only damage one target a time
/// Inheritrs from Ability superclass.
/// </summary>
public class SingleTargetAbility : Ability
{
    public int AbilityDamage;
    public float AbilityScalingDamage;

    public SingleTargetAbility(string pstrAbilityName) :
        base(pstrAbilityName)
    {
        
    }

    /// <summary>
    /// Load the single target ability and its properties
    /// </summary>
    /// <param name="pstrAbilityName"></param>
    /// <returns></returns>
    protected override bool LoadAbility(string pstrAbilityName)
    {
        bool blnAbilityFound = true;
        AbiltyType = ABILITYTYPE.SingleTarget;
        switch (pstrAbilityName.ToLower().Replace(" ", string.Empty))
        {
            case "throwmushroom":
                AbilityDescription = "Smite your enemy with a mushroom";
                AbilityDamage = 10;
                AbilityScalingDamage = 0.5f;
                Range = 3;
                FaithCost = 10;
                break;
            case "paintslap":
                AbilityDescription = "Smite your enemy with a paintbrush";
                AbilityDamage = 10;
                AbilityScalingDamage = 0.5f;
                Range = 2;
                FaithCost = 10;
                break;
            case "throwfork":
                AbilityDescription = "Smite your enemy with a fork";
                AbilityDamage = 10;
                AbilityScalingDamage = 0.5f;
                Range = 3;
                FaithCost = 10;
                break;
            case "kick":
                AbilityDescription = "Smite your enemy with a foot";
                AbilityDamage = 10;
                AbilityScalingDamage = 0.5f;
                Range = 1;
                FaithCost = 10;
                break;
            case "quack":
                AbilityDescription = "Smite your enemy with a quack";
                AbilityDamage = 10;
                AbilityScalingDamage = 0.5f;
                Range = 3;
                FaithCost = 10;
                break;
            case "chihuahua":
                AbilityDescription = "Smite your enemy with a dog";
                AbilityDamage = 10;
                AbilityScalingDamage = 0.5f;
                Range = 3;
                FaithCost = 10;
                break;
            case "batonslap":
                AbilityDescription = "Smite your enemy with a baton";
                AbilityDamage = 10;
                AbilityScalingDamage = 0.5f;
                Range = 2;
                FaithCost = 10;
                break;
            case "blowakiss":
                AbilityDescription = "Smite your enemy with a kiss";
                AbilityDamage = 10;
                AbilityScalingDamage = 0.5f;
                Range = 3;
                FaithCost = 10;
                break;
            case "hammerslap":
                AbilityDescription = "Smite your enemy with a hammer";
                AbilityDamage = 10;
                AbilityScalingDamage = 0.5f;
                Range = 1;
                FaithCost = 10;
                break;
            case "fireball":
                AbilityDescription = "Smite your enemy with a fireball";
                AbilityDamage = 30;
                AbilityScalingDamage = 0.5f;
                Range = 4;
                FaithCost = 10;
                break;
            case "drown":
                AbilityDescription = "Smite your enemy with a drowning";
                AbilityDamage = 50;
                AbilityScalingDamage = 0.5f;
                Range = 1;
                FaithCost = 10;
                break;
            case "smite":
                AbilityDescription = "Smite your enemy with a smite";
                AbilityDamage = 20;
                AbilityScalingDamage = 0.5f;
                Range = 5;
                FaithCost = 10;
                break;
            case "tornado":
                AbilityDescription = "Smite your enemy with a tornado";
                AbilityDamage = 40;
                AbilityScalingDamage = 0.5f;
                Range = 3;
                FaithCost = 10;
                break;
            default:
                blnAbilityFound = false;
                break;
        }
        return blnAbilityFound;
    }

    public string GetDamageString(int GodStrength)
    {
        return string.Format("Does {0} + {1} damage to a single unit.", AbilityDamage, AbilityScalingDamage * GodStrength);
    }
}
