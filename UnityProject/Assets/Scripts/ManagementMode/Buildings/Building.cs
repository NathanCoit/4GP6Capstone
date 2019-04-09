using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class containing methods for creating, interacting with,and deleting building objects
/// </summary>
public class Building : MapObject{
    
    static public float BuildingCostModifier = 1;
    private static Dictionary<string, UnityEngine.Object> mdictBuildingResources = new Dictionary<string, UnityEngine.Object>();

    

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

    /// <summary>
    /// Building contructor for a default building of a certain type
    /// </summary>
    /// <param name="penumBuildingType">The building type to create.</param>
    /// <param name="pmusFactionOwner">The owning faction of the building.</param>
	public Building(BUILDING_TYPE penumBuildingType, Faction pmusFactionOwner)
    {
        MapGameObject = CreateBuildingObject(penumBuildingType, pmusFactionOwner.Type);
        BuildingType = penumBuildingType;
        OwningFaction = pmusFactionOwner;
        BuildingCost = CalculateBuildingCost(penumBuildingType);
        ObjectType = MapObjectType.Building;
    }

    /// <summary>
    /// Building constructor for loading a serialized building
    /// </summary>
    /// <param name="pmusSavedBuilding">A serialized building.</param>
    /// <param name="pmusFactionOwner">The owning faction of the building to be loaded.</param>
    public Building(GameInfo.SavedBuilding pmusSavedBuilding, Faction pmusFactionOwner)
    {
        UpgradeLevel = pmusSavedBuilding.UpgradeLevel;
        MapGameObject = CreateBuildingObject(pmusSavedBuilding.BuildingType, pmusFactionOwner.Type);
        BuildingType = pmusSavedBuilding.BuildingType;
        OwningFaction = pmusFactionOwner;
        BuildingCost = CalculateBuildingCost(pmusSavedBuilding.BuildingType);
        ObjectType = MapObjectType.Building;
    }

    /// <summary>
    /// Method used to create a building game object based on the type of building.
    /// </summary>
    /// <param name="penumBuildingType"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject CreateBuildingObject(BUILDING_TYPE penumBuildingType, Faction.GodType type)
    {
        GameObject uniBuildingGameObject = null;
        // TODO, reduce all seperate into a single create functin and pass in building type
        // Seperate needed while not all models are present.
        switch (penumBuildingType)
        {
            case BUILDING_TYPE.ALTAR:
                uniBuildingGameObject = CreateAltarBuildingObject(type);
                break;
            case BUILDING_TYPE.VILLAGE:
                uniBuildingGameObject = CreateVillageBuildingObject(type);
                break;
            case BUILDING_TYPE.MATERIAL:
                uniBuildingGameObject = CreateMaterialBuildingObject(type);
                break;
            case BUILDING_TYPE.HOUSING:
                uniBuildingGameObject = CreateHousingBuildingObject(type);
                break;
            case BUILDING_TYPE.UPGRADE:
                uniBuildingGameObject = CreateUpgradeBuildingObject(type);
                break;
        }
        if(uniBuildingGameObject != null)
        {
            // Add a linerenderer for outlining, scale based on scene parmeters
            uniBuildingGameObject.AddComponent<LineRenderer>().positionCount = 0;
            uniBuildingGameObject.transform.localScale = new Vector3(ObjectRadius, ObjectRadius, ObjectRadius);
        }
        return uniBuildingGameObject;
    }

