using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    private int turnsInEffect;
    private Ability.BUFFTYPE buffType;
    private Ability.DEBUFFTYPE debuffType;
    private bool isBuff;
    private BuffAbility sourceBuffAbility;
    private DebuffAbility sourceDebuffAbility;


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

    public string getType()
    {
        if (isBuff)
            return buffType.ToString();
        else
            return debuffType.ToString();
    }

    public string getAbility()
    {
        if (isBuff)
            return sourceBuffAbility.AbilityName;
        else
            return sourceDebuffAbility.AbilityName;
    }

    public void decreaseDuration()
    {
        turnsInEffect = 0;
    }

    public int checkTurnsLeft()
    {
        return turnsInEffect;
    }

}
