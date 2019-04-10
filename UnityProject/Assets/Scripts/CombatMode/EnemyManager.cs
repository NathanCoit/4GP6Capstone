using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Enemy AI!
 */

public class EnemyManager : MonoBehaviour {

    private MapManager MapMan;
    private BoardManager BoardMan;
    private SoundManager SoundMan;
    public bool newEnemyTurn;

    // Use this for initialization
    void Start ()
    {
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
        SoundMan = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        newEnemyTurn = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //I dont need no update!
	}

    private Tile[,] getTiles()
    {
        return MapMan.tiles;
    }

    //Puts a moveable tile on the optimal tile for an enemy unit to move to
    public void showClosestTile(Unit enemyUnit)
    {
        Tile[,] tiles = getTiles();

        /*
        //Setup Invalid Tiles (the one with units on)
        List<Unit> invalidTiles = new List<Unit>();
        foreach (Unit u in BoardMan.enemyUnits)
        {
            if (!(u.getPos() == Unit.getPos()))
                invalidTiles.Add(u);
        }
        */

        Tile validTile = null;


        List<Tile> targetTiles = new List<Tile>();

        //Find target tiles that are in the attack range of this unit
        foreach(Unit playerUnit in BoardMan.playerUnits)
        {
            if (playerUnit.getPos().x != -1 && playerUnit.getPos().y != -1 && enemyUnit.getPos().x != -1 && enemyUnit.getPos().y != -1)
                targetTiles.AddRange(tiles[(int)enemyUnit.getPos().x, (int)enemyUnit.getPos().y].findAtDistance(
                    MapMan.tiles[(int)playerUnit.getPos().x, (int)playerUnit.getPos().y], (enemyUnit.attackRange - 1), new List<Tile>(), new List<Tile>(), MapMan.tiles));
        }
        if (enemyUnit.getPos().x != -1 && enemyUnit.getPos().y != -1)
        {

            validTile = tiles[(int)enemyUnit.getPos().x, (int)enemyUnit.getPos().y].getClosestTile(
                targetTiles, enemyUnit.MaxMovement, BoardMan.findTeamTiles(BoardMan.enemyUnits), BoardMan.findTeamTiles(BoardMan.playerUnits), tiles);

            //Redefine connection because we broke some above
            MapMan.DefineConnections();

            enemyUnit.MovePriority = tiles[(int)enemyUnit.getPos().x, (int)enemyUnit.getPos().y].MovePriority;
        }
        

        //If we actually found a tile (we may not if the map is huge or I messed up)
        if (validTile != null)
        {
            GameObject temp = Instantiate(BoardMan.MovableTile);
            temp.GetComponent<Movable>().pos = new Vector2((int)validTile.getX(), (int)validTile.getZ());

            temp.transform.position = new Vector3(validTile.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2,
                validTile.getY() + 0.5f, validTile.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);

            temp.GetComponent<Movable>().setTarget(new Vector3(validTile.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2,
                validTile.getY() + 0.5f, validTile.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2));
        }

    }

