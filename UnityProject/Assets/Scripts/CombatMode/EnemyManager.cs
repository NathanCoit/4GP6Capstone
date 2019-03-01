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
    public bool newEnemyTurn;

    // Use this for initialization
    void Start ()
    {
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
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
    public void showClosestTile(Unit Unit)
    {
        Tile[,] tiles = getTiles();

        //Setup Invalid Tiles (the one with units on)
        List<Unit> invalidTiles = new List<Unit>();
        foreach (Unit u in BoardMan.enemyUnits)
        {
            if (!(u.getPos() == Unit.getPos()))
                invalidTiles.Add(u);
        }

        Tile validTile = null;

        if (Unit.getPos().x != -1 && Unit.getPos().y != -1)
        {
            validTile = tiles[(int)Unit.getPos().x, (int)Unit.getPos().y].getClosestTile(BoardMan.playerUnits, Unit.MaxMovement, invalidTiles, BoardMan.playerUnits, tiles);

            //Redefine connection because we broke some above
            MapMan.DefineConnections();

            Unit.MovePriority = tiles[(int)Unit.getPos().x, (int)Unit.getPos().y].MovePriority;
        }

        //If we actually found a tile (we may not if the map is huge or I messed up)
        if (validTile != null)
        {
            GameObject tempTile = Instantiate(BoardMan.MovableTile);
            tempTile.GetComponent<Movable>().pos = new Vector2((int)validTile.getX(), (int)validTile.getZ());
            tempTile.transform.position = new Vector3(validTile.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, validTile.getY() + 0.5f, validTile.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
        }

    }

    //Yes this is redundant. We lookup everything before to get the priorities right. It's dumb but it works.
    public void updatePriorities()
    {
        Tile[,] tiles = getTiles();

        foreach (Unit enemyUnit in BoardMan.enemyUnits)
        {
            //Setup Invalid Tiles (the one with units on)
            List<Unit> invalidTiles = new List<Unit>();
            foreach (Unit otherEnemyUnit in BoardMan.enemyUnits)
            {
                if (!(otherEnemyUnit.getPos() == enemyUnit.getPos()))
                    invalidTiles.Add(otherEnemyUnit);
            }

            Tile t;

            if (enemyUnit.getPos().x != -1 && enemyUnit.getPos().y != -1)
            {
                t = tiles[(int)enemyUnit.getPos().x, (int)enemyUnit.getPos().y].getClosestTile(BoardMan.playerUnits, enemyUnit.MaxMovement, invalidTiles, BoardMan.playerUnits, tiles);
                MapMan.DefineConnections();
                enemyUnit.MovePriority = tiles[(int)enemyUnit.getPos().x, (int)enemyUnit.getPos().y].MovePriority;
            }
        }
    }

    //Fancy cooroutines because we need delays for the AI to work
    public IEnumerator EnemyActions(float delay)
    {
        //Updates the order the enemy units move in (no move and attack goes first, followed by move + attack, followed by move no attack) also order by how far away they are
        updatePriorities();

        //Sort by priority
        BoardMan.enemyUnits.Sort((x, y) => x.MovePriority.CompareTo(y.MovePriority));
        
        foreach (Unit enemyUnit in BoardMan.enemyUnits)
        {
            //Logic for units in battle
            if (enemyUnit.getPos().x != -1 && enemyUnit.getPos().y != -1)
            {
                showClosestTile(enemyUnit);

                yield return new WaitForSeconds(delay);

                MapMan.Selected = enemyUnit.unitGameObject();
                GameObject closestTile = GameObject.FindGameObjectWithTag("MoveableTile");

                //Wait a frame so we can check if we can actually move (or need to)
                yield return null;

                if (closestTile != null)
                {
                    //Woo for using function we made for testing
                    closestTile.GetComponent<Movable>().TestClick();
                    closestTile.GetComponent<Movable>().OnMouseOver();
                    yield return new WaitForSeconds(delay);
                }

                MapMan.Selected = enemyUnit.unitGameObject();

                BoardMan.showAttackable(enemyUnit);

                //Wait a frame to see if we can attack
                yield return null;

                //Yes this is semi random if there's more than one. Will be based off remaining health in future
                GameObject AttackableTile = GameObject.FindGameObjectWithTag("AttackableTile");

                //Attack if we can, then end turn
                if (AttackableTile != null)
                {
                    yield return new WaitForSeconds(delay);
                    AttackableTile.GetComponent<Attackable>().TestClick();
                    AttackableTile.GetComponent<Attackable>().OnMouseOver();
                }
                else
                {
                    enemyUnit.EndTurnButton();
                }
            }
            //TODOm logic for enemy god out of battle
            else
            {
                enemyUnit.EndTurnButton();
            }

        }
    }
}
