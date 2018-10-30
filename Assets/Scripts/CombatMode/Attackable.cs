using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attackable : MonoBehaviour
{
    public Vector2 pos;
    private GameObject MapMan;
    private GameObject BoardMan;

    void Start()
    {
        MapMan = GameObject.FindGameObjectWithTag("MapManager");

        BoardMan = GameObject.FindGameObjectWithTag("BoardManager");
    }


    void Update()
    {

    }

    private Tile[,] getTiles()
    {
        return MapMan.GetComponent<MapManager>().tiles;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Tile[,] tiles = getTiles();
            GameObject attackedUnit = new GameObject();

            foreach(GameObject g in BoardMan.GetComponent<BoardManager>().enemyUnits)
                if (g.GetComponent<Units>().getPos() == pos)
                    attackedUnit = g;

            attackedUnit.GetComponent<Units>().setWorshiperCount(attackedUnit.GetComponent<Units>().getWorshiperCount() - (int)MapMan.GetComponent<MapManager>().Selected.GetComponent<Units>().getAttackStrength());

            Debug.Log(attackedUnit.GetComponent<Units>().getWorshiperCount());

        }
    }
}
