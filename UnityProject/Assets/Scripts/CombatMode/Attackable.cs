using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attackable : MonoBehaviour
{
    public Vector2 pos;
    private MapManager MapMan;
    private BoardManager BoardMan;
    private SetupManager SetupMan;

    private bool autoClick;

    void Start()
    {
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();

        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        SetupMan = GameObject.Find("SetupManager").GetComponent<SetupManager>();
    }


    void Update()
    {

    }

    private Tile[,] getTiles()
    {
        return MapMan.tiles;
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) || autoClick)
        {
            List<GameObject> targets = new List<GameObject>();
            if (BoardMan.playerUnits.Contains(MapMan.Selected))
            {
                targets = BoardMan.enemyUnits;
            }
            else
            {
                targets = BoardMan.playerUnits;
            }
                Tile[,] tiles = getTiles();
            GameObject attackedUnit = new GameObject();

            foreach(GameObject g in targets)
                if (g.GetComponent<Units>().getPos() == pos)
                    attackedUnit = g;

            //MapMan.Selected.GetComponent<Units>().updateAttackStrength();

            //Decreases the "HP" of the attacked unit - decreases their worshipper count
            int damage = (int)MapMan.Selected.GetComponent<Units>().getAttackStrength();
            //Set remaining worshippers accordingly
            attackedUnit.GetComponent<Units>().setWorshiperCount(attackedUnit.GetComponent<Units>().getWorshiperCount() - damage);

            //Adjust that team's morale
            if (BoardMan.playerUnits.Contains(MapMan.Selected)) //check to see who initiated the attack
            {
                SetupMan.enemyMorale = ((BoardMan.GetRemainingWorshipers(false)) * 1.0f / (SetupMan.enemyWorshiperCount)); 
                //SetupMan.enemyWorshiperCount already takes into consideration the 0.8f (i.e. only 80% of that god's worshipper participates in war)
            }
            else
            {
                SetupMan.playerMorale = ((BoardMan.GetRemainingWorshipers(true)) * 1.0f / (SetupMan.playerWorshiperCount));
            }

            
            //Did the attacked unit's HP reach 0? If so, remove them from the board AND from the appropriate unit array
            if (attackedUnit.GetComponent<Units>().getWorshiperCount() <= 0)
            {
                if (BoardMan.playerUnits.Contains(attackedUnit))
                    BoardMan.playerUnits.Remove(attackedUnit);
                else
                    BoardMan.enemyUnits.Remove(attackedUnit);
                Destroy(attackedUnit);
            }

            //End the turn of the unit who initiated the attack
            MapMan.Selected.GetComponent<Units>().EndAct();
            BoardMan.DecreaseNumActions();

            //check if someone won the game
            if (BoardMan.playerUnits.Count == 0)
            {
                BoardMan.Defeat();
            }
            else if (BoardMan.enemyUnits.Count == 0)
            {
                BoardMan.Victory();
            }

            //Hide Menu
            MapMan.Selected.transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(false);

            //Unslecting
            MapMan.Selected = null;

            //Clean up Tiles
            MapMan.ClearSelection();

        }
    }

    //For spoofing clicks for testing
    public void testClick()
    {
        autoClick = true;
    }
}
