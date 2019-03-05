using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * The class responsible for keeping track of:
 *      - each team (i.e. the list of worshippers and God)
 *      - their respective morale values
 *      - each God's faith values (basically mana for Godly abilities)
 *      - whose turn it is (player or enemy God)
 *      - the Victory, Defeat and Retreat functions
 *      
 *      BASICALLY: This class is concerned with the STATE of the combat mode
 */
public class BoardManager : MonoBehaviour
{

    public bool playerTurn;

    private SetupManager SetupMan;
    private EnemyManager EnemyMan;
    private MapManager MapMan;
    private UIManager UIMan;
    private SoundManager SoundMan;

    public List<Unit> playerUnits; //List of player's units
    public List<Unit> enemyUnits; //List of enemy's worshipper units
    public int numActionsLeft;

    public bool endBattle = false; //used for testing purposes - to see if the battle has ended even if there are units left
    public float PlayerMorale;
    public float enemyMorale;
    public float playerFaith;
    public float enemyFaith;

    public float faithCap;

    public GameObject MovableTile;
    public GameObject InMoveRangeTile;
    public GameObject PreviewMoveTile;
    public GameObject AttackableTile;
    public GameObject PreviewAttackTile;
    public GameObject TargetableTile;

    public GameObject endAllButton;
    public GameObject selectNextButton;

    public int selectNextIndex;

    public int abilityDirection;

    void Start()
    {
        playerUnits = new List<Unit>();
        enemyUnits = new List<Unit>();

        /*
        numActionsLeft = playerUnits.Count; //since player always starts first
        playerTurn = true;
        */
        
        SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();

        //The baddest of them all, its EnemyMan
        EnemyMan = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();

        //Here to spit some hot ui
        UIMan = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        SoundMan = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();

        abilityDirection = 0;
    }
    
    void Update()
    {
        //Updates Morale frequently. Involved with attack strength calculation so we need to update it frequently. Maybe. We're not really sure. But this works!
        // TODO fix
        PlayerMorale = SetupMan.playerMorale;
        enemyMorale = SetupMan.enemyMorale;

        /*
        if(playerUnits.Count != 0 && enemyUnits.Count != 0)
            if (!HasActionsLeft()) //any actions left to take? if not, switch turns
                SwitchTurns();
        */
    }

    public void CheckEnd()
    {
        //check if someone won the game (note we're checking if its 1 since we dont have killing gods in yet)
        if (playerUnits.Count == 0)
        {
            Defeat();
        }
        else if (enemyUnits.Count == 0)
        {
            Victory();
        }
    }

    public void checkIfGodShouldBeInBattle()
    {
        if (playerUnits.Count == 1)
        {
            if(!(playerUnits[0] as God).isInBattle)
                (playerUnits[0] as God).forcedEnterBattle();
        }
        else if (enemyUnits.Count == 1)
        {
            if (!(enemyUnits[0] as God).isInBattle)
                (enemyUnits[0] as God).forcedEnterBattle();
        }
    }

    public void Victory()
    {
        SetupMan.battleResult = GameInfo.BATTLESTATUS.Victory;
        SetupMan.finishedBattle = true;
    }

    public void Defeat()
    {
        SetupMan.battleResult = GameInfo.BATTLESTATUS.Defeat;
        SetupMan.finishedBattle = true;
    }

    public void Retreat()
    {
        SetupMan.battleResult = GameInfo.BATTLESTATUS.Retreat;
        SetupMan.finishedBattle = true;

        int worshipersLeft = GetRemainingWorshippers(true);
        SetupMan.playerWorshiperCount = worshipersLeft; //incorrect value atm

    }

    public int GetRemainingWorshippers(bool player)
    {
        int worshippers = 0;
        if (player)
        {
            foreach (Unit u in playerUnits)
            {
                worshippers += u.getWorshiperCount();
            }
        }
        else //need to calculate enemy worshiper count if player decided to kill enemy god first/early, however not implemented yet
        {
            foreach (Unit u in enemyUnits)
            {
                worshippers += u.getWorshiperCount();
            }
        }
        return worshippers;
    }

    //Find the tiles that a list of units are on
    public List<Tile> findTeamTiles(List<Unit> team)
    {
        List<Tile> teamTiles = new List<Tile>();

        foreach (Unit currentUnit in team)
            if((int)currentUnit.getPos().x != -1 && (int)currentUnit.getPos().y != -1)
                teamTiles.Add(MapMan.tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y]);

