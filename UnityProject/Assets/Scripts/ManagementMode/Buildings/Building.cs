using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing methods for creating, interacting with,and deleting building objects
/// Because each building only differs by model, and not in functionality, all but the mine building is contained here.
/// </summary>
public class Building : MapObject
{
    // Constants to define the cost of each building by default
    // Scaled by the game manager
    protected const int mcintAltarCost = 10;
    protected const int mcintMaterialCost = 10;
    protected const int mcintVillageCost = 10;
    protected const int mcintHousingCost = 10;
    protected const int mcintUpgradeCost = 100;

    static public float BuildingCostModifier = 1; // Static to allow cost calculations before building is created

    // Static lost of prefab objects to avoid reloading models
    // Store models in memory for faster loading
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
    /// Allows creating different types of buildings based on input parameter.
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
    /// Allows the removal of duplicate code in places where save states must be loaded
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
    /// Allows one method call to decide which type of building model shall be loaded
    /// </summary>
    /// <param name="penumBuildingType"></param>
    /// <param name="penumGodType"></param>
    /// <returns></returns>
    private GameObject CreateBuildingObject(BUILDING_TYPE penumBuildingType, Faction.GodType penumGodType)
    {
        GameObject uniBuildingGameObject = null;
        string strResourceKey = "Buildings/" + penumGodType.ToString() + penumBuildingType.ToString() + UpgradeLevel.ToString();

        // Load the building model from memory
        if (mdictBuildingResources.ContainsKey(strResourceKey) && mdictBuildingResources[strResourceKey] != null)
        {
            uniBuildingGameObject = (GameObject)GameObject.Instantiate(
            mdictBuildingResources[strResourceKey]);
        }
        else
        {
#if DEBUG
            // Model was not found in resources folder, this should not occur as all models have been created now
#endif
        }
        // Check to ensure a game model was found when creating
        if (uniBuildingGameObject != null)
        {
            // Add a linerenderer for outlining, scale based on scene parameters
            // Allows for the outlining of buildings when selected or when building other buildings
            uniBuildingGameObject.AddComponent<LineRenderer>().positionCount = 0;
            uniBuildingGameObject.transform.localScale = new Vector3(ObjectRadius, ObjectRadius, ObjectRadius);
        }
        return uniBuildingGameObject;
    }

    /// <summary>
    /// Method for upgrading the current instance of building
    /// Performs checks for material count and current upgrade tier
    /// Virtual method to allow overriding for different upgrade cost calculations or functionality for future building classes.
    /// </summary>
    /// <param name="pblnOutline">specify false to not outline on upgrade. When enemies upgrade.</param>
    /// <param name="pblnNoCost">Specify true to ignore cost calculations</param>
    /// <returns></returns>
    public virtual bool UpgradeBuilding(bool pblnOutline = true, bool pblnNoCost = false)
    {
        bool blnUpgraded = false;
        int intBuildingCost = 0;
        Vector3 uniOriginalPosVector3;
        string strResourceKey;

        if (!pblnNoCost)
        {
            CalculateBuildingUpgradeCost(BuildingType, UpgradeLevel + 1);
        }
        // Check if upgrade is allowed
        if (OwningFaction.MaterialCount > intBuildingCost
            && UpgradeLevel < 3 // Can't upgrade past 3 
            && OwningFaction.GodTier >= UpgradeLevel)
        {
            blnUpgraded = true;
            OwningFaction.MaterialCount -= intBuildingCost;
            UpgradeLevel++;

            // Load the new model, swap the models, and move the building back into place.
            // Allows for completely different models on upgrade
            strResourceKey = "Buildings/" + OwningFaction.Type.ToString() + BuildingType.ToString() + UpgradeLevel.ToString();
            if (mdictBuildingResources.ContainsKey(strResourceKey) && mdictBuildingResources[strResourceKey] != null)
            {
                uniOriginalPosVector3 = ObjectPosition;
                GameObject.Destroy(MapGameObject);
                MapGameObject = (GameObject)GameObject.Instantiate(
                    mdictBuildingResources[strResourceKey]);
                MapGameObject.transform.localScale = new Vector3(ObjectRadius, ObjectRadius, ObjectRadius);
                ObjectPosition = uniOriginalPosVector3;
                MapGameObject.AddComponent<LineRenderer>();
                if (pblnOutline)
                {
                    ToggleObjectOutlines(true);
                }
            }
            else
            {
                MapGameObject.transform.localScale += new Vector3(1, 1, 1); // In case no model exists yet
            }
        }
        // Return whether the upgrade was successfull to allow feedback to the player
        return blnUpgraded;
    }

