using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour {

    GameObject GameInfoObject;

    public GameObject BoardMan;
    public GameObject Mapman;
    public GameObject Unit;

    private Tile[,] tiles;
    private bool startup = true;
    
    private int playerWorshiperCount;
    private float playerMorale;
    private int enemyWorshiperCount;
    private float enemyMorale;
    private bool finishedBattle = false;

    private double worshiperPercentage = 0.80;


    //NOTE THIS CLASS IS WHERE WE WILL GET INFO FROM MANAGEMENT MODE

	void Start ()
    {
        GameInfoObject = GameObject.Find("GameInfo");
        if (GameInfoObject != null)
        {
            //Found a game object, load values
            //Since only 80% of the worshippers go to war
            double a = GameInfoObject.GetComponent<GameInfo>().PlayerWorshipperCount * worshiperPercentage;
            this.playerWorshiperCount = (int) a;
            
            //Since only 80% of the worshippers go to war
            double b = GameInfoObject.GetComponent<GameInfo>().EnemyWorshipperCount * worshiperPercentage;
            //and then truncate it so you don't have a partial worshiper lol
            this.enemyWorshiperCount = (int) b;
        }
    }
	
	void Update ()
    {
        //Battle has ended, send stats to GameInfo object
        if (finishedBattle)
        {
            //get remaining worshiper count for both sides
            //int playWorLeft = BoardMan.GetComponent<BoardManager>().GetRemainingWorshipers(true); //need this to be public
            //int enemWorLeft = BoardMan.GetComponent<BoardManager>().GetRemainingWorshipers(false);
            //load back into GameInfoObject

            //GameInfoObject.GetComponent<GameInfo>().PlayerWorshiperCount -= playWorLeft;
            //GameInfoObject.GetComponent<GameInfo>().EnemyWorshiperCount -= enemyWorLeft*1.0;
            GameInfoObject.GetComponent<GameInfo>().FinishedBattle = true;
        }

        //Note this can't be done in start as tiles hasn't been made yet.
        if (startup)
        {
            tiles = Mapman.GetComponent<MapManager>().tiles;
            //Test Setup
            CreatePlayerUnit(new Vector2(0, 3), tiles, playerWorshiperCount/3); //hello integer division
            CreatePlayerUnit(new Vector2(0, 4), tiles, playerWorshiperCount/3); //also assumes we have 3 units per team
            CreatePlayerUnit(new Vector2(0, 5), tiles, playerWorshiperCount/3);

            CreateEnemyUnit(new Vector2(9, 3), tiles, enemyWorshiperCount/3);
            CreateEnemyUnit(new Vector2(9, 4), tiles, enemyWorshiperCount/3);
            CreateEnemyUnit(new Vector2(9, 5), tiles, enemyWorshiperCount/3);
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
        temp.GetComponent<Units>().EndAct();
        BoardMan.GetComponent<BoardManager>().enemyUnits.Add(temp);
    }

    public void EndBattle()
    {
        finishedBattle = true;
    }

}
