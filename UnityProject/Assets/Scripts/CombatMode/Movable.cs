using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script associated with the Movable tiles (the blue tiles where worshippers can move). Pretty self explanatory.
 */
public class Movable : MonoBehaviour {

    public Vector2 pos;
    private GameObject MapMan;
    private GameObject BoardMan;

    private bool autoClick;

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

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) || autoClick)
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



            //Unselect it
            MapMan.GetComponent<MapManager>().Selected = null;

            //Get rid of blue tiles
            MapMan.GetComponent<MapManager>().ClearSelection();

            autoClick = false;
        }
    }

    //For spoofing clicks for testing
    public void testClick()
    {
        autoClick = true;
    }
}
