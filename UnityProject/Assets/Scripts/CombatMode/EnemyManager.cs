using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

	}

    private Tile[,] getTiles()
    {
        return MapMan.tiles;
    }

    public void showClosestTile(Units Unit)
    {
        Tile[,] tiles = getTiles();

        //Setup Invalid Tiles (the one with units on)
        List<GameObject> invalidTiles = new List<GameObject>();
        foreach (GameObject g in BoardMan.enemyUnits)
        {
            if (!(g.GetComponent<Units>().getPos() == Unit.getPos()))
                invalidTiles.Add(g);
        }

        Tile t = tiles[(int)Unit.getPos().x, (int)Unit.getPos().y].getClosestTile(BoardMan.playerUnits, Unit.MaxMovement, invalidTiles, BoardMan.playerUnits, tiles);

        MapMan.DefineConnections();

        Unit.MovePriority = tiles[(int)Unit.getPos().x, (int)Unit.getPos().y].MovePriority;

        if (t != null)
        {
            GameObject temp = Instantiate(Unit.MovableTile);
            temp.GetComponent<Movable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
            temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
        }

    }

    //Yes this is redundant. We lookup everything before to get the priorities right. It's dumb but it works
    public void updatePriorities()
    {
        Tile[,] tiles = getTiles();

        

        foreach (GameObject g in BoardMan.enemyUnits)
        {
            //Setup Invalid Tiles (the one with units on)
            List<GameObject> invalidTiles = new List<GameObject>();
            foreach (GameObject go in BoardMan.enemyUnits)
            {
                if (!(go.GetComponent<Units>().getPos() == g.GetComponent<Units>().getPos()))
                    invalidTiles.Add(go);
            }

            Units u = g.GetComponent<Units>();
            Tile t = tiles[(int)u.getPos().x, (int)u.getPos().y].getClosestTile(BoardMan.playerUnits, u.MaxMovement, invalidTiles, BoardMan.playerUnits, tiles);
            MapMan.DefineConnections();
            u.MovePriority = tiles[(int)u.getPos().x, (int)u.getPos().y].MovePriority;
        }
    }

    public IEnumerator EnemyActions(float delay)
    {
        updatePriorities();

        BoardMan.enemyUnits.Sort((x, y) => x.GetComponent<Units>().MovePriority.CompareTo(y.GetComponent<Units>().MovePriority));
        
        foreach (GameObject g in BoardMan.enemyUnits)
        {
            Units u = g.GetComponent<Units>();

            showClosestTile(u);

            yield return new WaitForSeconds(delay);

            MapMan.Selected = g;
            GameObject closestTile = GameObject.FindGameObjectWithTag("MoveableTile");
            yield return null;
            if (closestTile != null)
            {
                closestTile.GetComponent<Movable>().testClick();
                closestTile.GetComponent<Movable>().OnMouseOver();
                yield return new WaitForSeconds(delay);
            }

            MapMan.Selected = g;
            u.showAttackable();
            yield return null;
            GameObject AttackableTile = GameObject.FindGameObjectWithTag("AttackableTile");

            if (AttackableTile != null)
            {
                yield return new WaitForSeconds(delay);
                AttackableTile.GetComponent<Attackable>().testClick();
                AttackableTile.GetComponent<Attackable>().OnMouseOver();
            }
            else
            {
                g.GetComponent<Units>().EndTurnButton();
            }

        }
    }
}
