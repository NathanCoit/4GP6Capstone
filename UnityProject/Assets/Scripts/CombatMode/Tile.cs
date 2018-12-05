﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public int MovePriority;

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
    public HashSet<Tile> findAtDistance(Tile start, int distance, List<GameObject> invalidTiles, List<GameObject> solidTiles, Tile[,] tiles)
    {
        visited = new HashSet<Tile>();
        depths = new List<int>();
        Queue<Tile> queue = new Queue<Tile>();
        List<Tile> toBeRemoved = new List<Tile>();
        Tile root = start;

        //Removing solid tiles connections
        foreach (Tile t in tiles)
        {
            foreach (GameObject g in solidTiles)
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
                    foreach (GameObject g in invalidTiles)
                        visited.Remove(tiles[(int)g.GetComponent<Units>().getPos().x, (int)g.GetComponent<Units>().getPos().y]);
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
        foreach (GameObject g in invalidTiles)
            visited.Remove(tiles[(int)g.GetComponent<Units>().getPos().x, (int)g.GetComponent<Units>().getPos().y]);
        return visited;
    }


    public Tile getClosestTile(List<GameObject> endingPoints, int range, List<GameObject> invalidTiles, List<GameObject> solidTiles, Tile[,] tiles)
    {
        List<Tile> MovableTiles = findAtDistance(this, range, invalidTiles, new List<GameObject>(), tiles).ToList();
        HashSet<Tile> TargetTiles = new HashSet<Tile>();

        foreach (Tile t in tiles)
        {
            foreach (GameObject g in endingPoints)
                if (g.GetComponent<Units>().getPos() == new Vector2(t.getX(), t.getZ()))
                {
                    foreach(Tile ta in t.getConnected())
                        TargetTiles.Add(ta);
                }
        }

        //Remove Invalid Target Tiles
        foreach (Tile t in tiles)
        {
            foreach (GameObject g in invalidTiles)
                if (g.GetComponent<Units>().getPos() == new Vector2(t.getX(), t.getZ()))
                {
                    TargetTiles.Remove(t);
                }
            foreach (GameObject s in solidTiles)
                if (s.GetComponent<Units>().getPos() == new Vector2(t.getX(), t.getZ()))
                {
                    TargetTiles.Remove(t);
                }
        }

        //If we're already there
        if (TargetTiles.Contains(this) || TargetTiles.Count == 0)
        {
            MovePriority = 0;
            return null;
        }

        //If we're in range
        foreach (Tile t in MovableTiles)
        {
            foreach (Tile ta in TargetTiles)
                if (t == ta)
                {
                    MovePriority = 1;
                    return t;
                }
        }

        //If we're not in range
        MovableTiles.Remove(this);

        bool found = false;
        int newRange = 1;
        HashSet<Tile> newTiles;

        while(!found)
        {
            newTiles = new HashSet<Tile>();
            foreach(Tile tm in MovableTiles)
            {
                newTiles = findAtDistance(tm, newRange, new List<GameObject>(), new List<GameObject>(), tiles);
                
                foreach(Tile t in newTiles)
                {
                    foreach (Tile ta in TargetTiles)
                        if (t == ta)
                        {
                            MovePriority = 2;
                            return tm;
                        }
                }

            }
            newRange++;
            if (newRange > 100)
            {
                Debug.Log("Nothing Found Boss");
                return null;
            }
        }
        return null;
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
