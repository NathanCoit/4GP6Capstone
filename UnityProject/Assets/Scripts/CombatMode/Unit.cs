using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    protected GameObject parentObject;

    public bool canAct;

    private bool isGod;

    protected MapManager MapMan;
    private BoardManager BoardMan;
    protected UIManager UIMan;

    protected Vector2 pos;

    public int Movement;
    public int MaxMovement;
    public int attackRange;
    public float morale;
    public bool isPlayer;
    public int MovePriority;

    private List<StatusEffect> activeStatusEffects;

    //For use without models, can be removed later
    public int WorshiperCount;
    public float AttackStrength;


    public bool autoClick = false;

    public Unit()
    {
        //You know who to call, ITS MAP MAN!
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();

        //Boardman is here also
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        UIMan = GameObject.Find("UIManager").GetComponent<UIManager>();

        //List for active status effect on us
        activeStatusEffects = new List<StatusEffect>();

        AttackStrength = WorshiperCount * 0.25f * morale;

        //For AI
        MovePriority = 0;
    }

    //Set to assosiated game object
    public void assignGameObject(GameObject unitGameObject)
    {
        parentObject = unitGameObject;
    }

    //Get the assosiated game object
    public GameObject unitGameObject()
    {
        return parentObject;
    }

    //Actually draw the unit where it should be
    public virtual void Draw(Tile[,] tiles)
    {
        //Centers Unit on tile (I know it looks ugly but it SHOULD work for any model)
        parentObject.transform.position = new Vector3(tiles[(int)pos.x, (int)pos.y].getX() + ((1 - parentObject.transform.lossyScale.x) / 2) + parentObject.transform.lossyScale.x / 2, tiles[(int)pos.x, (int)pos.y].getY() + parentObject.transform.lossyScale.y + 0.5f, tiles[(int)pos.x, (int)pos.y].getZ() + ((1 - parentObject.transform.lossyScale.z) / 2) + parentObject.transform.lossyScale.x / 2);
    }

    //Change the direction
    public void turnToFace(int direction)
    {
        switch(direction)
        {
            case 0:
                unitGameObject().transform.eulerAngles = new Vector3(0, 0, 0);
                unitGameObject().transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case 1:
                unitGameObject().transform.eulerAngles = new Vector3(0, 90, 0);
                unitGameObject().transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case 2:
                unitGameObject().transform.eulerAngles = new Vector3(0, 180, 0);
                unitGameObject().transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case 3:
                unitGameObject().transform.eulerAngles = new Vector3(0, 270, 0);
                unitGameObject().transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
                break;
        }
    }

    //Set our direction based on which direction has the most enemy units in it
    public void updateDirection()
    {
        Vector2 average = new Vector2();
        if(isPlayer)
        {
            foreach(Unit u in BoardMan.enemyUnits)
            {
                average.x += u.getPos().x - getPos().x;
                average.y += u.getPos().y - getPos().y;
            }
            average.x /= BoardMan.enemyUnits.Count;
            average.y /= BoardMan.enemyUnits.Count;
        }
        else
        {
            foreach (Unit u in BoardMan.playerUnits)
            {
                average.x += u.getPos().x - getPos().x;
                average.y += u.getPos().y - getPos().y;
            }
            average.x /= BoardMan.playerUnits.Count;
            average.y /= BoardMan.playerUnits.Count;
        }

        if(Mathf.Abs(average.y) > Mathf.Abs(average.x))
        {
            if (average.y > 0)
                turnToFace(2);
            else
                turnToFace(0);
        }
        else
        {
            if (average.x > 0)
                turnToFace(3);
            else
                turnToFace(1);
        }

    }

    //This is what we actually use to move (who knows what the above is for) (god i wish past me knew how dumb he sounds sometimes)
    public void MoveTo(Vector2 pos, Tile[,] tiles)
    {
        this.pos = pos;

        Draw(tiles);
    }

    //For moving unit by a certain amount, rather than to a specfic tile. Could be used to knockback, for example.
    public void Move(Vector2 amount, Tile[,] tiles)
    {
        //Check array bounds on tiles
        if (pos.x + amount.x < tiles.GetLength(0) && pos.y + amount.y < tiles.GetLength(1))
            if (pos.x + amount.x >= 0 && pos.y + amount.y >= 0)
                this.pos += amount;

        Draw(tiles);
    }

    //Resets units on new turn
    public void AllowAct() //this Unit has not yet acted in this round
    {
        canAct = true;

        if (!paralyzeDebuff)
            Movement = MaxMovement;
        else
            Movement = 0;

        //Render stuff for use without proper models, can be removed later
        if (isPlayer)
            parentObject.GetComponent<MeshRenderer>().material = parentObject.GetComponent<UnitObjectScript>().playerAvailable;
        else
            parentObject.GetComponent<MeshRenderer>().material = parentObject.GetComponent<UnitObjectScript>().enemyAvailable;
    }

    //When a units end their turn
    public void EndAct() //this Unit has completed their allotted actions in this round
    {
        canAct = false;

        //Render stuff for use without proper models, can be removed later
        if (isPlayer)
            parentObject.GetComponent<MeshRenderer>().material = parentObject.GetComponent<UnitObjectScript>().playerNotAvailable;
        else
            parentObject.GetComponent<MeshRenderer>().material = parentObject.GetComponent<UnitObjectScript>().enemyNotAvailable;
    }

    //Button used by the end turn button (and many other things, like AI and gods)
    public void EndTurnButton()
    {
        //check if somebody won
        if (BoardMan.playerUnits.Count == 0)
        {
            BoardMan.Defeat();
        }
        else if (BoardMan.enemyUnits.Count == 0)
        {
            BoardMan.Victory();
        }

        MapMan.ClearSelection();
        UIMan.removeMenu();
        EndAct();
        BoardMan.DecreaseNumActions();
        BoardMan.checkIfSwitchTurn();
    }

    //Add a new buff or debuff
    public void addNewStatusEffect(Ability a, bool isBuff)
    {
        if(isBuff)
        {
            activeStatusEffects.Add(new StatusEffect(a as BuffAbility, 2));
        }
        else
        {
            activeStatusEffects.Add(new StatusEffect(a as DebuffAbility, 2));
        }

        Debug.Log(a.AbilityName);

        UIMan.updateStatusEffectDisplay(this);
    }

    //Upadte the durations of status effects and remove finished ones
    public void updateStatusEffects()
    {
        List<StatusEffect> effectsToBeRemoved = new List<StatusEffect>();

        foreach(StatusEffect effect in activeStatusEffects)
        {
            if (effect.checkTurnsLeft() == 0)
            {
                effectsToBeRemoved.Add(effect);
            }
            effect.decreaseDuration();

        }

        foreach(StatusEffect effect in effectsToBeRemoved)
        {
            activeStatusEffects.Remove(effect);
        }

        UIMan.updateStatusEffectDisplay(this);
    }

    //Gett all active effects
    public List<StatusEffect> getActiveEffects()
    {
        return activeStatusEffects;
    }

    //Function for dealing damage to unit
    public void dealDamage(int damage)
    {
        //If there's no shield, damage as usual
        if (!shieldBuff)
        {
            setWorshiperCount(getWorshiperCount() - Mathf.Clamp(damage - getDefenseBuff(), 0, 10000000));
            if (WorshiperCount <= 0)
                BoardMan.killUnit(this);
        }
        //Otherwise, take no damage and remove one shield buff
        else
        {
            foreach (StatusEffect effect in activeStatusEffects)
            {
                if ((Ability.LoadAbilityFromName(effect.getAbility()) as BuffAbility).BuffType == Ability.BUFFTYPE.Shield)
                {
                    activeStatusEffects.Remove(effect);
                    break;
                }
            }
        }
    }

    //Variables for buffs and debuffs
    private int damageBuff;
    private int defenseBuff;
    private int speedBuff;
    private bool shieldBuff;

    private bool stunDebuff;
    private bool paralyzeDebuff;
    private bool blindDebuff;

    //Getters for buffs and debuffs
    public int getSpeedBuff()
    {
        return speedBuff;
    }

    public int getDefenseBuff()
    {
        return defenseBuff;
    }

    public bool getBlindDebuff()
    {
        return blindDebuff;
    }

    //Resolve the effects of effects
    public void appyStatusEffects()
    {
        //Reset everything
        damageBuff = 0;
        defenseBuff = 0;
        speedBuff = 0;
        shieldBuff = false;
        stunDebuff = false;
        paralyzeDebuff = false;

        //Grab gameInfo for scaling
        GameObject GameInfoObject = GameObject.Find("GameInfo");
        GameInfo gameInfo = GameInfoObject.GetComponent<GameInfo>();

        //Calcualte what everything does
        foreach (StatusEffect effect in activeStatusEffects)
        {
            switch(effect.getType())
            {
                //Buffs
                case "Healing":
                    WorshiperCount += (int)(Ability.LoadAbilityFromName(effect.getAbility()) as BuffAbility).BuffAmount;
                    break;
                case "Damage":
                    damageBuff += (int)(Ability.LoadAbilityFromName(effect.getAbility()) as BuffAbility).BuffAmount;
                    break;
                case "Defense":
                    defenseBuff += (int)(Ability.LoadAbilityFromName(effect.getAbility()) as BuffAbility).BuffAmount;
                    break;
                case "Shield":
                    shieldBuff = true;
                    break;
                case "Speed":
                    speedBuff += (int)(Ability.LoadAbilityFromName(effect.getAbility()) as BuffAbility).BuffAmount;
                    break;

                //Debuffs
                case "DamageReduction":
                    damageBuff -= (Ability.LoadAbilityFromName(effect.getAbility()) as DebuffAbility).DebuffAmount;
                    break;
                case "DefenseReduction":
                    defenseBuff -= (Ability.LoadAbilityFromName(effect.getAbility()) as DebuffAbility).DebuffAmount;
                    break;
                case "Stun":
                    //Stun is borked, so its just a move debuff now
                    speedBuff -= (Ability.LoadAbilityFromName(effect.getAbility()) as DebuffAbility).DebuffAmount;
                    break;
                case "Paralyze":
                    paralyzeDebuff = true;
                    break;
                case "Burn":
                    //Does fixed damage
                    dealDamage((Ability.LoadAbilityFromName(effect.getAbility()) as DebuffAbility).DebuffAmount);
                    break;
                case "Poison":
                    //Does proportional damage (of remaining health)
                    dealDamage(WorshiperCount * (Ability.LoadAbilityFromName(effect.getAbility()) as DebuffAbility).DebuffAmount/ 100);
                    break;
                case "Slow":
                    speedBuff -= (Ability.LoadAbilityFromName(effect.getAbility()) as DebuffAbility).DebuffAmount;
                    break;
                case "Blind":
                    blindDebuff = true;
                    break;
            }
        }
    }

    //Called at the beginning of turn. We don't geto our turn if we're stunned
    public void beginTurn()
    {
        //Currently should never be set to true
        if (!stunDebuff)
        {
            AllowAct();
        }
        else
        {
            BoardMan.DecreaseNumActions();
        }
        
    }

    //For spoofing clicks for testing
    public void TestClick()
    {
        autoClick = true;
    }

    //Many getters
    public Vector2 getPos()
    {
        return pos;
    }

    public bool HasAct()
    {
        return canAct;
    }

    private Tile[,] getTiles()
    {
        return MapMan.tiles;
    }

    public bool CheckIfGod()
    {
        return isGod;
    }

    public int getWorshiperCount()
    {
        return WorshiperCount;
    }

    public void setWorshiperCount(int count)
    {
        WorshiperCount = count;
    }

    public float getMorale()
    {
        return morale;
    }
    public void setMorale(float mor)
    {
        morale = mor;
    }

    public float getAttackStrength()
    {
        return AttackStrength + damageBuff;
    }

    //For using skill, will be done later (if units ever have skills)
    public void useSkill(int number)
    {
        //Still unused, but we'll leave it here (for fun)
    }

    //If we're a god
    public void setGod()
    {
        isGod = true;
    }

    //Attack strength = 1/4 of worshipper count * morale
    public virtual void updateAttackStrength()
    {
        AttackStrength = WorshiperCount * 0.25f * morale;
    }
}