    /// <summary>
    /// Method for loading and creating a blacksmith building game object
    /// </summary>
    /// <param name="type">The god type which owns this building.</param>
    /// <returns></returns>
    private GameObject CreateUpgradeBuildingObject(Faction.GodType type)
    {
        GameObject uniUpgradeBuildingGameObject = null;
        // Build the resource key for the building type and check if it exists
        string strResourceKey = "Buildings/" + type.ToString() + BUILDING_TYPE.UPGRADE.ToString() + UpgradeLevel.ToString();
        if (mdictBuildingResources.ContainsKey(strResourceKey) && mdictBuildingResources[strResourceKey] != null)
        {
            uniUpgradeBuildingGameObject = (GameObject)GameObject.Instantiate(
            mdictBuildingResources[strResourceKey]);
        }
        else
        {
            // Resource doesn't exist, create a cube
            uniUpgradeBuildingGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            uniUpgradeBuildingGameObject.transform.localScale += new Vector3(UpgradeLevel - 1, UpgradeLevel - 1, UpgradeLevel - 1);
        }
        return uniUpgradeBuildingGameObject;
    }

    /// <summary>
    /// Method for loading and creating an altar building game object
    /// </summary>
    /// <param name="type">The god type which owns this building.</param>
    /// <returns></returns>
    private GameObject CreateHousingBuildingObject(Faction.GodType type)
    {
        GameObject uniHousingBuildingGameObject = null;
        string strResourceKey = "Buildings/" + type.ToString() + BUILDING_TYPE.HOUSING.ToString() + UpgradeLevel.ToString();
        if (mdictBuildingResources.ContainsKey(strResourceKey) && mdictBuildingResources[strResourceKey] != null)
        {
            uniHousingBuildingGameObject = (GameObject)GameObject.Instantiate(
            mdictBuildingResources[strResourceKey]);
        }
        else
        {
            // If resource does not exist, load a capsule game object.
            uniHousingBuildingGameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            uniHousingBuildingGameObject.transform.localScale += new Vector3(UpgradeLevel - 1, UpgradeLevel - 1, UpgradeLevel - 1);
        }
        return uniHousingBuildingGameObject;
    }

    /// <summary>
    /// Method for loading or creating material building game objects
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject CreateMaterialBuildingObject(Faction.GodType type)
    {
        GameObject uniMaterialBuildingGameObject = null;
        string strResourceKey = "Buildings/" + type.ToString() + BUILDING_TYPE.MATERIAL.ToString() + UpgradeLevel.ToString();
        if(mdictBuildingResources.ContainsKey(strResourceKey) && mdictBuildingResources[strResourceKey] != null)
        {
            uniMaterialBuildingGameObject = (GameObject)GameObject.Instantiate(
            mdictBuildingResources[strResourceKey]);
        }
        else
        {
            uniMaterialBuildingGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            uniMaterialBuildingGameObject.transform.localScale += new Vector3(UpgradeLevel - 1, UpgradeLevel - 1, UpgradeLevel - 1);
        }
		return uniMaterialBuildingGameObject;
    }

    /// <summary>
    /// Method for loading or creating a village building game object
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject CreateVillageBuildingObject(Faction.GodType type)
    {
        GameObject uniVillageBuildingGameObject = null;
        string strResourceKey = "Buildings/" + type.ToString() + BUILDING_TYPE.VILLAGE.ToString() + UpgradeLevel.ToString();
        if (mdictBuildingResources.ContainsKey(strResourceKey) && mdictBuildingResources[strResourceKey] != null)
        {
            uniVillageBuildingGameObject = (GameObject)GameObject.Instantiate(
            mdictBuildingResources[strResourceKey]);
        }
        else
        {
            uniVillageBuildingGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            uniVillageBuildingGameObject.transform.localScale += new Vector3(UpgradeLevel - 1, UpgradeLevel - 1, UpgradeLevel - 1);
        }
        return uniVillageBuildingGameObject;
    }

    /// <summary>
    /// Method for loading or creating an altar building game object
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject CreateAltarBuildingObject(Faction.GodType type)
    {
        GameObject uniAltarBuildingGameObject = null;
        string strResourceKey = "Buildings/" + type.ToString() + BUILDING_TYPE.ALTAR.ToString() + UpgradeLevel.ToString();
        if (mdictBuildingResources.ContainsKey(strResourceKey) && mdictBuildingResources[strResourceKey] != null)
        {
            uniAltarBuildingGameObject = (GameObject)GameObject.Instantiate(
            mdictBuildingResources[strResourceKey]);
        }
        else
        {
            uniAltarBuildingGameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            uniAltarBuildingGameObject.transform.localScale += new Vector3(UpgradeLevel - 1, UpgradeLevel - 1, UpgradeLevel - 1);
        }
        return uniAltarBuildingGameObject;
    }

