using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
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
	ResourceUIScript ResourceUIController;
    public GameObject RewardUI;
    public GameObject AudioObject;
    private ExecuteSound sound;
    public TerrainMap GameMap;
    public Building PlayerVillage;
    public float MapRadius;
    public List<Faction> CurrentFactions;
    public int BuildingCostModifier = 1;
    private MENUSTATE CurrentMenuState = MENUSTATE.Default_State;
    public Building BufferedBuilding = null;
	public Faction PlayerFaction = null;
    public List<Faction> EnemyFactions = null;
    private GameObject SelectedGameObject = null;
    public Building SelectedBuilding = null;
    private Vector3 OriginalBuildingPosition;
    public float MinimumMorale = 0.2f;
    public List<TierReward> PlayerRewardTree;
    public int TierUnlockPoint = 100; // Initial tier unlock count
    public int TierUnlockPointMultiplier = 2; // After every tier, tier count is multiplied by this
    public int MapTierCount = 3;
    public int EnemiesPerTier = 3;
    public int CurrentTier = 0;
    public int TierDifficultyIncrease = 10; // Each tier gets X*Tier buildings to start
    private MENUSTATE LastMenuState = MENUSTATE.Default_State;
    private List<float> MaterialMultipliers = new List<float>();
    private List<float> WorshipperMultipliers = new List<float>();
    public int EnemyChallengeTimer = 300; // Seconds until an enemy attacks
    private int CurrentTimer = 0;
    public float BuildingRadius = 10;
    public int ResourceTicks { get; private set; }
    public GameObject PausedMenuPanel = null;
    public GameObject MenuControlObject = null;
    public MenuPanelControls MenuPanelController { get; private set; }
    public float PlayerMoraleCap = 1.0f;
    // Use this for any initializations needed by other scripts
    void Awake()
    {
        Building.BuildingRadiusSize = BuildingRadius;
        Building.BuildingCostModifier = BuildingCostModifier;
		ResourceUIController = FindObjectOfType<ResourceUIScript>();
        sound = AudioObject.GetComponent<ExecuteSound>();
        MenuPanelController = MenuControlObject.GetComponent<MenuPanelControls>();
        Building bldEnemyBuilding = null;
        Faction facEnemyFaction = null;
        Vector3 vec3BuildngPos;
        List<Faction.GodType> GodTypes;
        List<Faction> CurrentTierFactions;
        List<string> GodNames = new List<string>(Faction.GodNames);
        GodNames.Shuffle();
        
        float TierRadius = (MapRadius / 2) / MapTierCount;
        int Attempts = 0;
        if (!InitializeGameInfoObject())
        {
            // Remove chance of player having same name as another god.
            GodNames.Remove(gameInfo.PlayerFaction.GodName);
            // Starting new game scene, initialize map, players, and buildings
            CurrentFactions = new List<Faction>();
            EnemyFactions = new List<Faction>();
            //Create map terrain
			GameMap = new TerrainMap(MapRadius, mapTexture);

            // Create the player Faction
            PlayerFaction = new Faction(gameInfo.PlayerFaction.GodName, gameInfo.PlayerFaction.Type, 0)
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
                    GodTypes.Shuffle();
                }
                else if(TierIndex == 1)
                {
                    GodTypes = new List<Faction.GodType>(Faction.TierTwoGods);
                    GodTypes.Shuffle();
                }
                else
                {
                    GodTypes = new List<Faction.GodType>(Faction.TierThreeGods);
                    GodTypes.Shuffle();
                }
                for(int enemyCount = 0; enemyCount < EnemiesPerTier; enemyCount++)
                {
                    facEnemyFaction = new Faction(GodNames[enemyCount + TierIndex * EnemiesPerTier], GodTypes[enemyCount], TierIndex)
                    {
                        FactionDifficulty = (enemyCount + 1) + ((TierIndex + 1) * TierDifficultyIncrease),
                        MaterialCount = (int)Math.Pow(10, TierIndex + 2),
                        WorshipperCount = (int)Math.Pow(10,TierIndex+2),
                    };
                    facEnemyFaction.CurrentAbilites = Faction.GetGodAbilities(facEnemyFaction.Type);
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
            PlayerVillage = new Building(Building.BUILDING_TYPE.VILLAGE, PlayerFaction);
            vec3BuildngPos = GameMap.CalculateRandomPosition(PlayerFaction);
            GameMap.PlaceBuilding(PlayerVillage, vec3BuildngPos);

            //Create and place enemy villages
            foreach (Faction enemyFaction in EnemyFactions)
            {
                bldEnemyBuilding = new Building(Building.BUILDING_TYPE.VILLAGE, enemyFaction);
                vec3BuildngPos = GameMap.CalculateRandomPosition(enemyFaction);
                while(!GameMap.PlaceBuilding(bldEnemyBuilding, vec3BuildngPos) && Attempts < 100)
                {
                    vec3BuildngPos = GameMap.CalculateRandomPosition(enemyFaction);
                    Attempts++;
                }
                for (int i = 0; i < enemyFaction.GodTier; i++)
                {
                    bldEnemyBuilding.UpgradeBuilding();
                }
                Attempts = 0;
                // Generate starting buildings based on enemy difficulty
                for(int i = 0; i < enemyFaction.FactionDifficulty; i++)
                {
                    PlaceRandomBuilding(enemyFaction);
                }
                for (int i = 0; i < enemyFaction.GodTier + 1; i++)
                {
                    bldEnemyBuilding = new MineBuilding(Building.BUILDING_TYPE.MATERIAL, enemyFaction);
                    vec3BuildngPos = GameMap.CalculateRandomPosition(enemyFaction);
                    while (!GameMap.PlaceBuilding(bldEnemyBuilding, vec3BuildngPos) && Attempts < 100)
                    {
                        vec3BuildngPos = GameMap.CalculateRandomPosition(enemyFaction);
                        Attempts++;
                    }
                    Attempts = 0;
                    for (int j = 0; j < enemyFaction.GodTier; j++)
                    {
                        bldEnemyBuilding.UpgradeBuilding(false,true);
                    }
                    ((MineBuilding)bldEnemyBuilding).Miners = ((MineBuilding)bldEnemyBuilding).MinerCap;
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
        List<Faction> arrFactionsLeft = null;
        List<Faction.GodType> GodTypes = null;
        bool blnFromSave = false;
        if (GameInfoObject != null)
        {
            // Found a gameinfo object, load values
            gameInfo = GameInfoObject.GetComponent<GameInfo>();
            blnFromSave = gameInfo.FromSave;
            if (!gameInfo.NewGame)
            {
                if(blnFromSave)
                {
                    // Load building resources as they may not have been loaded yet.
                    GodTypes = new List<Faction.GodType>();
                    GodTypes.Add(gameInfo.PlayerFaction.Type);
                    foreach(GameInfo.SavedFaction savedFaction in gameInfo.SavedFactions)
                    {
                        GodTypes.Add(savedFaction.Type);
                    }
                    Building.LoadBuildingResources(GodTypes);
                }
                CurrentTier = gameInfo.CurrentTier;
                PlayerMoraleCap = gameInfo.PlayerMoraleCap;
                CurrentFactions = new List<Faction>();
                EnemyFactions = new List<Faction>();
                // Create scene with values from gameInfo
                // Load Game Map
                GameMap = new TerrainMap(gameInfo.MapRadius, mapTexture);
                // Load factions
                foreach (GameInfo.SavedFaction savedFaction in gameInfo.SavedFactions)
                {
                    InitFaction = new Faction(savedFaction.GodName, savedFaction.Type, savedFaction.GodTier)
                    {
                        MaterialCount = savedFaction.MatieralCount,
                        Morale = savedFaction.Morale > MinimumMorale ? savedFaction.Morale : MinimumMorale,
                        WorshipperCount = savedFaction.WorshipperCount,
                        FactionArea = new List<float[]>()
                    };
                    foreach(GameInfo.SavedArea area in savedFaction.FactionArea)
                    {
                        InitFaction.FactionArea.Add(
                            new float[]
                            {
                                area.StartingRad,
                                area.EndingRad,
                                area.StartingAngle,
                                area.EndingAngle
                            });
                    }
                    CurrentFactions.Add(InitFaction);
                    EnemyFactions.Add(InitFaction);
                    foreach (GameInfo.SavedBuilding building in savedFaction.OwnedBuildings)
                    {
                        switch (building.BuildingType)
                        {
                            case Building.BUILDING_TYPE.MATERIAL:
                                InitBuilding = new MineBuilding(building, InitFaction);
                                break;
                            default:
                                InitBuilding = new Building(building, InitFaction);
                                break;
                        }
                        GameMap.PlaceBuilding(InitBuilding, new Vector3(building.x, building.y, building.z));
                    }
                    foreach(string AbilityName in savedFaction.Abilities)
                    {
                        InitFaction.CurrentAbilites.Add(GameInfo.LoadAbility(AbilityName));
                    }
                }
                PlayerFaction = new Faction(gameInfo.PlayerFaction.GodName, gameInfo.PlayerFaction.Type, gameInfo.PlayerFaction.GodTier)
                {
                    MaterialCount = gameInfo.PlayerFaction.MatieralCount,
                    Morale = gameInfo.PlayerFaction.Morale > MinimumMorale ? gameInfo.PlayerFaction.Morale : MinimumMorale,
                    WorshipperCount = gameInfo.PlayerFaction.WorshipperCount,
                    FactionArea = new List<float[]>()
                };
                foreach (GameInfo.SavedArea area in gameInfo.PlayerFaction.FactionArea)
                {
                    PlayerFaction.FactionArea.Add(
                        new float[]
                        {
                                area.StartingRad,
                                area.EndingRad,
                                area.StartingAngle,
                                area.EndingAngle
                        });
                }
                foreach (GameInfo.SavedBuilding building in gameInfo.PlayerFaction.OwnedBuildings)
                {
                    switch (building.BuildingType)
                    {
                        case Building.BUILDING_TYPE.MATERIAL:
                            InitBuilding = new MineBuilding(building, PlayerFaction);
                            break;
                        default:
                            InitBuilding = new Building(building, PlayerFaction);
                            break;
                    }

                    GameMap.PlaceBuilding(InitBuilding, new Vector3(building.x, building.y, building.z));
                    if (building.BuildingType == Building.BUILDING_TYPE.VILLAGE)
                    {
                        PlayerVillage = InitBuilding;
                    }
                }
                foreach(string AbilityName in gameInfo.PlayerFaction.Abilities)
                {
                    PlayerFaction.CurrentAbilites.Add(GameInfo.LoadAbility(AbilityName));
                }
                CurrentFactions.Add(PlayerFaction);
                if (!blnFromSave)
                {
                    EnemyFaction = new Faction(gameInfo.EnemyFaction.GodName, gameInfo.EnemyFaction.Type, gameInfo.EnemyFaction.GodTier)
                    {
                        MaterialCount = gameInfo.EnemyFaction.MatieralCount,
                        Morale = gameInfo.EnemyFaction.Morale > MinimumMorale ? EnemyFaction.Morale : MinimumMorale,
                        WorshipperCount = gameInfo.EnemyFaction.WorshipperCount,
                        FactionArea = new List<float[]>()
                    };
                    foreach (GameInfo.SavedArea area in gameInfo.EnemyFaction.FactionArea)
                    {
                        EnemyFaction.FactionArea.Add(
                            new float[]
                            {
                                area.StartingRad,
                                area.EndingRad,
                                area.StartingAngle,
                                area.EndingAngle
                            });
                    }
                    foreach (GameInfo.SavedBuilding building in gameInfo.EnemyFaction.OwnedBuildings)
                    {
                        switch (building.BuildingType)
                        {
                            case Building.BUILDING_TYPE.MATERIAL:
                                InitBuilding = new MineBuilding(building, EnemyFaction);
                                break;
                            default:
                                InitBuilding = new Building(building, EnemyFaction);
                                break;
                        }
                        GameMap.PlaceBuilding(InitBuilding, new Vector3(building.x, building.y, building.z));
                    }
                    foreach (string AbilityName in gameInfo.EnemyFaction.Abilities)
                    {
                        EnemyFaction.CurrentAbilites.Add(GameInfo.LoadAbility(AbilityName));
                    }



                    if (gameInfo.LastBattleStatus == GameInfo.BATTLESTATUS.Victory)
                    {
                        // Take over enemy factions buildings, area, and resources
                        foreach (float[] enemyArea in EnemyFaction.FactionArea)
                        {
                            PlayerFaction.FactionArea.Add(enemyArea);
                        }
                        PlayerFaction.MaterialCount += EnemyFaction.MaterialCount;
                        foreach (Building building in EnemyFaction.OwnedBuildings)
                        {
                            building.OwningFaction = PlayerFaction;
                            building.ReloadBuildingObject();
                            PlayerFaction.OwnedBuildings.Add(building);
                            if (building.BuildingType == Building.BUILDING_TYPE.MATERIAL)
                            {
                                ((MineBuilding)building).Miners = 0;
                            }
                        }
                        arrFactionsLeft = CurrentFactions.FindAll(enemyFaction => enemyFaction.GodTier == CurrentTier && enemyFaction != PlayerFaction);
                        // Check if that was the last enemy in that tier, if so, unlock next tier
                        if (arrFactionsLeft.Count == 0)
                        {
                            // Check if there a no more gods in the next tier too
                            if (CurrentFactions.FindAll(enemyFaction => enemyFaction.GodTier == CurrentTier + 1 && enemyFaction != PlayerFaction).Count == 0)
                            {
                                // No gods in the current tier, no gods in the next tier => game over you win
                                // END GAME
                            }
                            else
                            {
                                UnlockNextTier();
                            }
                        }
                        else
                        {
                            if (arrFactionsLeft.Count >= 2)
                            {
                                BattleTwoFactions(arrFactionsLeft[0], arrFactionsLeft[1]);
                            }
                        }
                        PlayerMoraleCap = PlayerMoraleCap + 0.4f < 1.0f ? PlayerMoraleCap + 0.4f : 1.0f;
                    }
                    else if (gameInfo.LastBattleStatus == GameInfo.BATTLESTATUS.Retreat)
                    {
                        PlayerMoraleCap = PlayerMoraleCap - 0.2f > 0.2f ? PlayerMoraleCap - 0.2f : 0.2f;
                        CurrentFactions.Add(EnemyFaction);
                    }
                    else
                    {
                        // Run defeat animation/reset to tier checkpoint
                    }
                }
                else
                {
                    EnemyChallengeTimer = gameInfo.EnemyChallengeTimer;
                }
                foreach (Faction faction in CurrentFactions.FindAll(MatchingFaction => MatchingFaction.GodTier > CurrentTier))
                {
                    faction.SetHidden(true);
                }
                GameMap.DrawFactionArea(PlayerFaction);
                return true;
            }
            else
            {
                // New game, create new game scene
                gameInfo.NewGame = false;
                return false;
            }
        }
        else
        {
#if DEBUG
            Debug.Log("Loaded directly into management mode, creating a default god.");
            GameObject NewGameInfoObject = (GameObject)Instantiate(GameInfoObjectPrefab);
            NewGameInfoObject.name = "GameInfo";
            gameInfo = NewGameInfoObject.GetComponent<GameInfo>();
            gameInfo.PlayerFaction.GodName = "TestGod";
            gameInfo.PlayerFaction.Type = Faction.GodType.Mushrooms;
            gameInfo.NewGame = false;
            return false;
#else
            // Couldn't find gameinfo object
            throw new Exception("Something went wrong :(");
#endif
        }
    }

    /// <summary>
    /// Wrapper method for entering combat mode on the currently selected building
    /// </summary>
    public void EnterCombatMode()
    {
        EnterCombatMode(SelectedBuilding.OwningFaction);
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
        gameInfo.SavedFactions = new GameInfo.SavedFaction[EnemyFactions.Count - 1];
        int intFactionIndex = 0;
        foreach(Faction faction in EnemyFactions)
        {
            if(faction != EnemyFaction)
            {
                gameInfo.SavedFactions[intFactionIndex] = GameInfo.CreateSavedFaction(faction);
                intFactionIndex++;
            }
        }
        
        gameInfo.MapRadius = MapRadius;
        gameInfo.CurrentTier = CurrentTier;
        gameInfo.PlayerRewards = SaveRewardTree(PlayerRewardTree).ToArray();
        gameInfo.PlayerMoraleCap = PlayerMoraleCap;
        gameInfo.FromSave = false;
        SceneManager.LoadScene("CombatMode");
    }

    public void BattleTwoFactions(Faction pobjFactionOne, Faction pobjFactionTwo)
    {
        Faction WinningFaction = null;
        Faction LosingFaction = null;
        if(pobjFactionOne.WorshipperCount >= pobjFactionTwo.WorshipperCount)
        {
            WinningFaction = pobjFactionOne;
            LosingFaction = pobjFactionTwo;
        }
        else
        {
            LosingFaction = pobjFactionOne;
            WinningFaction = pobjFactionTwo;
        }
        WinningFaction.MaterialCount += LosingFaction.MaterialCount;
        WinningFaction.WorshipperCount +=(int)(0.5 * LosingFaction.MaterialCount);
        foreach(float[] factionArea in LosingFaction.FactionArea)
        {
            WinningFaction.FactionArea.Add(factionArea);
        }
        foreach(Building loserBuilding in LosingFaction.OwnedBuildings)
        {
            loserBuilding.OwningFaction = WinningFaction;
            loserBuilding.ReloadBuildingObject();
            WinningFaction.OwnedBuildings.Add(loserBuilding);
        }
        CurrentFactions.Remove(LosingFaction);
        EnemyFactions.Remove(LosingFaction);
    }

    // Use this for any initializations not needed by other scripts.
    void Start()
    {
        GameInfo.ApplyGameSettings();
        PausedMenuPanel.SetActive(false);
        CreateRewardTree();
        ResourceTicks = 0;
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
            PauseGame();
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
		ResourceUIController.UpdateResourceUIElements (PlayerFaction.MaterialCount, PlayerFaction.WorshipperCount, PlayerFaction.Morale, PlayerFaction.TierRewardPoints);
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
            UnPauseGame();
        }
    }

    private void CheckForSelectedBuilding()
    {
        // If mouse is not over a UI object
        if(!EventSystem.current.IsPointerOverGameObject())
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
                                GoToDefaultMenuState();
                            }
                        }
                    }
                    else
                    {
                        SetSelectedBuilding(GameMap.GetBuildings().Find(ClickedBuilding => ClickedBuilding.BuildingObject == SelectedGameObject));
                    }
                }
            }
        }
    }

    public void SetSelectedBuilding(Building building)
    {
        if (SelectedBuilding != null)
        {
            SelectedBuilding.ToggleBuildingOutlines(false);
        }
        SelectedBuilding = building;
        if (SelectedBuilding != null)
        {
            EnterBuildingSelectedMenuState();
            SelectedBuilding.ToggleBuildingOutlines(true);
        }
    }

    private void UpdateBufferedBuilding()
    {
        // Only if mouse is not on a UI object
        if (!EventSystem.current.IsPointerOverGameObject())
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
                    foreach (Building BuildingOnMap in GameMap.GetBuildings())
                    {
                        BuildingOnMap.ToggleBuildingOutlines(false);
                    }
                    if (CurrentMenuState == MENUSTATE.Moving_Building_State)
                    {
                        SelectedBuilding = BufferedBuilding;
                        SelectedBuilding.ToggleBuildingOutlines(true);
                        EnterBuildingSelectedMenuState();
                    }
                    else
                    {
                        EnterBuildMenuState();
                    }
                    BufferedBuilding = null;
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
                        intHousingTotal += (int)(Math.Pow(10, VillageBuilding.UpgradeLevel + 1));
                    }

                    // Get all the material buildings
                    MaterialBuildings = OwnedBuildings.FindAll(MaterialBuilding => MaterialBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL);
                    foreach (Building MaterialBuilding in MaterialBuildings)
                    {
                        // Calculatae Resource growth
                        intMaterialsToAdd += (int)Math.Ceiling(0.1 * ((MineBuilding)MaterialBuilding).Miners * MaterialBuilding.UpgradeLevel);
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
                        // Check if buildings exist that can be upgraded
                        if(CurrentFaction.OwnedBuildings.FindAll(
                            upgradeableBuilding => upgradeableBuilding.UpgradeLevel < CurrentFaction.GodTier + 1 
                            && upgradeableBuilding.BuildingType != Building.BUILDING_TYPE.VILLAGE).Count > 0)
                        {
                            // A building exists that can be upgraded
                            tempBuilding = CurrentFaction.OwnedBuildings.Find(
                                upgradeableBuilding => upgradeableBuilding.UpgradeLevel < CurrentFaction.GodTier + 1
                                && upgradeableBuilding.BuildingType != Building.BUILDING_TYPE.VILLAGE);
                            if(CurrentFaction.MaterialCount > tempBuilding.CalculateBuildingUpgradeCost() * 2)
                            {
                                CurrentFaction.MaterialCount -= tempBuilding.CalculateBuildingUpgradeCost();
                                tempBuilding.UpgradeBuilding();
                            }
                        }
                        else
                        {
                            // No upgradeable buildings exist, build a new one
                            tempBuilding = CreateRandomBuilding(CurrentFaction);
                            if (CurrentFaction.MaterialCount > tempBuilding.BuildingCost * 2)
                            {
                                if(GameMap.PlaceBuilding(tempBuilding, GameMap.CalculateRandomPosition(CurrentFaction)))
                                {
                                    CurrentFaction.MaterialCount -= 2 * tempBuilding.BuildingCost;
                                    if(CurrentFaction.GodTier > CurrentTier)
                                    {
                                        tempBuilding.BuildingObject.SetActive(false);
                                    }
                                }
                                else
                                {
                                    tempBuilding.Destroy();
                                }
                            }
                            else
                            {
                                tempBuilding.Destroy();
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
                        intHousingTotal += (int)(Math.Pow(10, HousingBuilding.UpgradeLevel + 1));
                    }
                    //Calculate morale losses/gains
                    // Each housing/village building can hold 100 * upgrade level worshippers
                    if (intHousingTotal > CurrentFaction.WorshipperCount)
                    {
                        // There is enough housing for the current population, morale goes towards 100%
                        if (CurrentFaction.Morale <= PlayerMoraleCap)
                        {
                            CurrentFaction.Morale += 0.05f;
                            if (CurrentFaction.Morale > PlayerMoraleCap)
                            {
                                CurrentFaction.Morale = PlayerMoraleCap;
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
            if(PlayerFaction.WorshipperCount > TierUnlockPoint)
            {
                // Player has unlocked a new reward tier point
                PlayerFaction.TierRewardPoints++;
                TierUnlockPoint *= TierUnlockPointMultiplier;
            }
            //Debug.Log(string.Format("{0}: Material Count({1}), Worshipper Count({2}), Morale({3}), Wor/sec({4}), Mat/sec({5}), MenuState({6}), RewardPoints({7})",
            //    PlayerFaction.GodName,
            //    PlayerFaction.MaterialCount,
            //    PlayerFaction.WorshipperCount,
            //    PlayerFaction.Morale,
            //    WorPerSec,
            //    MatPerSec,
            //    CurrentMenuState,
            //    PlayerFaction.TierRewardPoints));
        }
        ResourceTicks++;
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
            GoToDefaultMenuState();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            BufferAltar();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            BufferMine();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            BufferHousing();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            BufferUpgrade();
        }
    }

    private void CheckMovingBuildingStateInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (Building BuildingOnMap in GameMap.GetBuildings())
            {
                BuildingOnMap.ToggleBuildingOutlines(false);
            }
            if (BufferedBuilding != null)
            {
                GameMap.PlaceBuilding(BufferedBuilding, OriginalBuildingPosition);
                BufferedBuilding.ToggleBuildingOutlines(true);
                BufferedBuilding.BuildingObject.GetComponent<Collider>().enabled = true;
            }
            SelectedBuilding = BufferedBuilding;
            BufferedBuilding = null;
            EnterBuildingSelectedMenuState();
        }
    }

    private void CheckDefaultMenuStateInputs()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            EnterBuildMenuState();
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            EnterTierRewardsMenuState();
        }
        else if(Input.GetKeyDown(KeyCode.T))
        {
            // Cheat to move onto next tier
            UnlockNextTier();
        }
    }

    private void CheckSelectedBuildingStateInputs()
    {
        if (SelectedBuilding != null)
        {
            // Player's building
            if (SelectedBuilding.OwningFaction == PlayerFaction)
            {
                // Global hotkey for a selected building
                if (Input.GetKeyDown(KeyCode.U) && SelectedBuilding.BuildingType != Building.BUILDING_TYPE.VILLAGE)
                {
                    // Attempt to upgrade selected building
                    // 3 is max upgrade level of a building
                    UpgradeSelectedBuilding();

                }
                else if (Input.GetKeyDown(KeyCode.M))
                {
                    //Move player building if it isn't a village
                    if (SelectedBuilding.BuildingType != Building.BUILDING_TYPE.VILLAGE)
                    {
                        EnterMovingBuildingState();
                    }
                    else
                    {
                        // TODO add cannot move feedback
                    }

                }
                else if(Input.GetKeyDown(KeyCode.X) && SelectedBuilding.BuildingType == Building.BUILDING_TYPE.UPGRADE)
                {
                    CurrentMenuState = MENUSTATE.Upgrade_State;
                    SetUpgradeUIActive();
                }
                else if(Input.GetKeyDown(KeyCode.K) && SelectedBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL)
                {
                    BuyMinersForSelectedBuilding();
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
                        EnterCombatMode();
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SelectedBuilding.ToggleBuildingOutlines(false);
            SelectedBuilding = null;
            GoToDefaultMenuState();
        }
    }

    private void CheckTierRewardStateInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToDefaultMenuState();
            SetRewardsUIActive(false);
        }        
    }
    public void BufferAltar()
    {
        BufferBuilding(Building.BUILDING_TYPE.ALTAR);
    }

    public void BufferMine()
    {
        BufferBuilding(Building.BUILDING_TYPE.MATERIAL);
    }

    public void BufferHousing()
    {
        BufferBuilding(Building.BUILDING_TYPE.HOUSING);
    }

    public void BufferUpgrade()
    {
        BufferBuilding(Building.BUILDING_TYPE.UPGRADE);
    }

    public void BufferBuilding(Building.BUILDING_TYPE penumBuildingType)
    {
        if(BufferedBuilding != null)
        {
            BufferedBuilding.Destroy();
            BufferedBuilding = null;
        }
        // Part 1: Evaluates to true if not an upgrade building, or is an upgrade building and player does not currently have an upgrade building
        // Ensures only one upgrade building exists at a time for the player
        if((penumBuildingType != Building.BUILDING_TYPE.UPGRADE 
            || PlayerFaction.OwnedBuildings.Find(upgradeBuilding => upgradeBuilding.BuildingType == Building.BUILDING_TYPE.UPGRADE) == null)
        // Part 2: Evaluates to true if not a material builidng, or is a material building and the player owns less than mines than territories
        // Ensures player only has one mine per territory
            && (penumBuildingType != Building.BUILDING_TYPE.MATERIAL ||
            PlayerFaction.OwnedBuildings.FindAll(materialBuilding => materialBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL).Count < PlayerFaction.FactionArea.Count))
        {
            // Check if user has enough resources to build the building
            int BuildingCost = Building.CalculateBuildingCost(penumBuildingType);
            if (PlayerFaction.MaterialCount >= BuildingCost)
            {
                if(penumBuildingType == Building.BUILDING_TYPE.MATERIAL)
                {
                    BufferedBuilding = new MineBuilding(penumBuildingType, PlayerFaction);
                }
                else
                {
                    BufferedBuilding = new Building(penumBuildingType, PlayerFaction);
                }
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
        BasePlayerTierReward = new TierReward("Throw Mushroom");
        PlayerRewardTree.Add(BasePlayerTierReward);

        // Second tier, unlocked at 1 * TierCount (100)
        NextPlayerTierReward = new TierReward("Eat Mushroom", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        BasePlayerTierReward = NextPlayerTierReward;

        // Third tier, unlocked at 2 * TierCount (200). Final tier for Demo
        NextPlayerTierReward = new TierReward("Mushroom Laser", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("Spread Spores", BasePlayerTierReward);
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
        if (gameInfo.PlayerRewards.Length > 0 )
        {
            LoadRewardTree(new List<string>(gameInfo.PlayerRewards));
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
        Camera.main.GetComponent<Cam>().CameraMovementEnabled = !blnActive;
    }

    public void SetUpgradeUIActive(bool blnActive = true)
    {
        // Enable/ disable upgrade UI
        Camera.main.GetComponent<Cam>().CameraMovementEnabled = !blnActive;
    }

    public void UnlockNextTier()
    {
        
        CurrentTier++;
        PlayerFaction.GodTier++;
        List<Faction> NextTierFactions = CurrentFactions.FindAll(MatchingFaction => MatchingFaction.GodTier == CurrentTier);
        foreach(Building tempPlayerVillage in PlayerFaction.OwnedBuildings.FindAll(villageBuilding => villageBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE))
        {
            if(tempPlayerVillage.UpgradeLevel <= CurrentTier)
            {
                tempPlayerVillage.UpgradeBuilding(false);
            }
        }
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

    public void PauseGame()
    {
        Time.timeScale = 0;
        Camera.main.GetComponent<Cam>().CameraMovementEnabled = false;
        LastMenuState = CurrentMenuState;
        CurrentMenuState = MENUSTATE.Paused_State;
        PausedMenuPanel.SetActive(true);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        Camera.main.GetComponent<Cam>().CameraMovementEnabled = true;
        CurrentMenuState = LastMenuState;
        PausedMenuPanel.SetActive(false);
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

    public Building PlaceRandomBuilding(Faction placingFaction)
    {
        Building RandomBuilding = null;
        // Place a random building for that faction
        RandomBuilding = CreateRandomBuilding(placingFaction);
        if (!GameMap.PlaceBuilding(RandomBuilding, GameMap.CalculateRandomPosition(placingFaction)))
        {
            RandomBuilding.Destroy();
            return null;
        }
        return RandomBuilding;
    }

    public Building CreateRandomBuilding(Faction placingFaction)
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

    public void EnterBuildMenuState()
    {
        bool blnAllowedToBuildMine = 
            PlayerFaction.OwnedBuildings.FindAll(materialBuilding => materialBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL).Count 
            < PlayerFaction.FactionArea.Count;
        bool blnAllowedToBuildUpgradeBuilding =
            PlayerFaction.OwnedBuildings.Find(upgradeBuilding => upgradeBuilding.BuildingType == Building.BUILDING_TYPE.UPGRADE) == null;

        CurrentMenuState = MENUSTATE.Building_State;
        MenuPanelController.EnterBuildMenu(blnAllowedToBuildMine, blnAllowedToBuildUpgradeBuilding);
    }

    public void EnterTierRewardsMenuState()
    {
        CurrentMenuState = MENUSTATE.Tier_Reward_State;
        SetRewardsUIActive();
    }

    private void EnterBuildingSelectedMenuState()
    {
        if(SelectedBuilding != null)
        {
            bool blnBuildingCanBeUpgraded = SelectedBuilding.UpgradeLevel < SelectedBuilding.OwningFaction.GodTier + 1;
            bool blnIsPlayersBuilding = SelectedBuilding.OwningFaction == PlayerFaction;
            CurrentMenuState = MENUSTATE.Building_Selected_State;
            MenuPanelController.EnterSelectedBuildingMenu(SelectedBuilding, blnIsPlayersBuilding, blnBuildingCanBeUpgraded);
        }
    }

    private void GoToDefaultMenuState()
    {
        CurrentMenuState = MENUSTATE.Default_State;
        MenuPanelController.GoToDefaultMenu();
        if (BufferedBuilding != null)
        {
            BufferedBuilding.Destroy();
            BufferedBuilding = null;
            foreach (Building BuildingOnMap in GameMap.GetBuildings())
            {
                BuildingOnMap.ToggleBuildingOutlines(false);
            }
        }
        if(SelectedBuilding != null)
        {
            SelectedBuilding.ToggleBuildingOutlines(false);
            SelectedBuilding = null;
        }
    }

    public void EnterMovingBuildingState()
    {
        if(SelectedBuilding != null)
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
    }

    public void UpgradeSelectedBuilding()
    {
        if (SelectedBuilding.UpgradeBuilding())
        {
            sound.PlaySound("PlaceBuilding");
            EnterBuildingSelectedMenuState();
        }
        else
        {
            if (SelectedBuilding.UpgradeLevel == 3)
            {
                Debug.Log("Already max upgrade level");
            }
            else
            {
                Debug.Log(string.Format("Not enough materials to upgrade ({0} required)",
                Building.CalculateBuildingUpgradeCost(SelectedBuilding.BuildingType)));
            }
            sound.PlaySound("NotMaterials");
        }
    }

    public void BuyMinersForSelectedBuilding()
    {
        if (!((MineBuilding)SelectedBuilding).BuyMiners((int)Math.Pow(10, SelectedBuilding.UpgradeLevel)))
        {
            sound.PlaySound("NotMaterials");
        }
        EnterBuildingSelectedMenuState();
    }

    public void SaveGame()
    {
        // Set data to save
        gameInfo.PlayerFaction = GameInfo.CreateSavedFaction(PlayerFaction);

        gameInfo.SavedFactions = new GameInfo.SavedFaction[EnemyFactions.Count];
        int intFactionIndex = 0;
        foreach (Faction faction in EnemyFactions)
        {
            gameInfo.SavedFactions[intFactionIndex] = GameInfo.CreateSavedFaction(faction);
            intFactionIndex++;
        }
        gameInfo.MapRadius = MapRadius;
        gameInfo.CurrentTier = CurrentTier;
        gameInfo.PlayerRewards = SaveRewardTree(PlayerRewardTree).ToArray();
        gameInfo.PlayerMoraleCap = PlayerMoraleCap;
        gameInfo.FromSave = true;
        gameInfo.FinishedBattle = false;
        gameInfo.EnemyChallengeTimer = EnemyChallengeTimer;
        gameInfo.EnemyFaction = new GameInfo.SavedFaction();

        if(!GameInfo.SaveGame(Application.persistentDataPath, gameInfo))
        {
            // TODO Error occurred while saving
            
        }
        else
        {
            
        }
    }

    public void QuitToMenu()
    {
        // Destroy gameinfo object
        Destroy(gameInfo.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}

public static class ListExtension
{
    public static System.Random RandomNumberGenerator = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = RandomNumberGenerator.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}