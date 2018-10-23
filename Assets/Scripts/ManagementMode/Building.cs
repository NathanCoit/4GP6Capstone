﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class containing methods for creating, interacting with,and deleting building objects
/// </summary>
public class Building{
    private GameObject mgobjBuilding;
    public float BuildingRadiusSize = 1f; // Radius around a building that can not be built on
    public Vector3 BuildingPosition
    {
        get
        {
            return mgobjBuilding.transform.position;
        }
        set
        {
            mgobjBuilding.transform.position = value;
        }
    }

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

    public int BuildingCost = 0;

	public Building(BUILDING_TYPE penumBuildingType, Faction pFactionOwner, int pintBuildingCost)
    {
        mgobjBuilding = CreateBuildingObject(penumBuildingType);
        BuildingType = penumBuildingType;
        OwningFaction = pFactionOwner;
        OwningFaction.OwnedBuildings.Add(this);
        BuildingCost = pintBuildingCost;
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
            case BUILDING_TYPE.MATERIAL:
                gobjBuilding = CreateMaterialBuildingObject();
                break;
        }
        if(gobjBuilding != null)
        {
            gobjBuilding.AddComponent<LineRenderer>().positionCount = 0;
        }
        return gobjBuilding;
    }

    private GameObject CreateMaterialBuildingObject()
    {
        GameObject gobjVillageBuilding = null;
        gobjVillageBuilding = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        return gobjVillageBuilding;
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
        gobjAltarBuilding = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        return gobjAltarBuilding;
    }

    public void UpgradeBuilding()
    {
        UpgradeLevel++;
    }

    public void Destroy()
    {
        GameObject.Destroy(mgobjBuilding);
    }

    public void ToggleBuildingOutlines(bool pblnTurnOn)
    {
        LineRenderer lineRenderer = mgobjBuilding.GetComponent<LineRenderer>();
        // Turn on building outlines
        if (pblnTurnOn)
        {
            int vertexCount = 40; // 4 vertices == square
            float lineWidth = 0.2f;
            float radius = 1.0f;
            
            lineRenderer.useWorldSpace = false;
            lineRenderer.widthMultiplier = lineWidth;

            float deltaTheta = (2f * Mathf.PI) / vertexCount;
            float theta = 0f;

            lineRenderer.positionCount = vertexCount;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 pos = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
                lineRenderer.SetPosition(i, pos);
                theta += deltaTheta;
            }
        }

        // Turn off building outlines
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}