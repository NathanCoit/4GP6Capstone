using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class containing methods for creating, interacting with,and deleting building objects
/// </summary>
public class Building : MonoBehaviour {
    private GameObject mgobjBuilding;
    public Vector2 BuildingPosition;

    public enum BUILDING_TYPE
    {
        MATERIAL,
        HOUSING,
        VILLAGE,
        ALTAR
    }

    public BUILDING_TYPE BuildingType { get; private set; }

	public Building(BUILDING_TYPE penumBuildingType)
    {
        mgobjBuilding = CreateBuildingObject(penumBuildingType);
        BuildingType = penumBuildingType;
    }


    private GameObject CreateBuildingObject(BUILDING_TYPE penumBuildingType)
    {
        GameObject gobjBuilding = null;
        switch (penumBuildingType)
        {
            case BUILDING_TYPE.ALTAR:
                gobjBuilding = CreateAltarBuildingObject();
                break;
            case BUILDING_TYPE.VILLAGE:
                gobjBuilding = CreateVillageBuildingObject();
                break;
        }
        return gobjBuilding;
    }

    private GameObject CreateVillageBuildingObject()
    {
        GameObject gobjVillageBuilding = null;

        return gobjVillageBuilding;
    }
    private GameObject CreateAltarBuildingObject()
    {
        GameObject gobjAltarBuilding = null;

        return gobjAltarBuilding;
    }
    
}