    /// <summary>
    /// Method to ensure if a building model is removed, it is also removed from it's owning faction
    /// </summary>
    public void Destroy()
    {
        GameObject.Destroy(MapGameObject);
        OwningFaction.OwnedBuildings.Remove(this);
    }

    /// <summary>
    /// Static method to calculate the cost of a building based on the type of building.
    /// Allows UI feedback and information before a building has been created.
    /// </summary>
    /// <param name="penumBuildingType"></param>
    /// <returns></returns>
    public static int CalculateBuildingCost(Building.BUILDING_TYPE penumBuildingType)
    {
        // Set to max value in case cost calc fails, players don't get free building
        int intBuildingCost = int.MaxValue;
        switch (penumBuildingType)
        {
            case (Building.BUILDING_TYPE.ALTAR):
                intBuildingCost = Convert.ToInt32(mcintAltarCost * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.MATERIAL):
                intBuildingCost = Convert.ToInt32(mcintMaterialCost * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.HOUSING):
                intBuildingCost = Convert.ToInt32(mcintHousingCost * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.VILLAGE):
                intBuildingCost = Convert.ToInt32(mcintVillageCost * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.UPGRADE):
                intBuildingCost = Convert.ToInt32(mcintUpgradeCost * BuildingCostModifier);
                break;
        }
        return intBuildingCost;
    }

    /// <summary>
    /// Wrapper method to simplify calls to static upgrade cost calculation from within this object
    /// </summary>
    /// <returns></returns>
    public int CalculateBuildingUpgradeCost()
    {
        return CalculateBuildingUpgradeCost(BuildingType, UpgradeLevel);
    }

    /// <summary>
    /// Static method to calculate the cost of upgrading a building
    /// Allows checks for whether the player can upgrade a building and display UI information on upgrade cost
    /// </summary>
    /// <param name="penumBuildingType"></param>
    /// <param name="pintUpgradeLevel"></param>
    /// <returns></returns>
    public static int CalculateBuildingUpgradeCost(Building.BUILDING_TYPE penumBuildingType, int pintUpgradeLevel = 1)
    {
        // Cost calculation is exponential to scale with extra area player is given on tier unlock
        // Upgrading will be required to save space, so will cost more
        int intBuildingCost = int.MaxValue;
        switch (penumBuildingType)
        {
            case (Building.BUILDING_TYPE.ALTAR): // 100, 1000, 10000
                intBuildingCost = Convert.ToInt32(Mathf.Pow(mcintAltarCost, pintUpgradeLevel + 1) * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.MATERIAL): // 100, 1000, 10000
                intBuildingCost = Convert.ToInt32(Mathf.Pow(mcintMaterialCost, pintUpgradeLevel + 1) * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.HOUSING): // 100, 1000, 10000
                intBuildingCost = Convert.ToInt32(Mathf.Pow(mcintHousingCost, pintUpgradeLevel + 1) * BuildingCostModifier);
                break;
            case (Building.BUILDING_TYPE.VILLAGE): // Upgrade done by tier unlock
                intBuildingCost = 0;
                break;
            case (BUILDING_TYPE.UPGRADE): // 1000, 10000, 100000
                intBuildingCost = Convert.ToInt32(Mathf.Pow(mcintUpgradeCost, pintUpgradeLevel + 1) * BuildingCostModifier);
                break;
        }
        return intBuildingCost;
    }

