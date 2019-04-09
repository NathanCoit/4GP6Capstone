using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffAbility : Ability
{
    public BUFFTYPE BuffType;
    public float BuffAmount;
    public float BuffScalingAmount;
    public int MaxTargets; // Set to 0 to only be able to target yourself

    /// <summary>
    /// Buff Ability constructor.
    /// </summary>
    /// <param name="pstrAbilityName">The ability name to load</param>
    public BuffAbility(string pstrAbilityName) :
        base(pstrAbilityName)
    {
    }

    /// <summary>
    /// Load a buff ability. Set the properties of the ability.
    /// </summary>
    /// <param name="pstrAbilityName"></param>
    /// <returns></returns>
    protected override bool LoadAbility(string pstrAbilityName)
    {
        bool blnAbilityFound = true;
        AbiltyType = ABILITYTYPE.Buff;
        switch(pstrAbilityName.ToLower().Replace(" ", string.Empty))
        {
            case "eatmushroom":
                AbilityDescription = "Eat one of your holy mushrooms to heal yourself, cause poisonous mushrooms are good for you!";
                BuffType = BUFFTYPE.Healing;
                BuffAmount = 50;
                BuffScalingAmount = 0.5f;
                MaxTargets = 0;
                Range = 0;
                FaithCost = 20;
                break;
            case "warpaint":
                AbilityDescription = "Buff yourself.";
                BuffType = BUFFTYPE.Damage;
                BuffAmount = 50;
                BuffScalingAmount = 0.5f;
                MaxTargets = 0;
                Range = 0;
                FaithCost = 20;
                break;
            case "eatspaghett":
                AbilityDescription = "Buff yourself.";
                BuffType = BUFFTYPE.Shield;
                BuffAmount = 50;
                BuffScalingAmount = 0.5f;
                MaxTargets = 0;
                Range = 0;
                FaithCost = 20;
                break;
            case "yeezys":
                AbilityDescription = "Buff your feet.";
                BuffType = BUFFTYPE.Speed;
                BuffAmount = 50;
                BuffScalingAmount = 0.5f;
                MaxTargets = 2;
                Range = 3;
                FaithCost = 20;
                break;
            case "quack?":
                AbilityDescription = "Buff your ducks.";
                BuffType = BUFFTYPE.Defense;
                BuffAmount = 50;
                BuffScalingAmount = 0.5f;

                MaxTargets = 3;
                Range = 3;
                FaithCost = 20;
                break;
            case "betterprogramming":
                AbilityDescription = "Buff your programs.";
                BuffType = BUFFTYPE.Damage;
                BuffAmount = 50;
                BuffScalingAmount = 0.5f;
                MaxTargets = 2;
                Range = 3;
                FaithCost = 20;
                break;
            case "saxsolo":
                AbilityDescription = "Buff your ears.";
                BuffType = BUFFTYPE.Healing;
                BuffAmount = 50;
                BuffScalingAmount = 0.5f;
                MaxTargets = 2;
                Range = 3;
                FaithCost = 20;
                break;
            case "slapass":
                AbilityDescription = "Buff your bois.";
                BuffType = BUFFTYPE.Damage;
                BuffAmount = 50;
                BuffScalingAmount = 0.5f;
                MaxTargets = 2;
                Range = 3;
                FaithCost = 20;
                break;
            case "sharpenarms":
                AbilityDescription = "Buff your weapons.";
                BuffType = BUFFTYPE.Healing;
                BuffAmount = 50;
                BuffScalingAmount = 0.5f;
                MaxTargets = 2;
                Range = 3;
                FaithCost = 20;
                break;
            case "igniteweapons":
                AbilityDescription = "Buff your weapons.";
                BuffType = BUFFTYPE.Healing;
                BuffAmount = 50;
                BuffScalingAmount = 0.5f;
                MaxTargets = 3;
                Range = 3;
                FaithCost = 20;
                break;
            case "stayhydrated":
                AbilityDescription = "Buff your life.";
                BuffType = BUFFTYPE.Shield;
                BuffAmount = 50;
                BuffScalingAmount = 0.5f;
                MaxTargets = 4;
                Range = 3;
                FaithCost = 20;
                break;
            default:
                blnAbilityFound = false;
                break;
        }

        return blnAbilityFound;
    }
}
