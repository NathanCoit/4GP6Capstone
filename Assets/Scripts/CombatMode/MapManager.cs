using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Tile[,] tiles = new Tile[10,10];

    private List<GameObject> Movable;
    private GameObject SelectionIndicator;
    public int tileX = 10;
    public int tileY = 10;

    public GameObject Unit;
    public GameObject MovableTile;
    public GameObject Selected;
    private GameObject BoardMan;
    private GameObject previousSelected;
    public bool newSelected = false;

    // Use this for initialization
    void Start ()
    {
        //Initiliaze Grid
        for (int x = 0; x < tileX; x++)
            for (int y = 0; y < tileY; y++)
            {
                tiles[x, y] = new Tile(new Vector3(x, 0, y));
            }

        //Define Connections
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

        Movable = new List<GameObject>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager");

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

            //Setup Invalid Tiles (the one with units on)
            List<GameObject> invalidTiles = new List<GameObject>(BoardMan.GetComponent<BoardManager>().enemyUnits);
            invalidTiles.AddRange(BoardMan.GetComponent<BoardManager>().playerUnits);
            invalidTiles.Remove(Selected);

            //Show Menu
            Selected.transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(true);

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

    public void ClearSelection()
    {
        //Clean up tiles
        GameObject[] movableTiles = GameObject.FindGameObjectsWithTag("MoveableTile");
        for (var i = 0; i < movableTiles.GetLength(0); i++)
            Destroy(movableTiles[i]); ;
    }
}

public class Tile
{
    private Vector3 pos;

    private List<Tile> Connected = new List<Tile>();

    private List<int> depths = new List<int>();

    private HashSet<Tile> visited = new HashSet<Tile>();

    public Tile(Vector3 pos)
    {
        this.pos = pos;
    }

    public void updateConnections (List<Tile> newConnections)
    {
        Connected = newConnections;
    }

    public void showConnected(Tile[,] tiles)
    {
        for(int i = 0; i < Connected.Count; i++)
        {
            //new Unit(new Vector2(Connected[i].getX(), Connected[i].getZ()), tiles);
        }
    }

    //Code from https://stackoverflow.com/questions/10258305/how-to-implement-a-breadth-first-search-to-a-certain-depth
    public HashSet<Tile> findAtDistance(int distance, List<GameObject> invalidTiles, Tile[,] tiles)
    {
        visited = new HashSet<Tile>();
        depths = new List<int>();
        Queue<Tile> queue = new Queue<Tile>();
        List<Tile> toBeRemoved = new List<Tile>();
        Tile root = this;

        //Removing invalid tiles connections
        foreach(Tile t in tiles)
        {
            foreach (GameObject g in invalidTiles)
                if (g.GetComponent<Units>().getPos() == new Vector2(t.getX(), t.getZ()))
                {
                    t.Connected = new List<Tile>();
                    foreach (Tile t1 in tiles)
                        if (t1.Connected.Contains(t))
                            t1.Connected.Remove(t);
                }
        }

        int currentDepth = 0;
        int elementsToNextDepth = 1;
        int nextElementsToDepthIncrease = 0;

        queue.Enqueue(root);

        while(queue.Count > 0)
        {
            Tile current = queue.Dequeue();


            //Jumps to next itereation if we've already visited
            //if (visited.Contains(current))
            //continue;


            //If we have not visited, add to visited
            if(!visited.Contains(current))
                depths.Add(currentDepth);
            visited.Add(current);
            
            nextElementsToDepthIncrease += current.getConnected().Count;

            
            if (--elementsToNextDepth == 0)
            {
                if (++currentDepth > distance)
                {
                    /*
                    foreach (GameObject g in opposingTeam)
                    {
                        foreach (Tile t in visited)
                            if (g.GetComponent<Units>().getPos() == new Vector2(t.getX(), t.getZ()))
                                toBeRemoved.Add(t);

                    }
                    foreach (Tile t in toBeRemoved)
                        visited.Remove(t);
                    */
                    return visited;
                }
                elementsToNextDepth = nextElementsToDepthIncrease;
                nextElementsToDepthIncrease = 0;
            }


            foreach (Tile connect in current.getConnected())
            {
                //if (!visited.Contains(connect))
                    queue.Enqueue(connect);
            }
        }
        return visited;
    }

    public List<int> getDepths()
    {
        return depths;
    }

    public HashSet<Tile> getVisited()
    {
        return visited;
    }

    public float getX()
    {
        return pos.x;
    }

    public float getY()
    {
        return pos.y;
    }

    public float getZ()
    {
        return pos.z;
    }

    public List<Tile> getConnected()
    {
        return Connected;
    }

}