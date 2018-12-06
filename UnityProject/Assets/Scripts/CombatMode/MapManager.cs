using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Use this for initialization
    void Start ()
    {
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

        DefineConnections();

        Movable = new List<GameObject>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager");

        godFloatHeight = 3.5f;

        /*
        foreach (Tile t in tiles[5, 5].findAtDistance(2))
        {
            GameObject temp = Instantiate(Unit);
            temp.GetComponent<Units>().Move(new Vector2(t.getX(), t.getZ()), tiles);
        }
        */
    }

	// Update is called once per frame
	void Update ()
    {
        /*
        //tiles[5, 5].showConnected(tiles);
        if (Input.GetKeyDown("w"))
        {
            //test.Move(new Vector2(0, 1), tiles);
        }
        if (Input.GetKeyDown("s"))
        {
            //test.Move(new Vector2(0, -1), tiles);
        }
        if (Input.GetKeyDown("a"))
        {
            //test.Move(new Vector2(-1, 0), tiles);
        }
        if (Input.GetKeyDown("d"))
        {
            //test.Move(new Vector2(1, 0), tiles);
        }
        */

        //For Selected Unit

        if(Selected != null && newSelected)
        {
            //Clean up Previous selection
            ClearSelection();
            //Destroy(SelectionIndicator);
            if(previousSelected != null)
                previousSelected.transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(false);

            //Selection Indicator
            //SelectionIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //SelectionIndicator.transform.position = new Vector3(Selected.transform.position.x, Selected.transform.position.y + 2, Selected.transform.position.z);
            //Movable = new List<GameObject>();



            //Show Menu
            if(!Selected.GetComponent<Units>().CheckIfGod())
                Selected.transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(true);
            else if(Selected.GetComponent<Gods>().isInBattle())
                Selected.transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(true);
            else if(!Selected.GetComponent<Gods>().isInBattle())
                Selected.transform.GetChild(1).GetComponent<Canvas>().gameObject.SetActive(true);

            /*
            //Calculate Movable Tiles
            MovableTiles = tiles[(int)Selected.GetComponent<Units>().getPos().x, (int)Selected.GetComponent<Units>().getPos().y].findAtDistance(2, invalidTiles, tiles);

            //Restore Connections
            for (int x = 0; x < tileX; x++)
                for (int y = 0; y < tileY; y++)
                {
                    Tile temp = tiles[x, y];
                    List<Tile> tempConnections = new List<Tile>();
                    if (y < tiles.GetLength(1) - 1)
                        tempConnections.Add(tiles[x, y + 1]);
                    if (x < tiles.GetLength(0) - 1)
                        tempConnections.Add(tiles[x + 1, y]);
                    if (y > 0)
                        tempConnections.Add(tiles[x, y - 1]);
                    if (x > 0)
                        tempConnections.Add(tiles[x - 1, y]);
                    temp.updateConnections(tempConnections);
                    tiles[x, y] = temp;

                }

            //Draw movable tiles
            foreach (Tile t in MovableTiles)
            {
                GameObject temp = Instantiate(MovableTile);
                temp.GetComponent<Movable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
                temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
                temp.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
                Movable.Add(temp);
            }
            */

            previousSelected = Selected;

            //So we don't do this every frame
            newSelected = false;
            
        }


    }

    public void DefineConnections()
    {
        //Define Connections
        for (int x = 0; x < tiles.GetLength(0); x++)
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                Tile temp = tiles[x, y];
                List<Tile> tempConnections = new List<Tile>();
                if (y < tiles.GetLength(1) - 1 && tiles[x, y + 1].isTraversable())
                    tempConnections.Add(tiles[x, y + 1]);
                if (x < tiles.GetLength(0) - 1 && tiles[x + 1, y].isTraversable())
                    tempConnections.Add(tiles[x + 1, y]);
                if (y > 0 && tiles[x, y - 1].isTraversable())
                    tempConnections.Add(tiles[x, y - 1]);
                if (x > 0 && tiles[x - 1, y].isTraversable())
                    tempConnections.Add(tiles[x - 1, y]);
                temp.updateConnections(tempConnections);
                tiles[x, y] = temp;
            }
    }

    public void ClearSelection()
    {
        //Clean up tiles
        GameObject[] movableTiles = GameObject.FindGameObjectsWithTag("MoveableTile");
        for (var i = 0; i < movableTiles.GetLength(0); i++)
            Destroy(movableTiles[i]);

        GameObject[] attackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile");
        for (var i = 0; i < attackableTiles.GetLength(0); i++)
            Destroy(attackableTiles[i]);

        GameObject[] TargetableTiles = GameObject.FindGameObjectsWithTag("TargetableTile");
        for (var i = 0; i < TargetableTiles.GetLength(0); i++)
            Destroy(TargetableTiles[i]);
    }

    //For making the gameObject of a tile
    public void InstantiateTile(string typeID, Vector3 pos)
    {
        GameObject temp = Instantiate(Resources.Load("Tiles/" + typeID) as GameObject);

        //Cenetering
        temp.transform.position = new Vector3(pos.x + ((1 - temp.transform.lossyScale.x) / 2) + temp.transform.lossyScale.x / 2, 0, pos.z + ((1 - temp.transform.lossyScale.z) / 2) + temp.transform.lossyScale.x / 2);
    }
}