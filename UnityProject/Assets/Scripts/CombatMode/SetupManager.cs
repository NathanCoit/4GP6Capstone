using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Class responsible for:
 *      - starting up combat mode (placement of units and division of worshippers among each group)
 *      - communicating with management BEFORE and AFTER combat starts/ends (morale of each side, amount of worshippers left, battle result)
 *      
 * Basically concerned with SETTING UP combat mode and SENDING BACK updated stats
 * 
 */ 
public class SetupManager : MonoBehaviour
{

    public GameObject GameInfoObjectPrefab;
    private GameInfo gameInfo;

    public BoardManager BoardMan;
    public MapManager MapMan;

    public GameObject Unit;
    public GameObject God;

    private Tile[,] tiles;
    public bool startup = true;
    
    public int playerWorshiperCount;
    public float playerMorale;
    public int enemyWorshiperCount;
    public float enemyMorale;
    public bool finishedBattle = false;
    public GameInfo.BATTLESTATUS battleResult; //0 for victory, 1 for defeat, 2 for retreat

    private readonly double worshipperPercentage = 0.80;


    //NOTE THIS CLASS IS WHERE WE WILL GET INFO FROM MANAGEMENT MODE

    void Start()
    {
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        GameObject GameInfoObject = GameObject.Find("GameInfo");
        if (GameInfoObject != null)
        {
            gameInfo = GameInfoObject.GetComponent<GameInfo>();
            //Found a game object, load values
            //Since only 80% of the worshippers go to war
            double a = gameInfo.PlayerFaction.WorshipperCount * worshipperPercentage;
            this.playerWorshiperCount = (int)a;

            //Since only 80% of the worshippers go to war
            double b = gameInfo.EnemyFaction.WorshipperCount * worshipperPercentage;
            //and then truncate it so you don't have a partial worshiper lol
            this.enemyWorshiperCount = (int)b;

            playerMorale = gameInfo.PlayerFaction.Morale;
            enemyMorale = gameInfo.EnemyFaction.Morale;
        }
#if DEBUG
        else
        {
            Debug.Log("No gameInfo object found, setting a test scene for you boyo");
            Debug.Log("If this isn't someone working on Combat Mode, things are broken");
            GameObject NewGameInfoObject = (GameObject)Instantiate(GameInfoObjectPrefab);
            NewGameInfoObject.name = "GameInfo";
            gameInfo = NewGameInfoObject.GetComponent<GameInfo>();

            //Setup some test values (feel free to change)
            gameInfo.PlayerFaction.GodName = "JIMOTHY THEE GREAT";
            gameInfo.PlayerFaction.Type = Faction.GodType.Mushrooms;

            List<Ability> abilities = Faction.GetGodAbilities(gameInfo.PlayerFaction.Type);
            string[] sAbilites = new string[abilities.Count];
            for (int i = 0; i < abilities.Count; i++)
                sAbilites[i] = abilities[i].AbilityName;

            gameInfo.PlayerFaction.Abilities = sAbilites;
                
            Debug.Log(gameInfo.PlayerFaction.GodName + " reporting in, boss");

            gameInfo.EnemyFaction.GodName = "Nathan";
            gameInfo.EnemyFaction.Type = Faction.GodType.Ducks;

            abilities = Faction.GetGodAbilities(gameInfo.PlayerFaction.Type);
            sAbilites = new string[abilities.Count];
            for (int i = 0; i < abilities.Count; i++)
                sAbilites[i] = abilities[i].AbilityName;

            gameInfo.EnemyFaction.Abilities = sAbilites;

            Debug.Log(gameInfo.EnemyFaction.GodName + " reporting in, boss");

            gameInfo.PlayerFaction.WorshipperCount = 300;
            gameInfo.EnemyFaction.WorshipperCount = 200;

            gameInfo.PlayerFaction.Morale = 1;
            gameInfo.EnemyFaction.Morale = 1;

            //Call start setup stuff
            Start();
        }
#endif
    }

