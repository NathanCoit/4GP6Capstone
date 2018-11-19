using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetupManager : MonoBehaviour {

    public GameObject GameInfoObjectPrefab;
    private GameInfo gameInfo;

    public GameObject BoardMan;
    public GameObject Mapman;
    public GameObject Unit;

    private Tile[,] tiles;
    public bool startup = true;
    
    public int playerWorshiperCount;
    public float playerMorale;
    public int enemyWorshiperCount;
    public float enemyMorale;
    public bool finishedBattle = false;
    public int battleResult; //0 for victory, 1 for defeat, 2 for retreat

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
            double a = gameInfo.PlayerFaction.WorshipperCount * worshiperPercentage;
            this.playerWorshiperCount = (int) a;
            
            //Since only 80% of the worshippers go to war
            double b = gameInfo.EnemyFaction.WorshipperCount * worshiperPercentage;
            //and then truncate it so you don't have a partial worshiper lol
            this.enemyWorshiperCount = (int) b;

            //this.playerMorale = gameInfo.PlayerMorale;
            // not yet
            //this.enemyMorale = gameInfo.EnemyMorale;
            playerMorale = 1;
            enemyMorale = 1;

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
            this.gameInfo.PlayerFaction.WorshipperCount = playWorLeft + System.Convert.ToInt32(this.gameInfo.PlayerWorshipperCount*(1 - worshiperPercentage));
            this.gameInfo.EnemyFaction.WorshipperCount = eneWorLeft + System.Convert.ToInt32(this.gameInfo.EnemyWorshipperCount*(1 - worshiperPercentage));

            this.gameInfo.PlayerFaction.Morale = BoardMan.GetComponent<BoardManager>().getPlayerMorale();
            this.gameInfo.EnemyFaction.Morale = BoardMan.GetComponent<BoardManager>().getEnemyMorale();

            //The battle has been finished
            gameInfo.FinishedBattle = true;

            if (battleResult == 0)
                this.gameInfo.LastBattleStatus = GameInfo.BATTLESTATUS.Victory;
            else if (battleResult == 1)
                this.gameInfo.LastBattleStatus = GameInfo.BATTLESTATUS.Defeat;
            else if (battleResult == 2)
                this.gameInfo.LastBattleStatus = GameInfo.BATTLESTATUS.Retreat;

            //Missing:
            // Change in Player's abilities? (Do they gain them here?)
            // TO DO

            this.finishedBattle = false; //to avoid decrementing things FOREVER

            SceneManager.LoadScene("UnderGodScene"); //¿¿¿presumably loads back to management mode???
        }

        //Note this can't be done in start as tiles hasn't been made yet.
        if (startup)
        {
            tiles = Mapman.GetComponent<MapManager>().tiles;
            //Test Setup
            CreatePlayerUnit(new Vector2(4, 3), tiles, playerWorshiperCount/3, playerMorale); //hello integer division
            CreatePlayerUnit(new Vector2(4, 4), tiles, playerWorshiperCount/3, playerMorale); //also assumes we have 3 units per team
            CreatePlayerUnit(new Vector2(4, 5), tiles, playerWorshiperCount/3, playerMorale);

            CreateEnemyUnit(new Vector2(6, 3), tiles, enemyWorshiperCount/3, enemyMorale);
            CreateEnemyUnit(new Vector2(6, 4), tiles, enemyWorshiperCount/3, enemyMorale);
            CreateEnemyUnit(new Vector2(6, 5), tiles, enemyWorshiperCount/3, enemyMorale);

            startup = false;
        }
    }


    public void CreatePlayerUnit(Vector2 pos, Tile[,] tiles, int WorshiperCount, float morale)
    {
        GameObject temp = Instantiate(Unit);
        temp.GetComponent<Units>().setWorshiperCount(WorshiperCount);
        temp.GetComponent<Units>().setMorale(morale);
        temp.GetComponent<Units>().Move(new Vector2(pos.x, pos.y), tiles);
        BoardMan.GetComponent<BoardManager>().playerUnits.Add(temp);
    }

    public void CreateEnemyUnit(Vector2 pos, Tile[,] tiles, int WorshiperCount, float morale)
    {
        GameObject temp = Instantiate(Unit);
        temp.GetComponent<Units>().setWorshiperCount(WorshiperCount);
        temp.GetComponent<Units>().setMorale(morale);
        temp.GetComponent<Units>().Move(new Vector2(pos.x, pos.y), tiles);
        //Set so the players turn is first
        temp.GetComponent<Units>().EndAct();
        BoardMan.GetComponent<BoardManager>().enemyUnits.Add(temp);
    }

    public void EndBattle()
    {
        finishedBattle = true;
    }

}
