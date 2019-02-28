using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attackable : MonoBehaviour
{
    /*
     * Associated with the attack tile (the red tile that appears when trying to attack with a worshipper).
     * When a worshipper's Attack action is picked, this will be called to show the possible attackable targets that worshipper has.
     * Also involved with the damage infliction unto target victim.
     * Is attached to attackable tiles (the red ones)
     */

    public Vector2 pos;
    private MapManager MapMan;
    private BoardManager BoardMan;
    private SetupManager SetupMan;

    private bool autoClick;

    void Start()
    {
        //*******EVERYBODYS'S HERE********//
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();

        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        SetupMan = GameObject.Find("SetupManager").GetComponent<SetupManager>();
    }


    void Update()
    {

    }

    private Tile[,] GetTiles()
    {
        return MapMan.tiles;
    }

    public void OnMouseOver()
    {
        //Wait till target has been chosen, inflict damage, check (and remove if necessary) if that unit is dead, recalculate that team's morale
        //end the attacker's turn, ans check to see if one side won the battle.
        if (Input.GetMouseButtonDown(0) || autoClick)
        {
            // Get position of clicked tile
            List<Unit> targets = new List<Unit>();
            if (BoardMan.playerUnits.Contains(MapMan.Selected))
            {
                targets = BoardMan.enemyUnits;
            }
            else
            {
                targets = BoardMan.playerUnits;
            }
            // Find unit that matches tiles position
            Tile[,] tiles = GetTiles();
            Unit attackedUnit = new Unit();

            foreach (Unit u in targets)
                if (u.getPos() == pos)
                    attackedUnit = u;

            //Decreases the "HP" of the attacked unit - decreases their worshipper count
            int damage = (int)MapMan.Selected.getAttackStrength();
            //Set remaining worshippers accordingly
            attackedUnit.setWorshiperCount(attackedUnit.getWorshiperCount() - damage);

            //Adjust that team's morale
            if (BoardMan.playerUnits.Contains(MapMan.Selected)) //check to see who initiated the attack
            {
                SetupMan.enemyMorale = ((BoardMan.GetRemainingWorshippers(false)) * 1.0f / (SetupMan.enemyWorshiperCount));
                //SetupMan.enemyWorshiperCount already takes into consideration the 0.8f (i.e. only 80% of that god's worshipper participates in war)
            }
            else
            {
                SetupMan.playerMorale = ((BoardMan.GetRemainingWorshippers(true)) * 1.0f / (SetupMan.playerWorshiperCount));
            }


            //Did the attacked unit's HP reach 0? If so, remove them from the board AND from the appropriate unit array
            if (attackedUnit.getWorshiperCount() <= 0)
            {
                if (BoardMan.playerUnits.Contains(attackedUnit))
                    BoardMan.playerUnits.Remove(attackedUnit);
                else
                    BoardMan.enemyUnits.Remove(attackedUnit);
                Destroy(attackedUnit.unitGameObject());
                if (BoardMan.playerUnits.Contains(attackedUnit))
                {
                    BoardMan.playerUnits.Remove(attackedUnit);
                }
                else
                {
                    BoardMan.enemyUnits.Remove(attackedUnit);
                }
            }

            //End the turn of the unit who initiated the attack
            MapMan.Selected.EndAct();
            BoardMan.DecreaseNumActions();

            //Checking if anyone won
            CheckEnd();

            //Hide Menu
            MapMan.Selected.unitGameObject().transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(false);

            //Unslecting
            MapMan.Selected = null;

            //Clean up Tiles
            MapMan.ClearSelection();

        }
    }

    private void CheckEnd()
    {
        //check if someone won the game (note we're checking if its 1 since we dont have killing gods in yet)
        if (BoardMan.playerUnits.Count == 1)
        {
            BoardMan.Defeat();
        }
        else if (BoardMan.enemyUnits.Count == 1)
        {
            BoardMan.Victory();
        }
    }

    //For spoofing clicks for testing
    public void TestClick()
    {
        autoClick = true;
    }
}
