﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitObjectScript : MonoBehaviour
{
    private BoardManager BoardMan;
    private MapManager MapMan;
    private UIManager UIMan;

    private Unit relatedUnit;

    public GameObject infoPanel;

    public Material playerAvailable;
    public Material playerNotAvailable;
    public Material enemyAvailable;
    public Material enemyNotAvailable;

    private Canvas infoCanvas;

    private bool frameOne;

    // Start is called before the first frame update
    void Start()
    {
        //Mapman, here to save the day!
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();

        //Boardman is here also
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        //Here to give you the best ui, it's
        UIMan = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        //infoCanvas = GameObject.FindGameObjectWithTag("InfoCanvas").GetComponent<Canvas>();

        frameOne = true;

        //Assign Screenspace - Camera camera to the main camera (because you can't do it in the prefab)
        gameObject.transform.GetChild(0).GetComponent<Canvas>().worldCamera = UIMan.mainCamera;
    }

    // Update is called once per frame
    void Update()
    {
        
        //Unity takes a few frame to find the editor reference for some reason
        if (infoPanel != null)
        {
            infoPanel.transform.localEulerAngles = new Vector3(-Mathf.Atan2(gameObject.transform.position.y - Camera.main.transform.position.y, 
                gameObject.transform.position.z - Camera.main.transform.position.z) * 180 / Mathf.PI, 0, 0);
        }
        
    }

    public Unit getUnit()
    {
        return relatedUnit;
    }

    public void setUnit(Unit u)
    {
        relatedUnit = u;
    }

    //For drawing gods enter battle tiles
    public void drawEnterBattleTiles()
    {
        List<Tile> firstHalf = new List<Tile>();
        List<Tile> secondHalf = new List<Tile>();

        bool direction = true;

        MapMan.resetDepths();

        //Similar to abilty tiles
        for (int i = 0; i < MapMan.tiles.GetLength(0) / 2; i++)
        {
            for (int j = 0; j < MapMan.tiles.GetLength(1); j++)
            {
                if (direction)
                {
                    firstHalf.Add(MapMan.tiles[i, j]);
                    secondHalf.Add(MapMan.tiles[MapMan.tiles.GetLength(0) - i - 1, MapMan.tiles.GetLength(1) - j - 1]);
                }
                else
                {
                    firstHalf.Add(MapMan.tiles[i, MapMan.tiles.GetLength(1) - j - 1]);
                    secondHalf.Add(MapMan.tiles[MapMan.tiles.GetLength(0) - i - 1, j]);
                }
            }
            direction = !direction;
        }

        //If we have an odd number of rows
        if (MapMan.tiles.GetLength(0) % 2 != 0)
        {
            for (int j = 0; j < MapMan.tiles.GetLength(1) / 2; j++)
            {
                if (direction)
                {
                    firstHalf.Add(MapMan.tiles[MapMan.tiles.GetLength(0) / 2, j]);
                    secondHalf.Add(MapMan.tiles[MapMan.tiles.GetLength(0) / 2, MapMan.tiles.GetLength(1) - j - 1]);
                }
                else
                {
                    firstHalf.Add(MapMan.tiles[MapMan.tiles.GetLength(0) / 2, MapMan.tiles.GetLength(1) - j - 1]);
                    secondHalf.Add(MapMan.tiles[MapMan.tiles.GetLength(0) / 2, j]);
                }
            }
        }

        if (MapMan.tiles.GetLength(1) % 2 != 0)
            firstHalf.Add(MapMan.tiles[MapMan.tiles.GetLength(0) / 2, MapMan.tiles.GetLength(1) / 2]);

        StartCoroutine(drawEnterBattleTiles(firstHalf, 0.01f));
        StartCoroutine(drawEnterBattleTiles(secondHalf, 0.01f));
    }

    //Cooroutine for drawing enter battle tiles
    public IEnumerator drawEnterBattleTiles(List<Tile> locationsToDraw, float delay)
    {
        foreach (Tile t in locationsToDraw)
        {
            if (MapMan.tiles[(int)t.getX(), (int)t.getZ()].isTraversable() && !BoardMan.findTeamTiles(BoardMan.playerUnits).Contains(t) && !BoardMan.findTeamTiles(BoardMan.enemyUnits).Contains(t))
            {
                GameObject targetTile = Instantiate(BoardMan.MovableTile);
                targetTile.GetComponent<Movable>().pos = new Vector2((int)t.getX(), (int)t.getZ());

                targetTile.transform.position = new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2,
                    1f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2);

                targetTile.GetComponent<Movable>().setTarget(new Vector3(t.getX() + ((1 - transform.lossyScale.x) / 2) + transform.lossyScale.x / 2,
                    t.getY() + 0.5f, t.getZ() + ((1 - transform.lossyScale.z) / 2) + transform.lossyScale.x / 2));
            }
            yield return new WaitForSeconds(delay);
        }
        yield return null;
    }

    //Sets a clicked unit to be selected
    public void OnMouseOver()
    {
        if (relatedUnit != null)
        {
            if ((Input.GetMouseButtonDown(0) || relatedUnit.autoClick) && relatedUnit.canAct && BoardMan.playerUnits.Contains(relatedUnit) && MapMan.Selected != relatedUnit.unitGameObject() && !EventSystem.current.IsPointerOverGameObject())
            {
                MapMan.Selected = relatedUnit.unitGameObject();
                MapMan.newSelected = true;
                relatedUnit.autoClick = false;
            }
        }

    }
}
