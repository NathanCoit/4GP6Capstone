using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour {

    public Vector2 pos;
    private GameObject MapMan;

    // Use this for initialization
    void Start ()
    {
        //ITS THE MAP MAN
        MapMan = GameObject.FindGameObjectWithTag("MapManager");
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

            //Unselect it
            MapMan.GetComponent<MapManager>().Selected = null;

            //Get rid of blue tiles
            MapMan.GetComponent<MapManager>().ClearSelection();
        }

    }
}
