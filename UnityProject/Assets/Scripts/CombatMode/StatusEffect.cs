using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used as a wrapper for buff and debuff abilites so we can reference them as the same thing more easily

public class StatusEffect
{
    //Duration of the effect
    private int turnsInEffect;

    //Buff of debuff type
    private Ability.BUFFTYPE buffType;
    private Ability.DEBUFFTYPE debuffType;

    //If it is a buff
    private bool isBuff;
    private BuffAbility sourceBuffAbility;
    private DebuffAbility sourceDebuffAbility;


    //Two constructors for both buff and debuff
    public StatusEffect(BuffAbility a, int numOfTurns)
    {
        isBuff = true;
        sourceBuffAbility = a;
        buffType = a.BuffType;
        turnsInEffect = numOfTurns;
    }

    public StatusEffect(DebuffAbility a, int numOfTurns)
    {
        isBuff = false;
        sourceDebuffAbility = a;
        debuffType = a.DebuffType;
        turnsInEffect = numOfTurns;
    }

    //Check if we're a buff or debuff
    public string getType()
    {
        if (isBuff)
            return buffType.ToString();
        else
            return debuffType.ToString();
    }

    //Get the assosiated ability
    public string getAbility()
    {
        if (isBuff)
            return sourceBuffAbility.AbilityName;
        else
            return sourceDebuffAbility.AbilityName;
    }

    //Tick down our duration
    public void decreaseDuration()
    {
        turnsInEffect--;
    }

    //Check how long we're still in effect
    public int checkTurnsLeft()
    {
        return turnsInEffect;
    }

}
