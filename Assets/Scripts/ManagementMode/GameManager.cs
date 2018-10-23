using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Game manager for the Management mode. Controls intializing of level 
/// and transitions from Combat Mode to and from Management Mode.
/// </summary>
public class GameManager : MonoBehaviour
{

    public TerrainMap GameMap;
    public Building PlayerVillage;
    public List<Building> EnemyVillages;
    public int TotalEnemies;
    public List<Faction> CurrentFactions;
    public int BuildingCostModifier = 1;
    private int MenuState = 0;
    private Building BufferedBuilding = null;
    private Faction PlayerFaction = null;
    private List<Faction> EnemyFactions = null;

    // Use this for initialization
    void Start()
    {
        Building bldEnemyBuilding = null;
        CurrentFactions = new List<Faction>();
        //Create map terrain
        GameMap = new TerrainMap();

        // Create the player Faction
        PlayerFaction = new Faction("YourGod");
        CurrentFactions.Add(PlayerFaction);

        //Create and place player village
        PlayerVillage = new Building(Building.BUILDING_TYPE.VILLAGE, PlayerFaction, 0);
        Vector3 vec3VillagePos = CalculateStartingPlayerPosition();
        GameMap.PlaceBuilding(PlayerVillage, vec3VillagePos);

        ////Create and place enemy villages
        //for (int enemyCount = 0; enemyCount < TotalEnemies; enemyCount++)
        //{
        //    PlayerFaction = new Faction("EnemyGod" + enemyCount);
        //    CurrentFactions.Add(PlayerFaction);
        //    bldEnemyBuilding = new Building(Building.BUILDING_TYPE.VILLAGE, PlayerFaction);
        //    vec2VillagePos = CalculateStartingEnemyPosiion();
        //    GameMap.PlaceBuilding(bldEnemyBuilding, vec2VillagePos);
        //    EnemyVillages.Add(bldEnemyBuilding);
        //}

        InvokeRepeating("CalculateResources", 0.5f, 2.0f);
    }

    private Vector3 CalculateStartingEnemyPosiion()
    {
        throw new NotImplementedException();
    }

    private Vector3 CalculateStartingPlayerPosition()
    {
        return new Vector3(2, 0.5f, 2);
    }

    // Update is called once per frame
    void Update()
    {
        // ########### Input section ###########
        // Menu State Info
        /*
         * 0 - Main menu state: The main menu state
         *  Can Access: 1,
         * 1 - Building Menu State: The building menu state for selecting the different buildings to build
         *  Can Access: 1, 10, 
         *  
         * 10 - Building Buffered State: The menu when a building has been selected to be built
         *  Can Access: 1
         */
        switch (MenuState)
        {
            case 0:
                // Default menu state
                if (Input.GetKeyDown(KeyCode.B))
                {
                    MenuState = 1;
                }
                break;

            case 1:
                // Building state
                CheckBuildingStateInputs();
                break;

            case 10:
                // Building selected to build state
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    MenuState = 1;
                    if (BufferedBuilding != null)
                    {
                        BufferedBuilding.Destroy();
                        BufferedBuilding = null;
                    }
                    foreach (Building BuildingOnMap in GameMap.GetBuildings())
                    {
                        BuildingOnMap.ToggleBuildingOutlines(false);
                    }
                }
                break;
        }

