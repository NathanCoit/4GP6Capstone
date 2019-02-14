using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Concerned with the actual map (tiles) of combat mode.
 * Involves:
 *      - connections between tiles
 *      - which tiles a Unit can move onto
 *      - who is selected? in terms of units/worshippers/gods (helpful when trying to decide who is attacking or whose menu to bring up)
 */
public class MapManager : MonoBehaviour
{
    public Tile[,] tiles;

    private List<GameObject> Movable;
    private GameObject SelectionIndicator;

    public GameObject Unit;
    public GameObject MovableTile;
    public GameObject Selected;
    private GameObject BoardMan;
    private GameObject previousSelected;
    public bool newSelected = false;
    public string mapName;
    public float godFloatHeight;

    // Use this for initialization. We're loading our maps from a txt and building them here so we can easily add more maps without the need for more scenes.
    void Start ()
    {
        //If we don't have a specified map, it's testmap
        if (mapName == "")
            mapName = "testMap";

        TextAsset map = Resources.Load("CMaps/" + mapName) as TextAsset;

        //Splitting on newline from https://stackoverflow.com/questions/1547476/easiest-way-to-split-a-string-on-newlines-in-net
        string[] lines = map.text.Split(
        new[] { "\r\n", "\r", "\n" },
        System.StringSplitOptions.None);

        tiles = new Tile[lines[0].Split('-').Length, lines.Length];

        //Initiliaze Grid
        for (int y = 0; y < lines.Length; y++)
        {
            //Read back to front to match how it looks in the text file
            string[] chars = lines[(lines.Length - 1) - y].Split('-');

            for(int x = 0; x < chars.Length; x++)
            {
                tiles[x, y] = new Tile(new Vector3(x, 0, y), chars[x]);
            }
        }

        //Made as a seperate function as we need to do it a lot
        DefineConnections();

        Movable = new List<GameObject>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager");

        //How high gods float above the map
        godFloatHeight = 3.5f;

    }

	// Update is called once per frame
	void Update ()
    {

        //For Selected Unit
        if(Selected != null && newSelected)
        {
            //Clean up Previous selection
            ClearSelection();

            if(previousSelected != null)
                previousSelected.transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(false);


            //Show Menu
            if(!Selected.GetComponent<Units>().CheckIfGod())
                Selected.transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(true);
            else if(Selected.GetComponent<Gods>().isInBattle())
                Selected.transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(true);
            else if(!Selected.GetComponent<Gods>().isInBattle())
                Selected.transform.GetChild(1).GetComponent<Canvas>().gameObject.SetActive(true);

            previousSelected = Selected;

            //So we don't do this every frame
            newSelected = false;
            
        }


    }

    //Define which tiles are connected to which tiles
    public void DefineConnections()
    {
        //Define Connections
        for (int x = 0; x < tiles.GetLength(0); x++)
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                Tile temp = tiles[x, y];
                List<Tile> tempConnections = new List<Tile>();
                if (y < tiles.GetLength(1) - 1 && tiles[x, y + 1].isTraversable())
                {
                    tempConnections.Add(tiles[x, y + 1]);
                }
                if (x < tiles.GetLength(0) - 1 && tiles[x + 1, y].isTraversable())
                {
                    tempConnections.Add(tiles[x + 1, y]);
                }
                if (y > 0 && tiles[x, y - 1].isTraversable())
                {
                    tempConnections.Add(tiles[x, y - 1]);
                }
                if (x > 0 && tiles[x - 1, y].isTraversable())
                {
                    tempConnections.Add(tiles[x - 1, y]);
                }
                temp.updateConnections(tempConnections);
                tiles[x, y] = temp;
            }
    }

    //Cleans all the tiles (the ones that go on top of the actual tiles (the interactable ones))
    public void ClearSelection()
    {
        //Clean up tiles
        GameObject[] movableTiles = GameObject.FindGameObjectsWithTag("MoveableTile");
        for (int i = 0; i < movableTiles.GetLength(0); i++)
            Destroy(movableTiles[i]);

        GameObject[] attackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile");
        for (int i = 0; i < attackableTiles.GetLength(0); i++)
            Destroy(attackableTiles[i]);

        GameObject[] TargetableTiles = GameObject.FindGameObjectsWithTag("TargetableTile");
        for (int i = 0; i < TargetableTiles.GetLength(0); i++)
            Destroy(TargetableTiles[i]);
    }

    //For making the gameObject of a tile (the real ones)
    public void InstantiateTile(string typeID, Vector3 pos)
    {
        GameObject tileGameObject = Instantiate(Resources.Load("Tiles/" + typeID) as GameObject);

        //Cenetering
        tileGameObject.transform.position 
            = new Vector3(pos.x + ((1 - tileGameObject.transform.lossyScale.x) / 2) + tileGameObject.transform.lossyScale.x / 2,
            0,
            pos.z + ((1 - tileGameObject.transform.lossyScale.z) / 2) + tileGameObject.transform.lossyScale.x / 2);
    }
}