    void Update()
    {
#if DEBUG
        // Debug Precompile Directive to allow ending a battle immediatly.
        if(Input.GetKeyDown(KeyCode.J))
        {
            finishedBattle = true;
            battleResult = GameInfo.BATTLESTATUS.Defeat;
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            finishedBattle = true;
            battleResult = GameInfo.BATTLESTATUS.Victory;
        }
#endif
        //Battle has ended, send stats to GameInfo object (the object used to communicate between management mode and combat mode)
        if (finishedBattle)
        {
            //get remaining worshiper count for both sides
            int playWorLeft = BoardMan.GetRemainingWorshippers(true);
            int eneWorLeft = BoardMan.GetRemainingWorshippers(false);

            //Update all of the gameinfo variables:

            //Take original amount of worshipers and add how many worshipers are left after the war
            this.gameInfo.PlayerFaction.WorshipperCount = playWorLeft + System.Convert.ToInt32(this.gameInfo.PlayerFaction.WorshipperCount * (1 - worshipperPercentage));
            this.gameInfo.EnemyFaction.WorshipperCount = eneWorLeft + System.Convert.ToInt32(this.gameInfo.EnemyFaction.WorshipperCount * (1 - worshipperPercentage));

            this.gameInfo.PlayerFaction.Morale = BoardMan.GetPlayerMorale();
            this.gameInfo.EnemyFaction.Morale = BoardMan.GetEnemyMorale();

            gameInfo.FinishedBattle = true;

            gameInfo.LastBattleStatus = battleResult; //Victory, Defeat or Retreat

            //Missing:
            // Change in Player's abilities? (Do they gain them here?)
            // TODO

            this.finishedBattle = false; //to avoid decrementing things FOREVER

            SceneManager.LoadScene("UnderGodScene"); //load back to management mode
        }

        //Note this can't be done in start as tiles hasn't been made yet.
        if (startup)
        {
            tiles = MapMan.tiles;
            //Test Setup
            CreatePlayerUnit(new Vector2(4, 3), tiles, playerWorshiperCount / 3, 2, playerMorale); //hello integer division
            CreatePlayerUnit(new Vector2(4, 4), tiles, playerWorshiperCount / 3, 2, playerMorale); //also assumes we have 3 units per team
            CreatePlayerUnit(new Vector2(4, 5), tiles, playerWorshiperCount / 3, 2, playerMorale);
            CreateGod(tiles, true, gameInfo.PlayerFaction.GodName, 3);

            CreateEnemyUnit(new Vector2(6, 3), tiles, enemyWorshiperCount / 3, 2, enemyMorale);
            CreateEnemyUnit(new Vector2(6, 4), tiles, enemyWorshiperCount / 3, 2, enemyMorale);
            CreateEnemyUnit(new Vector2(6, 5), tiles, enemyWorshiperCount / 3, 2, enemyMorale);
            CreateGod(tiles, false, gameInfo.EnemyFaction.GodName, 3);

            startup = false;
        }
    }

    public bool SplitWorshipers()
    {
        //haha not implemented
        //will be done in a separate scene maybe?
        //assign worshiper count percentages in that scene to individual units
        //can have up to ten units per side

        return false;
    }

    public void CreateGod(Tile[,] tiles, bool isPlayer, string godName, int MaxMovement)
    {
        /*
        GameObject temp = Instantiate(God);
        Units u = temp.GetComponent<Units>();
        Gods g = temp.GetComponent<Gods>();

        u.setGod();

        

        if (isPlayer)
        {
            g.setName(gameInfo.PlayerFaction.GodName);
            g.setAbilites(gameInfo.PlayerFaction.Abilities);
            u.isPlayer = true;
            temp.transform.position = new Vector3(0, MapMan.godFloatHeight, MapMan.tiles.GetLength(1) / 2);
            BoardMan.playerUnits.Add(temp);
        }
        else
        {
            g.setName(gameInfo.EnemyFaction.GodName);
            g.setAbilites(gameInfo.EnemyFaction.Abilities);
            u.isPlayer = false;
            temp.transform.position = new Vector3(MapMan.tiles.GetLength(0), MapMan.godFloatHeight, MapMan.tiles.GetLength(1) / 2);
            BoardMan.enemyUnits.Add(temp);
        }
        */
        GameObject GodGo = Instantiate(God);
        God g = new God(godName);

        GodGo.GetComponent<UnitObjectScript>().setUnit(g);
        g.assignGameObject(GodGo);

        //TODO, don't know exactly how we're handling this
        g.setWorshiperCount(0);
        g.setMorale(1);


        g.MaxMovement = MaxMovement;

        if (isPlayer)
        {
            BoardMan.playerUnits.Add(g);
            g.isPlayer = true;
            g.setAbilities(gameInfo.PlayerFaction.Abilities);
            g.MoveTo(new Vector2(-1, -1), tiles);
            g.unitGameObject().transform.position = new Vector3(0, MapMan.godFloatHeight, MapMan.tiles.GetLength(1) / 2);
        }
        else
        {
            BoardMan.enemyUnits.Add(g);
            g.EndAct();
            g.isPlayer = false;
            g.setAbilities(gameInfo.PlayerFaction.Abilities);
            g.MoveTo(new Vector2(-1, -1), tiles);
            g.unitGameObject().transform.position = new Vector3(MapMan.tiles.GetLength(0), MapMan.godFloatHeight, MapMan.tiles.GetLength(1) / 2);
        }
    }


    public void CreatePlayerUnit(Vector2 pos, Tile[,] tiles, int WorshiperCount, int MaxMovement, float morale)
    {
        GameObject unitGo = Instantiate(Unit);
        Unit u = new Unit();

        unitGo.GetComponent<UnitObjectScript>().setUnit(u);
        u.assignGameObject(unitGo);
        u.setWorshiperCount(WorshiperCount);
        u.MaxMovement = MaxMovement;
        u.setMorale(morale);
        u.updateAttackStrength();
        u.isPlayer = true;
        u.MoveTo(new Vector2(pos.x, pos.y), tiles);

        BoardMan.playerUnits.Add(u);
    }

    public void CreateEnemyUnit(Vector2 pos, Tile[,] tiles, int WorshiperCount, int MaxMovement, float morale)
    {
        GameObject unitGo = Instantiate(Unit);
        Unit u = new Unit();

        unitGo.GetComponent<UnitObjectScript>().setUnit(u);
        u.assignGameObject(unitGo);
        u.setWorshiperCount(WorshiperCount);
        u.MaxMovement = MaxMovement;
        u.setMorale(morale);
        u.updateAttackStrength();
        u.isPlayer = false;
        u.MoveTo(new Vector2(pos.x, pos.y), tiles);

        //Set so the players turn is first
        u.EndAct();
        BoardMan.enemyUnits.Add(u);
    }

    public void EndBattle()
    {
        finishedBattle = true;
    }

}