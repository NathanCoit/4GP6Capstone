using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitObjectScript : MonoBehaviour
{
    private BoardManager BoardMan;
    private MapManager MapMan;

    private Unit relatedUnit;

    public Material playerAvailable;
    public Material playerNotAvailable;
    public Material enemyAvailable;
    public Material enemyNotAvailable;

    // Start is called before the first frame update
    void Start()
    {
        //Mapman, here to save the day!
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();

        //Boardman is here also
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Unit getUnit()
    {
        return relatedUnit;
    }

    public void setUnit(Unit u)
    {
        relatedUnit = u;
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
