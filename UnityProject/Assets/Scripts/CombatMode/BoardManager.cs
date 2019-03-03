﻿using System.Collections;
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
    private MapManager MapMan;

    public List<Unit> playerUnits; //List of player's units
    public List<Unit> enemyUnits; //List of enemy's worshipper units
    private int numActionsLeft;

    public bool endBattle = false; //used for testing purposes - to see if the battle has ended even if there are units left
    public float PlayerMorale;
    public float enemyMorale;
    public float playerFaith;
    public float enemyFaith;

    public float faithCap;

    public GameObject MovableTile;
    public GameObject AttackableTile;

    void Start()
    {
        playerUnits = new List<Unit>();
        enemyUnits = new List<Unit>();
        numActionsLeft = playerUnits.Count; //since player always starts first
        playerTurn = true;
        
        SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();

        //The baddest of them all, its EnemyMan
        EnemyMan = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
    }
    
    void Update()
    {
        //Updates Morale frequently. Involved with attack strength calculation so we need to update it frequently. Maybe. We're not really sure. But this works!
        // TODO fix
        PlayerMorale = SetupMan.playerMorale;
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

        int worshipersLeft = GetRemainingWorshippers(true);
        SetupMan.playerWorshiperCount = worshipersLeft; //incorrect value atm

    }

    public int GetRemainingWorshippers(bool player)
    {
        int worshippers = 0;
        if (player)
        {
            foreach (Unit u in playerUnits)
            {
                worshippers += u.getWorshiperCount();
            }
        }
        else //need to calculate enemy worshiper count if player decided to kill enemy god first/early, however not implemented yet
        {
            foreach (Unit u in enemyUnits)
            {
                worshippers += u.getWorshiperCount();
            }
        }
        return worshippers;
    }

    //Get all the tiles a unit can move to, based on their remaining movement
    public void showMovable(Unit currentUnit)
    {
        Tile[,] tiles = MapMan.tiles;

        HashSet<Tile> MovableTiles = new HashSet<Tile>();

        //Setup Invalid Tiles (the one with units on)
        List<Unit> invalidTiles = new List<Unit>();
        invalidTiles.AddRange(playerUnits);
        invalidTiles.Remove(currentUnit);

        //Calculate Movable Tiles
        MovableTiles = tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y].findAtDistance(tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y], currentUnit.Movement, invalidTiles, enemyUnits, tiles);

        //We need to do this because the above function breaks some connection (like the one that can't be moved through)
        MapMan.DefineConnections();

        //Clean up all the other tiles
        MapMan.ClearSelection();

        //Draw movable tiles
        foreach (Tile t in MovableTiles)
        {
            GameObject temp = Instantiate(MovableTile);
            temp.GetComponent<Movable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
            temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
            //temp.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
            //Movable.Add(temp);
        }
    }

    //Shows attackable tiles (for attacking)
    public void showAttackable(Unit currentUnit)
    {
        Tile[,] tiles = MapMan.tiles;
        HashSet<Tile> AttackableTiles = new HashSet<Tile>();
        List<Tile> ConnectedTiles = tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y].getConnected();
        List<Unit> targets = new List<Unit>();

        if (playerUnits.Contains(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()))
        {
            targets = enemyUnits;
        }
        else
        {
            targets = playerUnits;
        }

        //Take the tiles connect to this unit's tile and see if theres an enemy unit on it
        foreach (Tile t in ConnectedTiles)
            foreach (Unit u in targets)
                if (new Vector2(t.getX(), t.getZ()) == u.getPos())
                    AttackableTiles.Add(t);


        //Clean up all the other tiles
        MapMan.ClearSelection();

        //Draw movable tiles
        foreach (Tile t in AttackableTiles)
        {
            GameObject temp = Instantiate(AttackableTile);
            temp.GetComponent<Attackable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
            temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
            //temp.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
            //Movable.Add(temp);
        }

    }

    public float GetPlayerMorale()
    {
        return PlayerMorale;
    }

    public float GetEnemyMorale()
    {
        return enemyMorale;
    }

    public float GetPlayerFaith()
    {
        return playerFaith;
    }

    public float GetEnemyFaith()
    {
        return enemyFaith;
    }

    public float GetFaithCap()
    {
        return faithCap;
    }

    public void setPlayerMorale(float m)
    {
        PlayerMorale = m;
    }

    public void setEnemyMorale(float m)
    {
        enemyMorale = m;
    }

    void SwitchTurns()
    {
        if (playerTurn)
        { //it was player's turn
            foreach (Unit u in enemyUnits) //allow each of enemy units to act
                u.AllowAct();
            numActionsLeft = enemyUnits.Count;
            StartCoroutine(EnemyMan.EnemyActions(0.5f));
        }
        else
        { //it was the enemy's turn
            foreach (Unit u in playerUnits) //allow each of player's units to act
                u.AllowAct();
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