using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    bool playerTurn = true;

    //CombatGrid combatGrid;

    Units[] playerUnits; //array of player's units, element 0 is player's God Unit
    int playerActionsLeft;
    //Units[] playerUnits = combatGrid.getPlayerUnits();
    Units[] enemyUnits; //array of enemy's worshipper units, element 0 is enemy's God Unit
    int enemyActionsLeft;
    //Units[] enemyUnits = combatGrid.getEnemyUnits();

    void SwitchTurns() {
        if (playerTurn) { //it was player's turn
            foreach (Units i in enemyUnits) //allow each of enemy units to act
                i.AllowAct();
        } else { //it was the enemy's turn
            foreach (Units i in playerUnits) //allow each of player's units to act
                i.AllowAct();
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
    
	// Use this for initialization
	void Start () {

        playerActionsLeft = playerUnits.Length;
        enemyActionsLeft = enemyUnits.Length;


    }
	
	// Update is called once per frame
	void Update () {
		//CheckHasActionsLeft(); //this may be called too often once per frame? will it cause lag?
	}
}
