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
        Paused_State,
        Settings_Menu_State,
        End_Game_State
    }
    public int InitialPlayerMaterials = 0;
    public int InitialPlayerWorshippers = 0;
	public Texture mapTexture;
    public GameObject GameInfoObjectPrefab;
    public GameInfo gameInfo;
	ResourceUIScript ResourceUIController;
    public GameObject RewardUI;
    public GameObject AudioObject;
    private ExecuteSound sound;
    public TerrainMap GameMap;
    public Building PlayerVillage;
    public float MapRadius;
    public List<Faction> CurrentFactions;
    public int BuildingCostModifier = 1;
    public MENUSTATE CurrentMenuState = MENUSTATE.Default_State;
    public Building BufferedBuilding = null;
	public Faction PlayerFaction = null;
    public List<Faction> EnemyFactions = null;
    private GameObject SelectedGameObject = null;
    public Building SelectedBuilding = null;
    public Vector3 OriginalBuildingPosition;
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
    public GameObject OptionsMenuPanel;
    public GameObject GameOverPanel;
    public GameObject VictoryPanel;
    public MenuPanelControls MenuPanelController { get; private set; }
    public float PlayerMoraleCap = 1.0f;
    private bool mblnNewGame = false;
    private bool blnVictory = false;
    private bool blnGameOver = false;
    public HotKeyManager hotKeyManager = new HotKeyManager();
    // Use this for any initializations needed by other scripts
    void Awake()
    {
        InitializeGameInfo();
        if(!gameInfo.NewGame)
        {
            StartFromSaveState();
            if(!gameInfo.FromSave)
            {
                LoadBattleResults();
            }
            else
            {
                EnemyChallengeTimer = gameInfo.EnemyChallengeTimer;
            }
        }
        else
        {
            StartNewGame();
        }
        foreach (Faction faction in CurrentFactions.FindAll(MatchingFaction => MatchingFaction.GodTier > CurrentTier))
        {
            faction.SetHidden(true);
        }
        GameMap.DrawFactionArea(PlayerFaction);
    }

    private void InitializeGameInfo()
    {
        GameObject GameInfoObject = GameObject.Find("GameInfo");
        if(GameInfoObject != null)
        {
            gameInfo = GameInfoObject.GetComponent<GameInfo>();
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
            gameInfo.NewGame = true;
#else
            // Couldn't find gameinfo object
            throw new Exception("Something went wrong :(");
#endif
        }
        
    }
    private void StartNewGame()
    {
        Building.BuildingRadiusSize = BuildingRadius;
        Building.BuildingCostModifier = BuildingCostModifier;
        Building bldEnemyBuilding = null;
        Faction facEnemyFaction = null;
        Vector3 vec3BuildngPos;
        List<Faction.GodType> GodTypes;
        List<Faction> CurrentTierFactions;
        List<string> GodNames = new List<string>(Faction.GodNames);
        GodNames.Shuffle();
        float TierRadius = (MapRadius / 2) / MapTierCount;
        int Attempts = 0;

        mblnNewGame = true;
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

        for (int TierIndex = 0; TierIndex < MapTierCount; TierIndex++)
        {
            if (TierIndex == 0)
            {
                GodTypes = new List<Faction.GodType>(Faction.TierOneGods);
                GodTypes.Remove(PlayerFaction.Type);
                GodTypes.Shuffle();
            }
            else if (TierIndex == 1)
            {
                GodTypes = new List<Faction.GodType>(Faction.TierTwoGods);
                GodTypes.Shuffle();
            }
            else
            {
                GodTypes = new List<Faction.GodType>(Faction.TierThreeGods);
                GodTypes.Shuffle();
            }
            for (int enemyCount = 0; enemyCount < EnemiesPerTier; enemyCount++)
            {
                facEnemyFaction = new Faction(GodNames[enemyCount + TierIndex * EnemiesPerTier], GodTypes[enemyCount], TierIndex)
                {
                    FactionDifficulty = (enemyCount + 1) + ((TierIndex + 1) * TierDifficultyIncrease),
                    MaterialCount = (int)Math.Pow(10, TierIndex + 2),
                    WorshipperCount = (int)Math.Pow(10, TierIndex + 2),
                };
                facEnemyFaction.CurrentAbilites = Faction.GetGodAbilities(facEnemyFaction.Type);
                CurrentFactions.Add(facEnemyFaction);
                EnemyFactions.Add(facEnemyFaction);
            }
            CurrentTierFactions = CurrentFactions.FindAll(MatchingFaction => MatchingFaction.GodTier == TierIndex);
            GameMap.DivideMap(CurrentTierFactions, TierRadius * TierIndex, TierRadius * (TierIndex + 1));
        }
        GodTypes = new List<Faction.GodType>();
        foreach (Faction faction in CurrentFactions)
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
            while (!GameMap.PlaceBuilding(bldEnemyBuilding, vec3BuildngPos) && Attempts < 100)
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
            for (int i = 0; i < enemyFaction.FactionDifficulty; i++)
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
                    bldEnemyBuilding.UpgradeBuilding(false, true);
                }
                ((MineBuilding)bldEnemyBuilding).Miners = ((MineBuilding)bldEnemyBuilding).MinerCap;
            }
        }
    }
    /// <summary>
    /// Method for initializing the gameinfo object
    /// Either creates a gameinfo object if one does not exist, or reads the values if one does
    /// </summary>
    private void StartFromSaveState()
    {
        Faction InitFaction = null;
        List<Faction.GodType> GodTypes = null;
        bool blnFromSave = false;

        blnFromSave = gameInfo.FromSave;
        if (blnFromSave)
        {
            // Load building resources as they may not have been loaded yet.
            GodTypes = new List<Faction.GodType>
            {
                gameInfo.PlayerFaction.Type
            };
            foreach (GameInfo.SavedFaction savedFaction in gameInfo.SavedFactions)
            {
                GodTypes.Add(savedFaction.Type);
            }
            Building.LoadBuildingResources(GodTypes);
        }
        CurrentTier = gameInfo.CurrentTier;
        PlayerMoraleCap = gameInfo.PlayerMoraleCap;
        WorshipperMultipliers = new List<float>(gameInfo.WorshipperMultipliers);
        MaterialMultipliers = new List<float>(gameInfo.MaterialMultipliers);
        CurrentFactions = new List<Faction>();
        EnemyFactions = new List<Faction>();
        // Create scene with values from gameInfo
        // Load Game Map
        GameMap = new TerrainMap(gameInfo.MapRadius, mapTexture);
        // Load factions
        foreach (GameInfo.SavedFaction savedFaction in gameInfo.SavedFactions)
        {
            InitFaction = new Faction(savedFaction);
            GameMap.PlaceSavedFactionBuildings(savedFaction.OwnedBuildings, InitFaction);
            CurrentFactions.Add(InitFaction);
            EnemyFactions.Add(InitFaction);
        }
        PlayerFaction = new Faction(gameInfo.PlayerFaction);
        GameMap.PlaceSavedFactionBuildings(gameInfo.PlayerFaction.OwnedBuildings, PlayerFaction);
        PlayerVillage = PlayerFaction.OwnedBuildings.Find(villageBuilding => villageBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE);
        CurrentFactions.Add(PlayerFaction);
    }

    private void LoadBattleResults()
    {
        Faction EnemyFaction = null;
        List<Faction> arrFactionsLeft = null;
        EnemyFaction = new Faction(gameInfo.EnemyFaction);
        GameMap.PlaceSavedFactionBuildings(gameInfo.EnemyFaction.OwnedBuildings, EnemyFaction);

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
                    // Reset miners on conquered buildings
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
                    blnVictory = true;
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
            blnGameOver = true;
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
        gameInfo.PlayerRewards = TierReward.SaveRewardTree(PlayerRewardTree).ToArray();
        gameInfo.PlayerMoraleCap = PlayerMoraleCap;
        gameInfo.FromSave = false;
        gameInfo.MaterialMultipliers = MaterialMultipliers.ToArray();
        gameInfo.WorshipperMultipliers = WorshipperMultipliers.ToArray();
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
        ResourceUIController = FindObjectOfType<ResourceUIScript>();
        sound = AudioObject.GetComponent<ExecuteSound>();
        MenuPanelController = MenuControlObject.GetComponent<MenuPanelControls>();
        SaveAndSettingsHelper.ApplyGameSettings();
        Time.timeScale = 1;
        PausedMenuPanel.SetActive(false);
        OptionsMenuPanel.SetActive(false);
        GameOverPanel.SetActive(blnGameOver);
        VictoryPanel.SetActive(blnVictory);
        CreateRewardTree();
        ResourceTicks = 0;
        if(mblnNewGame)
        {
            SaveGame();
            mblnNewGame = false;
        }
        if(blnVictory || blnGameOver)
        {
            PauseGame();
            PausedMenuPanel.SetActive(false);
            CurrentMenuState = MENUSTATE.End_Game_State;
        }
        hotKeyManager.LoadHotkeyProfile();
        InvokeRepeating("CalculateResources", 0.5f, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
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
                            tempBuilding = Building.CreateRandomBuilding(CurrentFaction);
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
        PlayerRewardTree = TierReward.CreateTierRewardTree(PlayerFaction.Type);
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

    public void SetRewardsUIActive(bool blnActive = true)
    {
        RewardUI.SetActive(blnActive);
        Camera.main.GetComponent<Cam>().CameraMovementEnabled = !blnActive;
    }

    public void SetUpgradeUIActive(bool blnActive = true)
    {
        // Enable/ disable upgrade UI
        if(blnActive)
        {
            CurrentMenuState = MENUSTATE.Upgrade_State;
        }
        else
        {
            CurrentMenuState = MENUSTATE.Building_Selected_State;
        }
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


    public void LoadRewardTree(List<string> savedRewards)
    {
        TierReward reward; 
        foreach(string savedReward in savedRewards)
        {
            reward = TierReward.FindRewardByName(savedReward, PlayerRewardTree);
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
        RandomBuilding = Building.CreateRandomBuilding(placingFaction);
        if (!GameMap.PlaceBuilding(RandomBuilding, GameMap.CalculateRandomPosition(placingFaction)))
        {
            RandomBuilding.Destroy();
            return null;
        }
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

    public void EnterBuildingSelectedMenuState()
    {
        if(SelectedBuilding != null)
        {
            bool blnBuildingCanBeUpgraded = SelectedBuilding.UpgradeLevel < SelectedBuilding.OwningFaction.GodTier + 1;
            bool blnIsPlayersBuilding = SelectedBuilding.OwningFaction == PlayerFaction;
            CurrentMenuState = MENUSTATE.Building_Selected_State;
            MenuPanelController.EnterSelectedBuildingMenu(SelectedBuilding, blnIsPlayersBuilding, blnBuildingCanBeUpgraded);
        }
    }

    public void GoToDefaultMenuState()
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
        gameInfo.PlayerRewards = TierReward.SaveRewardTree(PlayerRewardTree).ToArray();
        gameInfo.PlayerMoraleCap = PlayerMoraleCap;
        gameInfo.FromSave = true;
        gameInfo.FinishedBattle = false;
        gameInfo.EnemyChallengeTimer = EnemyChallengeTimer;
        gameInfo.EnemyFaction = new GameInfo.SavedFaction();
        if(SaveAndSettingsHelper.SaveGame(Application.persistentDataPath + "/SaveFiles", gameInfo))
        {
            // TODO Display game saved
        }
        else
        {
            // TODO Error occurred while saving
        }
    }

    public void ReturnToPauseMenu()
    {
        PausedMenuPanel.SetActive(true);
        CurrentMenuState = GameManager.MENUSTATE.Paused_State;
        OptionsMenuPanel.SetActive(false);
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