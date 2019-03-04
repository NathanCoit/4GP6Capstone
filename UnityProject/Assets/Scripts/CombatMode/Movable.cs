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
            int depth = 0;
            HashSet<Tile> visited = new HashSet<Tile>();
            int j = 0;


            depth = tiles[(int)gameObject.transform.position.x, (int)gameObject.transform.position.z].getDepth();

            MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().Movement -= depth;

            //Move the selected unit
            MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().MoveTo(pos, MapMan.tiles);

            //Get rid of blue tiles
            MapMan.ClearSelection();

            //Show menu if we can still act, otherwise hide it
            if (!UIMan.godEnteringBattle)
            {
                if (MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().isPlayer)
                    UIMan.showMenuIfCanAct();
            }
            else
            {
                UIMan.makePanel(UIMan.unitPanel);
                UIMan.makeUnitButtons();
                UIMan.makeEndTurnButton();
                UIMan.godEnteringBattle = false;
            }

            

            autoClick = false;
        }
    }

    //For spoofing clicks for testing
    public void TestClick()
    {
        autoClick = true;
    }
}
