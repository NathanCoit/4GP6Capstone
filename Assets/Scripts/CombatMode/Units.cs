using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Units : MonoBehaviour {

    public bool canAct;

    private GameObject MapMan;
    private Vector2 pos;
    private GameObject BoardMan;
    private List<int> depths = new List<int>();
    private HashSet<Tile> visited = new HashSet<Tile>();


    public GameObject MovableTile;
    public GameObject AttackableTile;
    public int Movement = 2;
    public float morale;

    //For use without models, can be removed later
    public Material Available;
    public Material NotAvailable;
    public int WorshiperCount;
    public float AttackStrength;


    private bool autoClick = false;

    // Use this for initialization
    void Start()
    {
        //You know who to call, ITS MAP MAN!
        MapMan = GameObject.FindGameObjectWithTag("MapManager");

        BoardMan = GameObject.FindGameObjectWithTag("BoardManager");

        //AllowAct(); //this actually broke it for the longest time but i FOUND IT 
        GetComponent<MeshRenderer>().material = NotAvailable;

        AttackStrength = WorshiperCount * 0.25f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Draw(Tile[,] tiles)
    {
        //Centers Unit on tile (I know it looks ugly but it SHOULD work for any model)
        transform.position = new Vector3(tiles[(int)pos.x, (int)pos.y].getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, tiles[(int)pos.x, (int)pos.y].getY() + transform.lossyScale.y + 0.5f, tiles[(int)pos.x, (int)pos.y].getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
    }

    public void Move(Vector2 amount, Tile[,] tiles)
    {
        //Check array bounds on tiles
        if (pos.x + amount.x < tiles.GetLength(0) && pos.y + amount.y < tiles.GetLength(1))
            if (pos.x + amount.x >= 0 && pos.y + amount.y >= 0)
                this.pos += amount;

        this.Draw(tiles);
    }

    public void MoveTo(Vector2 pos, Tile[,] tiles)
    {
        this.pos = pos;

        this.Draw(tiles);
    }

    public Vector2 getPos()
    {
        return pos;
    }

    public bool HasAct()
    {
        return canAct;
    }

    public int getWorshiperCount()
    {
        return WorshiperCount;
    }

    public void setWorshiperCount(int count)
    {
        WorshiperCount = count;
    }

    public float getMorale()
    {
        return morale;
    }
    public void setMorale(float mor)
    {
        morale = mor;
    }

    public float getAttackStrength()
    {
        return AttackStrength;
    }

    public void updateAttackStrength()
    {
        AttackStrength = AttackStrength * morale;
    }

    public void AllowAct() //this Unit has not yet acted in this round
    {
        canAct = true;

        //Render stuff for use without proper models, can be removed later
        GetComponent<MeshRenderer>().material = Available;
    }

    public void EndAct() //this Unit has completed their allotted actions in this round
    {
        canAct = false;

        //Render stuff for use without proper models, can be removed later
        GetComponent<MeshRenderer>().material = NotAvailable;
        transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(false);

    }

    public void EndTurnButton()
    {
        //check if somebody won
        if (BoardMan.GetComponent<BoardManager>().playerUnits.Count == 0)
        {
            BoardMan.GetComponent<BoardManager>().Defeat();
        }
        else if (BoardMan.GetComponent<BoardManager>().enemyUnits.Count == 0)
        {
            BoardMan.GetComponent<BoardManager>().Victory();
        }

        EndAct();
        BoardMan.GetComponent<BoardManager>().DecreaseNumActions();
    }

    public void OnMouseOver()
    {
        if ((Input.GetMouseButtonDown(0) || autoClick) && canAct)
        {
            MapMan.GetComponent<MapManager>().Selected = this.gameObject;
            MapMan.GetComponent<MapManager>().newSelected = true;
            autoClick = false;
        }
            
    }

    //For spoofing clicks for testing
    public void testClick()
    {
        autoClick = true;
    }

    private Tile[,] getTiles()
    {
        return MapMan.GetComponent<MapManager>().tiles;
    }

    public void showMovable()
    {
        Tile[,] tiles = getTiles();

        HashSet<Tile> MovableTiles = new HashSet<Tile>();

        //Setup Invalid Tiles (the one with units on)
        List<GameObject> invalidTiles = new List<GameObject>(BoardMan.GetComponent<BoardManager>().enemyUnits);
        invalidTiles.AddRange(BoardMan.GetComponent<BoardManager>().playerUnits);
        invalidTiles.Remove(this.gameObject);

        //Calculate Movable Tiles
        MovableTiles = tiles[(int)getPos().x, (int)getPos().y].findAtDistance(Movement, invalidTiles, tiles);


        //Restore Connections
        for (int x = 0; x < tiles.GetLength(0); x++)
            for (int y = 0; y < tiles.GetLength(1); y++)
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
        MapMan.GetComponent<MapManager>().ClearSelection();

        foreach (Tile t in MovableTiles)
        {
            GameObject temp = Instantiate(MovableTile);
            temp.GetComponent<Movable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
            temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
            //temp.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
            //Movable.Add(temp);
        }
    }

    public void showAttackable()
    {
        Tile[,] tiles = getTiles();
        HashSet<Tile> AttackableTiles = new HashSet<Tile>();
        List<Tile> ConnectedTiles = tiles[(int)getPos().x, (int)getPos().y].getConnected();
        List<GameObject> targets = new List<GameObject>();
        if (BoardMan.GetComponent<BoardManager>().playerUnits.Contains(MapMan.GetComponent<MapManager>().Selected))
        {
            targets = BoardMan.GetComponent<BoardManager>().enemyUnits;
        }
        else
        {
            targets = BoardMan.GetComponent<BoardManager>().playerUnits;
        }

        foreach (Tile t in ConnectedTiles)
            foreach (GameObject g in targets)
                if (new Vector2(t.getX(), t.getZ()) == g.GetComponent<Units>().getPos())
                    AttackableTiles.Add(t);


        //Draw movable tiles
        MapMan.GetComponent<MapManager>().ClearSelection();

        foreach (Tile t in AttackableTiles)
        {
            GameObject temp = Instantiate(AttackableTile);
            temp.GetComponent<Attackable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
            temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
            //temp.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
            //Movable.Add(temp);
        }

    }

}
