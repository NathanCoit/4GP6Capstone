using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private UIManager UIMan;

    private int numGroupsWorshippers;
    private GameObject[] arrGroupLabels;
    private GameObject[] arrGroupInputs;
    private int[] arrGroupWorshippers;

    private GameObject OverlayCanvas;
    private GameObject BottomPanel;
    private GameObject SurrenderConfirmationPanel;
    private Text[] arrTexts;


    public GameObject Unit;
    public GameObject God;

    private Tile[,] tiles;
    public bool startup = false;
    
    public int playerWorshiperCount;
    public float playerMorale;
    public int enemyWorshiperCount;
    public float enemyMorale;
    public bool finishedBattle = false;
    public GameInfo.BATTLESTATUS battleResult;

    public int playerGodAttackStrength = 50;
    public int enemyGodAttackStrength = 50;

    private readonly double worshipperPercentage = 0.80;


    //NOTE THIS CLASS IS WHERE WE WILL GET INFO FROM MANAGEMENT MODE

    void Awake()
    {
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        UIMan = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        // required for startup stuff
        arrGroupLabels = new GameObject[5];
        arrGroupInputs = new GameObject[5];
        arrGroupWorshippers = new int[5];
        for (int i = 0; i < 5; i++) {
            arrGroupLabels[i] = GameObject.Find("Group" + (i + 1) + "Label");
            arrGroupInputs[i] = GameObject.Find("G" + (i + 1) + "Input");
        }
        OverlayCanvas = GameObject.Find("OverlayCanvas");
        arrTexts = OverlayCanvas.GetComponentsInChildren<Text>();
        BottomPanel = GameObject.Find("BottomPanel");
        SurrenderConfirmationPanel = GameObject.Find("AREYOUSUREPanel");

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

            // Let players know how many worshippers they can assign to their groups
            arrTexts[6].text = "The total amount of worshippers at your disposal : " + playerWorshiperCount;

           MapMan.mapName = gameInfo.EnemyFaction.Type + "1";
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
            gameInfo.EnemyFaction.Type = Faction.GodType.Lightning;

            abilities = Faction.GetGodAbilities(gameInfo.EnemyFaction.Type);
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
            Awake();
        }
