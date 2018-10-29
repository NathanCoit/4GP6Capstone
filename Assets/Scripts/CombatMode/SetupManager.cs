using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour {

    public GameObject BoardMan;
    public GameObject Mapman;
    public GameObject Unit;

    private Tile[,] tiles;
    private bool startup = true;

    //NOTE THIS CLASS IS WHERE WE WILL GET INFO FROM MANAGEMENT MODE

	void Start ()
    {
        
    }
	
	void Update ()
    {
        //Note this can't be done in start as tiles hasn't been made yet.
        if (startup)
        {
            tiles = Mapman.GetComponent<MapManager>().tiles;
            //Test Setup
            CreatePlayerUnit(new Vector2(4, 3), tiles, 100);
            CreatePlayerUnit(new Vector2(4, 4), tiles, 100);
            CreatePlayerUnit(new Vector2(4, 5), tiles, 100);

            CreateEnemyUnit(new Vector2(5, 3), tiles, 100);
            CreateEnemyUnit(new Vector2(5, 4), tiles, 100);
            CreateEnemyUnit(new Vector2(5, 5), tiles, 100);
            startup = false;
        }
    }


    public void CreatePlayerUnit(Vector2 pos, Tile[,] tiles, int WorshiperCount)
    {
        GameObject temp = Instantiate(Unit);
        temp.GetComponent<Units>().setWorshiperCount(WorshiperCount);
        temp.GetComponent<Units>().Move(new Vector2(pos.x, pos.y), tiles);
        BoardMan.GetComponent<BoardManager>().playerUnits.Add(temp);
    }

    public void CreateEnemyUnit(Vector2 pos, Tile[,] tiles, int WorshiperCount)
    {
        GameObject temp = Instantiate(Unit);
        temp.GetComponent<Units>().setWorshiperCount(WorshiperCount);
        temp.GetComponent<Units>().Move(new Vector2(pos.x, pos.y), tiles);
        //Set so the players turn is first
        temp.GetComponent<Units>().EndAct();
        BoardMan.GetComponent<BoardManager>().enemyUnits.Add(temp);
    }

}
