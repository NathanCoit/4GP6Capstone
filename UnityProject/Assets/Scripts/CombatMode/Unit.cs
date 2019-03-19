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
    
    private List<int> depths = new List<int>();
    private HashSet<Tile> visited = new HashSet<Tile>();

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
        /*
        if (isPlayer)
            parentObject.GetComponent<MeshRenderer>().material = parentObject.GetComponent<UnitObjectScript>().playerNotAvailable;
        else
            parentObject.GetComponent<MeshRenderer>().material = parentObject.GetComponent<UnitObjectScript>().enemyNotAvailable;
        */

        activeStatusEffects = new List<StatusEffect>();

        AttackStrength = WorshiperCount * 0.25f * morale;

        //For AI
        MovePriority = 0;
    }

    public void assignGameObject(GameObject unitGameObject)
    {
        parentObject = unitGameObject;
    }

    public GameObject unitGameObject()
    {
        return parentObject;
    }

    /*
    // Update is called once per frame
    void Update()
    {
        //Updating morale every frame becuase it's very broken on occasion.
        if (isPlayer)
            morale = BoardMan.PlayerMorale;
        else
            morale = BoardMan.enemyMorale;

        updateAttackStrength();
    }
    */

    public virtual void Draw(Tile[,] tiles)
    {
        //Centers Unit on tile (I know it looks ugly but it SHOULD work for any model)
        parentObject.transform.position = new Vector3(tiles[(int)pos.x, (int)pos.y].getX() + ((1 - parentObject.transform.lossyScale.x) / 2) + parentObject.transform.lossyScale.x / 2, tiles[(int)pos.x, (int)pos.y].getY() + parentObject.transform.lossyScale.y + 0.5f, tiles[(int)pos.x, (int)pos.y].getZ() + ((1 - parentObject.transform.lossyScale.z) / 2) + parentObject.transform.lossyScale.x / 2);
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

        /*
        if (!isGod)
            parentObject.transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(false);
        else if (parentObject.GetComponent<Gods>().isInBattle())
            parentObject.transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(false);
        else if (!parentObject.GetComponent<Gods>().isInBattle())
            parentObject.transform.GetChild(1).GetComponent<Canvas>().gameObject.SetActive(false);
        */

    }

    //Button used by the end turn button (and many other things, like AI and gods)
    public void EndTurnButton()
    {
        //*****TOFIX******
        //Why the heck are we doing this here???
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
    }

    public void updateStatusEffects()
    {
        foreach(StatusEffect effect in activeStatusEffects)
        {
            effect.decreaseDuration();
            if (effect.checkTurnsLeft() == 0)
            {
                activeStatusEffects.Remove(effect);
            }

        }
        //appyStatusEffects();
    }

    public void dealDamage(int damage)
    {
        //If there's no shield, damage as usual
        if (!shieldBuff)
            setWorshiperCount(getWorshiperCount() - Mathf.Clamp((damage - getDefenseBuff()), 0, 10000000));
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

    //Done
    private int damageBuff;
    //Done
    private int defenseBuff;
    //Done
    private int speedBuff;
    //Done
    private bool shieldBuff;

    //done
    private bool stunDebuff;

    //done
    private bool paralyzeDebuff;

    //done
    private bool blindDebuff;

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

    public void appyStatusEffects()
    {
        damageBuff = 0;
        defenseBuff = 0;
        speedBuff = 0;
        shieldBuff = false;
        stunDebuff = false;
        paralyzeDebuff = false;

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
                    damageBuff -= (Ability.LoadAbilityFromName(effect.getAbility()) as DebuffAbility).DebuffAmount;
                    break;
                case "Stun":
                    stunDebuff = true;
                    break;
                case "Paralyze":
                    paralyzeDebuff = true;
                    break;
                case "Burn":
                    //Does fixed damage
                    WorshiperCount -= (Ability.LoadAbilityFromName(effect.getAbility()) as DebuffAbility).DebuffAmount;
                    break;
                case "Poison":
                    //Does proportional damage
                    WorshiperCount -= WorshiperCount * ((Ability.LoadAbilityFromName(effect.getAbility()) as DebuffAbility).DebuffAmount / 10);
                    break;
                case "Slow":
                    damageBuff -= (Ability.LoadAbilityFromName(effect.getAbility()) as DebuffAbility).DebuffAmount;
                    break;
                case "Blind":
                    blindDebuff = true;
                    break;
            }
        }
    }

    public void beginTurn()
    {
        appyStatusEffects();
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

    }

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
