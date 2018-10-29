using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    bool playerTurn = true;
    private GameObject MapMan;

    //CombatGrid combatGrid;

    public List<GameObject> playerUnits; //List of player's units, element 0 is player's God Unit
    int playerActionsLeft;
    //Units[] playerUnits = combatGrid.getPlayerUnits();
    public List<GameObject> enemyUnits; //array of enemy's worshipper units, element 0 is enemy's God Unit
    int enemyActionsLeft;
    //Units[] enemyUnits = combatGrid.getEnemyUnits();

	// Use this for initialization
	void Start ()
    {
        playerUnits = new List<GameObject>();
        enemyUnits = new List<GameObject>();
        playerActionsLeft = playerUnits.Count;
        enemyActionsLeft = enemyUnits.Count;

        //It's ya boi, Map man.
        MapMan = GameObject.FindGameObjectWithTag("MapManager");


    }
	
	// Update is called once per frame
	void Update () {
        //Note new selected is only ever true on the frame when a new unit is selected. No trouble.
        if (MapMan.GetComponent<MapManager>().newSelected)
        {
            CheckHasActionsLeft();
            Debug.Log("Selectrorama");
        }
	}

    void SwitchTurns()
    {
        if (playerTurn)
        { //it was player's turn
            foreach (GameObject i in enemyUnits) //allow each of enemy units to act
                i.GetComponent<Units>().AllowAct();
        }
        else
        { //it was the enemy's turn
            foreach (GameObject i in playerUnits) //allow each of player's units to act
                i.GetComponent<Units>().AllowAct();
        }
        playerTurn = !playerTurn; //switch turn
    }

    //presumably, the combatGrid (or whatever you call it) will call Units.EndAct() whenever that unit does their action

    bool CheckHasActionsLeft()
    {
        if (playerTurn && playerActionsLeft > 0) return true;
        else if (!playerTurn && enemyActionsLeft > 0) return true;

        return false;
    }
}
