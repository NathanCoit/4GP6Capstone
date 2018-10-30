using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour {

    public Vector2 pos;
    private GameObject MapMan;
    private GameObject BoardMan;

    // Use this for initialization
    void Start ()
    {
        //ITS THE MAP MAN
        MapMan = GameObject.FindGameObjectWithTag("MapManager");
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager");
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private Tile[,] getTiles()
    {
        return MapMan.GetComponent<MapManager>().tiles;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Tile[,] tiles = getTiles();
            List<int> depths = new List<int>();
            HashSet<Tile> visited = new HashSet<Tile>();
            int j = 0;

            depths = tiles[(int)MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().getPos().x, (int)MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().getPos().y].getDepths();
            visited = tiles[(int)MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().getPos().x, (int)MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().getPos().y].getVisited();

            
            foreach(Tile t in visited)
            {
                if(new Vector2(t.getX(), t.getZ()) == pos)
                {
                    break;
                }
               j++;
            }

            MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().Movement -= depths[j];

            //Move the selected unit
            MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().MoveTo(pos, MapMan.GetComponent<MapManager>().tiles);


            //Hide Menu
            MapMan.GetComponent<MapManager>().Selected.transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(false);

            //Attacking
            //check selected unit's MoveTo pos against all opposing team's Unit's pos' and Destroy(enemy Unit) if they're on that tile
            //make it chess-like for now and then factor in stats in a later implementation/refinement
            if (BoardMan.GetComponent<BoardManager>().playerTurn)
            {
                foreach (GameObject i in BoardMan.GetComponent<BoardManager>().enemyUnits)
                {
                    if (this.pos.x == i.GetComponent<Units>().getPos().x && this.pos.y == i.GetComponent<Units>().getPos().y)
                    {
                        BoardMan.GetComponent<BoardManager>().enemyUnits.Remove(i);
                        Destroy(i);
                        break;
                    }
                }
            }
            else if (!BoardMan.GetComponent<BoardManager>().playerTurn)
            {
                foreach (GameObject i in BoardMan.GetComponent<BoardManager>().playerUnits)
                {
                    if (this.pos.x == i.GetComponent<Units>().getPos().x && this.pos.y == i.GetComponent<Units>().getPos().y)
                    {
                        BoardMan.GetComponent<BoardManager>().playerUnits.Remove(i);
                        Destroy(i);
                        break;
                    }
                }
            }

            //End unit's action
            if (MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().Movement == 0)
            {
                MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().EndAct();
                BoardMan.GetComponent<BoardManager>().DecreaseNumActions();
            }

            //Unselect it
            MapMan.GetComponent<MapManager>().Selected = null;

            //Get rid of blue tiles
            MapMan.GetComponent<MapManager>().ClearSelection();
        }
    }
}
