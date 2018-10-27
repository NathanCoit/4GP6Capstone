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
    public enum MENUSTATE
    {
        Default_State,
        Building_State,
        Buffered_Building_State, //BufferedBuilding != null
        Building_Selected_State, //SelectedBuilding != null
        Moving_Building_State //BufferedBuilding != null
    }

    public GameObject AudioObject;
    private ExecuteSound sound;
    public TerrainMap GameMap;
    public Building PlayerVillage;
    public List<Building> EnemyVillages;
    public int TotalEnemies = 3;
    public float MapRadius;
    public List<Faction> CurrentFactions;
    public int BuildingCostModifier = 1;
    private MENUSTATE CurrentMenuState = MENUSTATE.Default_State;
    private Building BufferedBuilding = null;
    private Faction PlayerFaction = null;
    private List<Faction> EnemyFactions = null;
    private GameObject SelectedGameObject = null;
    private Building SelectedBuilding = null;
    private Vector3 OriginalBuildingPosition;
    public float MinimumMorale = 0.2f;

    // Use this for any initializations needed by other scripts
    void Awake()
    {
        sound = AudioObject.GetComponent<ExecuteSound>();
        Building bldEnemyBuilding = null;
        Faction facEnemyFaction = null;
        Vector3 vec3VillagePos;
        CurrentFactions = new List<Faction>();
        EnemyFactions = new List<Faction>();
        //Create map terrain
        GameMap = new TerrainMap(MapRadius);

        // Create the player Faction
        PlayerFaction = new Faction("YourGod");
        CurrentFactions.Add(PlayerFaction);
        for (int enemyCount = 0; enemyCount < TotalEnemies; enemyCount++)
        {
            facEnemyFaction = new Faction("EnemyGod" + enemyCount);
            CurrentFactions.Add(facEnemyFaction);
            EnemyFactions.Add(facEnemyFaction);
        }

        GameMap.DivideMap(CurrentFactions, 0, MapRadius / 2, PlayerFaction);

        //Create and place player village
        PlayerVillage = new Building(Building.BUILDING_TYPE.VILLAGE, PlayerFaction, BuildingCostModifier);
        vec3VillagePos = GameMap.CalculateStartingPosition(PlayerFaction);
        GameMap.PlaceBuilding(PlayerVillage, vec3VillagePos);

        //Create and place enemy villages
        foreach (Faction enemyFaction in EnemyFactions)
        {
            bldEnemyBuilding = new Building(Building.BUILDING_TYPE.VILLAGE, enemyFaction, BuildingCostModifier);
            vec3VillagePos = GameMap.CalculateStartingPosition(enemyFaction);
            GameMap.PlaceBuilding(bldEnemyBuilding, vec3VillagePos);
        }
    }
    // Use this for any initializations not needed by other scripts.
    void Start()
    {
        InvokeRepeating("CalculateResources", 0.5f, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // ########### Input section ###########
        switch (CurrentMenuState)
        {
            case MENUSTATE.Default_State:
                CheckDefaultMenuStateInputs();
                break;
            case MENUSTATE.Building_State:
                CheckBuildingStateInputs();
                break;
            case MENUSTATE.Buffered_Building_State:
                CheckBufferedBuildingStateInputs();
                break;
            case MENUSTATE.Moving_Building_State:
                CheckMovingBuildingStateInputs();
                break;
            case MENUSTATE.Building_Selected_State:
                CheckSelectedBuildingStateInputs();
                break;

        }
        if (BufferedBuilding != null)
        {
            UpdateBufferedBuilding();
        }
        else
        {
            CheckForSelectedBuilding();
        }
    }

    private void CheckForSelectedBuilding()
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // User left clicked, check if they clicked a building object
        if (Input.GetMouseButtonDown(0))
        {
            // Code for selecting a building
            if (Physics.Raycast(ray, out hitInfo))
            {
                SelectedGameObject = hitInfo.collider.gameObject;
                // If the user clicked the map, do nothing
                if (SelectedGameObject == GameMap.GetMapObject())
                {
                    SelectedGameObject = null;
                    if (SelectedBuilding != null)
                    {
                        SelectedBuilding.ToggleBuildingOutlines(false);
                        SelectedBuilding = null;
                        if (CurrentMenuState == MENUSTATE.Building_Selected_State)
                        {
                            CurrentMenuState = MENUSTATE.Default_State;
                        }
                    }
                }
                else
                {
                    if (SelectedBuilding != null)
                    {
                        SelectedBuilding.ToggleBuildingOutlines(false);
                    }
                    SelectedBuilding = GameMap.GetBuildings().Find(ClickedBuilding => ClickedBuilding.OwningFaction == PlayerFaction && ClickedBuilding.BuildingObject == SelectedGameObject);
                    if (SelectedBuilding != null)
                    {
                        Debug.Log(string.Format("Selected {0} type building", SelectedBuilding.BuildingType));
                        CurrentMenuState = MENUSTATE.Building_Selected_State;
                        SelectedBuilding.ToggleBuildingOutlines(true);
                    }
                }
            }
        }
    }

    private void UpdateBufferedBuilding()
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
                // Reenable the collider component for selecting
                BufferedBuilding.BuildingObject.GetComponent<Collider>().enabled = true;
				sound.PlaySound("PlaceBuilding");
                // If the building did place, go back to building menu
                PlayerFaction.MaterialCount -= BufferedBuilding.BuildingCost;
                CurrentMenuState = MENUSTATE.Building_State;
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
        List<Building> HousingBuildings = null;
        int intHousingTotal = 0;
        int intMaterialsToAdd = 0;
        int intWorshippersToAdd = 0;

        float WorPerSec = 0;
        float MatPerSec = 0;
        if (CurrentFactions != null)
        {
            foreach (Faction CurrentFaction in CurrentFactions)
            {
                intHousingTotal = 0;
                intMaterialsToAdd = 0;
                intWorshippersToAdd = 0;
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
                        intWorshippersToAdd += (1 * AltarBuilding.UpgradeLevel);
                    }

                    // Get all the village buildings
                    VillageBuildings = OwnedBuildings.FindAll(VillageBuilding => VillageBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE);
                    foreach (Building VillageBuilding in VillageBuildings)
                    {
                        // Calculate worshipper growth
                        intWorshippersToAdd += (1 * VillageBuilding.UpgradeLevel);
                        // Calculatae Resource growth
                        intMaterialsToAdd += (1 * VillageBuilding.UpgradeLevel);
                        intHousingTotal += 100 * VillageBuilding.UpgradeLevel;
                    }

                    // Get all the material buildings
                    MaterialBuildings = OwnedBuildings.FindAll(MaterialBuilding => MaterialBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL);
                    foreach (Building MaterialBuilding in MaterialBuildings)
                    {
                        // Calculatae Resource growth
                        intMaterialsToAdd += (1 * MaterialBuilding.UpgradeLevel);
                    }

                    // Apply the morale modifier
                    intMaterialsToAdd = Mathf.CeilToInt(intMaterialsToAdd * CurrentFaction.Morale);
                    intWorshippersToAdd = Mathf.CeilToInt(intWorshippersToAdd * CurrentFaction.Morale);

                    if (CurrentFaction == PlayerFaction)
                    {
                        WorPerSec = (float)intWorshippersToAdd / 2;
                        MatPerSec = (float)intMaterialsToAdd / 2;
                    }

                    // Add the appropriate resources
                    CurrentFaction.MaterialCount += intMaterialsToAdd;
                    CurrentFaction.WorshipperCount += intWorshippersToAdd;

                    // Get all the housing buildings
                    HousingBuildings = OwnedBuildings.FindAll(HousingBuild => HousingBuild.BuildingType == Building.BUILDING_TYPE.HOUSING);
                    foreach (Building HousingBuilding in HousingBuildings)
                    {
                        intHousingTotal += 100 * HousingBuilding.UpgradeLevel;
                    }
                    //Calculate morale losses/gains
                    // Each housing/village building can hold 100 * upgrade level worshippers
                    if (intHousingTotal > CurrentFaction.WorshipperCount)
                    {
                        // There is enough housing for the current population, morale goes towards 100%
                        if (CurrentFaction.Morale <= 1)
                        {
                            CurrentFaction.Morale += 0.05f;
                            if (CurrentFaction.Morale > 1)
                            {
                                CurrentFaction.Morale = 1;
                            }
                        }
                    }
                    else
                    {
                        // Not enough housing for the population, morale will drop
                        if (CurrentFaction.Morale >= MinimumMorale)
                        {
                            CurrentFaction.Morale -= 0.05f;
                        }
                    }
                }
            }
            Debug.Log(string.Format("{0}: Material Count({1}), Worshipper Count({2}), Morale({3}), Wor/sec({4}), Mat/sec({5}), MenuState({6})",
                PlayerFaction.GodName,
                PlayerFaction.MaterialCount,
                PlayerFaction.WorshipperCount,
                PlayerFaction.Morale,
                WorPerSec,
                MatPerSec,
                CurrentMenuState));
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
            CurrentMenuState = 0;
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
        else if (Input.GetKeyDown(KeyCode.D))
        {
            BufferBuilding(Building.BUILDING_TYPE.HOUSING);
        }
    }

    private void CheckMovingBuildingStateInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CurrentMenuState = MENUSTATE.Building_Selected_State;
            if (BufferedBuilding != null)
            {
                GameMap.PlaceBuilding(BufferedBuilding, OriginalBuildingPosition);
            }
            foreach (Building BuildingOnMap in GameMap.GetBuildings())
            {
                BuildingOnMap.ToggleBuildingOutlines(false);
            }
            SelectedBuilding = null;
            BufferedBuilding = null;
        }
    }

    private void CheckDefaultMenuStateInputs()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            CurrentMenuState = MENUSTATE.Building_State;
        }
    }

    private void CheckBufferedBuildingStateInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CurrentMenuState = MENUSTATE.Building_State;
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
    }

    private void CheckSelectedBuildingStateInputs()
    {
        int intUpgradeCost;
        if (SelectedBuilding != null)
        {
            // Player's building
            if (SelectedBuilding.OwningFaction == PlayerFaction)
            {
                // Global hotkey for a selected building
                if (Input.GetKeyDown(KeyCode.U))
                {
                    // Attempt to upgrade selected building
                    intUpgradeCost = Building.CalculateBuildingUpgradeCost(SelectedBuilding.BuildingType, BuildingCostModifier);
                    // 3 is max upgrade level of a building
                    if (PlayerFaction.MaterialCount >= intUpgradeCost)
                    {
                        if (SelectedBuilding.UpgradeLevel < 3)
                        {
                            SelectedBuilding.UpgradeBuilding();
                            PlayerFaction.MaterialCount -= Building.CalculateBuildingUpgradeCost(SelectedBuilding.BuildingType, BuildingCostModifier);
                        }
                        else
                        {
                            Debug.Log("Building is at max upgrade level");
                        }

                    }
                    else
                    {
                        Debug.Log(string.Format("Not enough materials to upgrade ({0} required)", intUpgradeCost));
                    }

                }
                else if (Input.GetKeyDown(KeyCode.M))
                {
                    //Move player building if it isn't a village
                    if (SelectedBuilding.BuildingType != Building.BUILDING_TYPE.VILLAGE)
                    {
                        BufferedBuilding = SelectedBuilding;
                        SelectedBuilding = null;
                        CurrentMenuState = MENUSTATE.Moving_Building_State;
                        OriginalBuildingPosition = BufferedBuilding.BuildingPosition;
                        GameMap.GetBuildings().Remove(BufferedBuilding);
                        foreach (Building BuildingOnMap in GameMap.GetBuildings())
                        {
                            BuildingOnMap.ToggleBuildingOutlines(true);
                        }
                        BufferedBuilding.ToggleBuildingOutlines(true);
                        // Disable the collider to have the raycasting ignore the held building for placement purposes
                        BufferedBuilding.BuildingObject.GetComponent<Collider>().enabled = false;
                    }
                    else
                    {
                        // TODO add cannot move feedback
                    }

                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SelectedBuilding.ToggleBuildingOutlines(false);
                    SelectedBuilding = null;
                    CurrentMenuState = MENUSTATE.Default_State;
                }
            }
            else
            {
                // TODO add options when selecting an enemy building (start battle, view stats)
            }
        }
    }

    private void BufferBuilding(Building.BUILDING_TYPE penumBuildingType)
    {
        // Check if user has enough resources to build the building
        int BuildingCost = Building.CalculateBuildingCost(penumBuildingType, BuildingCostModifier);
        if (PlayerFaction.MaterialCount >= BuildingCost)
        {
            BufferedBuilding = new Building(penumBuildingType, PlayerFaction, BuildingCostModifier);
            CurrentMenuState = MENUSTATE.Buffered_Building_State;
            foreach (Building BuildingOnMap in GameMap.GetBuildings())
            {
                BuildingOnMap.ToggleBuildingOutlines(true);
            }
            BufferedBuilding.ToggleBuildingOutlines(true);
            // Disable the collider to have the raycasting ignore the held building for placement purposes
            BufferedBuilding.BuildingObject.GetComponent<Collider>().enabled = false;
        }
        else
        {
            sound.PlaySound("NotMaterials");
        }
    }
}
