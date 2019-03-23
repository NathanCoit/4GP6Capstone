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
        End_Game_State,
        God_Selected_State
    }

    // Public variables used and unity properties of the Game Manger object.
    // Defined in by Unity on the Game Object
    public int InitialPlayerMaterials;
    public int InitialPlayerWorshippers;
    public Texture MapTexture;
    public GameObject GameInfoObjectPrefab;
    public GameObject RewardUI;
    public GameObject AudioObject;
    public GameObject UpgradeUI;
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
    public InformationBoxDisplay InformationBoxController;
    public WorshipperUpgradeController UpgradeController;
    public PlayerGodController PlayerGod;

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
    private InformationBoxDisplay.TutorialFlag menumTutorialFlag = 0;

    /// <summary>
    /// Default function run by unity on scene start up
    /// Use this for any initializations needed by other scripts
    /// </summary>
    void Awake()
    {
        HotKeyManager = new HotKeyManager();
        Building.BuildingRadiusSize = BuildingRadius;
        InitializeGameInfo();
        // Keep mouse confined to screen, if player tabs out, game is paused.
        Cursor.lockState = CursorLockMode.Confined;
        if (!GameInfo.NewGame)
        {
            // Not a new game, either returning from combat mode or loading a save
            StartFromSaveState();
            if (!GameInfo.FromSave)
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
        // Hide all factions higher that current tier
        foreach (Faction musFaction in CurrentFactions.FindAll(musMatchingFaction => musMatchingFaction.GodTier > CurrentTier))
        {
            musFaction.SetHidden(true);
        }
        //GameMap.DrawFactionArea(PlayerFaction);
        //GameMap.DrawMultipleFactionAreas(CurrentFactions);
        // Hide the map. Keep map object for collision detection, but does not need to be visible as individual god textures cover it
        GameMap.HideMap();
        GameMap.AddGodLandscapes(CurrentFactions);
        PlayerGod.GameMap = GameMap;
        PlayerGod.CreatePlayerGod();
        UpgradeController.PlayerFaction = PlayerFaction;
    }

    /// <summary>
    /// Load the game info object from the scene. Game info object should be created by main menu before loading MM scene.
    /// </summary>
    private void InitializeGameInfo()
    {
        GameObject uniGameInfoObject = GameObject.Find("GameInfo");
        if (uniGameInfoObject != null)
        {
            GameInfo = uniGameInfoObject.GetComponent<GameInfo>();
        }
        else
        {
            // DEBUG directive to create a default god for testing when loading the management mode directly
#if DEBUG
            Debug.Log("Loaded directly into management mode, creating a default god.");
            GameObject NewGameInfoObject = (GameObject)Instantiate(GameInfoObjectPrefab);
            NewGameInfoObject.name = "GameInfo";
            GameInfo = NewGameInfoObject.GetComponent<GameInfo>();
            GameInfo.PlayerFaction.GodName = "@@@@@@@@@@@@@@@"; // Largest name possible for testing
            GameInfo.PlayerFaction.Type = Faction.GodType.Mushrooms;
            GameInfo.NewGame = true;
#else
            // Couldn't find gameinfo object
            throw new Exception("Something went wrong :(");
#endif
        }

    }
    /// <summary>
    /// Function run at the beginning of a new game to generate starting map, buildings, player and enemies.
    /// </summary>
    private void StartNewGame()
    {
        Building.BuildingCostModifier = BuildingCostModifier;
        Building musEnemyBuilding = null;
        Faction musEnemyFaction = null;
        Vector3 uniBuildngPosition;
        List<Faction.GodType> arrGodTypes;
        List<Faction> arrCurrentTierFactions;
        List<string> arrGodNames = new List<string>(Faction.GodNames);
        arrGodNames.Shuffle();
        float fTierRadius = (MapRadius / 2) / MapTierCount;
        int intAttempts = 0;

        mblnNewGame = true;
        // Remove chance of player having same name as another god.
        arrGodNames.Remove(GameInfo.PlayerFaction.GodName);
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
        // Create factions for each tier. Higher tier increased number of starting buildings and resources.

        for (int intTierIndex = 0; intTierIndex < MapTierCount; intTierIndex++)
        {
            if (intTierIndex == 0)
            {
                arrGodTypes = new List<Faction.GodType>(Faction.TierOneGods);
                arrGodTypes.Remove(PlayerFaction.Type);
                arrGodTypes.Shuffle();
            }
            else if (intTierIndex == 1)
            {
                arrGodTypes = new List<Faction.GodType>(Faction.TierTwoGods);
                arrGodTypes.Shuffle();
            }
            else
            {
                arrGodTypes = new List<Faction.GodType>(Faction.TierThreeGods);
                arrGodTypes.Shuffle();
            }
            for (int enemyCount = 0; enemyCount < EnemiesPerTier; enemyCount++)
            {
                // Higher tier, more starting resources. Exponential to increase difficulty as you grow
                musEnemyFaction = new Faction(arrGodNames[enemyCount + intTierIndex * EnemiesPerTier], arrGodTypes[enemyCount], intTierIndex)
                {
                    FactionDifficulty = (enemyCount + 1) + ((intTierIndex + 1) * TierDifficultyIncrease),
                    MaterialCount = (int)Math.Pow(10, intTierIndex + 2),
                    WorshipperCount = (int)Math.Pow(10, intTierIndex + 2),
                };
                musEnemyFaction.CurrentAbilites = Faction.GetGodAbilities(musEnemyFaction.Type);
                CurrentFactions.Add(musEnemyFaction);
                EnemyFactions.Add(musEnemyFaction);
            }
            arrCurrentTierFactions = CurrentFactions.FindAll(MatchingFaction => MatchingFaction.GodTier == intTierIndex);
            GameMap.DivideMap(arrCurrentTierFactions, fTierRadius * intTierIndex, fTierRadius * (intTierIndex + 1));
        }
        arrGodTypes = new List<Faction.GodType>();
        foreach (Faction faction in CurrentFactions)
        {
            arrGodTypes.Add(faction.Type);
        }
        Building.LoadBuildingResources(arrGodTypes);
        //Create and place player village
        PlayerVillage = new Building(Building.BUILDING_TYPE.VILLAGE, PlayerFaction);
        uniBuildngPosition = GameMap.CalculateRandomPosition(PlayerFaction);
        GameMap.PlaceBuilding(PlayerVillage, uniBuildngPosition);

        //Create and place enemy villages
        foreach (Faction enemyFaction in EnemyFactions)
        {
            musEnemyBuilding = new Building(Building.BUILDING_TYPE.VILLAGE, enemyFaction);
            uniBuildngPosition = GameMap.CalculateRandomPosition(enemyFaction);
            // Attempt to place the village building 100 times as placing could fail with one attempt.
            while (!GameMap.PlaceBuilding(musEnemyBuilding, uniBuildngPosition) && intAttempts < 100)
            {
                uniBuildngPosition = GameMap.CalculateRandomPosition(enemyFaction);
                intAttempts++;
            }
            // Enemy gods starting buildings are fully upgraded up to their tier level.
            for (int i = 0; i < enemyFaction.GodTier; i++)
            {
                musEnemyBuilding.UpgradeBuilding(false, true);
            }
            intAttempts = 0;
            // Generate starting buildings based on enemy difficulty
            for (int i = 0; i < enemyFaction.FactionDifficulty; i++)
            {
                PlaceRandomBuilding(enemyFaction);
            }
            // Generate Tier number of mines for each enemy and fill them up.
            // Gives enemy gods set amount of resource generation to start.
            for (int i = 0; i < enemyFaction.GodTier + 1; i++)
            {
                musEnemyBuilding = new MineBuilding(Building.BUILDING_TYPE.MATERIAL, enemyFaction);
                uniBuildngPosition = GameMap.CalculateRandomPosition(enemyFaction);
                while (!GameMap.PlaceBuilding(musEnemyBuilding, uniBuildngPosition) && intAttempts < 100)
                {
                    uniBuildngPosition = GameMap.CalculateRandomPosition(enemyFaction);
                    intAttempts++;
                }
                intAttempts = 0;
                for (int j = 0; j < enemyFaction.GodTier; j++)
                {
                    musEnemyBuilding.UpgradeBuilding(false, true);
                }
                ((MineBuilding)musEnemyBuilding).Miners = ((MineBuilding)musEnemyBuilding).MinerCap;
            }
        }
    }

    /// <summary>
    /// Load the game from a save state that is within the GameInfo object
    /// Assumes the GameInfo object has already been loaded
    /// </summary>
    private void StartFromSaveState()
    {
        Faction musInitFaction = null;
        List<Faction.GodType> arrGodTypes = null;
        bool blnFromSave = false;

        blnFromSave = GameInfo.FromSave;
        if (blnFromSave)
        {
            // Load building resources as they may not have been loaded yet.
            arrGodTypes = new List<Faction.GodType>
            {
                GameInfo.PlayerFaction.Type
            };
            foreach (GameInfo.SavedFaction savedFaction in GameInfo.SavedFactions)
            {
                arrGodTypes.Add(savedFaction.Type);
            }
            Building.LoadBuildingResources(arrGodTypes);
        }
        CurrentTier = GameInfo.CurrentTier;
        menumTutorialFlag = GameInfo.TutorialFlag;
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
            musInitFaction = new Faction(savedFaction);
            GameMap.PlaceSavedFactionBuildings(savedFaction.OwnedBuildings, musInitFaction);
            CurrentFactions.Add(musInitFaction);
            EnemyFactions.Add(musInitFaction);
        }
        PlayerFaction = new Faction(GameInfo.PlayerFaction);
        GameMap.PlaceSavedFactionBuildings(GameInfo.PlayerFaction.OwnedBuildings, PlayerFaction);
        PlayerVillage = PlayerFaction.OwnedBuildings.Find(villageBuilding => villageBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE);
        foreach (int intAttackUpgrade in GameInfo.WorshipperAttackBuffs)
        {
            PlayerFaction.CurrentUpgrades.Add(new AttackWorshipperUpgrade("", "", 0, intAttackUpgrade));
        }
        foreach (int intDefenseUpgrade in GameInfo.WorshipperDefenseBuffs)
        {
            PlayerFaction.CurrentUpgrades.Add(new DefenseWorshipperUpgrade("", "", 0, intDefenseUpgrade));
        }
        foreach (int intMovementUpgrade in GameInfo.WorshipperMovementBuffs)
        {
            PlayerFaction.CurrentUpgrades.Add(new MovementWorshipperUpgrade("", "", 0, intMovementUpgrade));
        }
        CurrentFactions.Add(PlayerFaction);
    }

    /// <summary>
    /// Load the results after returning from combat mode.
    /// </summary>
    private void LoadBattleResults()
    {
        Faction musEnemyFaction = null;
        List<Faction> arrFactionsLeft = null;
        Building musBuildingToRemove = null;
        musEnemyFaction = new Faction(GameInfo.EnemyFaction);
        GameMap.PlaceSavedFactionBuildings(GameInfo.EnemyFaction.OwnedBuildings, musEnemyFaction);

        if (GameInfo.LastBattleStatus == GameInfo.BATTLESTATUS.Victory)
        {
            // Take over enemy factions buildings, area, and resources
            foreach (float[] arrEnemyArea in musEnemyFaction.FactionArea)
            {
                PlayerFaction.FactionArea.Add(arrEnemyArea);
            }
            musBuildingToRemove = musEnemyFaction.OwnedBuildings.Find(musBuilding => musBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE);
            if(musBuildingToRemove != null)
            {
                GameMap.RemoveBuilding(musBuildingToRemove);
                musBuildingToRemove.Destroy();
            }
            PlayerFaction.MaterialCount += musEnemyFaction.MaterialCount;
            foreach (Building musBuilding in musEnemyFaction.OwnedBuildings)
            {

                musBuilding.OwningFaction = PlayerFaction;
                musBuilding.ReloadBuildingObject();
                PlayerFaction.OwnedBuildings.Add(musBuilding);
                if (musBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL)
                {
                    // Reset miners on conquered buildings
                    ((MineBuilding)musBuilding).Miners = 0;
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
            CurrentFactions.Add(musEnemyFaction);
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
    /// Method for setting the gameInfo values needed to go to combat mode
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
        foreach (Faction musFaction in EnemyFactions)
        {
            if (musFaction != EnemyFaction)
            {
                GameInfo.SavedFactions[intFactionIndex] = GameInfo.CreateSavedFaction(musFaction);
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
        List<int> arrAttackUpgrades = new List<int>();
        List<int> arrDefenseUpgrades = new List<int>();
        List<int> arrMovementUpgrades = new List<int>();
        foreach (WorshipperUpgrade musUpgrade in PlayerFaction.CurrentUpgrades)
        {
            if (musUpgrade.UpgradeType == WorshipperUpgrade.UPGRADETYPE.Attack)
            {
                arrAttackUpgrades.Add(((AttackWorshipperUpgrade)musUpgrade).DamageBuff);
            }
            else if (musUpgrade.UpgradeType == WorshipperUpgrade.UPGRADETYPE.Defense)
            {
                arrDefenseUpgrades.Add(((DefenseWorshipperUpgrade)musUpgrade).DefenseBuff);
            }
            else if (musUpgrade.UpgradeType == WorshipperUpgrade.UPGRADETYPE.Movement)
            {
                arrMovementUpgrades.Add(((MovementWorshipperUpgrade)musUpgrade).MovementBuff);
            }
        }
        GameInfo.WorshipperAttackBuffs = arrAttackUpgrades.ToArray();
        GameInfo.WorshipperDefenseBuffs = arrDefenseUpgrades.ToArray();
        GameInfo.WorshipperMovementBuffs = arrMovementUpgrades.ToArray();
        //SceneManager.LoadScene("CombatMode");
        StartCoroutine(LoadCombatSceneAsync());
    }

    private IEnumerator LoadCombatSceneAsync()
    {
        AsyncOperation uniAsyncLoad = SceneManager.LoadSceneAsync("CombatMode");

        // Wait until the asynchronous scene fully loads
        while (!uniAsyncLoad.isDone)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Method to decide the outcome of a battle between two enemy factions
    /// </summary>
    /// <param name="pmusFactionOne"></param>
    /// <param name="pmusFactionTwo"></param>
    public void BattleTwoFactions(Faction pmusFactionOne, Faction pmusFactionTwo)
    {
        Faction musWinningFaction = null;
        Faction musLosingFaction = null;
        Building musVillage;
        if (pmusFactionOne.WorshipperCount >= pmusFactionTwo.WorshipperCount)
        {
            musWinningFaction = pmusFactionOne;
            musLosingFaction = pmusFactionTwo;
        }
        else
        {
            musLosingFaction = pmusFactionOne;
            musWinningFaction = pmusFactionTwo;
        }
        musWinningFaction.MaterialCount += musLosingFaction.MaterialCount;
        musWinningFaction.WorshipperCount += (int)(0.2 * musLosingFaction.WorshipperCount);
        foreach (float[] arrFactionArea in musLosingFaction.FactionArea)
        {
            musWinningFaction.FactionArea.Add(arrFactionArea);
        }
        musVillage = musLosingFaction.OwnedBuildings.Find(musBuilding => musBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE);
        GameMap.RemoveBuilding(musVillage);
        musVillage.Destroy();
        foreach (Building musLoserBuilding in musLosingFaction.OwnedBuildings)
        {
            musLoserBuilding.OwningFaction = musWinningFaction;
            musLoserBuilding.ReloadBuildingObject();
            musWinningFaction.OwnedBuildings.Add(musLoserBuilding);
        }
        CurrentFactions.Remove(musLosingFaction);
        EnemyFactions.Remove(musLosingFaction);
    }

    /// <summary>
    /// Default start function run by unity when scene is loaded.
    /// Use this to intialize values not needed by other scripts.
    /// Otherwise see Awake function
    /// </summary>
    void Start()
    {
        mmusResourceUIController = FindObjectOfType<ResourceUIScript>();
        mmusSoundManager = AudioObject.GetComponent<ExecuteSound>();
        MenuPanelController = MenuControlObject.GetComponent<MenuPanelControls>();
        SaveAndSettingsHelper.ApplyGameSettings();
        // Ensure game isn't paused/slowed down/sped up
        Time.timeScale = 1;
        PausedMenuPanel.SetActive(false);
        OptionsMenuPanel.SetActive(false);
        GameOverPanel.SetActive(mblnGameOver);
        VictoryPanel.SetActive(mblnVictory);
        CreateRewardTree();
        ResourceTicks = 0;
        if (mblnNewGame)
        {
            SaveGame();
            mblnNewGame = false;
            CheckForAndDisplayTutorialBox(InformationBoxDisplay.TutorialFlag.NewGame);
        }
        if (mblnVictory || mblnGameOver)
        {
            PauseGame();
            PausedMenuPanel.SetActive(false);
            CurrentMenuState = MENUSTATE.End_Game_State;
        }
        HotKeyManager.LoadHotkeyProfile();
        InvokeRepeating("CalculateResources", 0.5f, 2.0f);
    }

    /// <summary>
    /// Default unity update function run once per frame.
    /// Used to check inputs for mouse position and building selection
    /// </summary>
    void Update()
    {
        if (CurrentMenuState != MENUSTATE.Paused_State && CurrentMenuState != MENUSTATE.Tier_Reward_State)
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
        mmusResourceUIController.UpdateResourceUIElements(PlayerFaction.MaterialCount, PlayerFaction.WorshipperCount, PlayerFaction.Morale, PlayerFaction.TierRewardPoints);
    }

    /// <summary>
    /// Method run once per frame to check if the plaer is clicking on a building
    /// </summary>
    private void CheckForSelectedBuilding()
    {
        RaycastHit uniHitInfo;
        Ray uniRay;
        // If mouse is not over a UI object
        if (!EventSystem.current.IsPointerOverGameObject())
        {

            uniRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            // User left clicked, check if they clicked a building object
            if (Input.GetMouseButtonDown(0))
            {
                // Code for selecting a building
                if (Physics.Raycast(uniRay, out uniHitInfo))
                {
                    muniSelectedGameObject = uniHitInfo.collider.gameObject;
                    // If the user clicked the map, do nothing
                    if (muniSelectedGameObject == GameMap.GetMapObject())
                    {
                        if (CurrentMenuState == MENUSTATE.God_Selected_State)
                        {
                            PlayerGod.SetPointToMoveTowards(new Vector3(uniHitInfo.point.x, 1.5f, uniHitInfo.point.z));
                        }
                        else
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
                    }
                    else
                    {
                        if (PlayerGod.PlayerGod == muniSelectedGameObject)
                        {
                            CurrentMenuState = MENUSTATE.God_Selected_State;
                            PlayerGod.TogglePlayerOutlines(true);
                        }
                        else if (CurrentMenuState != MENUSTATE.God_Selected_State)
                        {
                            SetSelectedBuilding(GameMap.GetBuildings().Find(ClickedBuilding => ClickedBuilding.BuildingObject == muniSelectedGameObject));
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Method for setting the input building as the currently selected building.
    /// </summary>
    /// <param name="pmusBuilding">building to be set as selected</param>
    public void SetSelectedBuilding(Building pmusBuilding)
    {
        // Disable outline of currently selected building if it exists
        if (SelectedBuilding != null)
        {
            SelectedBuilding.ToggleBuildingOutlines(false);
        }
        // Enable outline for new selcted building
        SelectedBuilding = pmusBuilding;
        if (SelectedBuilding != null)
        {
            EnterBuildingSelectedMenuState();
            SelectedBuilding.ToggleBuildingOutlines(true);
        }
    }

    /// <summary>
    /// Method run once per frame to update the position of the buffered building on the screen
    /// Also checks buildings placement if the player clicks a point on the map.
    /// </summary>
    private void UpdateBufferedBuilding()
    {
        RaycastHit uniHitInfo;
        Ray uniRay;
        // Only if mouse is not on a UI object
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            uniRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(uniRay, out uniHitInfo))
            {
                BufferedBuilding.BuildingPosition = new Vector3(uniHitInfo.point.x, 1.5f, uniHitInfo.point.z);
            }
            // User left clicked
            if (Input.GetMouseButtonDown(0))
            {
                // Try to place the building
                if (GameMap.PlaceBuilding(BufferedBuilding, new Vector3(uniHitInfo.point.x, 1.5f, uniHitInfo.point.z)))
                {
                    // Reenable the collider component for selecting
                    BufferedBuilding.BuildingObject.GetComponent<Collider>().enabled = true;
                    mmusSoundManager.PlaySound("PlaceBuilding");
                    // If the building did place, go back to building menu
                    foreach (Building musBuildingOnMap in GameMap.GetBuildings())
                    {
                        musBuildingOnMap.ToggleBuildingOutlines(false);
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
                        // Tutorial for first mine, how to buy miners
                        if (BufferedBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL)
                        {
                            CheckForAndDisplayTutorialBox(InformationBoxDisplay.TutorialFlag.FirstMine);
                        }
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
        List<Building> arrOwnedBuildings = null;
        List<Building> arrAltarBuildings = null;
        List<Building> arrVillageBuildings = null;
        List<Building> arrMaterialBuildings = null;
        List<Building> arrHousingBuildings = null;
        Building musTempBuilding = null;
        Faction musChallengingFaction = null;
        int intHousingTotal = 0;
        int intMaterialsToAdd = 0;
        int intWorshippersToAdd = 0;
        float fWorPerSec = 0;
        float fMatPerSec = 0;
        // Don't run in paused state.
        if (CurrentFactions != null
            && (CurrentMenuState != MENUSTATE.Paused_State
            || CurrentMenuState != MENUSTATE.Settings_Menu_State
            || CurrentMenuState != MENUSTATE.End_Game_State))
        {
            foreach (Faction musCurrentFaction in CurrentFactions)
            {
                intHousingTotal = 0;
                intMaterialsToAdd = 0;
                intWorshippersToAdd = 0;
                // Get all buildings belonging to this faction
                arrOwnedBuildings = GameMap.GetBuildings().FindAll(MatchingBuild => MatchingBuild.OwningFaction == musCurrentFaction);

                if (arrOwnedBuildings != null)
                {
                    // Get all the altars for worshipper calculations
                    arrAltarBuildings = arrOwnedBuildings.FindAll(AltarBuilding => AltarBuilding.BuildingType == Building.BUILDING_TYPE.ALTAR);
                    // Increase worshippers for each current altar
                    foreach (Building musAltarBuilding in arrAltarBuildings)
                    {
                        // Calculate worshipper growth
                        intWorshippersToAdd += (1 * musAltarBuilding.UpgradeLevel);
                    }

                    // Get all the village buildings
                    arrVillageBuildings = arrOwnedBuildings.FindAll(VillageBuilding => VillageBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE);
                    foreach (Building musVillageBuilding in arrVillageBuildings)
                    {
                        // Calculate worshipper growth
                        intWorshippersToAdd += (1 * musVillageBuilding.UpgradeLevel);
                        // Calculatae Resource growth
                        intMaterialsToAdd += (1 * musVillageBuilding.UpgradeLevel);
                        intHousingTotal += (int)(Math.Pow(10, musVillageBuilding.UpgradeLevel + 1));
                    }

                    // Get all the material buildings
                    arrMaterialBuildings = arrOwnedBuildings.FindAll(MaterialBuilding => MaterialBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL);
                    foreach (Building musMaterialBuilding in arrMaterialBuildings)
                    {
                        // Calculatae Resource growth
                        intMaterialsToAdd += (int)Math.Ceiling(0.1 * ((MineBuilding)musMaterialBuilding).Miners * musMaterialBuilding.UpgradeLevel);
                    }

                    // Apply the morale modifier
                    intMaterialsToAdd = Mathf.CeilToInt(intMaterialsToAdd * musCurrentFaction.Morale);
                    intWorshippersToAdd = Mathf.CeilToInt(intWorshippersToAdd * musCurrentFaction.Morale);

                    if (musCurrentFaction == PlayerFaction)
                    {
                        foreach (float fMatMultiplier in marrMaterialMultipliers)
                        {
                            intMaterialsToAdd += (int)((fMatMultiplier - 1) * intMaterialsToAdd);
                        }
                        foreach (float fWorshipperMultiplier in marrWorshipperMultipliers)
                        {
                            intWorshippersToAdd += (int)((fWorshipperMultiplier - 1) * intWorshippersToAdd);
                        }
                        fWorPerSec = (float)intWorshippersToAdd / 2.0f;
                        fMatPerSec = (float)intMaterialsToAdd / 2.0f;
                    }
                    else
                    {
                        // Check if enemy has enough resources to upgrade a building, if so upgrade a random building
                        // Everything costs double for bots to allow player to eventually catch up
                        // Check if buildings exist that can be upgraded
                        if (musCurrentFaction.OwnedBuildings.FindAll(
                            upgradeableBuilding => upgradeableBuilding.UpgradeLevel < musCurrentFaction.GodTier + 1
                            && upgradeableBuilding.BuildingType != Building.BUILDING_TYPE.VILLAGE).Count > 0)
                        {
                            // A building exists that can be upgraded
                            musTempBuilding = musCurrentFaction.OwnedBuildings.Find(
                                upgradeableBuilding => upgradeableBuilding.UpgradeLevel < musCurrentFaction.GodTier + 1
                                && upgradeableBuilding.BuildingType != Building.BUILDING_TYPE.VILLAGE);
                            if (musCurrentFaction.MaterialCount > musTempBuilding.CalculateBuildingUpgradeCost() * 2)
                            {
                                musCurrentFaction.MaterialCount -= musTempBuilding.CalculateBuildingUpgradeCost();
                                musTempBuilding.UpgradeBuilding(false);
                                if (musCurrentFaction.GodTier > CurrentTier)
                                {
                                    musTempBuilding.BuildingObject.SetActive(false);
                                }
                            }
                        }
                        else
                        {
                            // No upgradeable buildings exist, build a new one
                            musTempBuilding = Building.CreateRandomBuilding(musCurrentFaction);
                            if (musCurrentFaction.MaterialCount > musTempBuilding.BuildingCost * 2)
                            {
                                if (GameMap.PlaceBuilding(musTempBuilding, GameMap.CalculateRandomPosition(musCurrentFaction)))
                                {
                                    musCurrentFaction.MaterialCount -= 2 * musTempBuilding.BuildingCost;
                                    if (musCurrentFaction.GodTier > CurrentTier)
                                    {
                                        musTempBuilding.BuildingObject.SetActive(false);
                                    }
                                }
                                else
                                {
                                    // Failed to place, destroy and try again next time
                                    musTempBuilding.Destroy();
                                }
                            }
                            else
                            {
                                // Failed to place, destroy and try again next time
                                musTempBuilding.Destroy();
                            }
                        }
                    }

                    // Add the appropriate resources
                    musCurrentFaction.MaterialCount += intMaterialsToAdd;
                    musCurrentFaction.WorshipperCount += intWorshippersToAdd;

                    // Get all the housing buildings
                    arrHousingBuildings = arrOwnedBuildings.FindAll(HousingBuild => HousingBuild.BuildingType == Building.BUILDING_TYPE.HOUSING);
                    foreach (Building musHousingBuilding in arrHousingBuildings)
                    {
                        intHousingTotal += (int)(Math.Pow(10, musHousingBuilding.UpgradeLevel + 1));
                    }
                    //Calculate morale losses/gains
                    // Each housing/village building can hold 100 * upgrade level worshippers
                    if (intHousingTotal > musCurrentFaction.WorshipperCount)
                    {
                        // There is enough housing for the current population, morale goes towards 100%
                        if (musCurrentFaction.Morale <= PlayerMoraleCap)
                        {
                            musCurrentFaction.Morale += 0.05f;
                            if (musCurrentFaction.Morale > PlayerMoraleCap)
                            {
                                musCurrentFaction.Morale = PlayerMoraleCap;
                            }
                        }
                    }
                    else
                    {
                        // Not enough housing for the population, morale will drop
                        if (musCurrentFaction.Morale > MinimumMorale)
                        {
                            musCurrentFaction.Morale -= 0.05f;
                        }
                        else
                        {
                            musCurrentFaction.Morale = MinimumMorale;
                        }
                    }
                }
            }

            // Check if player has unlocked a new tier point
            if (PlayerFaction.WorshipperCount > TierUnlockPoint)
            {
                // Player has unlocked a new reward tier point
                PlayerFaction.TierRewardPoints++;
                TierUnlockPoint *= TierUnlockPointMultiplier;
            }
        }
        ResourceTicks++;
        mintCurrentTimer += 2;
        // If set time has elapsed, enemy god will challenge the player
        if (mintCurrentTimer > EnemyChallengeTimer)
        {
            musChallengingFaction = CurrentFactions.Find(MatchingFaction => MatchingFaction != PlayerFaction && MatchingFaction.GodTier == CurrentTier);
            Time.timeScale = 0;
            CurrentMenuState = MENUSTATE.End_Game_State;
            InformationBoxController.DisplayInformationBox(
                musChallengingFaction.GodName + " has challenged you! Prepare to battle.",
                () => EnterCombatMode(musChallengingFaction),
                "Fight");
        }

    }

    /// <summary>
    /// Method for buffering a building for the player.
    /// </summary>
    /// <param name="penumBuildingType">The type of building to buffer</param>
    public void BufferBuilding(Building.BUILDING_TYPE penumBuildingType)
    {
        int intBuildingCost;
        // If there is a current buffered building, destroy it and create a new one
        if (BufferedBuilding != null)
        {
            BufferedBuilding.Destroy();
            BufferedBuilding = null;
        }
        // Part 1: Evaluates to true if not an upgrade building, or is an upgrade building and player does not currently have an upgrade building
        // Ensures only one upgrade building exists at a time for the player
        if ((penumBuildingType != Building.BUILDING_TYPE.UPGRADE
            || PlayerFaction.OwnedBuildings.Find(upgradeBuilding => upgradeBuilding.BuildingType == Building.BUILDING_TYPE.UPGRADE) == null)
        // Part 2: Evaluates to true if not a material builidng, or is a material building and the player owns less than mines than territories
        // Ensures player only has one mine per territory
            && (penumBuildingType != Building.BUILDING_TYPE.MATERIAL ||
            PlayerFaction.OwnedBuildings.FindAll(materialBuilding => materialBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL).Count < PlayerFaction.FactionArea.Count))
        {
            // Check if user has enough resources to build the building
            intBuildingCost = Building.CalculateBuildingCost(penumBuildingType);
            if (PlayerFaction.MaterialCount >= intBuildingCost)
            {
                if (penumBuildingType == Building.BUILDING_TYPE.MATERIAL)
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

    /// <summary>
    /// Create the player reward tree object displayed in the tier rewards panel.
    /// </summary>
    private void CreateRewardTree()
    {
        // Create a reward tree for the player
        marrPlayerRewardTree = TierReward.CreateTierRewardTree(PlayerFaction.Type);
        if (GameInfo.PlayerRewards.Length > 0)
        {
            LoadRewardTree(new List<string>(GameInfo.PlayerRewards));
        }
        RewardUI.GetComponentInChildren<PopulateTierIcons>().InitializeButtons(marrPlayerRewardTree);
        RewardUI.SetActive(false);
    }

    /// <summary>
    /// Enable/disable the tier reward panels, enable/disable camera movement
    /// </summary>
    /// <param name="blnActive">True to enable, false to disable</param>
    public void SetRewardsUIActive(bool blnActive = true)
    {
        RewardUI.SetActive(blnActive);
        Camera.main.GetComponent<Cam>().CameraMovementEnabled = !blnActive;
    }

    /// <summary>
    /// Enable/disable the blacksmith upgrade UI panel
    /// Also enable/disbles camera movement
    /// </summary>
    /// <param name="blnActive">True to enable, false to disable</param>
    public void SetUpgradeUIActive(bool blnActive = true)
    {
        // Enable/ disable upgrade UI
        if (blnActive)
        {
            CurrentMenuState = MENUSTATE.Upgrade_State;
        }
        else
        {
            CurrentMenuState = MENUSTATE.Building_Selected_State;
        }
        UpgradeUI.SetActive(blnActive);
        Camera.main.GetComponent<Cam>().CameraMovementEnabled = !blnActive;
    }

    /// <summary>
    /// Unlock the next tier on the map. Unhides higher tier gods
    /// If there is no higher tier, game is won.
    /// </summary>
    public void UnlockNextTier()
    {
        CurrentTier++;
        PlayerFaction.GodTier++;
        List<Faction> arrNextTierFactions = CurrentFactions.FindAll(MatchingFaction => MatchingFaction.GodTier == CurrentTier);
        foreach (Building musTempPlayerVillage in PlayerFaction.OwnedBuildings.FindAll(villageBuilding => villageBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE))
        {
            if (musTempPlayerVillage.UpgradeLevel <= CurrentTier)
            {
                musTempPlayerVillage.UpgradeBuilding(false);
            }
        }
        if (arrNextTierFactions.Count > 0)
        {
            foreach (Faction musFactionToShow in arrNextTierFactions)
            {
                musFactionToShow.SetHidden(false);
            }
        }
    }

    /// <summary>
    /// Attempt to unlock a tier reward for the player
    /// </summary>
    /// <param name="pmusReward">The reward to be unlocked</param>
    /// <returns></returns>
    public bool UnlockReward(TierReward pmusReward)
    {
        bool blnAbleToUnlock = false;
        //check if this reward can be unlocked
        blnAbleToUnlock = (pmusReward.PreviousRequiredReward == null || pmusReward.PreviousRequiredReward.Unlocked) && PlayerFaction.TierRewardPoints > 0 && !pmusReward.Unlocked;
        if (blnAbleToUnlock)
        {
            PlayerFaction.TierRewardPoints--;
            pmusReward.Unlocked = true;
            switch (pmusReward.RewardType)
            {
                case TierReward.REWARDTYPE.Ability:
                    PlayerFaction.CurrentAbilites.Add(((AbilityTierReward)pmusReward).TierAbility);
                    break;
                case TierReward.REWARDTYPE.Resource:
                    switch (pmusReward.ResourceType)
                    {
                        case TierReward.RESOURCETYPE.Material:
                            PlayerFaction.MaterialCount += ((ResourceTierReward)pmusReward).Amount;
                            break;
                        case TierReward.RESOURCETYPE.Worshipper:
                            PlayerFaction.WorshipperCount += ((ResourceTierReward)pmusReward).Amount;
                            break;
                    }
                    break;
                case TierReward.REWARDTYPE.ResourceMultiplier:
                    switch (pmusReward.ResourceType)
                    {
                        case TierReward.RESOURCETYPE.Material:
                            marrMaterialMultipliers.Add(((ResourceMultiplierTierReward)pmusReward).Multiplier);
                            break;
                        case TierReward.RESOURCETYPE.Worshipper:
                            marrWorshipperMultipliers.Add(((ResourceMultiplierTierReward)pmusReward).Multiplier);
                            break;
                    }
                    break;
                default:
                    //do nothing
                    break;
            }
        }
        return blnAbleToUnlock;
    }

    /// <summary>
    /// Method to pause the game when pause menu is open.
    /// </summary>
    public void PauseGame(bool blnDisplayOptionsMenu = true)
    {
        // Set timescale to 0 and disable camera movement.
        Time.timeScale = 0;
        Camera.main.GetComponent<Cam>().CameraMovementEnabled = false;
        if (CurrentMenuState != MENUSTATE.Paused_State)
        {
            menumLastMenuState = CurrentMenuState;
        }
        CurrentMenuState = MENUSTATE.Paused_State;
        if (blnDisplayOptionsMenu)
        {
            PausedMenuPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Method to unpause the game.
    /// </summary>
    public void UnPauseGame()
    {
        Time.timeScale = 1;
        Camera.main.GetComponent<Cam>().CameraMovementEnabled = true;
        CurrentMenuState = menumLastMenuState;
        PausedMenuPanel.SetActive(false);
    }

    /// <summary>
    /// Method to load a reward tree from a given list of strings.
    /// Used to load tier rewards that have were previously unlocked by the player before being saved.
    /// </summary>
    /// <param name="parrSavedRewards">The list of rewards already owned by the player</param>
    public void LoadRewardTree(List<string> parrSavedRewards)
    {
        TierReward musReward;
        foreach (string strSavedReward in parrSavedRewards)
        {
            musReward = TierReward.FindRewardByName(strSavedReward, marrPlayerRewardTree);
            // If the reward is found, unlock it.
            if (musReward != null)
            {
                musReward.Unlocked = true;
            }
        }
    }

    /// <summary>
    /// Method to place a random building within the given factions territory
    /// </summary>
    /// <param name="pmusPlacingFaction">The faction to place the random building for.</param>
    /// <returns></returns>
    public Building PlaceRandomBuilding(Faction pmusPlacingFaction)
    {
        Building musRandomBuilding = null;
        // Place a random building for that faction
        musRandomBuilding = Building.CreateRandomBuilding(pmusPlacingFaction);
        if (!GameMap.PlaceBuilding(musRandomBuilding, GameMap.CalculateRandomPosition(pmusPlacingFaction)))
        {
            musRandomBuilding.Destroy();
            return null;
        }
        return musRandomBuilding;
    }

    /// <summary>
    /// Method for entering build menu state.
    /// </summary>
    public void EnterBuildMenuState()
    {
        // Enable Mine/Upgrade building button depending on if player has hit building cap or not.
        bool blnAllowedToBuildMine =
            PlayerFaction.OwnedBuildings.FindAll(materialBuilding => materialBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL).Count
            < PlayerFaction.FactionArea.Count;
        bool blnAllowedToBuildUpgradeBuilding =
            PlayerFaction.OwnedBuildings.Find(upgradeBuilding => upgradeBuilding.BuildingType == Building.BUILDING_TYPE.UPGRADE) == null;

        CurrentMenuState = MENUSTATE.Building_State;
        MenuPanelController.EnterBuildMenu(blnAllowedToBuildMine, blnAllowedToBuildUpgradeBuilding);
    }

    /// <summary>
    /// Method for entering the Tier Reward Menu state. Enables Tier Reward UI
    /// </summary>
    public void EnterTierRewardsMenuState()
    {
        CurrentMenuState = MENUSTATE.Tier_Reward_State;
        SetRewardsUIActive();
    }

    /// <summary>
    /// Method or entering the building selected state.
    /// </summary>
    public void EnterBuildingSelectedMenuState()
    {
        bool blnBuildingCanBeUpgraded = false;
        bool blnIsPlayersBuilding = false;
        if (SelectedBuilding != null)
        {
            // Enable/disable the upgrade button on the current building depending on current tier and current building level.
            // Can only upgrade buildings up to the tier you are currently on 
            blnBuildingCanBeUpgraded = SelectedBuilding.UpgradeLevel < SelectedBuilding.OwningFaction.GodTier + 1;
            blnIsPlayersBuilding = SelectedBuilding.OwningFaction == PlayerFaction;
            CurrentMenuState = MENUSTATE.Building_Selected_State;
            MenuPanelController.EnterSelectedBuildingMenu(SelectedBuilding, blnIsPlayersBuilding, blnBuildingCanBeUpgraded);
        }
    }

    /// <summary>
    /// Return to the default menu state
    /// </summary>
    public void GoToDefaultMenuState()
    {
        CurrentMenuState = MENUSTATE.Default_State;
        MenuPanelController.GoToDefaultMenu();
        // Clear buffered buildings or selected buildings
        if (BufferedBuilding != null)
        {
            BufferedBuilding.Destroy();
            BufferedBuilding = null;
            foreach (Building musBuildingOnMap in GameMap.GetBuildings())
            {
                musBuildingOnMap.ToggleBuildingOutlines(false);
            }
        }
        if (SelectedBuilding != null)
        {
            SelectedBuilding.ToggleBuildingOutlines(false);
            SelectedBuilding = null;
        }
    }

    /// <summary>
    /// Enter the moving building state
    /// Can be entered from the building selected state.
    /// Move a currently owned building on the map.
    /// </summary>
    public void EnterMovingBuildingState()
    {
        if (SelectedBuilding != null)
        {
            BufferedBuilding = SelectedBuilding;
            SelectedBuilding = null;
            CurrentMenuState = MENUSTATE.Moving_Building_State;
            // Save the building position in case player cancels move.
            OriginalBuildingPosition = BufferedBuilding.BuildingPosition;
            GameMap.GetBuildings().Remove(BufferedBuilding);
            foreach (Building musBuildingOnMap in GameMap.GetBuildings())
            {
                musBuildingOnMap.ToggleBuildingOutlines(true);
            }
            BufferedBuilding.ToggleBuildingOutlines(true);
            // Disable the collider to have the raycasting ignore the held building for placement purposes
            BufferedBuilding.BuildingObject.GetComponent<Collider>().enabled = false;
        }
    }

    /// <summary>
    /// Method to upgrade the currently selected building
    /// Assumes a building is currently selected.
    /// </summary>
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
                // TODO Add notification feedback.
                Debug.Log("Already max upgrade level");
            }
            else
            {
                // TODO Add notification feedback.
                Debug.Log(string.Format("Not enough materials to upgrade ({0} required)",
                Building.CalculateBuildingUpgradeCost(SelectedBuilding.BuildingType)));
            }
            mmusSoundManager.PlaySound("NotMaterials");
        }
    }

    /// <summary>
    /// Attempt to buy a set amount of miners for the currently selected building
    /// Assumes currently selected building is a mine building.
    /// </summary>
    public void BuyMinersForSelectedBuilding()
    {
        // Number of miners to buy is calculated as an exponential number as player grows exponentially with each tier.
        if (!((MineBuilding)SelectedBuilding).BuyMiners((int)Math.Pow(10, SelectedBuilding.UpgradeLevel)))
        {
            mmusSoundManager.PlaySound("NotMaterials");
        }
        CheckForAndDisplayTutorialBox(InformationBoxDisplay.TutorialFlag.FirstMiners);
        EnterBuildingSelectedMenuState();
    }

    /// <summary>
    /// Save the current game state to a file.
    /// </summary>
    public void SaveGame(bool pblnNotifyUser = false)
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
        GameInfo.TutorialFlag = menumTutorialFlag;
        List<int> arrAttackUpgrades = new List<int>();
        List<int> arrDefenseUpgrades = new List<int>();
        List<int> arrMovementUpgrades = new List<int>();
        foreach (WorshipperUpgrade musUpgrade in PlayerFaction.CurrentUpgrades)
        {
            if (musUpgrade.UpgradeType == WorshipperUpgrade.UPGRADETYPE.Attack)
            {
                arrAttackUpgrades.Add(((AttackWorshipperUpgrade)musUpgrade).DamageBuff);
            }
            else if (musUpgrade.UpgradeType == WorshipperUpgrade.UPGRADETYPE.Defense)
            {
                arrDefenseUpgrades.Add(((DefenseWorshipperUpgrade)musUpgrade).DefenseBuff);
            }
            else if (musUpgrade.UpgradeType == WorshipperUpgrade.UPGRADETYPE.Movement)
            {
                arrMovementUpgrades.Add(((MovementWorshipperUpgrade)musUpgrade).MovementBuff);
            }
        }
        GameInfo.WorshipperAttackBuffs = arrAttackUpgrades.ToArray();
        GameInfo.WorshipperDefenseBuffs = arrDefenseUpgrades.ToArray();
        GameInfo.WorshipperMovementBuffs = arrMovementUpgrades.ToArray();

        if (SaveAndSettingsHelper.SaveGame(Application.persistentDataPath + "/SaveFiles", GameInfo))
        {
            if (pblnNotifyUser)
            {
                PauseGame(false);
                InformationBoxController.DisplayInformationBox("Saved Successfully!", () => UnPauseGame());
            }
        }
        else
        {
            if (pblnNotifyUser)
            {
                PauseGame(false);
                InformationBoxController.DisplayInformationBox("Something went wrong while saving!", () => UnPauseGame());
            }
        }
    }

    /// <summary>
    /// Return to the pause menu
    /// Used from the settings menu to return to the pause menu.
    /// </summary>
    public void ReturnToPauseMenu()
    {
        PausedMenuPanel.SetActive(true);
        CurrentMenuState = MENUSTATE.Paused_State;
        OptionsMenuPanel.SetActive(false);
    }

    /// <summary>
    /// Enter the settings menu state.
    /// Entered from the pause menu.
    /// </summary>
    public void EnterSettingsMenuState()
    {
        CurrentMenuState = MENUSTATE.Settings_Menu_State;
    }

    private void CheckForAndDisplayTutorialBox(InformationBoxDisplay.TutorialFlag penumFlagToCheckFor)
    {
        if (menumTutorialFlag == penumFlagToCheckFor)
        {
            PauseGame(false);
            InformationBoxController.DisplayTutorialBox(menumTutorialFlag++, () => UnPauseGame());
        }
    }

    /// <summary>
    /// Method for unselecting a building from outside scripts
    /// </summary>
    public void UnselectBuilding()
    {
        SelectedBuilding.ToggleBuildingOutlines(false);
        SelectedBuilding = null;
    }

    /// <summary>
    /// Clear the currently buffered building
    /// </summary>
    /// <param name="pblnDestroyBuiling">Optional parameter, if true will also destroy the currently buffered building object</param>
    public void ClearBufferedBuilding(bool pblnDestroyBuiling = false)
    {
        if (BufferedBuilding != null)
        {
            if (pblnDestroyBuiling)
            {
                BufferedBuilding.Destroy();
            }
            BufferedBuilding = null;
        }
    }
}

/// <summary>
/// Exstension class for pseduo-randomly shuffling a list. stolen
/// </summary>
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