        return teamTiles;
    }

    //Helper function for show moveable and similar
    private HashSet<Tile> findMoveable(Unit currentUnit)
    {
        HashSet<Tile> MovableTiles = new HashSet<Tile>();

        if (playerUnits.Contains(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()))
        {
            //Use our lovely tile function <3
            MovableTiles = MapMan.tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y].findAtDistance(
                MapMan.tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y], currentUnit.Movement, findTeamTiles(playerUnits), findTeamTiles(enemyUnits), MapMan.tiles);
        }
        else
        {
            MovableTiles = MapMan.tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y].findAtDistance(
                MapMan.tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y], currentUnit.Movement, findTeamTiles(enemyUnits), findTeamTiles(playerUnits), MapMan.tiles);
        }

        //We need to do this because the above function breaks some connection (like the one that can't be moved through)
        MapMan.DefineConnections();

        return MovableTiles;
    }

    //Helper function for show attackable and similar
    private HashSet<Tile> findAttackable(Unit currentUnit)
    {
        HashSet<Tile> AttackableTiles = new HashSet<Tile>();

        List<Tile> invalidTiles = new List<Tile>();

        if (playerUnits.Contains(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()))
        {
            foreach (Tile t in MapMan.tiles)
                if (!findTeamTiles(enemyUnits).Contains(t))
                    invalidTiles.Add(t);
        }
        else
        {
            foreach (Tile t in MapMan.tiles)
                if (!findTeamTiles(playerUnits).Contains(t))
                    invalidTiles.Add(t);  
        }

        AttackableTiles = MapMan.tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y].findAtDistance(
                MapMan.tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y], currentUnit.attackRange, invalidTiles, new List<Tile>(), MapMan.tiles);

        return AttackableTiles;
    }

    //Checks if a unit can still move
    public bool canMove(Unit currentUnit)
    {
        if (findMoveable(currentUnit).Count != 0)
            return true;
        else
            return false;
    }

    //Check if a unit can still attack
    public bool canAttack(Unit currentUnit)
    {
        if (findAttackable(currentUnit).Count != 0)
            return true;
        else
            return false;
    }

    //Get all the tiles a unit can move to, based on their remaining movement
    public void showMovable(Unit currentUnit)
    {
        //Calculate Moveable tiles
        HashSet<Tile> MovableTiles = findMoveable(currentUnit);

        HashSet<Tile> inMoveRangeTiles = new HashSet<Tile>();

        inMoveRangeTiles = MapMan.tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y].findAtDistance(
            MapMan.tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y], currentUnit.Movement, new List<Tile>(), findTeamTiles(enemyUnits), MapMan.tiles);

        foreach (Tile t in MovableTiles)
            inMoveRangeTiles.Remove(t);

        //Clean up all the other tiles (before we make more tiles)
        MapMan.ClearSelection();

        UIMan.hideMenu();

        //Draw movable tiles
        foreach (Tile t in MovableTiles)
        {
            GameObject temp = Instantiate(MovableTile);
            temp.GetComponent<Movable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
            temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2,
                t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
        }

        foreach (Tile t in inMoveRangeTiles)
        {
            GameObject temp = Instantiate(InMoveRangeTile);
            temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2,
                t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
        }
    }

    //Previews units movement. Can't pass unit due to it being use with event tiggers (At least I couldn't figure out how)
    public void previewMoveable()
    {
        Unit currentUnit = MapMan.Selected.GetComponent<UnitObjectScript>().getUnit();
        HashSet<Tile> previewMoveTiles = new HashSet<Tile>();

        //Use our lovely tile function <3
        previewMoveTiles = MapMan.tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y].findAtDistance(
            MapMan.tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y], currentUnit.Movement, new List<Tile>(), findTeamTiles(enemyUnits), MapMan.tiles);

        MapMan.DefineConnections();

        //Draw movable tiles
        foreach (Tile t in previewMoveTiles)
        {
            GameObject temp = Instantiate(PreviewMoveTile);
            temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, 
                t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
        }
    }

    //Shows attackable tiles (for attacking)
    public void showAttackable(Unit currentUnit)
    {
        HashSet<Tile> AttackableTiles = findAttackable(currentUnit);

        //Clean up all the other tiles
        MapMan.ClearSelection();

        UIMan.hideMenu();

        //Draw movable tiles
        foreach (Tile t in AttackableTiles)
        {
            GameObject temp = Instantiate(AttackableTile);
            temp.GetComponent<Attackable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
            temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
            //temp.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
            //Movable.Add(temp);
        }

    }

    public void previewAttackable()
    {
        Unit currentUnit = MapMan.Selected.GetComponent<UnitObjectScript>().getUnit();

        HashSet<Tile> preveiwAttackTiles = new HashSet<Tile>();

        preveiwAttackTiles = MapMan.tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y].findAtDistance(
                MapMan.tiles[(int)currentUnit.getPos().x, (int)currentUnit.getPos().y], currentUnit.attackRange, new List<Tile>(), new List<Tile>(), MapMan.tiles);

        MapMan.DefineConnections();

        foreach (Tile t in preveiwAttackTiles)
        {
            GameObject temp = Instantiate(PreviewAttackTile);
            temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2,
                t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
        }
    }

    //Called on the button press. Starts the target selection.
    public void useAbility(string name)
    {
        //TODO ui things here (like hiding menu)

        Ability a = Ability.LoadAbilityFromName(name);

        //Put targetable tiles everywhere
        foreach (Tile t in MapMan.tiles)
        {
            GameObject targetTile = Instantiate(TargetableTile);
            targetTile.GetComponent<Targetable>().ability = a;
            targetTile.GetComponent<Targetable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
            targetTile.transform.position = new Vector3(t.getX() + ((1 - targetTile.transform.lossyScale.x) / 2) + targetTile.transform.lossyScale.x / 2, t.getY() + 0.5f, t.getZ() + ((1 - targetTile.transform.lossyScale.z) / 2) + targetTile.transform.lossyScale.x / 2);
        }

        Camera.main.GetComponent<CombatCam>().resetCamera();

        UIMan.removeMenu();
        UIMan.abilityPanelInUse = false;


    }

    public void killUnit(Unit killedUnit)
    {
        if (playerUnits.Contains(killedUnit))
            playerUnits.Remove(killedUnit);
        else
            enemyUnits.Remove(killedUnit);

        if (playerUnits.Contains(killedUnit))
            playerUnits.Remove(killedUnit);
        else
            enemyUnits.Remove(killedUnit);

        SoundMan.playUnitDeath();
        CheckEnd();
        checkIfGodShouldBeInBattle();
        Destroy(killedUnit.unitGameObject());
    }

    public void endAll()
    {
        foreach (Unit u in playerUnits)
            if(u.canAct)
                u.EndTurnButton();
    }

    public void selectNext()
    {
        for (int i = 0; i < playerUnits.Count; i++)
        {
            selectNextIndex++;
            if (selectNextIndex > playerUnits.Count - 1)
                selectNextIndex = 0;

            if (playerUnits[selectNextIndex].canAct)
            {
                MapMan.Selected = playerUnits[selectNextIndex].unitGameObject();
                MapMan.newSelected = true;
                break;
            }
        }
    }

    public float GetPlayerMorale()
    {
        return PlayerMorale;
    }

    public float GetEnemyMorale()
    {
        return enemyMorale;
    }

    public float GetPlayerFaith()
    {
        return playerFaith;
    }

    public float GetEnemyFaith()
    {
        return enemyFaith;
    }

    public float GetFaithCap()
    {
        return faithCap;
    }

    public void setPlayerMorale(float m)
    {
        PlayerMorale = m;
    }

    public void setEnemyMorale(float m)
    {
        enemyMorale = m;
    }

    public void checkIfSwitchTurn()
    {
        if (numActionsLeft == 0)
            SwitchTurns();
    }

    public void SwitchTurns()
    {
        if (playerTurn)
        { //it was player's turn
            Debug.Log(enemyUnits.Count);
            foreach (Unit u in enemyUnits) //allow each of enemy units to act
                u.AllowAct();
            numActionsLeft = enemyUnits.Count;

            endAllButton.GetComponent<Button>().interactable = false;
            selectNextButton.GetComponent<Button>().interactable = false;

            StartCoroutine(EnemyMan.EnemyActions(0.5f));
        }
        else
        { //it was the enemy's turn
            foreach (Unit u in playerUnits) //allow each of player's units to act
                u.AllowAct();
            numActionsLeft = playerUnits.Count;

            endAllButton.GetComponent<Button>().interactable = true;
            selectNextButton.GetComponent<Button>().interactable = true;

            Camera.main.GetComponent<CombatCam>().resetCamera();
        }
        playerTurn = !playerTurn; //switch turn
        
    }

    bool HasActionsLeft()
    {
        if (numActionsLeft > 0) return true;
        else return false;
    }

    public void DecreaseNumActions()
    {
        numActionsLeft--;
    }

    public bool isPlayerTurn()
    {
        return playerTurn;
    }


    //Used for testing purposes
    public bool surrender()
    {
        //not implemented yet so it's returning a bool
        Retreat();
        return false;
    }
}