#endif
    }

    private void Start()
    {
        Camera.main.GetComponent<CombatCam>().CameraMovementEnabled = false;
        BottomPanel.SetActive(false);
        SurrenderConfirmationPanel.SetActive(false);
        arrGroupLabels[3].SetActive(false);
        arrGroupInputs[3].SetActive(false);
        arrGroupLabels[4].SetActive(false);
        arrGroupInputs[4].SetActive(false);
    }

    void Update()
    {
//#if DEBUG
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
//#endif
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

            // Create and place units onto map
            numGroupsWorshippers = int.Parse(arrTexts[8].text); // need to get it again in case they never changed the # of groups
            for (int i = 0; i < numGroupsWorshippers; i++)
            {
                // TODO: get start tiles from map information to actually load units in a formation rather than just a line
                CreatePlayerUnit(new Vector2(MapMan.playerStartTiles[i].getX(), MapMan.playerStartTiles[i].getZ()), tiles, arrGroupWorshippers[i], 2, 1, playerMorale);
            }
            // TODO: find appropriate health for the god
            CreateGod(tiles, true, gameInfo.PlayerFaction.GodName, 3, 2, Convert.ToInt32(playerGodAttackStrength*gameInfo.GodAttackMultiplier), Convert.ToInt32(playerWorshiperCount*gameInfo.GodHealthMultiplier));

            CreateEnemyUnit(new Vector2(MapMan.enemyStartTiles[0].getX(), MapMan.enemyStartTiles[0].getZ()), tiles, enemyWorshiperCount / 3, 2, 2, enemyMorale);
            CreateEnemyUnit(new Vector2(MapMan.enemyStartTiles[1].getX(), MapMan.enemyStartTiles[1].getZ()), tiles, enemyWorshiperCount / 3, 2, 2, enemyMorale);
            CreateEnemyUnit(new Vector2(MapMan.enemyStartTiles[2].getX(), MapMan.enemyStartTiles[2].getZ()), tiles, enemyWorshiperCount / 3, 2, 2, enemyMorale);
            CreateGod(tiles, false, gameInfo.EnemyFaction.GodName, 3, 2, Convert.ToInt32(enemyGodAttackStrength*0.5*(gameInfo.EnemyFaction.GodTier+1)), Convert.ToInt32(enemyWorshiperCount*(gameInfo.EnemyFaction.GodTier+1)));

            BoardMan.playerTurn = true;
            BoardMan.numActionsLeft = BoardMan.playerUnits.Count;

            Camera.main.GetComponent<CombatCam>().CameraMovementEnabled = true;
            Camera.main.GetComponent<CombatCam>().resetCamera();

            UIMan.updateFaithLabels();

            startup = false;
        }
    }

    private void UpdateStartup(int old)
    {
        switch(old)
        {
            case 1:
                arrGroupLabels[1].SetActive(true);
                arrGroupInputs[1].SetActive(true);
                break;
            case 2:
                if ((old > numGroupsWorshippers))
                { // delete 2
                    arrGroupLabels[1].SetActive(false);
                    arrGroupInputs[1].SetActive(false);
                } else
                { // bring up 3
                    arrGroupLabels[2].SetActive(true);
                    arrGroupInputs[2].SetActive(true);
                }
                break;
            case 3: 
                if ((old > numGroupsWorshippers))
                { // delete 3
                    arrGroupLabels[2].SetActive(false);
                    arrGroupInputs[2].SetActive(false);
                } else
                { // bring up 4
                    arrGroupLabels[3].SetActive(true);
                    arrGroupInputs[3].SetActive(true);
                }
                break;
            case 4:
                if ((old > numGroupsWorshippers))
                { // delete 4
                    arrGroupLabels[3].SetActive(false);
                    arrGroupInputs[3].SetActive(false);
                }
                else
                { // bring up 5
                    arrGroupLabels[4].SetActive(true);
                    arrGroupInputs[4].SetActive(true);
                }
                break;
            case 5:
                arrGroupLabels[4].SetActive(false);
                arrGroupInputs[4].SetActive(false);
                break;
        }
    }

    public void IncreaseNumGroups()
    {
        numGroupsWorshippers = int.Parse(arrTexts[8].text);
        if (numGroupsWorshippers < 5)
        {
            numGroupsWorshippers++;
            arrTexts[8].text = numGroupsWorshippers.ToString();
            UpdateStartup(numGroupsWorshippers - 1);
        }
    }

    public void DecreaseNumGroups()
    {
        numGroupsWorshippers = int.Parse(arrTexts[8].text);
        if (numGroupsWorshippers > 1)
        {
            numGroupsWorshippers--;
            arrTexts[8].text = numGroupsWorshippers.ToString();
            UpdateStartup(numGroupsWorshippers + 1);
        }
    }

    public void ConfirmSplit()
    {
        // function called when startup/division screen's Confirm button is pressed
        // need to check the sum doesn't exceed the number of worshippers player has access to
        int worshipperSum = 0;
        bool success = true;
        bool divideEvenly = false;
        List<int> emptyGroupNumber = new List<int>();
        numGroupsWorshippers = int.Parse(arrTexts[8].text);

        // parse through the groups to see if we have any errors or need to fill in any groups
        for (int i = 0; i < numGroupsWorshippers; i++)
        {
            worshipperSum += arrGroupWorshippers[i];
            if (arrGroupWorshippers[i] == 0)
            {
                //arrTexts[7].text = "You can't have a group of zero!";
                emptyGroupNumber.Add(i);
                divideEvenly = true;
            } else if (worshipperSum > playerWorshiperCount)
            {
                arrTexts[7].text = "You've tried to assign too many worshippers!";
                success = false;
            }
        }
        
        // if we have some empty groups, divide the remaining worshippers to be allocated by the number of empty groups
        if (divideEvenly)
        {
            int dividedWorshippers = (int)((playerWorshiperCount - worshipperSum) / emptyGroupNumber.Count);
            for (int i = 0; i < emptyGroupNumber.Count; i++)
            {
                arrGroupWorshippers[emptyGroupNumber[i]] = dividedWorshippers;
            }
        }

        if (success)
        {
            //close startup/division canvas and commence battle
            GameObject StartupPanel = GameObject.Find("StartupPanel");
            StartupPanel.SetActive(false);
            
            BottomPanel.SetActive(true);

            startup = true;
        }
    }

    public void Group1Worshippers(string value)
    {
        arrGroupWorshippers[0] = int.Parse(value);
    }

    public void Group2Worshippers(string value)
    {
        arrGroupWorshippers[1] = int.Parse(value);
    }

    public void Group3Worshippers(string value)
    {
        arrGroupWorshippers[2] = int.Parse(value);
    }

    public void Group4Worshippers(string value)
    {
        arrGroupWorshippers[3] = int.Parse(value);
    }

    public void Group5Worshippers(string value)
    {
        arrGroupWorshippers[4] = int.Parse(value);
    }

    public void CheckSurrender()
    {
        SurrenderConfirmationPanel.SetActive(true);
    }

    public void CancelSurrender()
    {
        SurrenderConfirmationPanel.SetActive(false);
    }

    public void CreateGod(Tile[,] tiles, bool isPlayer, string godName, int MaxMovement, int attackRange, int attackStregnth, int health)
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
        g.attackRange = attackRange;
        g.AttackStrength = attackStregnth;
        g.WorshiperCount = health;

        if (isPlayer)
        {
            BoardMan.playerUnits.Add(g);
            g.isPlayer = true;
            g.setAbilities(gameInfo.PlayerFaction.Abilities);
            g.MoveTo(new Vector2(-1, -1), tiles);
            g.unitGameObject().transform.position = new Vector3(0, MapMan.godFloatHeight, MapMan.tiles.GetLength(1) / 2);

            GameObject godModel;

            try
            {
                godModel = Instantiate(Resources.Load("Gods/" + gameInfo.PlayerFaction.Type.ToString(), typeof(GameObject))) as GameObject;
            }
            catch (Exception e)
            {
                godModel = Instantiate(Resources.Load("Gods/Mushrooms", typeof(GameObject))) as GameObject;
            }

            godModel.transform.SetParent(g.unitGameObject().transform);
            godModel.transform.position = new Vector3(GodGo.transform.position.x, GodGo.transform.position.y + godModel.GetComponent<GroundOffset>().groundOffset,
                GodGo.transform.position.z + godModel.GetComponent<GroundOffset>().zOffset);

            GodGo.GetComponent<CapsuleCollider>().center = new Vector3(0, godModel.GetComponent<GroundOffset>().colliderCenter, 0);

            GodGo.GetComponent<CapsuleCollider>().height = godModel.GetComponent<GroundOffset>().colliderHeight;

            //Face east
            g.turnToFace(3);

            g.AllowAct();
        }
        else
        {
            BoardMan.enemyUnits.Add(g);
            g.EndAct();
            g.isPlayer = false;
            g.setAbilities(gameInfo.EnemyFaction.Abilities);
            g.MoveTo(new Vector2(-1, -1), tiles);
            g.unitGameObject().transform.position = new Vector3(MapMan.tiles.GetLength(0), MapMan.godFloatHeight, MapMan.tiles.GetLength(1) / 2);

            GameObject godModel;

            try
            {
                godModel = Instantiate(Resources.Load("Gods/" + gameInfo.EnemyFaction.Type.ToString(), typeof(GameObject))) as GameObject;
            }
            catch (Exception e)
            {
                godModel = Instantiate(Resources.Load("Gods/Mushrooms", typeof(GameObject))) as GameObject;
            }

            godModel.transform.SetParent(g.unitGameObject().transform);
            godModel.transform.position = new Vector3(GodGo.transform.position.x, GodGo.transform.position.y + godModel.GetComponent<GroundOffset>().groundOffset,
                GodGo.transform.position.z + godModel.GetComponent<GroundOffset>().zOffset);

            GodGo.GetComponent<CapsuleCollider>().center = new Vector3(0, godModel.GetComponent<GroundOffset>().colliderCenter, 0);

            GodGo.GetComponent<CapsuleCollider>().height = godModel.GetComponent<GroundOffset>().colliderHeight;

            //Face west
            g.turnToFace(1);
        }
    }


    public void CreatePlayerUnit(Vector2 pos, Tile[,] tiles, int WorshiperCount, int MaxMovement, int attackRange, float morale)
    {
        GameObject unitGo = Instantiate(Unit);
        Unit u = new Unit();

        unitGo.GetComponent<UnitObjectScript>().setUnit(u);
        u.assignGameObject(unitGo);
        u.setWorshiperCount(WorshiperCount);
        u.MaxMovement = MaxMovement;
        u.attackRange = attackRange;
        u.setMorale(morale);
        u.updateAttackStrength();
        u.isPlayer = true;
        u.MoveTo(new Vector2(pos.x, pos.y), tiles);

        GameObject unitModel;

        try
        {
            unitModel = Instantiate(Resources.Load("Units/" + gameInfo.PlayerFaction.Type.ToString(), typeof(GameObject))) as GameObject;
        }
        catch (Exception e)
        {
            unitModel = Instantiate(Resources.Load("Units/Mushrooms", typeof(GameObject))) as GameObject;
        }
        unitModel.transform.SetParent(u.unitGameObject().transform);
        unitModel.transform.position = new Vector3(unitGo.transform.position.x, unitGo.transform.position.y + unitModel.GetComponent<GroundOffset>().groundOffset, 
            unitGo.transform.position.z + unitModel.GetComponent<GroundOffset>().zOffset);

        unitGo.GetComponent<CapsuleCollider>().center = new Vector3(0, unitModel.GetComponent<GroundOffset>().colliderCenter, 0);

        unitGo.GetComponent<CapsuleCollider>().height = unitModel.GetComponent<GroundOffset>().colliderHeight;

        u.turnToFace(3);

        u.AllowAct();

        BoardMan.playerUnits.Add(u);
    }

    public void CreateEnemyUnit(Vector2 pos, Tile[,] tiles, int WorshiperCount, int MaxMovement, int attackRange, float morale)
    {
        GameObject unitGo = Instantiate(Unit);
        Unit u = new Unit();

        unitGo.GetComponent<UnitObjectScript>().setUnit(u);
        u.assignGameObject(unitGo);
        u.setWorshiperCount(WorshiperCount);
        u.MaxMovement = MaxMovement;
        u.attackRange = attackRange;
        u.setMorale(morale);
        u.updateAttackStrength();
        u.isPlayer = false;
        u.MoveTo(new Vector2(pos.x, pos.y), tiles);

        Debug.Log(gameInfo.EnemyFaction.Type.ToString());

        GameObject unitModel;

        try
        {
            unitModel = Instantiate(Resources.Load("Units/" + gameInfo.EnemyFaction.Type.ToString(), typeof(GameObject))) as GameObject;
        }
        catch (Exception e)
        {
            unitModel = Instantiate(Resources.Load("Units/Mushrooms", typeof(GameObject))) as GameObject;
        }

        unitModel.transform.SetParent(u.unitGameObject().transform);
        unitModel.transform.position = new Vector3(unitGo.transform.position.x, unitGo.transform.position.y + unitModel.GetComponent<GroundOffset>().groundOffset,
            unitGo.transform.position.z + unitModel.GetComponent<GroundOffset>().zOffset);

        unitGo.GetComponent<CapsuleCollider>().center = new Vector3(0, unitModel.GetComponent<GroundOffset>().colliderCenter, 0);

        unitGo.GetComponent<CapsuleCollider>().height = unitModel.GetComponent<GroundOffset>().colliderHeight;

        u.turnToFace(1);

        //Set so the players turn is first
        u.EndAct();

        BoardMan.enemyUnits.Add(u);
    }

    public void EndBattle()
    {
        finishedBattle = true;
    }

}