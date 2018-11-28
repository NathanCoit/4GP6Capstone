using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffAbility : Ability
{
    public BuffType buffType;
    public float buffAmount;
    public float buffScalingAmount;
    public int MaxTargets; // Set to 0 to only be able to target yourself

    public BuffAbility(string pstrAbilityName) :
        base(pstrAbilityName)
    {
    }

    protected override bool LoadAbility(string pstrAbilityName)
    {
        bool AbilityFound = true;
        Type = AbilityType.Buff;
        switch(pstrAbilityName.ToLower().Replace(" ", string.Empty))
        {
            case "eatmushroom":
                AbilityDescription = "Eat one of your holy mushrooms to heal yourself, cause poisonous mushrooms are good for you!";
                buffType = BuffType.Healing;
                buffAmount = 50;
                buffScalingAmount = 0.5f;
                MaxTargets = 0;
                Range = 0;
                FaithCost = 10;
                break;
            case "warpaint":
                AbilityDescription = "Buff yourself.";
                buffType = BuffType.Damage;
                buffAmount = 50;
                buffScalingAmount = 0.5f;
                MaxTargets = 0;
                Range = 0;
                FaithCost = 10;
                break;
            case "eatspaghett":
                AbilityDescription = "Buff yourself.";
                buffType = BuffType.Shield;
                buffAmount = 50;
                buffScalingAmount = 0.5f;
                MaxTargets = 0;
                Range = 0;
                FaithCost = 10;
                break;
            case "yeezys":
                AbilityDescription = "Buff your feet.";
                buffType = BuffType.Speed;
                buffAmount = 50;
                buffScalingAmount = 0.5f;
                MaxTargets = 2;
                Range = 3;
                FaithCost = 10;
                break;
            case "quack?":
                AbilityDescription = "Buff your ducks.";
                buffType = BuffType.Defense;
                buffAmount = 50;
                buffScalingAmount = 0.5f;

                MaxTargets = 3;
                Range = 3;
                FaithCost = 10;
                break;
            case "poodle":
                AbilityDescription = "Buff your dogs.";
                buffType = BuffType.Damage;
                buffAmount = 50;
                buffScalingAmount = 0.5f;
                MaxTargets = 2;
                Range = 3;
                FaithCost = 10;
                break;
            case "saxsolo":
                AbilityDescription = "Buff your ears.";
                buffType = BuffType.Healing;
                buffAmount = 50;
                buffScalingAmount = 0.5f;
                MaxTargets = 2;
                Range = 3;
                FaithCost = 10;
                break;
            case "slapass":
                AbilityDescription = "Buff your bois.";
                buffType = BuffType.Damage;
                buffAmount = 50;
                buffScalingAmount = 0.5f;
                MaxTargets = 2;
                Range = 3;
                FaithCost = 10;
                break;
            case "sharpenarms":
                AbilityDescription = "Buff your weapons.";
                buffType = BuffType.Healing;
                buffAmount = 50;
                buffScalingAmount = 0.5f;
                MaxTargets = 2;
                Range = 3;
                FaithCost = 10;
                break;
            case "igniteweapons":
                AbilityDescription = "Buff your weapons.";
                buffType = BuffType.Healing;
                buffAmount = 50;
                buffScalingAmount = 0.5f;
                MaxTargets = 3;
                Range = 3;
                FaithCost = 10;
                break;
            case "stayhydrated":
                AbilityDescription = "Buff your life.";
                buffType = BuffType.Shield;
                buffAmount = 50;
                buffScalingAmount = 0.5f;
                MaxTargets = 4;
                Range = 3;
                FaithCost = 10;
                break;
            default:
                AbilityFound = false;
                break;
        }

        return AbilityFound;
    }
}