    /// <summary>
    /// Static method called once at the beginning of management mode scene loading
    /// Loads all current god building models into memory to speed up the creation of new buildings
    /// and avoid having to load buildings on each create.
    /// </summary>
    /// <param name="parrLoadedGodTypes"></param>
    public static void LoadBuildingResources(List<Faction.GodType> parrLoadedGodTypes)
    {
        string strResourcePath = string.Empty;
        foreach (Faction.GodType enumGodType in parrLoadedGodTypes)
        {
            strResourcePath = "Buildings/" + enumGodType.ToString() + BUILDING_TYPE.ALTAR.ToString();
            // Load all 5 building types and the upgrade levels of each
            // Only load buildings not already in memory in case this method is called again
            LoadAllUpgradeLevelsOfBuilding(strResourcePath);

            strResourcePath = "Buildings/" + enumGodType.ToString() + BUILDING_TYPE.MATERIAL.ToString();
            LoadAllUpgradeLevelsOfBuilding(strResourcePath);

            strResourcePath = "Buildings/" + enumGodType.ToString() + BUILDING_TYPE.VILLAGE.ToString();
            LoadAllUpgradeLevelsOfBuilding(strResourcePath);

            strResourcePath = "Buildings/" + enumGodType.ToString() + BUILDING_TYPE.HOUSING.ToString();
            LoadAllUpgradeLevelsOfBuilding(strResourcePath);

            strResourcePath = "Buildings/" + enumGodType.ToString() + BUILDING_TYPE.UPGRADE.ToString();
            LoadAllUpgradeLevelsOfBuilding(strResourcePath);
        }
    }

    /// <summary>
    /// Helper method for loading each upgrade level of a building.
    /// </summary>
    /// <param name="pstrBuildingResourcePath"></param>
    private static void LoadAllUpgradeLevelsOfBuilding(string pstrBuildingResourcePath)
    {
        if (!mdictBuildingResources.ContainsKey(pstrBuildingResourcePath + "1"))
        {
            mdictBuildingResources.Add(pstrBuildingResourcePath + "1", Resources.Load(pstrBuildingResourcePath + "1"));
        }
        if (!mdictBuildingResources.ContainsKey(pstrBuildingResourcePath + "2"))
        {
            mdictBuildingResources.Add(pstrBuildingResourcePath + "2", Resources.Load(pstrBuildingResourcePath + "2"));
        }
        if (!mdictBuildingResources.ContainsKey(pstrBuildingResourcePath + "3"))
        {
            mdictBuildingResources.Add(pstrBuildingResourcePath + "3", Resources.Load(pstrBuildingResourcePath + "3"));
        }
    }

    /// <summary>
    /// Method to reload the game object for this building.
    /// Used by game manager to reload buildings in the case that the owning faction has changed
    /// </summary>
    public void ReloadBuildingObject()
    {
        Vector3 uniOriginalPosVector3;
        uniOriginalPosVector3 = ObjectPosition;
        GameObject.Destroy(MapGameObject);
        MapGameObject = CreateBuildingObject(BuildingType, OwningFaction.Type);
        MapGameObject.transform.localScale = new Vector3(ObjectRadius, ObjectRadius, ObjectRadius);
        ObjectPosition = uniOriginalPosVector3;
        if (MapGameObject.GetComponent<LineRenderer>() == null)
        {
            MapGameObject.AddComponent<LineRenderer>();
        }
    }

    /// <summary>
    /// Static method to generate a pseudo random building.
    /// Used by game manager when creating buildings for a new game and
    /// for enemy god new building generation.
    /// Only generates altars and houses as villages, blacksmiths, and mines cannot 
    /// be randomly generated as those are specifically created once per faction
    /// </summary>
    /// <param name="pmusPlacingFaction"></param>
    /// <returns></returns>
    public static Building CreateRandomBuilding(Faction pmusPlacingFaction)
    {
        Building.BUILDING_TYPE enumRandomBuildingType;
        Building musRandomBuilding = null;

        // Increase odds of altars for worshipper/difficulty growth
        switch ((int)(UnityEngine.Random.value * 100 / 25))
        {
            case 0:
                enumRandomBuildingType = Building.BUILDING_TYPE.ALTAR;
                break;
            case 1:
                enumRandomBuildingType = Building.BUILDING_TYPE.HOUSING;
                break;
            case 2:
                enumRandomBuildingType = Building.BUILDING_TYPE.HOUSING;
                break;
            case 3:
                enumRandomBuildingType = Building.BUILDING_TYPE.ALTAR;
                break;
            default:
                enumRandomBuildingType = Building.BUILDING_TYPE.ALTAR;
                break;
        }
        musRandomBuilding = new Building(enumRandomBuildingType, pmusPlacingFaction);
        return musRandomBuilding;
    }
}
