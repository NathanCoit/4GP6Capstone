using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffAbility : Ability
{
    public BuffType buffType;
    public float buffAmount;
    public float buffScalingAmount;
    public bool SingleTarget;
    public bool Targetable;

    public BuffAbility(string pstrAbilityName) :
        base(pstrAbilityName)
    {
    }

    protected override bool LoadAbility(string pstrAbilityName)
    {
        bool AbilityFound = true;
        Type = AbilityType.Buff;
        switch(pstrAbilityName.ToLower())
        {
            case "eatmushroom":
                AbilityDescription = "Eat one of your holy mushrooms to heal yourself, cause poisonous mushrooms are good for you!";
                buffType = BuffType.Healing;
                buffAmount = 50;
                buffScalingAmount = 0.5f;
                SingleTarget = true;
                Targetable = false;
                break;
            default:
                AbilityFound = false;
                break;
        }

        return AbilityFound;
    }
}
