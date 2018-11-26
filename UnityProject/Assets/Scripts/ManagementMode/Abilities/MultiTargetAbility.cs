using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected override bool LoadAbility(string pstrAbilityName)
    {
        bool AbilityFound = true;
        Type = AbilityType.MultiTarget;
        switch(pstrAbilityName.ToLower())
        {
            case "mushroomlaser":
                AbilityDescription = "Shoot a godly mushroom laser to erase your foes.";
                AbilityDamage = 100;
                AbilityScalingDamage = 0.8f;
                AbilityShape = MultiTargetShape.Line;
                Length = 3;
                break;
            default:
                AbilityFound = false;
                break;
        }
        return AbilityFound;
    }
}
