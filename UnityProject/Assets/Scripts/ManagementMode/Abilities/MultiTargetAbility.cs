using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Multi target ability class containing properties for multi target abilites
/// Multi target abilities may target multiple units
/// Inheritrs from Ability superclass.
/// </summary>
public class MultiTargetAbility : Ability
{
    public int AbilityDamage;
    public float AbilityScalingDamage;
    public MultiTargetShape AbilityShape;
    public int Length; // Size of the ability in tiles (for line length, for square is L x L size)

    public MultiTargetAbility(string pstrAbilityName) :
        base(pstrAbilityName)
    {
    }

    /// <summary>
    /// Override method for loading multi target abilties from ability name.
    /// </summary>
    /// <param name="pstrAbilityName">Name of ability to load.</param>
    /// <returns></returns>
    protected override bool LoadAbility(string pstrAbilityName)
    {
        bool blnAbilityFound = true;
        AbiltyType = ABILITYTYPE.MultiTarget;
        switch(pstrAbilityName.ToLower().Replace(" ", string.Empty))
        {
            case "mushroomlaser":
                AbilityDescription = "Shoot a godly mushroom laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Line;
                Length = 3;
                Range = 3;
                FaithCost = 10;
                break;
            case "paintcannon":
                AbilityDescription = "Shoot a godly paint laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Square;
                Length = 2;
                Range = 3;
                FaithCost = 10;
                break;
            case "forksweep":
                AbilityDescription = "Shoot a godly fork laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Cone;
                Length = 3;
                Range = 1;
                FaithCost = 10;
                break;
            case "legsweep":
                AbilityDescription = "Shoot a godly leg laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Cone;
                Length = 3;
                Range = 1;
                FaithCost = 10;
                break;
            case "quack!!":
                AbilityDescription = "Shoot a godly quack laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Square;
                Length = 3;
                Range = 3;
                FaithCost = 10;
                break;
            case "eyelaser":
                AbilityDescription = "Shoot a godly eye laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Line;
                Length = 4;
                Range = 3;
                FaithCost = 10;
                break;
            case "jazzhands":
                AbilityDescription = "Shoot a godly jazz laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Cone;
                Length = 5;
                Range = 2;
                FaithCost = 10;
                break;
            case "giantheartslap":
                AbilityDescription = "Shoot a godly heart laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Square;
                Length = 3;
                Range = 3;
                FaithCost = 10;
                break;
            case "dropanvil":
                AbilityDescription = "Shoot a godly anvil laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Line;
                Length = 3;
                Range = 3;
                FaithCost = 10;
                break;
            case "lavariver":
                AbilityDescription = "Shoot a laser lava laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Line;
                Length = 3;
                Range = 3;
                FaithCost = 10;
                break;
            case "tsunami":
                AbilityDescription = "Shoot a water water laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Line;
                Length = 3;
                Range = 3;
                FaithCost = 10;
                break;
            case "electricfield":
                AbilityDescription = "Shoot a electric electric laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Line;
                Length = 3;
                Range = 3;
                FaithCost = 10;
                break;
            case "earthquake":
                AbilityDescription = "Shoot a godly earth laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Line;
                Length = 3;
                Range = 3;
                FaithCost = 10;
                break;
            default:
                blnAbilityFound = false;
                break;
        }
        return blnAbilityFound;
    }
}
