using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Basic building block of the game board.
 * Can be a tile that can be traversed onto or impassable terrain (like water or a mountain).
 */ 
public class Tile
{
    private Vector3 pos;
    private List<Tile> Connected = new List<Tile>();
    private int depth;
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
        MovePriority = 0;
        Mapman = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        Mapman.InstantiateTile(typeID, pos);

        //Defining tile type
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
            //Tree
            case "03":
                traversable = false;
                break;
            //Dirt
            case "04":
                traversable = true;
                break;
            //Rock
            case "05":
                traversable = false;
                break;
            //Lily Pad
            case "06":
                traversable = false;
                break;
           //Countertop
            case "07":
                traversable = true;
                break;
            //Pink cloud
            case "08":
                traversable = true;
                break;
            //Metal
            case "09":
                traversable = true;
                break;
            //Energy
            case "10":
                traversable = false;
                break;
            //Weapons
            case "11":
                traversable = false;
                break;
            //Lava
            case "12":
                traversable = false;
                break;
            //Darker Dirt
            case "13":
                traversable = true;
                break;
            //Passable Lily Pad Water
            case "14":
                traversable = true;
                break;
            //Cobblestone rock
            case "15":
                traversable = true;
                break;
        }
    }

    //Updates the connects (what tiles a tile is connected to)
    public void updateConnections(List<Tile> newConnections)
    {
        Connected = newConnections;
    }

    //Code (partially) from https://stackoverflow.com/questions/10258305/how-to-implement-a-breadth-first-search-to-a-certain-depth
    //This is how we figure our what tiles Units can move to or attack.
    public HashSet<Tile> findAtDistance(Tile start, int distance, List<Tile> invalidTiles, List<Tile> solidTiles, Tile[,] tiles)
    {
        visited = new HashSet<Tile>();
        depth = 0;
        Queue<Tile> queue = new Queue<Tile>();
        List<Tile> toBeRemoved = new List<Tile>();
        Tile root = start;

        //Removing solid tiles connections
        foreach (Tile t in tiles)
        {
            foreach (Tile u in solidTiles)
                if (u.pos == new Vector3(t.getX(), 0, t.getZ()))
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
            //If we have not visited, add to visited
            if (!visited.Contains(current))
                current.depth = currentDepth;
            visited.Add(current);

            nextElementsToDepthIncrease += current.getConnected().Count;


            if (--elementsToNextDepth == 0)
            {
                if (++currentDepth > distance)
                {

                    foreach (Tile t in invalidTiles)
                        //-1 is a special position for gods that are not in battle
                        if(t.getX() != -1 && t.getZ() != -1)
                            visited.Remove(tiles[(int)t.getX(), (int)t.getZ()]);
                            
                    return visited;
                }
                elementsToNextDepth = nextElementsToDepthIncrease;
                nextElementsToDepthIncrease = 0;
            }


            foreach (Tile connect in current.getConnected())
            {
                queue.Enqueue(connect);
            }
        }

        foreach (Tile t in invalidTiles)
            if (t.getX() != -1 && t.getZ() != -1)
                visited.Remove(tiles[(int)t.getX(), (int)t.getZ()]);

        return visited;
        
    }

    //Finds the optimal tile to move towards any tile in endingPoints (this is how the AI knows where to move)
    public Tile getClosestTile(List<Tile> endingPoints, int range, List<Tile> invalidTiles, List<Tile> solidTiles, Tile[,] tiles)
    {
        List<Tile> MovableTiles = findAtDistance(this, range, invalidTiles, new List<Tile>(), tiles).ToList();
        HashSet<Tile> TargetTiles = new HashSet<Tile>();

        //Figure out where we're trying to go (the tiles next to player units)
        foreach (Tile t in tiles)
        {
            foreach (Tile u in endingPoints)
                if (u.pos == new Vector3(t.getX(), 0, t.getZ()))
                {
                    foreach(Tile ta in t.getConnected())
                        TargetTiles.Add(ta);
                }
        }

        //Remove Invalid Target Tiles (either invalid(friendly unit already there) or solid(enemy unit already there)
        foreach (Tile t in tiles)
        {
            foreach (Tile u in invalidTiles)
                if (u.pos == new Vector3(t.getX(), 0, t.getZ()))
                {
                    TargetTiles.Remove(t);
                }
            foreach (Tile s in solidTiles)
                if (s.pos == new Vector3(t.getX(), 0, t.getZ()))
                {
                    TargetTiles.Remove(t);
                }
        }

        //Removing solid tiles connections (so we dont move through them)
        //This could be done in findAtDistance, but we need those connections to exist when we find target tiles so we do it here
        foreach (Tile t in tiles)
        {
            foreach (Tile u in solidTiles)
                if (u.pos == new Vector3(t.getX(), 0, t.getZ()))
                {
                    t.Connected = new List<Tile>();
                    foreach (Tile t1 in tiles)
                        if (t1.Connected.Contains(t))
                            t1.Connected.Remove(t);
                }
        }

        //Note we keep track of priority (and actually call this function to update it)
        //This is to avoid the AI making dumbs moves and ruining oppertunities to attack

        //If we're already there or there is no available TargetTiles (all taken up by team mates)
        if (TargetTiles.Contains(this) || TargetTiles.Count == 0)
        {
            MovePriority = 0;
            return null;
        }

        //If we're in range (move then attack)
        foreach (Tile t in MovableTiles)
        {
            foreach (Tile ta in TargetTiles)
                if (t == ta)
                {
                    MovePriority = 1;
                    return t;
                }
        }

        //If we're not in range (find the closest tile we can move to)

        //New range is how far out of range we are
        int newRange = 1;
        //The set of tiles we can move to from the tiles we can move to
        HashSet<Tile> newTiles;

        //Remove this because we actually want to move so moving 0 is useless
        MovableTiles.Remove(this);

        //Terrifying loop where we increase new range until we find a TargetTile
        while (true)
        {
            newTiles = new HashSet<Tile>();
            foreach(Tile tm in MovableTiles)
            {
                newTiles = findAtDistance(tm, newRange, invalidTiles, solidTiles, tiles);
                
                foreach(Tile t in newTiles)
                {
                    foreach (Tile ta in TargetTiles)
                        if (t == ta)
                        {
                            MovePriority = 2 + newRange;
                            return tm;
                        }
                }

            }
            newRange++;

            //Just in case
            if (newRange > 100)
            {
                Debug.Log("Nothing Found Boss");
                return null;
            }
        }
    }

    //Many getters
    public int getDepth()
    {
        return depth;
    }

    public void resetDepth()
    {
        depth = 0;
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
