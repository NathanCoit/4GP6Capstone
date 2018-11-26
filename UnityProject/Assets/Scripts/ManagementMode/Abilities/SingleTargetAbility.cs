using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTargetAbility : Ability
{
    public int AbilityDamage;
    public float AbilityScalingDamage;

    public SingleTargetAbility(string pstrAbilityName) :
        base(pstrAbilityName)
    {
        
    }

    protected override bool LoadAbility(string pstrAbilityName)
    {
        bool AbilityFound = true;
        Type = AbilityType.SingleTarget;
        switch (pstrAbilityName.ToLower())
        {
            case "throwmushroom":
                AbilityDescription = "Smite your enemy with a mushroom";
                AbilityDamage = 10;
                AbilityScalingDamage = 0.5f;
                break;
            default:
                AbilityFound = false;
                break;
        }
        return AbilityFound;
    }

    public string GetDamageString(int GodStrength)
    {
        return string.Format("Does {0} + {1} damage to a single unit.", AbilityDamage, AbilityScalingDamage * GodStrength);
    }
}
