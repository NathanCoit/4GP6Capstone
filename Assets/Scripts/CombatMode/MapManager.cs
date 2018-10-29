using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Tile[,] tiles = new Tile[10,10];

    private List<GameObject> Movable;
    private GameObject SelectionIndicator;

    public GameObject Unit;
    public GameObject MovableTile;
    public GameObject Selected;
    public bool newSelected = false;

    // Use this for initialization
    void Start ()
    {
        //Initiliaze Grid
        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
            {
                tiles[x, y] = new Tile(new Vector3(x, 0, y));
            }

        //Define Connections
        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
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

        if(Selected != null && newSelected)
        {
            ClearSelection();
            Destroy(SelectionIndicator);
            SelectionIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            SelectionIndicator.transform.position = new Vector3(Selected.transform.position.x, Selected.transform.position.y + 2, Selected.transform.position.z);
            Movable = new List<GameObject>();
            HashSet<Tile> MovableTiles = tiles[(int)Selected.GetComponent<Units>().getPos().x, (int)Selected.GetComponent<Units>().getPos().y].findAtDistance(2);
            foreach (Tile t in MovableTiles)
            {
                GameObject temp = Instantiate(MovableTile);
                temp.GetComponent<Movable>().pos = new Vector2((int)t.getX(), (int)t.getZ());
                temp.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2, t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);
                temp.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
                Movable.Add(temp);
            }
            newSelected = false;
            
        }


    }

    public void ClearSelection()
    {
        foreach (GameObject g in Movable)
            Destroy(g);
        Destroy(SelectionIndicator);
    }
}

public class Tile
{
    private Vector3 pos;

    private List<Tile> Connected = new List<Tile>();

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
    public HashSet<Tile> findAtDistance(int distance)
    {
        HashSet<Tile> visited = new HashSet<Tile>();
        Queue<Tile> queue = new Queue<Tile>();
        Tile root = this;

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
            visited.Add(current);
            nextElementsToDepthIncrease += current.getConnected().Count;

            
            if (--elementsToNextDepth == 0)
            {
                if (++currentDepth > distance)
                    return visited;
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

/*
public class Unit
{
    private GameObject untiGO;
    private Vector2 pos;

    public Unit(Vector2 pos, Tile[,] tiles)
    {
        this.pos = pos;
        untiGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        untiGO.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        this.Draw(tiles);
    }

    public void Draw(Tile[,] tiles)
    {
        //Centers Unit on tile (I know it looks ugly but it SHOULD work for any model)
        untiGO.transform.position = new Vector3(tiles[(int)pos.x, (int)pos.y].getX() + ((1 - untiGO.transform.lossyScale.x) / 2) + untiGO.transform.lossyScale.x / 2, tiles[(int)pos.x, (int)pos.y].getY() + untiGO.transform.lossyScale.y + 0.5f, tiles[(int)pos.x, (int)pos.y].getZ() + ((1 - untiGO.transform.lossyScale.z) / 2) + untiGO.transform.lossyScale.x / 2);
    }

    public void Move(Vector2 amount, Tile[,] tiles)
    {
        //Check array bounds on tiles
        if(pos.x + amount.x < tiles.GetLength(0) && pos.y + amount.y < tiles.GetLength(1))
            if(pos.x + amount.x >= 0 && pos.y + amount.y >= 0)
            this.pos += amount;

        this.Draw(tiles);
    }
}
*/