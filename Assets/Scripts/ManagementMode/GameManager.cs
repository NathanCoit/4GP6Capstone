using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


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
        Moving_Building_State, //BufferedBuilding != null
        Tier_Reward_State
    }

	public Texture mapTexture;
    public GameObject GameInfoObjectPrefab;
    private GameInfo gameInfo;
    modeTextScript text1;
	resourceScript resourcetext;
    public GameObject RewardUI;
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
    public List<TierReward> PlayerRewardTree;
    public int TierWorshipperCount = 100; // Initial tier unlock count
    public int TierWoshipperCountMultiplier = 2; // After every tier, tier count is multiplied by this

    // Use this for any initializations needed by other scripts
    void Awake()
    {
		text1 = FindObjectOfType<modeTextScript> ();
		resourcetext = FindObjectOfType<resourceScript> ();
        sound = AudioObject.GetComponent<ExecuteSound>();
        if (!InitializeGameInfoObject())
        {
            // Starting new game scene, initialize map, players, and buildings
            Building bldEnemyBuilding = null;
            Faction facEnemyFaction = null;
            Vector3 vec3VillagePos;
            CurrentFactions = new List<Faction>();
            EnemyFactions = new List<Faction>();
            //Create map terrain
			GameMap = new TerrainMap(MapRadius, mapTexture);

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
    }

    /// <summary>
    /// Method for initializing the gameinfo object
    /// Either creates a gameinfo object if one does not exist, or reads the values if one does
    /// </summary>
    private bool InitializeGameInfoObject()
    {
        Faction InitFaction = null;
        Faction EnemyFaction = null;
        Building InitBuilding = null;
        GameObject GameInfoObject = GameObject.Find("GameInfo");
        if (GameInfoObject != null)
        {
            // Found a gameinfo object, load values
            gameInfo = GameInfoObject.GetComponent<GameInfo>();
            CurrentFactions = new List<Faction>();
            EnemyFactions = new List<Faction>();
            // Create scene with values from gameInfo
            // Load Game Map
            GameMap = new TerrainMap(gameInfo.MapRadius, mapTexture);
            // Load factions
            foreach(GameInfo.SavedFaction savedFaction in gameInfo.SavedFactions)
            {
                InitFaction = new Faction(savedFaction.GodName)
                {
                    MaterialCount = savedFaction.MatieralCount,
                    Morale = savedFaction.Morale,
                    WorshipperCount = savedFaction.WorshipperCount,
                    FactionArea = savedFaction.FactionArea
                };
                CurrentFactions.Add(InitFaction);
                EnemyFactions.Add(InitFaction);
                foreach(GameInfo.SavedBuilding building in savedFaction.OwnedBuildings)
                {
                    InitBuilding = new Building(building.BuildingType, InitFaction);
                    GameMap.PlaceBuilding(InitBuilding, building.BuildingPosition);
                }
            }
            PlayerFaction = new Faction(gameInfo.PlayerFaction.GodName)
            {
                MaterialCount = gameInfo.PlayerFaction.MatieralCount,
                Morale = gameInfo.PlayerFaction.Morale,
                WorshipperCount = gameInfo.PlayerFaction.WorshipperCount,
                FactionArea = gameInfo.PlayerFaction.FactionArea
            };
            foreach (GameInfo.SavedBuilding building in gameInfo.PlayerFaction.OwnedBuildings)
            {
                InitBuilding = new Building(building.BuildingType, PlayerFaction);
                GameMap.PlaceBuilding(InitBuilding, building.BuildingPosition);
                if (building.BuildingType == Building.BUILDING_TYPE.VILLAGE)
                {
                    PlayerVillage = InitBuilding;
                }
            }
            CurrentFactions.Add(PlayerFaction);

            EnemyFaction = new Faction(gameInfo.EnemyFaction.GodName)
            {
                MaterialCount = gameInfo.EnemyFaction.MatieralCount,
                Morale = gameInfo.EnemyFaction.Morale,
                WorshipperCount = gameInfo.EnemyFaction.WorshipperCount,
                FactionArea = gameInfo.EnemyFaction.FactionArea
            };
            foreach (GameInfo.SavedBuilding building in gameInfo.EnemyFaction.OwnedBuildings)
            {
                InitBuilding = new Building(building.BuildingType, EnemyFaction);
                GameMap.PlaceBuilding(InitBuilding, building.BuildingPosition);
            }

            // Load tech tree


            if(gameInfo.LastBattleStatus == GameInfo.BATTLESTATUS.Victory)
            {
                // Take over enemy factions buildings, area, and resources
                foreach(float[] enemyArea in gameInfo.EnemyFaction.FactionArea)
                {
                    PlayerFaction.FactionArea.Add(enemyArea);
                }
                PlayerFaction.MaterialCount += EnemyFaction.MaterialCount;
                foreach(Building building in EnemyFaction.OwnedBuildings)
                {
                    building.OwningFaction = PlayerFaction;
                }
            }
            else if (gameInfo.LastBattleStatus == GameInfo.BATTLESTATUS.Retreat)
            {
                // Lower morale
                CurrentFactions.Add(EnemyFaction);
            }
            else
            {
                // Run defeat animation/reset to tier checkpoint
            }
            GameMap.DrawFactionArea(PlayerFaction);
            return true;
        }
        else
        {
            // Couldn't find gameinfo object, create new game scene
            GameObject NewGameInfoObject = (GameObject)Instantiate(GameInfoObjectPrefab);
            NewGameInfoObject.name = "GameInfo";
            gameInfo = NewGameInfoObject.GetComponent<GameInfo>();
            return false;
        }
    }

    /// <summary>
    /// Method for setting the gameInfo values needed to go to/from management mode scenes
    /// </summary>
    private void EnterCombatMode(Faction EnemyFaction)
    {
        GameInfo.SavedFaction initFaction;
        // save player faction
        gameInfo.PlayerFaction = new GameInfo.SavedFaction
        {
            GodName = PlayerFaction.GodName,
            MatieralCount = PlayerFaction.MaterialCount,
            Morale = PlayerFaction.Morale,
            WorshipperCount = PlayerFaction.WorshipperCount,
            FactionArea = PlayerFaction.FactionArea,
            OwnedBuildings = new List<GameInfo.SavedBuilding>()
        };
        foreach(Building building in PlayerFaction.OwnedBuildings)
        {
            gameInfo.PlayerFaction.OwnedBuildings.Add(
                new GameInfo.SavedBuilding
                {
                    BuildingPosition = building.BuildingPosition,
                    BuildingType = building.BuildingType,
                    UpgradeLevel = building.UpgradeLevel
                });
        }

        // save challenging faction
        gameInfo.EnemyFaction= new GameInfo.SavedFaction
        {
            GodName = EnemyFaction.GodName,
            MatieralCount = EnemyFaction.MaterialCount,
            Morale = EnemyFaction.Morale,
            WorshipperCount = EnemyFaction.WorshipperCount,
            FactionArea = EnemyFaction.FactionArea,
            OwnedBuildings = new List<GameInfo.SavedBuilding>()
        };
        foreach (Building building in EnemyFaction.OwnedBuildings)
        {
            gameInfo.EnemyFaction.OwnedBuildings.Add(
                new GameInfo.SavedBuilding
                {
                    BuildingPosition = building.BuildingPosition,
                    BuildingType = building.BuildingType,
                    UpgradeLevel = building.UpgradeLevel
                });
        }

        gameInfo.SavedFactions = new List<GameInfo.SavedFaction>();
        // Save the rest of the factions
        foreach(Faction faction in EnemyFactions)
        {
            if(faction != EnemyFaction)
            {
                initFaction = new GameInfo.SavedFaction
                {
                    GodName = faction.GodName,
                    MatieralCount = faction.MaterialCount,
                    Morale = faction.Morale,
                    WorshipperCount = faction.WorshipperCount,
                    FactionArea = faction.FactionArea,
                    OwnedBuildings = new List<GameInfo.SavedBuilding>()
                };
                foreach (Building building in faction.OwnedBuildings)
                {
                    initFaction.OwnedBuildings.Add(
                        new GameInfo.SavedBuilding
                        {
                            BuildingPosition = building.BuildingPosition,
                            BuildingType = building.BuildingType,
                            UpgradeLevel = building.UpgradeLevel
                        });
                }
                gameInfo.SavedFactions.Add(initFaction);
            }
        }

        foreach (Ability PlayerAbility in PlayerFaction.CurrentAbilites)
        {
            gameInfo.PlayerAbilities.Add(PlayerAbility.AbilityName);
        }
        gameInfo.EnemyWorshipperCount = EnemyFaction.WorshipperCount;
        gameInfo.EnemyMorale = EnemyFaction.Morale;
        foreach (Ability PlayerAbility in EnemyFaction.CurrentAbilites)
        {
            gameInfo.EnemyAbilites.Add(PlayerAbility.AbilityName);
        }

        SceneManager.LoadScene("CombatMode");
    }

    // Use this for any initializations not needed by other scripts.
    void Start()
    {
        CreateRewardTree();
        NotifyPlayerOfAvaiableRewards();
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
            case MENUSTATE.Tier_Reward_State:
                CheckTierRewardStateInputs();
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

		if (CurrentMenuState == MENUSTATE.Default_State) {
			text1.textChange ("explore");
		}
		else if (CurrentMenuState == MENUSTATE.Building_State || CurrentMenuState == MENUSTATE.Buffered_Building_State) {
			text1.textChange ("building");
		}
		else if (CurrentMenuState == MENUSTATE.Moving_Building_State || CurrentMenuState == MENUSTATE.Building_Selected_State) {
			text1.textChange ("upgrade");
		}

		resourcetext.resourceUIUpdate (PlayerFaction.MaterialCount, PlayerFaction.WorshipperCount, PlayerFaction.Morale);
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
                    SelectedBuilding = GameMap.GetBuildings().Find(ClickedBuilding => ClickedBuilding.BuildingObject == SelectedGameObject);
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

            // Check if player has unlocked a new tier point
            if(PlayerFaction.WorshipperCount > TierWorshipperCount)
            {
                // Player has unlocked a new reward tier point
                PlayerFaction.TierRewardPoints++;
                TierWorshipperCount *= TierWoshipperCountMultiplier;
                NotifyPlayerOfAvaiableRewards();
            }
            Debug.Log(string.Format("{0}: Material Count({1}), Worshipper Count({2}), Morale({3}), Wor/sec({4}), Mat/sec({5}), MenuState({6}), RewardPoints({7})",
                PlayerFaction.GodName,
                PlayerFaction.MaterialCount,
                PlayerFaction.WorshipperCount,
                PlayerFaction.Morale,
                WorPerSec,
                MatPerSec,
                CurrentMenuState,
                PlayerFaction.TierRewardPoints));
        }

    }

    private void CheckBuildingStateInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CurrentMenuState = MENUSTATE.Default_State;
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
        else if (Input.GetKeyDown(KeyCode.V))
        {
            CurrentMenuState = MENUSTATE.Tier_Reward_State;
            EnableRewardsUI();
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
							sound.PlaySound ("PlaceBuilding");
                        }
                        else
                        {
                            Debug.Log("Building is at max upgrade level");
							sound.PlaySound ("NotMaterials");
                        }

                    }
                    else
                    {
                        Debug.Log(string.Format("Not enough materials to upgrade ({0} required)", intUpgradeCost));
						sound.PlaySound ("NotMaterials");
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
                // Owning faction is not player faction, enemy building
                if(SelectedBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE)
                {
                    // Enemy Village building, can start battle here
                    if(Input.GetKeyDown(KeyCode.B))
                    {
                        // Initialize info file variables, save game state, move to combat mode scene
                        EnterCombatMode(SelectedBuilding.OwningFaction);
                    }
                }
            }
        }
    }

    private void CheckTierRewardStateInputs()
    {
        List<TierReward> UnlockableRewards = null;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CurrentMenuState = MENUSTATE.Default_State;
            DisableRewardsUI();
        }

        if(PlayerFaction.TierRewardPoints > 0)
        {
            // Check if the input is between 1-9
            for (int intRewardNum = 1; intRewardNum < 9; intRewardNum++)
            {
                if (Input.GetKeyDown(Convert.ToString(intRewardNum)))
                {
                    UnlockableRewards = GetUnlockableRewards();
                    // Check if the reward to unlock exists
                    if (intRewardNum <= UnlockableRewards.Count)
                    {
                        // Player has selected a reward, and has enough to unlock it
                        switch(UnlockableRewards[intRewardNum-1].RewardType)
                        {
                            case TierReward.REWARDTYPE.Ability:
                                PlayerFaction.CurrentAbilites.Add(UnlockableRewards[intRewardNum-1].UnlockAbility());
                                break;
                        }
                        UnlockableRewards[intRewardNum-1].Unlocked = true;
                        PlayerFaction.TierRewardPoints--;
                        EnableRewardsUI();
                    }
                }
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

    private void CreateRewardTree()
    {
        // Create a reward tree for the player
        // Static at the moment since only one player god is defined
        // Single line tree for demo, only one tree path for now
        TierReward BasePlayerTierReward;
        TierReward NextPlayerTierReward;
        PlayerRewardTree = new List<TierReward>();

        // First tier, player gets this tier upon game start
        BasePlayerTierReward = new TierReward(0, "ThrowMushroom", "Starting Mushroom God ability to smite enemies with divine mushroom punishment.");
        PlayerRewardTree.Add(BasePlayerTierReward);

        // Second tier, unlocked at 1 * TierCount (100)
        NextPlayerTierReward = new TierReward(1, "SecondAbility", "Second Ability", BasePlayerTierReward);
        PlayerRewardTree.Add(NextPlayerTierReward);
        BasePlayerTierReward = NextPlayerTierReward;

        // Third tier, unlocked at 2 * TierCount (200). Final tier for Demo
        NextPlayerTierReward = new TierReward(2, "ThirdAbility", "Third Ability", BasePlayerTierReward);
        PlayerRewardTree.Add(NextPlayerTierReward);
    }

    private List<TierReward> GetUnlockableRewards()
    {
        // Get this list of all rewards that can be unlocked now.
        return PlayerRewardTree.FindAll(UnlockableReward => 
        ((UnlockableReward.PreviousRequiredReward!= null 
        && UnlockableReward.PreviousRequiredReward.Unlocked) 
        || UnlockableReward.PreviousRequiredReward == null) 
        && !UnlockableReward.Unlocked);
    }

    private void EnableRewardsUI()
    {
        string strRewardsText = string.Empty;
        RewardUI.SetActive(true);
        strRewardsText = string.Format("Current Reward Points: {0}, Next Point at {1} worshippers\n", PlayerFaction.TierRewardPoints, TierWorshipperCount);
        if(PlayerFaction.TierRewardPoints > 0)
        {
            foreach (TierReward UnlockableReward in GetUnlockableRewards())
            {
                strRewardsText += string.Format("1. {0}\n", UnlockableReward.RewardName);
            }
        }
        RewardUI.GetComponent<Text>().text = strRewardsText;
    }

    private void DisableRewardsUI()
    {
        RewardUI.GetComponent<Text>().text = "";
        RewardUI.SetActive(false);
    }

    private void NotifyPlayerOfAvaiableRewards()
    {
        if(CurrentMenuState != MENUSTATE.Tier_Reward_State)
        {
            RewardUI.SetActive(true);
            RewardUI.GetComponent<Text>().text = "A reward is available, Press 'V' to see!";
        }
    }
}
