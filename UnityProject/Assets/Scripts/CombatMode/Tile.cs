using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private Vector3 pos;
    private List<Tile> Connected = new List<Tile>();
    private List<int> depths = new List<int>();
    private HashSet<Tile> visited = new HashSet<Tile>();
    private string typeID;
    private string type;
    private bool traversable;
    private MapManager Mapman;

    public Tile(Vector3 pos, string typeID)
    {
        this.pos = pos;
        this.typeID = typeID;
        Mapman = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        Mapman.InstantiateTile(typeID, pos);

        switch (typeID)
        {
            //Air
            case "00":
                traversable = false;
                break;
            //Grass
            case "01":
                traversable = true;
                break;
            //Water
            case "02":
                traversable = false;
                break;

        }
    }

    public void updateConnections(List<Tile> newConnections)
    {
        Connected = newConnections;
    }

    public void showConnected(Tile[,] tiles)
    {
        for (int i = 0; i < Connected.Count; i++)
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
        foreach (Tile t in tiles)
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

        while (queue.Count > 0)
        {
            Tile current = queue.Dequeue();


            //Jumps to next itereation if we've already visited
            //if (visited.Contains(current))
            //continue;


            //If we have not visited, add to visited
            if (!visited.Contains(current))
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

    public bool isTraversable()
    {
        return traversable;
    }

    public List<Tile> getConnected()
    {
        return Connected;
    }

}
