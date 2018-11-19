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
        Tier_Reward_State,
        Upgrade_State,
        Paused_State
    }
    public int InitialPlayerMaterials = 0;
    public int InitialPlayerWorshippers = 0;
	public Texture mapTexture;
    public GameObject GameInfoObjectPrefab;
    private GameInfo gameInfo;
    modeTextScript text1;
	resourceScript resourcetext;
    public GameObject RewardUI;
    public GameObject RewardNotifier;
    public GameObject AudioObject;
    private ExecuteSound sound;
    public TerrainMap GameMap;
    public Building PlayerVillage;
    public List<Building> EnemyVillages;
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
    public int MapTierCount = 3;
    public int EnemiesPerTier = 3;
    private int CurrentTier = 0;
    public int TierDifficultyIncrease = 10; // Each tier gets X*Tier buildings to start
    private MENUSTATE LastMenuState = MENUSTATE.Default_State;
    private List<float> MaterialMultipliers = new List<float>();
    private List<float> WorshipperMultipliers = new List<float>();
    public int EnemyChallengeTimer = 300; // Seconds until an enemy attacks
    private int CurrentTimer = 0;
    public float BuildingRadius = 10;
    // Use this for any initializations needed by other scripts
    void Awake()
    {
        Building.BuildingRadiusSize = BuildingRadius;
		text1 = FindObjectOfType<modeTextScript>();
		resourcetext = FindObjectOfType<resourceScript>();
        sound = AudioObject.GetComponent<ExecuteSound>();
        Building bldEnemyBuilding = null;
        Faction facEnemyFaction = null;
        Vector3 vec3VillagePos;
        List<Faction.GodType> GodTypes;
        List<Faction> CurrentTierFactions;
        float TierRadius = (MapRadius / 2) / MapTierCount;
        int Attempts = 0;
        if (!InitializeGameInfoObject())
        {
            // Starting new game scene, initialize map, players, and buildings
            CurrentFactions = new List<Faction>();
            EnemyFactions = new List<Faction>();
            //Create map terrain
			GameMap = new TerrainMap(MapRadius, mapTexture);

            // Create the player Faction
            //Mushroom for now
            PlayerFaction = new Faction("YourGod", Faction.GodType.Mushroom, 0)
            {
                WorshipperCount = InitialPlayerWorshippers,
                MaterialCount = InitialPlayerMaterials
            };
            CurrentFactions.Add(PlayerFaction);
            // Create tier one factions

            for(int TierIndex = 0; TierIndex < MapTierCount; TierIndex++)
            {
                if(TierIndex == 0)
                {
                    GodTypes = new List<Faction.GodType>(Faction.TierOneGods);
                    GodTypes.Remove(PlayerFaction.Type);
                }
                else if(TierIndex == 1)
                {
                    GodTypes = new List<Faction.GodType>(Faction.TierTwoGods);
                }
                else
                {
                    GodTypes = new List<Faction.GodType>(Faction.TierThreeGods);
                }
                for(int enemyCount = 0; enemyCount < EnemiesPerTier; enemyCount++)
                {
                    facEnemyFaction = new Faction("EnemyGod" + enemyCount, GodTypes[enemyCount], TierIndex)
                    {
                        FactionDifficulty = (enemyCount + 1) + ((TierIndex + 1) * TierDifficultyIncrease),
                        MaterialCount = (int)Math.Pow(10, TierIndex + 2),
                        WorshipperCount = (int)Math.Pow(10,TierIndex+2),
                    };
                    CurrentFactions.Add(facEnemyFaction);
                    EnemyFactions.Add(facEnemyFaction);
                }
                CurrentTierFactions = CurrentFactions.FindAll(MatchingFaction => MatchingFaction.GodTier == TierIndex);
                GameMap.DivideMap(CurrentTierFactions, TierRadius * TierIndex, TierRadius * (TierIndex + 1));
            }
            GodTypes = new List<Faction.GodType>();
            foreach(Faction faction in CurrentFactions)
            {
                GodTypes.Add(faction.Type);
            }
            Building.LoadBuildingResources(GodTypes);
            //Create and place player village
            PlayerVillage = new Building(Building.BUILDING_TYPE.VILLAGE, PlayerFaction, BuildingCostModifier);
            vec3VillagePos = GameMap.CalculateRandomPosition(PlayerFaction);
            GameMap.PlaceBuilding(PlayerVillage, vec3VillagePos);

            //Create and place enemy villages
            foreach (Faction enemyFaction in EnemyFactions)
            {
                bldEnemyBuilding = new Building(Building.BUILDING_TYPE.VILLAGE, enemyFaction, BuildingCostModifier);
                vec3VillagePos = GameMap.CalculateRandomPosition(enemyFaction);
                while(!GameMap.PlaceBuilding(bldEnemyBuilding, vec3VillagePos) && Attempts < 100)
                {
                    vec3VillagePos = GameMap.CalculateRandomPosition(enemyFaction);
                    Attempts++;
                }
                Attempts = 0;
                // Generate starting buildings based on enemy difficulty
                for(int i = 0; i < enemyFaction.FactionDifficulty; i++)
                {
                    PlaceRandomBuilding(enemyFaction);
                }
            }

            // Hide all buildings outside starting tier
            CurrentTierFactions = CurrentFactions.FindAll(MatchingFaction => MatchingFaction.GodTier != 0);
            foreach(Faction faction in CurrentTierFactions)
            {
                faction.SetHidden(true);
            }
            GameMap.DrawFactionArea(PlayerFaction);
			//GameMap.DrawMultipleFactionAreas(CurrentFactions);
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
            CurrentTier = gameInfo.CurrentTier;
            CurrentFactions = new List<Faction>();
            EnemyFactions = new List<Faction>();
            // Create scene with values from gameInfo
            // Load Game Map
            GameMap = new TerrainMap(gameInfo.MapRadius, mapTexture);
            // Load factions
            foreach(GameInfo.SavedFaction savedFaction in gameInfo.SavedFactions)
            {
                InitFaction = new Faction(savedFaction.GodName, savedFaction.Type, savedFaction.GodTier)
                {
                    MaterialCount = savedFaction.MatieralCount,
                    Morale = savedFaction.Morale > MinimumMorale ? savedFaction.Morale : MinimumMorale,
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
            PlayerFaction = new Faction(gameInfo.PlayerFaction.GodName, gameInfo.PlayerFaction.Type, gameInfo.PlayerFaction.GodTier)
            {
                MaterialCount = gameInfo.PlayerFaction.MatieralCount,
                Morale = gameInfo.PlayerFaction.Morale  > MinimumMorale ? PlayerFaction.Morale : MinimumMorale,
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

            EnemyFaction = new Faction(gameInfo.EnemyFaction.GodName, gameInfo.EnemyFaction.Type, gameInfo.EnemyFaction.GodTier)
            {
                MaterialCount = gameInfo.EnemyFaction.MatieralCount,
                Morale = gameInfo.EnemyFaction.Morale > MinimumMorale ? EnemyFaction.Morale : MinimumMorale,
                WorshipperCount = gameInfo.EnemyFaction.WorshipperCount,
                FactionArea = gameInfo.EnemyFaction.FactionArea
            };
            foreach (GameInfo.SavedBuilding building in gameInfo.EnemyFaction.OwnedBuildings)
            {
                InitBuilding = new Building(building.BuildingType, EnemyFaction);
                GameMap.PlaceBuilding(InitBuilding, building.BuildingPosition);
            }
            


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
                    building.ReloadBuildingObject();
                    PlayerFaction.OwnedBuildings.Add(building);
                }
                // Check if that was the last enemy in that tier, if so, unlock next tier
                if(CurrentFactions.FindAll(enemyFaction => enemyFaction.GodTier == CurrentTier && enemyFaction != PlayerFaction).Count == 0)
                {
                    // Check if there a no more gods in the next tier too
                    if(CurrentFactions.FindAll(enemyFaction => enemyFaction.GodTier == CurrentTier + 1 && enemyFaction != PlayerFaction).Count == 0)
                    {
                        // No gods in the current tier, no gods in the next tier => game over you win
                        // END GAME
                    }
                    UnlockNextTier();
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
            foreach(Faction faction in CurrentFactions.FindAll(MatchingFaction => MatchingFaction.GodTier > CurrentTier))
            {
                faction.SetHidden(true);
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
        // save player faction
        gameInfo.PlayerFaction = GameInfo.CreateSavedFaction(PlayerFaction);

        // save challenging faction
        gameInfo.EnemyFaction = GameInfo.CreateSavedFaction(EnemyFaction);

        // Save the rest of the factions
        gameInfo.SavedFactions = new List<GameInfo.SavedFaction>();
        foreach(Faction faction in EnemyFactions)
        {
            if(faction != EnemyFaction)
            {
                gameInfo.SavedFactions.Add(GameInfo.CreateSavedFaction(faction));
            }
        }
        
        gameInfo.MapRadius = MapRadius;
        gameInfo.CurrentTier = CurrentTier;
        gameInfo.PlayerRewards = SaveRewardTree(PlayerRewardTree);
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
            case MENUSTATE.Paused_State:
                CheckPausedStateInputs();
                break;
            case MENUSTATE.Upgrade_State:
                CheckUpgradeStateInput();
                break;
        }
        // Global pause hotkey, game can be paused from any menu state
        if(Input.GetKeyDown(KeyCode.P) && CurrentMenuState != MENUSTATE.Paused_State)
        {
            SetPaused(true);
        }
        if(CurrentMenuState != MENUSTATE.Paused_State && CurrentMenuState != MENUSTATE.Tier_Reward_State)
        {
            if (BufferedBuilding != null)
            {
                UpdateBufferedBuilding();
            }
            else
            {
                CheckForSelectedBuilding();
            }
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

    private void CheckUpgradeStateInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CurrentMenuState = MENUSTATE.Building_Selected_State;
            SetUpgradeUIActive(false);
        }
    }

    private void CheckPausedStateInputs()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SetPaused(false);
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
        Building tempBuilding = null;
        Faction ChallengingFaction = null;
        int intHousingTotal = 0;
        int intMaterialsToAdd = 0;
        int intWorshippersToAdd = 0;

        float WorPerSec = 0;
        float MatPerSec = 0;
        if (CurrentFactions != null && CurrentMenuState != MENUSTATE.Paused_State)
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
                        foreach(float multiplier in MaterialMultipliers)
                        {
                            intMaterialsToAdd += (int)((multiplier - 1) * intMaterialsToAdd);
                        }
                        foreach (float multiplier in WorshipperMultipliers)
                        {
                            intWorshippersToAdd += (int)((multiplier - 1) * intWorshippersToAdd);
                        }
                        WorPerSec = (float)intWorshippersToAdd / 2;
                        MatPerSec = (float)intMaterialsToAdd / 2;
                    }
                    else
                    {
                        // Check if enemy has enough resources to upgrade a building, if so upgrade a random building
                        // Everything costs double for bots to allow player to eventually catch up
                        if(CurrentFaction.MaterialCount > 100)
                        {
                            tempBuilding = CurrentFaction.OwnedBuildings.Find(MatchingBuilding => MatchingBuilding.UpgradeLevel <= 2);
                            if(tempBuilding != null)
                            {
                                tempBuilding.UpgradeBuilding(false);
                                CurrentFaction.MaterialCount -= 2 * (Building.CalculateBuildingUpgradeCost(tempBuilding.BuildingType, BuildingCostModifier));
                            }
                            else
                            {
                                // Try to place a new building
                                tempBuilding = PlaceRandomBuilding(CurrentFaction);
                                if (tempBuilding != null)
                                {
                                    CurrentFaction.MaterialCount -= 2 * tempBuilding.BuildingCost;
                                }
                            }
                        }
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
                        if (CurrentFaction.Morale > MinimumMorale)
                        {
                            CurrentFaction.Morale -= 0.05f;
                        }
                        else
                        {
                            CurrentFaction.Morale = MinimumMorale;
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
        CurrentTimer += 2;
        if(CurrentTimer > EnemyChallengeTimer)
        {
            ChallengingFaction = CurrentFactions.Find(MatchingFaction => MatchingFaction != PlayerFaction && MatchingFaction.GodTier == CurrentTier);
            Debug.Log(string.Format("{0} has challenged you!", ChallengingFaction.GodName));
            EnterCombatMode(ChallengingFaction);
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
        else if (Input.GetKeyDown(KeyCode.F))
        {
            // Only allow 1 upgrade building
            if(PlayerFaction.OwnedBuildings.Find(upgradeBuilding => upgradeBuilding.BuildingType == Building.BUILDING_TYPE.UPGRADE) == null)
            BufferBuilding(Building.BUILDING_TYPE.UPGRADE);
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
            SetRewardsUIActive();
        }
        else if(Input.GetKeyDown(KeyCode.T))
        {
            // Cheat to move onto next tier
            UnlockNextTier();
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
                else if(Input.GetKeyDown(KeyCode.X))
                {
                    CurrentMenuState = MENUSTATE.Upgrade_State;
                    SetUpgradeUIActive();
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CurrentMenuState = MENUSTATE.Default_State;
            SetRewardsUIActive(false);
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
        BasePlayerTierReward = new TierReward("ThrowMushroom", "Starting Mushroom God ability to smite enemies with divine mushroom punishment.");
        PlayerRewardTree.Add(BasePlayerTierReward);

        // Second tier, unlocked at 1 * TierCount (100)
        NextPlayerTierReward = new TierReward("SecondAbility", "Second Ability", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        BasePlayerTierReward = NextPlayerTierReward;

        // Third tier, unlocked at 2 * TierCount (200). Final tier for Demo
        NextPlayerTierReward = new TierReward("ThirdAbility", "Third Ability", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("Fourth Ability", "Fourth Ability", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("Fifth Ability", "Fifth Ability", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        BasePlayerTierReward = NextPlayerTierReward;

        NextPlayerTierReward = new TierReward("Sixth Ability", "Sixth Ability", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("Seventh Ability", "Sixth Ability", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        BasePlayerTierReward = new TierReward("Materials", "100 Materials", TierReward.RESOURCETYPE.Material, 100);
        PlayerRewardTree.Add(BasePlayerTierReward);

        NextPlayerTierReward = new TierReward("2xMat", "2x material growth", TierReward.RESOURCETYPE.Material, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("2xWorshipper", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        BasePlayerTierReward = NextPlayerTierReward;

        NextPlayerTierReward = new TierReward("2xWorshipperA", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

		NextPlayerTierReward = new TierReward("2xWorshipperB", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
		BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        //PlayerRewardTree.Add(new TierReward("Worshippers", "100 Worshippers", TierReward.RESOURCETYPE.Worshipper, 100));

        //PlayerRewardTree.Add(new TierReward("WorshippersA", "100 Worshippers", TierReward.RESOURCETYPE.Worshipper, 100));
        if (gameInfo.PlayerRewards.Count > 0 )
        {
            LoadRewardTree(gameInfo.PlayerRewards);
        }
        RewardUI.GetComponentInChildren<PopulateTierIcons>().InitializeButtons(PlayerRewardTree);
        RewardUI.SetActive(false);
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

    private void SetRewardsUIActive(bool blnActive = true)
    {
        RewardUI.SetActive(blnActive);
        RewardNotifier.SetActive(false);
        Camera.main.GetComponent<Cam>().CameraMovementEnabled = !blnActive;
    }

    private void SetUpgradeUIActive(bool blnActive = true)
    {
        // Enable/ disable upgrade UI
        Camera.main.GetComponent<Cam>().CameraMovementEnabled = !blnActive;
    }


    private void NotifyPlayerOfAvaiableRewards()
    {
        if(CurrentMenuState != MENUSTATE.Tier_Reward_State)
        {
            RewardNotifier.SetActive(true);
            RewardNotifier.GetComponent<Text>().text = "A reward is available, Press 'V' to see!";
        }
    }

    private void UnlockNextTier()
    {
        
        CurrentTier++;
        List<Faction> NextTierFactions = CurrentFactions.FindAll(MatchingFaction => MatchingFaction.GodTier == CurrentTier);
        if (NextTierFactions.Count > 0)
        {
            foreach(Faction factionToShow in NextTierFactions)
            {
                factionToShow.SetHidden(false);
            }
        }
        else
        {
            // You've won
        }
    }

    private TierReward FindRewardByName(string RewardName, List<TierReward> Rewards)
    {
        TierReward Found = null;
        if(Rewards != null)
        {
            foreach (TierReward reward in Rewards)
            {
                if (reward.RewardName.Equals(RewardName))
                {
                    return reward;
                }
                Found = FindRewardByName(RewardName, reward.ChildRewards);
                if(Found != null)
                {
                    return Found;
                }
            }
        }
        
        return null;
    }
    
    public bool UnlockReward(TierReward reward)
    {
        bool AbleToUnlock = false;
        //check if this reward can be unlocked
        AbleToUnlock = (reward.PreviousRequiredReward == null || reward.PreviousRequiredReward.Unlocked) && PlayerFaction.TierRewardPoints > 0 && !reward.Unlocked;
        if(AbleToUnlock)
        {
            PlayerFaction.TierRewardPoints--;
            reward.Unlocked = true;
            switch(reward.RewardType)
            {
                case TierReward.REWARDTYPE.Ability:
                    PlayerFaction.CurrentAbilites.Add(reward.UnlockAbility());
                    break;
                case TierReward.REWARDTYPE.Resource:
                    switch(reward.ResourceType)
                    {
                        case TierReward.RESOURCETYPE.Material:
                            PlayerFaction.MaterialCount += reward.Amount;
                            break;
                        case TierReward.RESOURCETYPE.Worshipper:
                            PlayerFaction.WorshipperCount += reward.Amount;
                            break;
                    }
                    break;
                case TierReward.REWARDTYPE.ResourceMultiplier:
                    switch(reward.ResourceType)
                    {
                        case TierReward.RESOURCETYPE.Material:
                            MaterialMultipliers.Add(reward.Multiplier);
                            break;
                        case TierReward.RESOURCETYPE.Worshipper:
                            WorshipperMultipliers.Add(reward.Multiplier);
                            break;
                    }
                    break;
                default:
                    //do nothing
                    break;
            }
        }
        return AbleToUnlock;
    }

    public void SetPaused(bool pause = true)
    {

        
        if(pause)
        {
            Time.timeScale = 0;
            Camera.main.GetComponent<Cam>().CameraMovementEnabled = false;
            LastMenuState = CurrentMenuState;
            CurrentMenuState = MENUSTATE.Paused_State;
        }
        else
        {
            Time.timeScale = 1;
            Camera.main.GetComponent<Cam>().CameraMovementEnabled = true;
            CurrentMenuState = LastMenuState;
        }
    }

    public List<string> SaveRewardTree(List<TierReward> rewards)
    {
        List<string> savedRewards = new List<string>();
        List<string> childRewards = null;
        foreach(TierReward reward in rewards)
        {
            if(reward.Unlocked)
            {
                savedRewards.Add(reward.RewardName);
                childRewards = SaveRewardTree(reward.ChildRewards);
                if (childRewards != null)
                {
                    foreach (string savedReward in childRewards)
                    {
                        savedRewards.Add(savedReward);
                    }
                }
            }
        }
        return savedRewards;
    }

    public void LoadRewardTree(List<string> savedRewards)
    {
        TierReward reward; 
        foreach(string savedReward in savedRewards)
        {
            reward = FindRewardByName(savedReward, PlayerRewardTree);
            if(reward != null)
            {
                reward.Unlocked = true;
            }
        }
    }

    private Building PlaceRandomBuilding(Faction placingFaction)
    {
        Building.BUILDING_TYPE RandomType;
        Building blnRandomBuilding = null;
        switch ((int)(UnityEngine.Random.value * 100 / 25))
        {
            case 0:
                RandomType = Building.BUILDING_TYPE.ALTAR;
                break;
            case 1:
                RandomType = Building.BUILDING_TYPE.HOUSING;
                break;
            case 2:
                RandomType = Building.BUILDING_TYPE.MATERIAL;
                break;
            case 3:
                RandomType = Building.BUILDING_TYPE.ALTAR;
                break;
            default:
                RandomType = Building.BUILDING_TYPE.MATERIAL;
                break;
        }
        // Place a random building for that faction
        blnRandomBuilding = new Building(RandomType, placingFaction);
        if (!GameMap.PlaceBuilding(blnRandomBuilding, GameMap.CalculateRandomPosition(placingFaction)))
        {
            blnRandomBuilding.Destroy();
            return null;
        }
        return blnRandomBuilding;
    }
}
