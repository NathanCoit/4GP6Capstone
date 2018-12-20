using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * The class responsible for keeping track of:
 *      - each team (i.e. the list of worshippers and God)
 *      - their respective morale values
 *      - each God's faith values (basically mana for Godly abilities)
 *      - whose turn it is (player or enemy God)
 *      - the Victory, Defeat and Retreat functions
 *      
 *      BASICALLY: This class is concerned with the STATE of the combat mode
 */
public class BoardManager : MonoBehaviour
{

    public bool playerTurn;

    private SetupManager SetupMan;
    private EnemyManager EnemyMan;

    public List<GameObject> playerUnits; //List of player's units
    public List<GameObject> enemyUnits; //List of enemy's worshipper units
    int numActionsLeft;

    public bool endBattle = false; //used for testing purposes - to see if the battle has ended even if there are units left
    public float playerMorale;
    public float enemyMorale;
    public float playerFaith;
    public float enemyFaith;

    public float faithCap;

    void Start()
    {
        playerUnits = new List<GameObject>();
        enemyUnits = new List<GameObject>();
        numActionsLeft = playerUnits.Count; //since player always starts first
        playerTurn = true;
        
        SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();

        //The baddest of them all, its EnemyMan
        EnemyMan = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
    }
    
    void Update()
    {
        //Updates Morale frequently. Involved with attack strength calculation so we need to update it frequently. Maybe. We're not really sure. But this works!
        playerMorale = SetupMan.playerMorale;
        enemyMorale = SetupMan.enemyMorale;

        if (!HasActionsLeft()) //any actions left to take? if not, switch turns
            SwitchTurns();
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

        int worshipersLeft = GetRemainingWorshipers(true);
        SetupMan.playerWorshiperCount = worshipersLeft; //incorrect value atm

    }

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
        else //need to calculate enemy worshiper count if player decided to kill enemy god first/early, however not implemented yet
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
            StartCoroutine(EnemyMan.EnemyActions(0.5f));
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


    //Used for testing purposes
    public bool surrender()
    {
        //not implemented yet so it's returning a bool
        Retreat();
        return false;
    }
}