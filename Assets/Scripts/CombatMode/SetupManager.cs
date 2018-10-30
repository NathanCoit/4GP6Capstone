using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour {

    public GameObject GameInfoObjectPrefab;
    private GameInfo gameInfo;

    public GameObject BoardMan;
    public GameObject Mapman;
    public GameObject Unit;

    private Tile[,] tiles;
    private bool startup = true;
    
    private int playerWorshiperCount;
    private float playerMorale;
    private int enemyWorshiperCount;
    private float enemyMorale;
    public bool finishedBattle = false;

    private double worshiperPercentage = 0.80;


    //NOTE THIS CLASS IS WHERE WE WILL GET INFO FROM MANAGEMENT MODE

	void Start ()
    {
        GameObject GameInfoObject = GameObject.Find("GameInfo");
        if (GameInfoObject != null)
        {
            gameInfo = GameInfoObject.GetComponent<GameInfo>();
            //Found a game object, load values
            //Since only 80% of the worshippers go to war
            double a = gameInfo.PlayerWorshipperCount * worshiperPercentage;
            this.playerWorshiperCount = (int) a;
            
            //Since only 80% of the worshippers go to war
            double b = gameInfo.EnemyWorshipperCount * worshiperPercentage;
            //and then truncate it so you don't have a partial worshiper lol
            this.enemyWorshiperCount = (int) b;
        } else
        {
            GameObject NewGameInfoObject = (GameObject)Instantiate(GameInfoObjectPrefab);
            NewGameInfoObject.name = "GameInfo";
            gameInfo = NewGameInfoObject.GetComponent<GameInfo>();
            Start();
        }
    }
	
	void Update ()
    {
        //Battle has ended, send stats to GameInfo object
        if (finishedBattle)
        {
            //get remaining worshiper count for both sides
            int playWorLeft = BoardMan.GetComponent<BoardManager>().GetRemainingWorshipers(true);
            int eneWorLeft = BoardMan.GetComponent<BoardManager>().GetRemainingWorshipers(false);
            //load back into GameInfoObject

            //Take original amount of worshipers and add how many worshipers are left after the war
            this.gameInfo.PlayerWorshipperCount = playWorLeft + System.Convert.ToInt32(this.gameInfo.PlayerWorshipperCount*(1 - worshiperPercentage));
            this.gameInfo.EnemyWorshipperCount = eneWorLeft + System.Convert.ToInt32(this.gameInfo.EnemyWorshipperCount*(1 - worshiperPercentage));

            //Missing:
            // Change in Morale
            // Change in Player's abilities? (Do they gain them here?)
            // TO DO

            //The battle has been finished
            gameInfo.FinishedBattle = true;
            this.finishedBattle = false; //to avoid decrementing things FOREVER
        }

        //Note this can't be done in start as tiles hasn't been made yet.
        if (startup)
        {
            tiles = Mapman.GetComponent<MapManager>().tiles;
            //Test Setup
            CreatePlayerUnit(new Vector2(4, 3), tiles, playerWorshiperCount/3); //hello integer division
            CreatePlayerUnit(new Vector2(4, 4), tiles, playerWorshiperCount/3); //also assumes we have 3 units per team
            CreatePlayerUnit(new Vector2(4, 5), tiles, playerWorshiperCount/3);

            CreateEnemyUnit(new Vector2(5, 3), tiles, enemyWorshiperCount/3);
            CreateEnemyUnit(new Vector2(5, 4), tiles, enemyWorshiperCount/3);
            CreateEnemyUnit(new Vector2(5, 5), tiles, enemyWorshiperCount/3);
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
