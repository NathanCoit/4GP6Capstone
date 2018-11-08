using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class containing methods for creating, interacting with,and deleting building objects
/// </summary>
public class Building{
    public GameObject BuildingObject;
    static public float BuildingRadiusSize = 10f; // Radius around a building that can not be built on
    private static Dictionary<string, UnityEngine.Object> BuildingResources = new Dictionary<string, UnityEngine.Object>();
    public Vector3 BuildingPosition
    {
        get
        {
            return BuildingObject.transform.position;
        }
        set
        {
            BuildingObject.transform.position = value;
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

	public Building(BUILDING_TYPE penumBuildingType, Faction pFactionOwner, float pfBuildingCostModifier = 1.0f)
    {
        BuildingObject = CreateBuildingObject(penumBuildingType, pFactionOwner.Type);
        BuildingType = penumBuildingType;
        OwningFaction = pFactionOwner;
        OwningFaction.OwnedBuildings.Add(this);
        BuildingCost = CalculateBuildingCost(penumBuildingType, pfBuildingCostModifier);
    }



    private GameObject CreateBuildingObject(BUILDING_TYPE penumBuildingType, Faction.GodType type)
    {
        GameObject gobjBuilding = null;
        switch (penumBuildingType)
        {
            case BUILDING_TYPE.ALTAR:
                gobjBuilding = CreateAltarBuildingObject(type);
                break;
            case BUILDING_TYPE.VILLAGE:
                gobjBuilding = CreateVillageBuildingObject(type);
                break;
            case BUILDING_TYPE.MATERIAL:
                gobjBuilding = CreateMaterialBuildingObject(type);
                break;
            case BUILDING_TYPE.HOUSING:
                gobjBuilding = CreateHousingBuildingObject(type);
                break;
        }
        if(gobjBuilding != null)
        {
            gobjBuilding.AddComponent<LineRenderer>().positionCount = 0;
            gobjBuilding.transform.localScale = new Vector3(BuildingRadiusSize, BuildingRadiusSize, BuildingRadiusSize);
        }
        return gobjBuilding;
    }

    private GameObject CreateHousingBuildingObject(Faction.GodType type)
    {
        GameObject gobjHousingBuilding = null;
        string strResourceKey = type.ToString() + BUILDING_TYPE.HOUSING.ToString() + "0";
        if (BuildingResources.ContainsKey(strResourceKey) && BuildingResources[strResourceKey] != null)
        {
            gobjHousingBuilding = (GameObject)GameObject.Instantiate(
            BuildingResources[strResourceKey]);
        }
        else
        {
            gobjHousingBuilding = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        }
        return gobjHousingBuilding;
    }

    private GameObject CreateMaterialBuildingObject(Faction.GodType type)
    {
        GameObject gobjMaterialBuilding = null;
        string strResourceKey = type.ToString() + BUILDING_TYPE.MATERIAL.ToString() + "0";
        if(BuildingResources.ContainsKey(strResourceKey) && BuildingResources[strResourceKey] != null)
        {
            gobjMaterialBuilding = (GameObject)GameObject.Instantiate(
            BuildingResources[strResourceKey]);
        }
        else
        {
            gobjMaterialBuilding = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }
		return gobjMaterialBuilding;
    }

    private GameObject CreateVillageBuildingObject(Faction.GodType type)
    {
        GameObject gobjVillageBuilding = null;
        string strResourceKey = type.ToString() + BUILDING_TYPE.VILLAGE.ToString() + "0";
        if (BuildingResources.ContainsKey(strResourceKey) && BuildingResources[strResourceKey] != null)
        {
            gobjVillageBuilding = (GameObject)GameObject.Instantiate(
            BuildingResources[strResourceKey]);
        }
        else
        {
            gobjVillageBuilding = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
        return gobjVillageBuilding;
    }
    private GameObject CreateAltarBuildingObject(Faction.GodType type)
    {
        GameObject gobjAltarBuilding = null;
        string strResourceKey = type.ToString() + BUILDING_TYPE.ALTAR.ToString() + "0";
        if (BuildingResources.ContainsKey(strResourceKey) && BuildingResources[strResourceKey] != null)
        {
            gobjAltarBuilding = (GameObject)GameObject.Instantiate(
            BuildingResources[strResourceKey]);
        }
        else
        {
            gobjAltarBuilding = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        }
        return gobjAltarBuilding;
    }

    public void UpgradeBuilding()
    {
        UpgradeLevel++;
        string strResourceKey = OwningFaction.Type.ToString() + BuildingType.ToString() + UpgradeLevel.ToString();
        if (BuildingResources.ContainsKey(strResourceKey) && BuildingResources[strResourceKey] != null)
        {
            BuildingObject = (GameObject)GameObject.Instantiate(
                BuildingResources[strResourceKey]);
        }
        else
        {
            BuildingObject.transform.localScale += new Vector3(1, 1, 1); // In case no model exists yet
        }
    }

    public void Destroy()
    {
        GameObject.Destroy(BuildingObject);
        OwningFaction.OwnedBuildings.Remove(this);
    }

    public void ToggleBuildingOutlines(bool pblnTurnOn)
    {
        LineRenderer lineRenderer = BuildingObject.GetComponent<LineRenderer>();
        Vector3 pos;
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

            lineRenderer.positionCount = vertexCount + 2;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                pos = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
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

    public static int CalculateBuildingCost(Building.BUILDING_TYPE penumBuildingType, float BuildingCostModifier)
    {
        int BuildingCost = int.MaxValue;
        switch (penumBuildingType)
        {
            case (Building.BUILDING_TYPE.ALTAR):
                BuildingCost = Convert.ToInt32(10 * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.MATERIAL):
                BuildingCost = Convert.ToInt32(5 * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.HOUSING):
                BuildingCost = Convert.ToInt32(10 * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.VILLAGE):
                BuildingCost = Convert.ToInt32(10 * BuildingCostModifier);
                break;
        }
        return BuildingCost;
    }

    public static int CalculateBuildingUpgradeCost(Building.BUILDING_TYPE penumBuildingType, float BuildingCostModifier)
    {
        int BuildingCost = int.MaxValue;
        switch (penumBuildingType)
        {
            case (Building.BUILDING_TYPE.ALTAR):
                BuildingCost = Convert.ToInt32(50 * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.MATERIAL):
                BuildingCost = Convert.ToInt32(50 * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.HOUSING):
                BuildingCost = Convert.ToInt32(50 * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.VILLAGE):
                BuildingCost = Convert.ToInt32(50 * BuildingCostModifier);
                break;
        }
        return BuildingCost;
    }

    public static void LoadBuildingResources(List<Faction.GodType> LoadedGodTypes)
    {
        string strResourcePath = string.Empty;
        foreach(Faction.GodType type in LoadedGodTypes)
        {
            strResourcePath = type.ToString() + BUILDING_TYPE.ALTAR.ToString();
            // Load all 4 building types and the upgrade levels of each
            if(!BuildingResources.ContainsKey(strResourcePath + "0"))
            {
                BuildingResources.Add(strResourcePath + "0", Resources.Load(strResourcePath + "0"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "1"))
            {
                BuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "2"))
            {
                BuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }

            strResourcePath = type.ToString() + BUILDING_TYPE.MATERIAL.ToString();
            if (!BuildingResources.ContainsKey(strResourcePath + "0"))
            {
                BuildingResources.Add(strResourcePath + "0", Resources.Load(strResourcePath + "0"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "1"))
            {
                BuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "0"))
            {
                BuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }

            strResourcePath = type.ToString() + BUILDING_TYPE.VILLAGE.ToString();
            if (!BuildingResources.ContainsKey(strResourcePath + "0"))
            {
                BuildingResources.Add(strResourcePath + "0", Resources.Load(strResourcePath + "0"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "1"))
            {
                BuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "0"))
            {
                BuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }

            strResourcePath = type.ToString() + BUILDING_TYPE.HOUSING.ToString();
            if (!BuildingResources.ContainsKey(strResourcePath + "0"))
            {
                BuildingResources.Add(strResourcePath + "0", Resources.Load(strResourcePath + "0"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "1"))
            {
                BuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "0"))
            {
                BuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }
        }
    }
}
