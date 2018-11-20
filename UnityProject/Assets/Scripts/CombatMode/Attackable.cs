using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attackable : MonoBehaviour
{
    public Vector2 pos;
    private GameObject MapMan;
    private GameObject BoardMan;
    private SetupManager SetupMan;

    private bool autoClick;

    void Start()
    {
        MapMan = GameObject.FindGameObjectWithTag("MapManager");

        BoardMan = GameObject.FindGameObjectWithTag("BoardManager");

        SetupMan = GameObject.Find("SetupManager").GetComponent<SetupManager>();
    }


    void Update()
    {

    }

    private Tile[,] getTiles()
    {
        return MapMan.GetComponent<MapManager>().tiles;
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) || autoClick)
        {
            List<GameObject> targets = new List<GameObject>();
            if (BoardMan.GetComponent<BoardManager>().playerUnits.Contains(MapMan.GetComponent<MapManager>().Selected))
            {
                targets = BoardMan.GetComponent<BoardManager>().enemyUnits;
            }
            else
            {
                targets = BoardMan.GetComponent<BoardManager>().playerUnits;
            }
                Tile[,] tiles = getTiles();
            GameObject attackedUnit = new GameObject();

            foreach(GameObject g in targets)
                if (g.GetComponent<Units>().getPos() == pos)
                    attackedUnit = g;

            MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().updateAttackStrength();

            attackedUnit.GetComponent<Units>().setWorshiperCount(attackedUnit.GetComponent<Units>().getWorshiperCount() - (int)MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().getAttackStrength());

            if (BoardMan.GetComponent<BoardManager>().playerUnits.Contains(MapMan.GetComponent<MapManager>().Selected))
            {
                //change .8f to something else later (at the end of this line)
                BoardMan.GetComponent<BoardManager>().setEnemyMorale((BoardMan.GetComponent<BoardManager>().GetRemainingWorshipers(false) - MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().getAttackStrength()) / (SetupMan.enemyWorshiperCount * .8f));
                Debug.Log("Morale" + BoardMan.GetComponent<BoardManager>().enemyMorale);
            }
            else
            {
                //change .8f to something else later (at the end of this line)
                // to do: fix this
                BoardMan.GetComponent<BoardManager>().setPlayerMorale((BoardMan.GetComponent<BoardManager>().GetRemainingWorshipers(true) - MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().getAttackStrength()) / (SetupMan.playerWorshiperCount * .8f));
                Debug.Log("Morale" + BoardMan.GetComponent<BoardManager>().playerMorale);
            }

            

            if (attackedUnit.GetComponent<Units>().getWorshiperCount() <= 0)
            {
                BoardMan.GetComponent<BoardManager>().enemyUnits.Remove(attackedUnit);
                Destroy(attackedUnit);
            }

            MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().EndAct();
            BoardMan.GetComponent<BoardManager>().DecreaseNumActions();

            //check if someone won the game
            if (BoardMan.GetComponent<BoardManager>().playerUnits.Count == 0)
            {
                BoardMan.GetComponent<BoardManager>().Defeat();
            }
            else if (BoardMan.GetComponent<BoardManager>().enemyUnits.Count == 0)
            {
                BoardMan.GetComponent<BoardManager>().Victory();
            }

            //Hide Menu
            MapMan.GetComponent<MapManager>().Selected.transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(false);

            //Unslecting
            MapMan.GetComponent<MapManager>().Selected = null;

            //Clean up Tiles
            MapMan.GetComponent<MapManager>().ClearSelection();

        }
    }

    //For spoofing clicks for testing
    public void testClick()
    {
        autoClick = true;
    }
}
