using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public bool playerTurn = true;
    //private GameObject MapMan;

    private SetupManager SetupMan;

    public List<GameObject> playerUnits; //List of player's units, element 0 is player's God Unit
    public List<GameObject> enemyUnits; //List of enemy's worshipper units, element 0 is enemy's God Unit
    int numActionsLeft;

    public bool endBattle = false;
    public float playerMorale;
    public float enemyMorale;
    public float playerFaith;
    public float enemyFaith;

    public float faithCap;


    // Use this for initialization
    void Start()
    {
        playerUnits = new List<GameObject>();
        enemyUnits = new List<GameObject>();
        numActionsLeft = playerUnits.Count; //since player always starts first

        //It's ya boi, Map man.
        //MapMan = GameObject.FindGameObjectWithTag("MapManager");
        SetupMan = GameObject.Find("SetupManager").GetComponent<SetupManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Updates Morale
        playerMorale = SetupMan.playerMorale;
        enemyMorale = SetupMan.enemyMorale;

        if (!HasActionsLeft()) //any actions left to take?
            SwitchTurns();

        if (endBattle) //need to have units check if there are any enemies left after each attack
        { //right now, endBattle will only be true if you click it in the inspector
            SetupMan.finishedBattle = true;
            endBattle = false; //don't want this to loop infinitely

            //wow much win such proud of u
            //insert wonderful text box appearing saying you win
            //insert delightful victory music
        }
        else if (playerUnits.Count == 0)
        {
            //much disappoint such loss
            //pepehands

            //loss text box
            //loss music
        }

        //Don't think the if statement below is ever true, for some weird reason

        //Note new selected is only ever true on the frame when a new unit is selected. No trouble.
        /*if (MapMan.GetComponent<MapManager>().newSelected)
        {
            HasActionsLeft();
            Debug.Log("Selectrorama");
        }*/
    }

    public void Victory()
    {
        SetupMan.battleResult = GameInfo.BATTLESTATUS.Victory;
        SetupMan.finishedBattle = true;
    }

    public void Defeat()
    {
        SetupMan.battleResult = GameInfo.BATTLESTATUS.Defeat;
        SetupMan.finishedBattle = true;
    }

    public void Retreat()
    {
        SetupMan.battleResult = GameInfo.BATTLESTATUS.Retreat;
        SetupMan.finishedBattle = true;

        //morale is broken atm

        int worshipersLeft = GetRemainingWorshipers(true);
        SetupMan.playerWorshiperCount = worshipersLeft;

    }

    //Post fight stuff

    //to be sent back to setup manager?
    public int GetRemainingWorshipers(bool player)
    {
        int worshipers = 0;
        if (player)
        {
            foreach (GameObject i in playerUnits)
            {
                worshipers += i.GetComponent<Units>().getWorshiperCount();
            }
        }
        else //need to calculate enemy worshiper count if player decided to kill enemy god first/early
        {
            foreach (GameObject i in enemyUnits)
            {
                worshipers += i.GetComponent<Units>().getWorshiperCount();
            }
        }
        return worshipers;
    }

    public float getPlayerMorale()
    {
        return playerMorale;
    }

    public float getEnemyMorale()
    {
        return enemyMorale;
    }

    public float getPlayerFaith()
    {
        return playerFaith;
    }

    public float getEnemyFaith()
    {
        return enemyFaith;
    }

    public float getFaithCap()
    {
        return faithCap;
    }

    public void setPlayerMorale(float m)
    {
        playerMorale = m;
    }

    public void setEnemyMorale(float m)
    {
        enemyMorale = m;
    }

    void SwitchTurns()
    {
        if (playerTurn)
        { //it was player's turn
            foreach (GameObject i in enemyUnits) //allow each of enemy units to act
                i.GetComponent<Units>().AllowAct();
            numActionsLeft = enemyUnits.Count;
        }
        else
        { //it was the enemy's turn
            foreach (GameObject i in playerUnits) //allow each of player's units to act
                i.GetComponent<Units>().AllowAct();
            numActionsLeft = playerUnits.Count;
        }
        playerTurn = !playerTurn; //switch turn

    }

    bool HasActionsLeft()
    {
        if (numActionsLeft > 0) return true;
        else return false;
    }

    public void DecreaseNumActions()
    {
        numActionsLeft--;
    }

    public bool isPlayerTurn()
    {
        return playerTurn;
    }

    public bool surrender()
    {
        //not implemented yet so it's returning a bool
        Retreat();
        return false;
    }
}