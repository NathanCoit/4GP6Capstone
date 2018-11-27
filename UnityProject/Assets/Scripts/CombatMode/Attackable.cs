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
            attackedUnit.GetComponent<Units>().setWorshiperCount(attackedUnit.GetComponent<Units>().getWorshiperCount() - damage);

            //Adjust that unit's morale
            if (BoardMan.playerUnits.Contains(MapMan.Selected))
            {
                //change .8f to something else later (at the end of this line)
                SetupMan.enemyMorale = ((BoardMan.GetRemainingWorshipers(false) - damage) / (SetupMan.enemyWorshiperCount * .8f));
                Debug.Log("Enemy Morale" + SetupMan.enemyMorale);
            }
            else
            {
                //change .8f to something else later (at the end of this line)
                // to do: fix this
                SetupMan.playerMorale = ((BoardMan.GetRemainingWorshipers(true) - damage) / (SetupMan.playerWorshiperCount * .8f));
                Debug.Log("Player Morale" + SetupMan.playerMorale);
            }

            

            if (attackedUnit.GetComponent<Units>().getWorshiperCount() <= 0)
            {
                BoardMan.enemyUnits.Remove(attackedUnit);
                Destroy(attackedUnit);
            }

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
