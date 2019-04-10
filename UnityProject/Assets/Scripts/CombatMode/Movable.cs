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
    private Vector3 target;
    private int depth;
    private bool inPlace;
    private Vector3 SmoothDampV;

    public float targetTolerance;
    public bool scriptEnabled;

    public Material hoverMaterial;
    public Material baseMaterial;

    private bool autoClick;

    // Use this for initialization
    void Start()
    {
        //ITS THE MAP MAN
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
        UIMan = GameObject.Find("UIManager").GetComponent<UIManager>();
        SoundMan = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();

        inPlace = false;

        depth = MapMan.tiles[(int)gameObject.transform.position.x, (int)gameObject.transform.position.z].getDepth();
    }

    // Update is called once per frame
    void Update()
    {
        //Logic for pretty falling animation
        //If we're not in position
        if (!(System.Math.Abs(transform.position.y - target.y) < targetTolerance))
        {
            //Set height based on depth (distance)
            if (depth != 0)
            {
                transform.position = Vector3.SmoothDamp(
                        transform.position, target, ref SmoothDampV, 0.1f * (depth * Random.Range(0.9f, 1.1f)));
            }
            else
            {
                transform.position = Vector3.SmoothDamp(
                        transform.position, target, ref SmoothDampV, 0.1f);
            }

            //Set alplha based on how clase we are
            gameObject.GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g,
                GetComponent<Renderer>().material.color.b, Mathf.Abs(target.y / transform.position.y) * baseMaterial.color.a);

            //For grid lines
            for(int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).GetComponent<Renderer>().material.color = 
                    new Color(gameObject.transform.GetChild(i).GetComponent<Renderer>().GetComponent<Renderer>().material.color.r,
                    gameObject.transform.GetChild(i).GetComponent<Renderer>().GetComponent<Renderer>().material.color.g,
                    gameObject.transform.GetChild(i).GetComponent<Renderer>().GetComponent<Renderer>().material.color.b, Mathf.Abs(target.y / transform.position.y) / 2);
            }
        }
        //When we arrive
        else
            inPlace = true;
    }

    //Getter for tiles
    private Tile[,] getTiles()
    {
        return MapMan.GetComponent<MapManager>().tiles;
    }

    
    //On mouse over for actually moveing
    public void OnMouseOver()
    {
        //Hover and change material (script enabled is a flag that is false for invalid move tiles (like the ones under friendly units))
        //Done so we can still use the pretty animations
        if (inPlace && scriptEnabled)
        {
            gameObject.GetComponent<Renderer>().material = hoverMaterial;
        }

        //When actually clicked
        if ((Input.GetMouseButtonDown(0) || autoClick) && inPlace && scriptEnabled)
        {

            HashSet<Tile> visited = new HashSet<Tile>();

            MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().Movement -= depth;

            //Move the selected unit
            MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().MoveTo(pos, MapMan.tiles);

            //Update dirction that they're facing
            MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().updateDirection();

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

    //Get inital height for animation
    public float getStartYvalue()
    {
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        depth = MapMan.tiles[(int)pos.x, (int)pos.y].getDepth();
        return Random.Range((2 * depth) + 2, (2 * depth) + 3);
    }

    //Set where we want to end up for animation
    public void setTarget(Vector3 target)
    {
        this.target = target;
    }
        
    //Hover material
    private void OnMouseEnter()
    {
        if (inPlace && scriptEnabled)
        {
            SoundMan.playUiHover();
            gameObject.GetComponent<Renderer>().material = hoverMaterial;
        }
    }

    //Return to regualr
    private void OnMouseExit()
    {
        if(inPlace && scriptEnabled)
            gameObject.GetComponent<Renderer>().material = baseMaterial;
    }

    //For spoofing clicks for testing
    public void TestClick()
    {
        autoClick = true;
    }
}