    //Yes this is redundant. We lookup everything before to get the priorities right. It's dumb but it works.
    public void updatePriorities()
    {
        Tile[,] tiles = getTiles();

        foreach (Unit enemyUnit in BoardMan.enemyUnits)
        {
            /*
            //Setup Invalid Tiles (the one with units on)
            List<Unit> invalidTiles = new List<Unit>();
            foreach (Unit otherEnemyUnit in BoardMan.enemyUnits)
            {
                if (!(otherEnemyUnit.getPos() == enemyUnit.getPos()))
                    invalidTiles.Add(otherEnemyUnit);
            }
            */

            List<Tile> targetTiles = new List<Tile>();
            Tile validTile = null;

            foreach (Unit playerUnit in BoardMan.playerUnits)
            {
                if(playerUnit.getPos().x != -1 && playerUnit.getPos().y != -1 && enemyUnit.getPos().x != -1 && enemyUnit.getPos().y != -1)
                    targetTiles.AddRange(tiles[(int)enemyUnit.getPos().x, (int)enemyUnit.getPos().y].findAtDistance(
                        MapMan.tiles[(int)playerUnit.getPos().x, (int)playerUnit.getPos().y], (enemyUnit.attackRange - 1), new List<Tile>(), new List<Tile>(), MapMan.tiles));
            }
            if (enemyUnit.getPos().x != -1 && enemyUnit.getPos().y != -1)
            {

                validTile = tiles[(int)enemyUnit.getPos().x, (int)enemyUnit.getPos().y].getClosestTile(
                    targetTiles, enemyUnit.MaxMovement, BoardMan.findTeamTiles(BoardMan.enemyUnits), BoardMan.findTeamTiles(BoardMan.playerUnits), tiles);

                //Redefine connection because we broke some above
                MapMan.DefineConnections();

                enemyUnit.MovePriority = tiles[(int)enemyUnit.getPos().x, (int)enemyUnit.getPos().y].MovePriority;
            }
        }
    }

