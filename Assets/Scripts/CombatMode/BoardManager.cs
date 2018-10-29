using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public bool playerTurn = true;
    private GameObject MapMan;

    public List<GameObject> playerUnits; //List of player's units, element 0 is player's God Unit
    public int numActionsLeft;
    public List<GameObject> enemyUnits; //List of enemy's worshipper units, element 0 is enemy's God Unit

	// Use this for initialization
	void Start ()
    {
        playerUnits = new List<GameObject>();
        enemyUnits = new List<GameObject>();
        numActionsLeft = playerUnits.Count; //since player always starts first

        //It's ya boi, Map man.
        MapMan = GameObject.FindGameObjectWithTag("MapManager");


    }
	
	// Update is called once per frame
	void Update () {
        if (!HasActionsLeft()) //any actions left to take?
            SwitchTurns();

        //Don't think the if statement below is ever true, for some weird reason

        //Note new selected is only ever true on the frame when a new unit is selected. No trouble.
        /*if (MapMan.GetComponent<MapManager>().newSelected)
        {
            HasActionsLeft();
            Debug.Log("Selectrorama");
        }*/
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
}
