using System;
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
    public Faction OwningFaction;

    public BUILDING_TYPE BuildingType { get; private set; }

    public int UpgradeLevel = 1;

	public Building(BUILDING_TYPE penumBuildingType, Faction pFactionOwner)
    {
        mgobjBuilding = CreateBuildingObject(penumBuildingType);
        BuildingType = penumBuildingType;
        OwningFaction = pFactionOwner;
        OwningFaction.OwnedBuildings.Add(this);
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
        gobjVillageBuilding = GameObject.CreatePrimitive(PrimitiveType.Cube);
        return gobjVillageBuilding;
    }
    private GameObject CreateAltarBuildingObject()
    {
        GameObject gobjAltarBuilding = null;

        return gobjAltarBuilding;
    }

    public void UpgradeBuilding()
    {
        UpgradeLevel++;
    }
    
}
