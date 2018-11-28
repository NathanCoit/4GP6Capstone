using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffAbility : Ability{
    public DebuffType debuffType;
    public int debuffAmount;
    public float debuffScalingAmount;
    public bool SingleTarget;
    public int EffectDuration;

    public DebuffAbility(string pstrAbilityName) :
        base(pstrAbilityName)
    {
    }

    protected override bool LoadAbility(string pstrAbilityName)
    {
        bool AbilityFound = true;
        Type = AbilityType.Buff;
        switch (pstrAbilityName.ToLower().Replace(" ", string.Empty))
        {
            case "spreadspores":
                AbilityDescription = "Spread the great word of the mushroom god, through deadly posionous spores.";
                debuffType = DebuffType.Poison;
                debuffAmount = 50;
                debuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 10;
                break;
            case "forkflash":
                AbilityDescription = "Spread the great word of the fork god, through flashy forks.";
                debuffType = DebuffType.Blind;
                debuffAmount = 50;
                debuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 10;
                break;
            case "coloursplash":
                AbilityDescription = "Spread the great word of the paint god, through deadly paint.";
                debuffType = DebuffType.Slow;
                debuffAmount = 50;
                debuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 10;
                break;
            case "brokenankles":
                AbilityDescription = "Spread the great word of the shoe god, through deadly ankle.";
                debuffType = DebuffType.Slow;
                debuffAmount = 50;
                debuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 10;
                break;
            case "quack¿":
                AbilityDescription = "Spread the great word of the duck god, through deadly quacks.";
                debuffType = DebuffType.Burn;
                debuffAmount = 50;
                debuffScalingAmount = 0.5f;
                SingleTarget = true;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 10;
                break;
            case "cat":
                AbilityDescription = "Spread the great word of the hound god, through deadly cats.";
                debuffType = DebuffType.DefenseReduction;
                debuffAmount = 50;
                debuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 10;
                break;
            case "outoftunesolo":
                AbilityDescription = "Spread the great word of the jazz god, through deadly terrible music.";
                debuffType = DebuffType.Paralyze;
                debuffAmount = 50;
                debuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                break;
            case "charm":
                AbilityDescription = "Spread the great word of the love god, through deadly love.";
                debuffType = DebuffType.Charm;
                debuffAmount = 50;
                debuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 10;
                break;
            case "armorbreak":
                AbilityDescription = "Spread the great word of the smith god, through deadly broken armor.";
                debuffType = DebuffType.DefenseReduction;
                debuffAmount = 50;
                debuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 10;
                break;
            case "burn":
                AbilityDescription = "Spread the great word of the fire god, through deadly fire.";
                debuffType = DebuffType.Burn;
                debuffAmount = 50;
                debuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 10;
                break;
            case "stun":
                AbilityDescription = "Spread the great word of the thunder god, through deadly thunder.";
                debuffType = DebuffType.Stun;
                debuffAmount = 50;
                debuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 10;
                break;
            case "root":
                AbilityDescription = "Spread the great word of the nature god, through deadly roots.";
                debuffType = DebuffType.Paralyze;
                debuffAmount = 50;
                debuffScalingAmount = 0.5f;
                SingleTarget = false;
                Range = 3;
                EffectDuration = 2;
                FaithCost = 10;
                break;
            default:
                AbilityFound = false;
                break;
        }
        return AbilityFound;
    }
}
