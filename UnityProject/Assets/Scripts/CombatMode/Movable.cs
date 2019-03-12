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
        if (!(System.Math.Abs(transform.position.y - target.y) < targetTolerance))
        {
            transform.position = Vector3.SmoothDamp(
                    transform.position, target, ref SmoothDampV, 0.1f * (depth * Random.Range(0.9f, 1.1f)));

            gameObject.GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g,
                GetComponent<Renderer>().material.color.b, Mathf.Abs(target.y / transform.position.y) * baseMaterial.color.a);
            for(int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).GetComponent<Renderer>().material.color = 
                    new Color(gameObject.transform.GetChild(i).GetComponent<Renderer>().GetComponent<Renderer>().material.color.r,
                    gameObject.transform.GetChild(i).GetComponent<Renderer>().GetComponent<Renderer>().material.color.g,
                    gameObject.transform.GetChild(i).GetComponent<Renderer>().GetComponent<Renderer>().material.color.b, Mathf.Abs(target.y / transform.position.y) / 2);
            }
        }
        else
            inPlace = true;
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
        if (inPlace && scriptEnabled)
        {
            gameObject.GetComponent<Renderer>().material = hoverMaterial;
        }
        if ((Input.GetMouseButtonDown(0) || autoClick) && inPlace && scriptEnabled)
        {

            HashSet<Tile> visited = new HashSet<Tile>();

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

    public float getStartYvalue()
    {
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        depth = MapMan.tiles[(int)pos.x, (int)pos.y].getDepth();
        return Random.Range((2 * depth) + 2, (2 * depth) + 3);
    }

    public void setTarget(Vector3 target)
    {
        this.target = target;
    }
        

    private void OnMouseEnter()
    {
        if (inPlace && scriptEnabled)
        {
            SoundMan.playUiHover();
            gameObject.GetComponent<Renderer>().material = hoverMaterial;
        }
    }

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
