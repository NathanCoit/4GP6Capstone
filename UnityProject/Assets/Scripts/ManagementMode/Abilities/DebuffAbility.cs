using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffAbility : Ability{
    public DebuffType debuffType;
    public int debuffAmount;
    public float debuffScalingAmount;
    public bool SingleTarget;

    public DebuffAbility(string pstrAbilityName) :
        base(pstrAbilityName)
    {
    }

    protected override bool LoadAbility(string pstrAbilityName)
    {
        bool AbilityFound = true;
        Type = AbilityType.Buff;
        switch (pstrAbilityName.ToLower())
        {
            case "spreadspores":
                AbilityDescription = "Spread the great word of the mushroom god, through deadly posionous spores.";
                debuffType = DebuffType.Poison;
                debuffAmount = 50;
                debuffScalingAmount = 0.5f;
                SingleTarget = false;
                break;
            default:
                AbilityFound = false;
                break;
        }

        return AbilityFound;
    }
}
