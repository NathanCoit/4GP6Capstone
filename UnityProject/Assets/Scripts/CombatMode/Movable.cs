using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script associated with the Movable tiles (the blue tiles where worshippers can move). Pretty self explanatory.
 */
public class Movable : MonoBehaviour {

    public Vector2 pos;
    private MapManager MapMan;
    private BoardManager BoardMan;
    private UIManager UIMan;

    private bool autoClick;

    // Use this for initialization
    void Start ()
    {
        //ITS THE MAP MAN
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
        UIMan = GameObject.Find("UIManager").GetComponent<UIManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private Tile[,] getTiles()
    {
        return MapMan.GetComponent<MapManager>().tiles;
    }


    /// <summary>
    /// 
    /// </summary>
    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) || autoClick)
        {
            Tile[,] tiles = getTiles();
            List<int> depths = new List<int>();
            HashSet<Tile> visited = new HashSet<Tile>();
            int j = 0;

            depths = tiles[(int)MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().getPos().x, 
                (int)MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().getPos().y].getDepths();
            visited = tiles[(int)MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().getPos().x,
                (int)MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().getPos().y].getVisited();


            foreach(Tile visitedTile in visited)
            {
                if(new Vector2(visitedTile.getX(), visitedTile.getZ()) == pos)
                {
                    break;
                }
               j++;
            }

            MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().Movement -= depths[j] + 1;

            //Move the selected unit
            MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().MoveTo(pos, MapMan.tiles);

            //Unselect it
            MapMan.Selected = null;

            //Hide Menu
            UIMan.removeMenu();

            //Get rid of blue tiles
            MapMan.ClearSelection();

            autoClick = false;
        }
    }

    //For spoofing clicks for testing
    public void TestClick()
    {
        autoClick = true;
    }
}
