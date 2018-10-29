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

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Move the selected unit
            MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().MoveTo(pos, MapMan.GetComponent<MapManager>().tiles);

            //End unit's action
            MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().EndAct();

            BoardMan.GetComponent<BoardManager>().DecreaseNumActions();

            //Unselect it
            MapMan.GetComponent<MapManager>().Selected = null;

            //Get rid of blue tiles
            MapMan.GetComponent<MapManager>().ClearSelection();
        }

    }
}