    /// <summary>
    /// Method for upgrading the current instance of building
    /// Updates the buildings model
    /// </summary>
    /// <param name="outline"></param>
    /// <param name="pblnNoCost"></param>
    /// <returns></returns>
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
            if (mdictBuildingResources.ContainsKey(strResourceKey) && mdictBuildingResources[strResourceKey] != null)
            {
                OriginalPos = ObjectPosition;
                GameObject.Destroy(MapGameObject);
                MapGameObject = (GameObject)GameObject.Instantiate(
                    mdictBuildingResources[strResourceKey]);
                MapGameObject.transform.localScale = new Vector3(ObjectRadius, ObjectRadius, ObjectRadius);
                ObjectPosition = OriginalPos;
                MapGameObject.AddComponent<LineRenderer>();
                if (outline)
                {
                    ToggleObjectOutlines(true);
                }
            }
            else
            {
                MapGameObject.transform.localScale += new Vector3(1, 1, 1); // In case no model exists yet
            }
        }
        return Upgraded;
    }

    public void Destroy()
    {
        GameObject.Destroy(MapGameObject);
        OwningFaction.OwnedBuildings.Remove(this);
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
            if(!mdictBuildingResources.ContainsKey(strResourcePath + "1"))
            {
                mdictBuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "2"))
            {
                mdictBuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "3"))
            {
                mdictBuildingResources.Add(strResourcePath + "3", Resources.Load(strResourcePath + "3"));
            }

            strResourcePath = "Buildings/" + type.ToString() + BUILDING_TYPE.MATERIAL.ToString();
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "1"))
            {
                mdictBuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "2"))
            {
                mdictBuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "3"))
            {
                mdictBuildingResources.Add(strResourcePath + "3", Resources.Load(strResourcePath + "3"));
            }

            strResourcePath = "Buildings/" + type.ToString() + BUILDING_TYPE.VILLAGE.ToString();
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "1"))
            {
                mdictBuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "2"))
            {
                mdictBuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "3"))
            {
                mdictBuildingResources.Add(strResourcePath + "3", Resources.Load(strResourcePath + "3"));
            }

            strResourcePath = "Buildings/" + type.ToString() + BUILDING_TYPE.HOUSING.ToString();
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "1"))
            {
                mdictBuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "2"))
            {
                mdictBuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "3"))
            {
                mdictBuildingResources.Add(strResourcePath + "3", Resources.Load(strResourcePath + "3"));
            }

            strResourcePath = "Buildings/" + type.ToString() + BUILDING_TYPE.UPGRADE.ToString();
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "1"))
            {
                mdictBuildingResources.Add(strResourcePath + "1", Resources.Load(strResourcePath + "1"));
            }
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "2"))
            {
                mdictBuildingResources.Add(strResourcePath + "2", Resources.Load(strResourcePath + "2"));
            }
            if (!mdictBuildingResources.ContainsKey(strResourcePath + "3"))
            {
                mdictBuildingResources.Add(strResourcePath + "3", Resources.Load(strResourcePath + "3"));
            }
        }
    }

    public void ReloadBuildingObject()
    {
        Vector3 OriginalPos;
        OriginalPos = ObjectPosition;
        GameObject.Destroy(MapGameObject);
        MapGameObject = CreateBuildingObject(BuildingType, OwningFaction.Type);
        MapGameObject.transform.localScale = new Vector3(ObjectRadius, ObjectRadius, ObjectRadius);
        ObjectPosition = OriginalPos;
        if(MapGameObject.GetComponent<LineRenderer>() == null)
        {
            MapGameObject.AddComponent<LineRenderer>();
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