        // ########### Building Placement section ###########
        if (BufferedBuilding != null)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo))
            {
                BufferedBuilding.BuildingPosition = new Vector3(hitInfo.point.x, 0.5f, hitInfo.point.z);
            }
            // User left clicked
            if (Input.GetMouseButtonDown(0))
            {
                // Try to place the building
                if (GameMap.PlaceBuilding(BufferedBuilding, new Vector3(hitInfo.point.x, 0.5f, hitInfo.point.z)))
                {
                    // If the building did place, go back to building menu
                    PlayerFaction.MaterialCount -= BufferedBuilding.BuildingCost;
                    MenuState = 1;
                    BufferedBuilding = null;
                    foreach (Building BuildingOnMap in GameMap.GetBuildings())
                    {
                        BuildingOnMap.ToggleBuildingOutlines(false);
                    }
                }
                else
                {
                    // Play error noise, display error, something to say can't place building there
                    Debug.Log("Can't build there");
                }
            }

        }
    }

    /// <summary>
    /// The function for calculating resource growth based on the currently built resource buildings.
    /// Function is invoked once every x seconds (2 for now).
    /// </summary>
    void CalculateResources()
    {
        List<Building> OwnedBuildings = null;
        List<Building> AltarBuildings = null;
        List<Building> VillageBuildings = null;
        List<Building> MaterialBuildings = null;
        if (CurrentFactions != null)
        {
            foreach (Faction CurrentFaction in CurrentFactions)
            {
                // Get all buildings belonging to this faction
                OwnedBuildings = GameMap.GetBuildings().FindAll(MatchingBuild => MatchingBuild.OwningFaction == CurrentFaction);

                if (OwnedBuildings != null)
                {
                    // Get all the altars for worshipper calculations
                    AltarBuildings = OwnedBuildings.FindAll(AltarBuilding => AltarBuilding.BuildingType == Building.BUILDING_TYPE.ALTAR);
                    // Increase worshippers for each current altar
                    foreach (Building AltarBuilding in AltarBuildings)
                    {
                        // Calculate worshipper growth
                        CurrentFaction.WorshipperCount += (1 * AltarBuilding.UpgradeLevel);
                    }

                    // Get all the village buildings
                    VillageBuildings = OwnedBuildings.FindAll(VillageBuilding => VillageBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE);
                    foreach (Building VillageBuilding in VillageBuildings)
                    {
                        // Calculate worshipper growth
                        CurrentFaction.WorshipperCount += (1 * VillageBuilding.UpgradeLevel);
                        // Calculatae Resource growth
                        CurrentFaction.MaterialCount += (1 * VillageBuilding.UpgradeLevel);
                    }

                    // Get all the material buildings
                    MaterialBuildings = OwnedBuildings.FindAll(MaterialBuilding => MaterialBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL);
                    foreach (Building MaterialBuilding in MaterialBuildings)
                    {
                        // Calculatae Resource growth
                        CurrentFaction.MaterialCount += (1 * MaterialBuilding.UpgradeLevel);
                    }
                }
                Debug.Log(string.Format("{0}: Material Count({1}), Worshipper Count({2})", CurrentFaction.GodName, CurrentFaction.MaterialCount, CurrentFaction.WorshipperCount));
            }
        }



        // Update the player's UI with resource counts
        // UI1 = PlayerFaction.WorshipperCount;
        // UI2 = PlayerFaction.MaterialCount;
        // UI3 = PlayerFaction.Morale;
    }

    private void CheckBuildingStateInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuState = 0;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            // Altar building
            BufferBuilding(Building.BUILDING_TYPE.ALTAR);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            // Mine building
            BufferBuilding(Building.BUILDING_TYPE.MATERIAL);
        }
    }
    /// <summary>
    /// Function for calculating the cost of buildings
    /// </summary>
    /// <param name="penumBuildingType"></param>
    /// <returns></returns>
    private int CalculateBuildingCost(Building.BUILDING_TYPE penumBuildingType)
    {
        int BuildingCost = int.MaxValue;
        switch (penumBuildingType)
        {
            case (Building.BUILDING_TYPE.ALTAR):
                BuildingCost = 10 * BuildingCostModifier;
                break;
            case (Building.BUILDING_TYPE.MATERIAL):
                BuildingCost = 5 * BuildingCostModifier;
                break;
        }
        return BuildingCost;
    }

    private void BufferBuilding(Building.BUILDING_TYPE penumBuildingType)
    {
        // Check if user has enough resources to build the building
        int BuildingCost = CalculateBuildingCost(penumBuildingType);
        if (PlayerFaction.MaterialCount >= BuildingCost)
        {
            BufferedBuilding = new Building(penumBuildingType, PlayerFaction, BuildingCost);
            MenuState = 10;
            foreach (Building BuildingOnMap in GameMap.GetBuildings())
            {
                BuildingOnMap.ToggleBuildingOutlines(true);
            }
            BufferedBuilding.ToggleBuildingOutlines(true);
        }
        else
        {
            Debug.Log("Not enough materials!");
        }
    }
}
