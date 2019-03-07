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
    private SoundManager SoundMan;

    public Material hoverMaterial;
    public Material baseMaterial;

    private bool autoClick;

    // Use this for initialization
    void Start ()
    {
        //ITS THE MAP MAN
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
        UIMan = GameObject.Find("UIManager").GetComponent<UIManager>();
        SoundMan = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
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

            Camera.main.GetComponent<CombatCam>().lookAt(MapMan.Selected.transform.position);

            //Case where we dont have a god entering battle
            if (!UIMan.godEnteringBattle)
            {
                if (MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().isPlayer)
                    UIMan.showMenuIfCanAct();
            }
            //Case where we do
            else
            {
                MapMan.Selected.transform.GetChild(0).gameObject.SetActive(true);
                UIMan.makePanel(UIMan.unitPanel);
                UIMan.makeUnitButtons();
                UIMan.makeEndTurnButton();
                UIMan.godEnteringBattle = false;
                SoundMan.playGodEnterBattle(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit());
            }

            SoundMan.playUnitMove();

            autoClick = false;
        }
    }

    private void OnMouseEnter()
    {
        SoundMan.playUiHover();
        gameObject.GetComponent<Renderer>().material = hoverMaterial;
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<Renderer>().material = baseMaterial;
    }

    //For spoofing clicks for testing
    public void TestClick()
    {
        autoClick = true;
    }
}