    //Fancy cooroutines because we need delays for the AI to work
    public IEnumerator EnemyActions(float delay)
    {
        SoundMan.playEnemyGodTurnStart();

        //Updates the order the enemy units move in (no move and attack goes first, followed by move + attack, followed by move no attack) also order by how far away they are
        updatePriorities();

        //Sort by priority
        BoardMan.enemyUnits.Sort((x, y) => x.MovePriority.CompareTo(y.MovePriority));
        
        foreach (Unit enemyUnit in BoardMan.enemyUnits)
        {
            Camera.main.GetComponent<CombatCam>().lookAt(enemyUnit.unitGameObject().transform.position);
            //Logic for units in battle
            if (enemyUnit.getPos().x != -1 && enemyUnit.getPos().y != -1)
            {
                yield return new WaitForSeconds(delay);

                showClosestTile(enemyUnit);

                yield return new WaitForSeconds(delay);

                MapMan.Selected = enemyUnit.unitGameObject();
                GameObject closestTile = GameObject.FindGameObjectWithTag("MoveableTile");

                //Wait a frame so we can check if we can actually move (or need to)
                yield return null;

                if (closestTile != null)
                {
                    //Woo for using function we made for testing
                    Camera.main.GetComponent<CombatCam>().lookAt(closestTile.transform.position);
                    closestTile.GetComponent<Movable>().TestClick();
                    closestTile.GetComponent<Movable>().OnMouseOver();
                    yield return new WaitForSeconds(delay);
                }

                MapMan.Selected = enemyUnit.unitGameObject();

                BoardMan.showAttackable(enemyUnit);

                yield return new WaitForSeconds(delay);

                //Wait a frame to see if we can attack
                yield return null;

                //Yes this is semi random if there's more than one. Will be based off remaining health in future
                GameObject AttackableTile = GameObject.FindGameObjectWithTag("AttackableTile");

                //Attack if we can, then end turn
                if (AttackableTile != null)
                {
                    Camera.main.GetComponent<CombatCam>().lookAt(AttackableTile.transform.position);
                    AttackableTile.GetComponent<Attackable>().TestClick();
                    AttackableTile.GetComponent<Attackable>().OnMouseOver();
                    yield return new WaitForSeconds(delay);
                }
                else
                {
                    enemyUnit.EndTurnButton();
                }
            }
            
            //Logic for enemy god that is not in battle (using abilities)
            else
            {
                MapMan.Selected = enemyUnit.unitGameObject();

                //Check if we can use any abilites
                List<Ability> useableAbilities = new List<Ability>();

                foreach(string ability in (enemyUnit as God).getAbilites())
                {
                    Debug.Log(ability);
                    if((enemyUnit as God).faith >= Ability.LoadAbilityFromName(ability).FaithCost)
                    {
                        useableAbilities.Add(Ability.LoadAbilityFromName(ability));
                    }
                }

                //Chose to use an ability or not based chance weighted by how many abilties we can use
                float roll = Random.Range(0.0f, 100.0f);
                Ability abilityToBeUsed = null;

                if(roll < useableAbilities.Count * 25 || useableAbilities.Count == 4)
                {
                    //Use system.random to pick a random ability
                    System.Random r = new System.Random();
                    abilityToBeUsed = useableAbilities[r.Next(useableAbilities.Count)];
                }

                if(abilityToBeUsed != null)
                {
                    BoardMan.useAbility(abilityToBeUsed.AbilityName);
                    List<Targetable> validTiles = new List<Targetable>();
                    List<Unit> parallelTargets = new List<Unit>();

                    //Wait for tiles to be done drawing
                    while(BoardMan.areAbilityTilesDrawing())
                    {
                        yield return new WaitForSeconds(1);
                    }

                    foreach(GameObject tile in GameObject.FindGameObjectsWithTag("TargetableTile"))
                    {
                        Targetable tileScript = tile.GetComponent<Targetable>();
                        tileScript.OnMouseOver();
                        if (tileScript.valid)
                            validTiles.Add(tileScript);
                    }

                    //Make a parallel list of units to valid tiles
                    foreach(Targetable tile in validTiles)
                        foreach(Unit unit in tile.targets)
                        {
                            if (tile.pos.x == unit.getPos().x && tile.pos.y == unit.getPos().y)
                                parallelTargets.Add(unit);
                        }


                    yield return new WaitForSeconds(delay);

                    Targetable target = null;
                    int mostWorshipperCount = 0;
                    int leastWorshipperCount = int.MaxValue;

                    //Do stuff based on ability type
                    switch (abilityToBeUsed.AbiltyType)
                    {
                        
                        case Ability.ABILITYTYPE.SingleTarget:
                            Debug.Log("Using a single target ability!");

                            //Use single target on weakest unit
                            leastWorshipperCount = int.MaxValue;

                            for(int i = 0; i < parallelTargets.Count; i++)
                            {
                                if (parallelTargets[i].WorshiperCount < leastWorshipperCount)
                                    target = validTiles[i];
                            }

                            if (target != null)
                            {
                                target.testClick();
                                target.OnMouseOver();
                            }

                            break;
                        case Ability.ABILITYTYPE.MultiTarget:
                            Debug.Log("Using a multi target ability!");

                            //Use multi target centered on weakest unit
                            leastWorshipperCount = int.MaxValue;

                            for (int i = 0; i < parallelTargets.Count; i++)
                            {
                                if (parallelTargets[i].WorshiperCount < leastWorshipperCount)
                                    target = validTiles[i];
                            }

                            if (target != null)
                            {
                                target.testClick();
                                target.OnMouseOver();
                            }

                            break;
                        case Ability.ABILITYTYPE.Buff:
                            Debug.Log("Using a buff ability!");

                            //Use buff on healthiest unit
                            mostWorshipperCount = 0;

                            for (int i = 0; i < parallelTargets.Count; i++)
                            {
                                if (parallelTargets[i].WorshiperCount > mostWorshipperCount)
                                    target = validTiles[i];
                            }

                            if (target != null)
                            {
                                target.testClick();
                                target.OnMouseOver();
                            }

                            break;
                        case Ability.ABILITYTYPE.Debuff:
                            Debug.Log("Using a debuff ability!");

                            //Use debuff on healthiest unit
                            mostWorshipperCount = 0;

                            for (int i = 0; i < parallelTargets.Count; i++)
                            {
                                if (parallelTargets[i].WorshiperCount > mostWorshipperCount)
                                    target = validTiles[i];
                            }

                            if (target != null)
                            {
                                target.testClick();
                                target.OnMouseOver();
                            }

                            break;
                    }
                }

                enemyUnit.EndTurnButton();
            }

        }
    }
}
