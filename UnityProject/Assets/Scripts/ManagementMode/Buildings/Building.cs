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
    static public float BuildingCostModifier = 1;
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
        ALTAR,
        UPGRADE
    }
    public Faction OwningFaction;

    public BUILDING_TYPE BuildingType { get; private set; }

    public int UpgradeLevel = 1;

    public int BuildingCost = 0;

	public Building(BUILDING_TYPE penumBuildingType, Faction pFactionOwner)
    {
        BuildingObject = CreateBuildingObject(penumBuildingType, pFactionOwner.Type);
        BuildingType = penumBuildingType;
        OwningFaction = pFactionOwner;
        BuildingCost = CalculateBuildingCost(penumBuildingType);
    }

    public Building(GameInfo.SavedBuilding pobjSavedBuilding, Faction pobjFactionOwner)
    {
        UpgradeLevel = pobjSavedBuilding.UpgradeLevel;
        BuildingObject = CreateBuildingObject(pobjSavedBuilding.BuildingType, pobjFactionOwner.Type);
        BuildingType = pobjSavedBuilding.BuildingType;
        OwningFaction = pobjFactionOwner;
        BuildingCost = CalculateBuildingCost(pobjSavedBuilding.BuildingType);
    }

    private GameObject CreateBuildingObject(BUILDING_TYPE penumBuildingType, Faction.GodType type)
    {
        GameObject gobjBuilding = null;
        // TODO, reduce all seperate into a single create functin and pass in building type
        // Seperate needed while not all models are present.
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
            case BUILDING_TYPE.UPGRADE:
                gobjBuilding = CreateUpgradeBuildingObject(type);
                break;
        }
        if(gobjBuilding != null)
        {
            gobjBuilding.AddComponent<LineRenderer>().positionCount = 0;
            gobjBuilding.transform.localScale = new Vector3(BuildingRadiusSize, BuildingRadiusSize, BuildingRadiusSize);
        }
        return gobjBuilding;
    }

    private GameObject CreateUpgradeBuildingObject(Faction.GodType type)
    {
        GameObject gobjUpgradeBuilding = null;
        string strResourceKey = "Buildings/" + type.ToString() + BUILDING_TYPE.UPGRADE.ToString() + UpgradeLevel.ToString();
        if (BuildingResources.ContainsKey(strResourceKey) && BuildingResources[strResourceKey] != null)
        {
            gobjUpgradeBuilding = (GameObject)GameObject.Instantiate(
            BuildingResources[strResourceKey]);
        }
        else
        {
            gobjUpgradeBuilding = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gobjUpgradeBuilding.transform.localScale += new Vector3(UpgradeLevel - 1, UpgradeLevel - 1, UpgradeLevel - 1);
        }
        return gobjUpgradeBuilding;
    }

    private GameObject CreateHousingBuildingObject(Faction.GodType type)
    {
        GameObject gobjHousingBuilding = null;
        string strResourceKey = "Buildings/" + type.ToString() + BUILDING_TYPE.HOUSING.ToString() + UpgradeLevel.ToString();
        if (BuildingResources.ContainsKey(strResourceKey) && BuildingResources[strResourceKey] != null)
        {
            gobjHousingBuilding = (GameObject)GameObject.Instantiate(
            BuildingResources[strResourceKey]);
        }
        else
        {
            gobjHousingBuilding = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            gobjHousingBuilding.transform.localScale += new Vector3(UpgradeLevel - 1, UpgradeLevel - 1, UpgradeLevel - 1);
        }
        return gobjHousingBuilding;
    }

    private GameObject CreateMaterialBuildingObject(Faction.GodType type)
    {
        GameObject gobjMaterialBuilding = null;
        string strResourceKey = "Buildings/" + type.ToString() + BUILDING_TYPE.MATERIAL.ToString() + UpgradeLevel.ToString();
        if(BuildingResources.ContainsKey(strResourceKey) && BuildingResources[strResourceKey] != null)
        {
            gobjMaterialBuilding = (GameObject)GameObject.Instantiate(
            BuildingResources[strResourceKey]);
        }
        else
        {
            gobjMaterialBuilding = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gobjMaterialBuilding.transform.localScale += new Vector3(UpgradeLevel - 1, UpgradeLevel - 1, UpgradeLevel - 1);
        }
		return gobjMaterialBuilding;
    }

    private GameObject CreateVillageBuildingObject(Faction.GodType type)
    {
        GameObject gobjVillageBuilding = null;
        string strResourceKey = "Buildings/" + type.ToString() + BUILDING_TYPE.VILLAGE.ToString() + UpgradeLevel.ToString();
        if (BuildingResources.ContainsKey(strResourceKey) && BuildingResources[strResourceKey] != null)
        {
            gobjVillageBuilding = (GameObject)GameObject.Instantiate(
            BuildingResources[strResourceKey]);
        }
        else
        {
            gobjVillageBuilding = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gobjVillageBuilding.transform.localScale += new Vector3(UpgradeLevel - 1, UpgradeLevel - 1, UpgradeLevel - 1);
        }
        return gobjVillageBuilding;
    }
    private GameObject CreateAltarBuildingObject(Faction.GodType type)
    {
        GameObject gobjAltarBuilding = null;
        string strResourceKey = "Buildings/" + type.ToString() + BUILDING_TYPE.ALTAR.ToString() + UpgradeLevel.ToString();
        if (BuildingResources.ContainsKey(strResourceKey) && BuildingResources[strResourceKey] != null)
        {
            gobjAltarBuilding = (GameObject)GameObject.Instantiate(
            BuildingResources[strResourceKey]);
        }
        else
        {
            gobjAltarBuilding = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gobjAltarBuilding.transform.localScale += new Vector3(UpgradeLevel - 1, UpgradeLevel - 1, UpgradeLevel - 1);
        }
        return gobjAltarBuilding;
    }

    public virtual bool UpgradeBuilding(bool outline = true, bool pblnNoCost = false)
    {
        bool Upgraded = false;
        int BuildingCost = CalculateBuildingUpgradeCost(BuildingType, UpgradeLevel+1);
        if(pblnNoCost)
        {
            BuildingCost = 0;
        }
        if(OwningFaction.MaterialCount > BuildingCost && UpgradeLevel < 3 && OwningFaction.GodTier >= UpgradeLevel)
        {
            Upgraded = true;
            OwningFaction.MaterialCount -= BuildingCost;
            UpgradeLevel++;
            Vector3 OriginalPos;
            string strResourceKey = "Buildings/" + OwningFaction.Type.ToString() + BuildingType.ToString() + UpgradeLevel.ToString();
            if (BuildingResources.ContainsKey(strResourceKey) && BuildingResources[strResourceKey] != null)
            {
                OriginalPos = BuildingPosition;
                GameObject.Destroy(BuildingObject);
                BuildingObject = (GameObject)GameObject.Instantiate(
                    BuildingResources[strResourceKey]);
                BuildingObject.transform.localScale = new Vector3(BuildingRadiusSize, BuildingRadiusSize, BuildingRadiusSize);
                BuildingPosition = OriginalPos;
                BuildingObject.AddComponent<LineRenderer>();
                if (outline)
                {
                    ToggleBuildingOutlines(true);
                }
            }
            else
            {
                BuildingObject.transform.localScale += new Vector3(1, 1, 1); // In case no model exists yet
            }
        }
        return Upgraded;
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

    public static int CalculateBuildingCost(Building.BUILDING_TYPE penumBuildingType)
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
            case (Building.BUILDING_TYPE.UPGRADE):
                BuildingCost = Convert.ToInt32(100 * BuildingCostModifier);
                break;
        }
        return BuildingCost;
    }

    public int CalculateBuildingUpgradeCost()
    {
        int BuildingCost = int.MaxValue;
        switch (BuildingType)
        {
            case (Building.BUILDING_TYPE.ALTAR): // 100, 1000, 10000
                BuildingCost = Convert.ToInt32(Mathf.Pow(10, UpgradeLevel + 1) * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.MATERIAL): // 100, 1000, 10000
                BuildingCost = Convert.ToInt32(Mathf.Pow(10, UpgradeLevel + 1) * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.HOUSING): // 100, 1000, 10000
                BuildingCost = Convert.ToInt32(Mathf.Pow(10, UpgradeLevel + 1) * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.VILLAGE): // Upgrade done by tier unlock
                BuildingCost = 0;
                break;
            case (BUILDING_TYPE.UPGRADE): // 1000, 10000, 100000
                BuildingCost = Convert.ToInt32(Mathf.Pow(10, UpgradeLevel + 2) * BuildingCostModifier);
                break;
        }
        return BuildingCost;
    }

    public static int CalculateBuildingUpgradeCost(Building.BUILDING_TYPE penumBuildingType, int pintUpgradeLevel = 1)
    {
        int BuildingCost = int.MaxValue;
        switch (penumBuildingType)
        {
            case (Building.BUILDING_TYPE.ALTAR): // 100, 1000, 10000
                BuildingCost = Convert.ToInt32(Mathf.Pow(10, pintUpgradeLevel + 1) * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.MATERIAL): // 100, 1000, 10000
                BuildingCost = Convert.ToInt32(Mathf.Pow(10, pintUpgradeLevel + 1) * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.HOUSING): // 100, 1000, 10000
                BuildingCost = Convert.ToInt32(Mathf.Pow(10, pintUpgradeLevel + 1) * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.VILLAGE): // Upgrade done by tier unlock
                BuildingCost = 0;
                break;
            case (BUILDING_TYPE.UPGRADE): // 1000, 10000, 100000
                BuildingCost = Convert.ToInt32(Mathf.Pow(10, pintUpgradeLevel + 2) * BuildingCostModifier);
                break;
        }
        return BuildingCost;
    }

    public static void LoadBuildingResources(List<Faction.GodType> LoadedGodTypes)
    {
        string strResourcePath = string.Empty;
        foreach(Faction.GodType type in LoadedGodTypes)
        {
            strResourcePath = "Buildings/" + type.ToString() + BUILDING_TYPE.ALTAR.ToString();
            // Load all 5 building types and the upgrade levels of each
            if(!BuildingResources.ContainsKey(strResourcePath + "1"))
            {
                BuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "2"))
            {
                BuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "2"))
            {
                BuildingResources.Add(strResourcePath + "3", Resources.Load(strResourcePath + "3"));
            }

            strResourcePath = "Buildings/" + type.ToString() + BUILDING_TYPE.MATERIAL.ToString();
            if (!BuildingResources.ContainsKey(strResourcePath + "1"))
            {
                BuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "2"))
            {
                BuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "2"))
            {
                BuildingResources.Add(strResourcePath + "3", Resources.Load(strResourcePath + "3"));
            }

            strResourcePath = "Buildings/" + type.ToString() + BUILDING_TYPE.VILLAGE.ToString();
            if (!BuildingResources.ContainsKey(strResourcePath + "1"))
            {
                BuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "2"))
            {
                BuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "2"))
            {
                BuildingResources.Add(strResourcePath + "3", Resources.Load(strResourcePath + "3"));
            }

            strResourcePath = "Buildings/" + type.ToString() + BUILDING_TYPE.HOUSING.ToString();
            if (!BuildingResources.ContainsKey(strResourcePath + "1"))
            {
                BuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "2"))
            {
                BuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "2"))
            {
                BuildingResources.Add(strResourcePath + "3", Resources.Load(strResourcePath + "3"));
            }

            strResourcePath = "Buildings/" + type.ToString() + BUILDING_TYPE.UPGRADE.ToString();
            if (!BuildingResources.ContainsKey(strResourcePath + "1"))
            {
                BuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "2"))
            {
                BuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }
            if (!BuildingResources.ContainsKey(strResourcePath + "2"))
            {
                BuildingResources.Add(strResourcePath + "3", Resources.Load(strResourcePath + "3"));
            }
        }
    }

    public void ReloadBuildingObject()
    {
        Vector3 OriginalPos;
        OriginalPos = BuildingPosition;
        GameObject.Destroy(BuildingObject);
        BuildingObject = CreateBuildingObject(BuildingType, OwningFaction.Type);
        BuildingObject.transform.localScale = new Vector3(BuildingRadiusSize, BuildingRadiusSize, BuildingRadiusSize);
        BuildingPosition = OriginalPos;
        if(BuildingObject.GetComponent<LineRenderer>() == null)
        {
            BuildingObject.AddComponent<LineRenderer>();
        }
    }

    public static Building CreateRandomBuilding(Faction placingFaction)
    {
        Building.BUILDING_TYPE RandomType;
        Building RandomBuilding = null;
        switch ((int)(UnityEngine.Random.value * 100 / 25))
        {
            case 0:
                RandomType = Building.BUILDING_TYPE.ALTAR;
                break;
            case 1:
                RandomType = Building.BUILDING_TYPE.HOUSING;
                break;
            case 2:
                RandomType = Building.BUILDING_TYPE.HOUSING;
                break;
            case 3:
                RandomType = Building.BUILDING_TYPE.ALTAR;
                break;
            default:
                RandomType = Building.BUILDING_TYPE.ALTAR;
                break;
        }
        RandomBuilding = new Building(RandomType, placingFaction);
        return RandomBuilding;
    }
}
