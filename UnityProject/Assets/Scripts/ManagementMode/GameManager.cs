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

    // Public variables used and unity properties of the Game Manger object.
    // Defined in by Unity on the Game Object
    public int InitialPlayerMaterials;
    public int InitialPlayerWorshippers;
	public Texture MapTexture;
    public GameObject GameInfoObjectPrefab;
    public GameObject RewardUI;
    public GameObject AudioObject;
    public float MapRadius;
    public int BuildingCostModifier;
    public float MinimumMorale;
    public int TierUnlockPoint; // Initial tier unlock count
    public int TierUnlockPointMultiplier; // After every tier, tier count is multiplied by this
    public int MapTierCount;
    public int EnemiesPerTier;
    public int CurrentTier;
    public int TierDifficultyIncrease; // Each tier gets X*Tier buildings to start
    public int EnemyChallengeTimer; // Seconds until an enemy attacks
    public float BuildingRadius;
    public GameObject PausedMenuPanel;
    public GameObject MenuControlObject;
    public GameObject OptionsMenuPanel;
    public GameObject GameOverPanel;
    public GameObject VictoryPanel;
    public float PlayerMoraleCap;

    // Public accessor variables, only to be accessed by other scripts, not modified
    // Mainly for testing purposes
    public MenuPanelControls MenuPanelController { get; private set; }
    public int ResourceTicks { get; private set; }
    public GameInfo GameInfo { get; private set; }
    public Building BufferedBuilding { get; private set; }
    public MENUSTATE CurrentMenuState { get; private set; }
    public Faction PlayerFaction { get; private set; }
    public List<Faction> EnemyFactions { get; private set; }
    public List<Faction> CurrentFactions { get; private set; }
    public Building PlayerVillage { get; private set; }
    public TerrainMap GameMap { get; private set; }
    public Building SelectedBuilding { get; private set; }
    public Vector3 OriginalBuildingPosition { get; private set; }
    public HotKeyManager HotKeyManager { get; private set; }

    // Private member variables for the game manager script
    private GameObject muniSelectedGameObject = null;
    private ExecuteSound mmusSoundManager;
    private List<float> marrMaterialMultipliers = new List<float>();
    private List<float> marrWorshipperMultipliers = new List<float>();
    private MENUSTATE menumLastMenuState = MENUSTATE.Default_State;
    private bool mblnNewGame = false;
    private bool mblnVictory = false;
    private bool mblnGameOver = false;
    private int mintCurrentTimer = 0;
    private ResourceUIScript mmusResourceUIController;
    private List<TierReward> marrPlayerRewardTree;

    // Use this for any initializations needed by other scripts
    void Awake()
    {
        HotKeyManager = new HotKeyManager();
        Building.BuildingRadiusSize = BuildingRadius;
        InitializeGameInfo();
        if(!GameInfo.NewGame)
        {
            StartFromSaveState();
            if(!GameInfo.FromSave)
            {
                LoadBattleResults();
            }
            else
            {
                EnemyChallengeTimer = GameInfo.EnemyChallengeTimer;
            }
        }
        else
        {
            StartNewGame();
            GameInfo.NewGame = false;
        }
        foreach (Faction musFaction in CurrentFactions.FindAll(musMatchingFaction => musMatchingFaction.GodTier > CurrentTier))
        {
            musFaction.SetHidden(true);
        }
        GameMap.DrawFactionArea(PlayerFaction);
    }

    private void InitializeGameInfo()
    {
        GameObject GameInfoObject = GameObject.Find("GameInfo");
        if(GameInfoObject != null)
        {
            GameInfo = GameInfoObject.GetComponent<GameInfo>();
        }
        else
        {
#if DEBUG
            Debug.Log("Loaded directly into management mode, creating a default god.");
            GameObject NewGameInfoObject = (GameObject)Instantiate(GameInfoObjectPrefab);
            NewGameInfoObject.name = "GameInfo";
            GameInfo = NewGameInfoObject.GetComponent<GameInfo>();
            GameInfo.PlayerFaction.GodName = "TestGod";
            GameInfo.PlayerFaction.Type = Faction.GodType.Mushrooms;
            GameInfo.NewGame = true;
#else
            // Couldn't find gameinfo object
            throw new Exception("Something went wrong :(");
#endif
        }
        
    }
    private void StartNewGame()
    {
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
        GodNames.Remove(GameInfo.PlayerFaction.GodName);
        // Starting new game scene, initialize map, players, and buildings
        CurrentFactions = new List<Faction>();
        EnemyFactions = new List<Faction>();
        //Create map terrain
        GameMap = new TerrainMap(MapRadius, MapTexture);

        // Create the player Faction
        PlayerFaction = new Faction(GameInfo.PlayerFaction.GodName, GameInfo.PlayerFaction.Type, 0)
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

    public void UnselectBuilding()
    {
        SelectedBuilding.ToggleBuildingOutlines(false);
        SelectedBuilding = null;
    }

    public void ClearBufferedBuilding(bool pblnDestroyBuiling = false)
    {
        if (BufferedBuilding != null)
        {
            if(pblnDestroyBuiling)
            {
                BufferedBuilding.Destroy();
            }
            BufferedBuilding = null;
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

        blnFromSave = GameInfo.FromSave;
        if (blnFromSave)
        {
            // Load building resources as they may not have been loaded yet.
            GodTypes = new List<Faction.GodType>
            {
                GameInfo.PlayerFaction.Type
            };
            foreach (GameInfo.SavedFaction savedFaction in GameInfo.SavedFactions)
            {
                GodTypes.Add(savedFaction.Type);
            }
            Building.LoadBuildingResources(GodTypes);
        }
        CurrentTier = GameInfo.CurrentTier;
        PlayerMoraleCap = GameInfo.PlayerMoraleCap;
        marrWorshipperMultipliers = new List<float>(GameInfo.WorshipperMultipliers);
        marrMaterialMultipliers = new List<float>(GameInfo.MaterialMultipliers);
        CurrentFactions = new List<Faction>();
        EnemyFactions = new List<Faction>();
        // Create scene with values from gameInfo
        // Load Game Map
        GameMap = new TerrainMap(GameInfo.MapRadius, MapTexture);
        // Load factions
        foreach (GameInfo.SavedFaction savedFaction in GameInfo.SavedFactions)
        {
            InitFaction = new Faction(savedFaction);
            GameMap.PlaceSavedFactionBuildings(savedFaction.OwnedBuildings, InitFaction);
            CurrentFactions.Add(InitFaction);
            EnemyFactions.Add(InitFaction);
        }
        PlayerFaction = new Faction(GameInfo.PlayerFaction);
        GameMap.PlaceSavedFactionBuildings(GameInfo.PlayerFaction.OwnedBuildings, PlayerFaction);
        PlayerVillage = PlayerFaction.OwnedBuildings.Find(villageBuilding => villageBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE);
        CurrentFactions.Add(PlayerFaction);
    }

    private void LoadBattleResults()
    {
        Faction EnemyFaction = null;
        List<Faction> arrFactionsLeft = null;
        EnemyFaction = new Faction(GameInfo.EnemyFaction);
        GameMap.PlaceSavedFactionBuildings(GameInfo.EnemyFaction.OwnedBuildings, EnemyFaction);

        if (GameInfo.LastBattleStatus == GameInfo.BATTLESTATUS.Victory)
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
                    mblnVictory = true;
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
        else if (GameInfo.LastBattleStatus == GameInfo.BATTLESTATUS.Retreat)
        {
            PlayerMoraleCap = PlayerMoraleCap - 0.2f > 0.2f ? PlayerMoraleCap - 0.2f : 0.2f;
            CurrentFactions.Add(EnemyFaction);
        }
        else
        {
            // Run defeat animation/reset to tier checkpoint
            mblnGameOver = true;
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
        GameInfo.PlayerFaction = GameInfo.CreateSavedFaction(PlayerFaction);

        // save challenging faction
        GameInfo.EnemyFaction = GameInfo.CreateSavedFaction(EnemyFaction);

        // Save the rest of the factions
        GameInfo.SavedFactions = new GameInfo.SavedFaction[EnemyFactions.Count - 1];
        int intFactionIndex = 0;
        foreach(Faction faction in EnemyFactions)
        {
            if(faction != EnemyFaction)
            {
                GameInfo.SavedFactions[intFactionIndex] = GameInfo.CreateSavedFaction(faction);
                intFactionIndex++;
            }
        }
        
        GameInfo.MapRadius = MapRadius;
        GameInfo.CurrentTier = CurrentTier;
        GameInfo.PlayerRewards = TierReward.SaveRewardTree(marrPlayerRewardTree).ToArray();
        GameInfo.PlayerMoraleCap = PlayerMoraleCap;
        GameInfo.FromSave = false;
        GameInfo.MaterialMultipliers = marrMaterialMultipliers.ToArray();
        GameInfo.WorshipperMultipliers = marrWorshipperMultipliers.ToArray();
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
        mmusResourceUIController = FindObjectOfType<ResourceUIScript>();
        mmusSoundManager = AudioObject.GetComponent<ExecuteSound>();
        MenuPanelController = MenuControlObject.GetComponent<MenuPanelControls>();
        SaveAndSettingsHelper.ApplyGameSettings();
        Time.timeScale = 1;
        PausedMenuPanel.SetActive(false);
        OptionsMenuPanel.SetActive(false);
        GameOverPanel.SetActive(mblnGameOver);
        VictoryPanel.SetActive(mblnVictory);
        CreateRewardTree();
        ResourceTicks = 0;
        if(mblnNewGame)
        {
            SaveGame();
            mblnNewGame = false;
        }
        if(mblnVictory || mblnGameOver)
        {
            PauseGame();
            PausedMenuPanel.SetActive(false);
            CurrentMenuState = MENUSTATE.End_Game_State;
        }
        HotKeyManager.LoadHotkeyProfile();
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
		mmusResourceUIController.UpdateResourceUIElements (PlayerFaction.MaterialCount, PlayerFaction.WorshipperCount, PlayerFaction.Morale, PlayerFaction.TierRewardPoints);
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
                    muniSelectedGameObject = hitInfo.collider.gameObject;
                    // If the user clicked the map, do nothing
                    if (muniSelectedGameObject == GameMap.GetMapObject())
                    {
                        muniSelectedGameObject = null;
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
                        SetSelectedBuilding(GameMap.GetBuildings().Find(ClickedBuilding => ClickedBuilding.BuildingObject == muniSelectedGameObject));
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
                    mmusSoundManager.PlaySound("PlaceBuilding");
                    // If the building did place, go back to building menu
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
                        PlayerFaction.MaterialCount -= BufferedBuilding.BuildingCost;
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
                        foreach(float multiplier in marrMaterialMultipliers)
                        {
                            intMaterialsToAdd += (int)((multiplier - 1) * intMaterialsToAdd);
                        }
                        foreach (float multiplier in marrWorshipperMultipliers)
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
        mintCurrentTimer += 2;
        if(mintCurrentTimer > EnemyChallengeTimer)
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
                mmusSoundManager.PlaySound("NotMaterials");
            }
        }
        else
        {
            mmusSoundManager.PlaySound("NotMaterials");
        }
    }

    private void CreateRewardTree()
    {
        // Create a reward tree for the player
        marrPlayerRewardTree = TierReward.CreateTierRewardTree(PlayerFaction.Type);
        if (GameInfo.PlayerRewards.Length > 0 )
        {
            LoadRewardTree(new List<string>(GameInfo.PlayerRewards));
        }
        RewardUI.GetComponentInChildren<PopulateTierIcons>().InitializeButtons(marrPlayerRewardTree);
        RewardUI.SetActive(false);
    }

    private List<TierReward> GetUnlockableRewards()
    {
        // Get this list of all rewards that can be unlocked now.
        return marrPlayerRewardTree.FindAll(UnlockableReward => 
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
                            marrMaterialMultipliers.Add(reward.Multiplier);
                            break;
                        case TierReward.RESOURCETYPE.Worshipper:
                            marrWorshipperMultipliers.Add(reward.Multiplier);
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
        menumLastMenuState = CurrentMenuState;
        CurrentMenuState = MENUSTATE.Paused_State;
        PausedMenuPanel.SetActive(true);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        Camera.main.GetComponent<Cam>().CameraMovementEnabled = true;
        CurrentMenuState = menumLastMenuState;
        PausedMenuPanel.SetActive(false);
    }


    public void LoadRewardTree(List<string> savedRewards)
    {
        TierReward reward; 
        foreach(string savedReward in savedRewards)
        {
            reward = TierReward.FindRewardByName(savedReward, marrPlayerRewardTree);
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
            mmusSoundManager.PlaySound("PlaceBuilding");
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
            mmusSoundManager.PlaySound("NotMaterials");
        }
    }

    public void BuyMinersForSelectedBuilding()
    {
        if (!((MineBuilding)SelectedBuilding).BuyMiners((int)Math.Pow(10, SelectedBuilding.UpgradeLevel)))
        {
            mmusSoundManager.PlaySound("NotMaterials");
        }
        EnterBuildingSelectedMenuState();
    }

    public void SaveGame()
    {
        // Set data to save
        GameInfo.PlayerFaction = GameInfo.CreateSavedFaction(PlayerFaction);
        GameInfo.SavedFactions = new GameInfo.SavedFaction[EnemyFactions.Count];
        int intFactionIndex = 0;
        foreach (Faction faction in EnemyFactions)
        {
            GameInfo.SavedFactions[intFactionIndex] = GameInfo.CreateSavedFaction(faction);
            intFactionIndex++;
        }
        GameInfo.MapRadius = MapRadius;
        GameInfo.CurrentTier = CurrentTier;
        GameInfo.PlayerRewards = TierReward.SaveRewardTree(marrPlayerRewardTree).ToArray();
        GameInfo.PlayerMoraleCap = PlayerMoraleCap;
        GameInfo.FromSave = true;
        GameInfo.FinishedBattle = false;
        GameInfo.EnemyChallengeTimer = EnemyChallengeTimer;
        GameInfo.EnemyFaction = new GameInfo.SavedFaction();
        if(SaveAndSettingsHelper.SaveGame(Application.persistentDataPath + "/SaveFiles", GameInfo))
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

    public void EnterSettingsMenuState()
    {
        CurrentMenuState = MENUSTATE.Settings_Menu_State;
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