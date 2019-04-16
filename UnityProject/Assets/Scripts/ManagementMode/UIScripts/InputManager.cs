using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for tracking keyboard inputs from the user in Management mode.
/// Controls what actions are to be taken depending on key strokes and call other scripts to perform actions.
/// </summary>
public class InputManager : MonoBehaviour
{
    public GameObject GameManagerObject;
    public ConfirmationBoxController ConfirmationBoxScript;

    private GameManager mmusGameManagerScript;
    private HotKeyManager mmusHotKeyManager = new HotKeyManager();
    private KeyCode muniCurrentKeyDown = KeyCode.None;
    private Cam mmusGameCamera;

    // Use this for initialization
    void Start()
    {
        mmusGameManagerScript = GameManagerObject.GetComponent<GameManager>();
        mmusHotKeyManager.LoadHotkeyProfile();
        mmusGameCamera = Camera.main.GetComponent<Cam>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for keybaord inputs depending on current gamestate
        switch (mmusGameManagerScript.CurrentMenuState)
        {
            case GameManager.MENUSTATE.Default_State:
                CheckDefaultMenuStateInputs();
                break;
            case GameManager.MENUSTATE.Building_State:
                CheckBuildingStateInputs();
                break;
            case GameManager.MENUSTATE.Moving_Building_State:
                CheckMovingBuildingStateInputs();
                break;
            case GameManager.MENUSTATE.Building_Selected_State:
                CheckSelectedBuildingStateInputs();
                break;
            case GameManager.MENUSTATE.Tier_Reward_State:
                CheckTierRewardStateInputs();
                break;
            case GameManager.MENUSTATE.Upgrade_State:
                CheckUpgradeStateInput();
                break;
            case GameManager.MENUSTATE.Paused_State:
                CheckPausedMenuStateInputs();
                break;
            case GameManager.MENUSTATE.Settings_Menu_State:
                CheckSettingsMenuStateInputs();
                break;
            case GameManager.MENUSTATE.God_Selected_State:
                CheckGodSelectedMenuState();
                break;
        }

        // Check to ensure a single keystroke does not count as two menu inputs.
        // Fixes issue where pressing escape closes multiple menus
        if (muniCurrentKeyDown != KeyCode.None && Input.GetKeyUp(muniCurrentKeyDown))
        {
            muniCurrentKeyDown = KeyCode.None;
        }
    }

    /// <summary>
    /// Keyboard inputs while the player's god is selected
    /// </summary>
    private void CheckGodSelectedMenuState()
    {
        if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["EscapeKeyCode"])
            && muniCurrentKeyDown != mmusHotKeyManager.HotKeys["EscapeKeyCode"])
        {
            muniCurrentKeyDown = mmusHotKeyManager.HotKeys["EscapeKeyCode"];
            mmusGameManagerScript.GoToDefaultMenuState();
            mmusGameManagerScript.PlayerGod.TogglePlayerOutlines(false);
        }
    }

    /// <summary>
    /// Check the paused menu inputs. Only keyboard input is to return to game.
    /// </summary>
    private void CheckPausedMenuStateInputs()
    {
        // If confirmation box is showing, use escape to close it instead of the pause menu
        if (ConfirmationBoxScript.BoxIsActive
            && Input.GetKeyDown(mmusHotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            ConfirmationBoxScript.HideConfirmationBox();
        }
        else if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["EscapeKeyCode"])
        && muniCurrentKeyDown != mmusHotKeyManager.HotKeys["EscapeKeyCode"])
        {
            muniCurrentKeyDown = mmusHotKeyManager.HotKeys["EscapeKeyCode"];
            mmusGameManagerScript.UnPauseGame();
        }
    }

    /// <summary>
    /// Check the inputs in the Settings menu state. Only keycode input is to return to pause menu.
    /// </summary>
    private void CheckSettingsMenuStateInputs()
    {
        if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["EscapeKeyCode"])
        && muniCurrentKeyDown != mmusHotKeyManager.HotKeys["EscapeKeyCode"])
        {
            // Check for changes in options before closing
            muniCurrentKeyDown = mmusHotKeyManager.HotKeys["EscapeKeyCode"];
            if (SaveAndSettingsHelper.CheckForChangesInOptionsMenu())
            {
                ConfirmationBoxScript.AttachCallbackToConfirmationBox(
                mmusGameManagerScript.ReturnToPauseMenu,
                "Unsaved changes will be lost. Are you sure you don't want to save?",
                "Don't Save",
                "Cancel");
            }
            else
            {
                mmusGameManagerScript.ReturnToPauseMenu();
            }
        }
    }

    /// <summary>
    /// Check the inputs in the default menu state.
    /// Build, open tier rewards, or pause.
    /// </summary>
    private void CheckDefaultMenuStateInputs()
    {
        if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["BuildKeyCode"]) && muniCurrentKeyDown != mmusHotKeyManager.HotKeys["BuildKeyCode"])
        {
            mmusGameManagerScript.EnterBuildMenuState();
        }
        else if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["TierRewardKeyCode"]))
        {
            mmusGameManagerScript.EnterTierRewardsMenuState();
        }
        else if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["EscapeKeyCode"])
            && muniCurrentKeyDown != mmusHotKeyManager.HotKeys["EscapeKeyCode"])
        {
            muniCurrentKeyDown = mmusHotKeyManager.HotKeys["EscapeKeyCode"];
            mmusGameManagerScript.PauseGame();
        }
        else if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["CentreOnGodKeyCode"]))
        {
            mmusGameCamera.CentreOnGod();
        }
        else if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["CentreOnVillageKeyCode"]))
        {
            mmusGameCamera.CentreOnVillage();
        }
#if DEBUG
        // Debug precompile directive to give option to unlock next tier.
        else if (Input.GetKeyDown(KeyCode.T))
        {
            // Cheat to move onto next tier
            mmusGameManagerScript.UnlockNextTier();
        }
#endif
    }

    /// <summary>
    /// Check the inputs in the Building Selected State
    /// Upgrade, Move, Open upgrade UI, Buy Miners, Start Battle, and Exit to default state
    /// </summary>
    private void CheckSelectedBuildingStateInputs()
    {
        if (mmusGameManagerScript.SelectedBuilding != null)
        {
            // If the selected building is owned by the player, allow interaction
            if (mmusGameManagerScript.SelectedBuilding.OwningFaction == mmusGameManagerScript.PlayerFaction)
            {
                // Hotkeys for all buildings except villages
                if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["BuildingUpgradeKeyCode"])
                    && mmusGameManagerScript.SelectedBuilding.BuildingType != Building.BUILDING_TYPE.VILLAGE)
                {
                    // Attempt to upgrade selected building
                    // 3 is max upgrade level of a building
                    mmusGameManagerScript.UpgradeSelectedBuilding();

                }
                else if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["BuildingMoveKeyCode"]))
                {
                    //Move player building if it isn't a village
                    if (mmusGameManagerScript.SelectedBuilding.BuildingType != Building.BUILDING_TYPE.VILLAGE)
                    {
                        mmusGameManagerScript.EnterMovingBuildingState();
                    }

                }
                // Building specific hotkeys
                else if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["BlackSmithUIKeyCode"])
                    && mmusGameManagerScript.SelectedBuilding.BuildingType == Building.BUILDING_TYPE.UPGRADE)
                {
                    mmusGameManagerScript.SetUpgradeUIActive();
                }
                else if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["BuyMinersKeyCode"])
                    && mmusGameManagerScript.SelectedBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL)
                {
                    mmusGameManagerScript.BuyMinersForSelectedBuilding();
                }
            }
            else
            {
                // Owning faction is not player faction, enemy building
                if (mmusGameManagerScript.SelectedBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE)
                {
                    // Enemy Village building, can start battle here
                    if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["StartBattleKeyCode"]))
                    {
                        // Initialize info file variables, save game state, move to combat mode scene
                        mmusGameManagerScript.EnterCombatMode();
                    }
                }
            }
        }
        // Exit back to default state.
        if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            mmusGameManagerScript.UnselectBuilding();
            mmusGameManagerScript.GoToDefaultMenuState();
            muniCurrentKeyDown = mmusHotKeyManager.HotKeys["EscapeKeyCode"];
        }
    }

    /// <summary>
    /// Check building state inputs
    /// Buffer an Altar, a Mine, a House, or an Upgrade Building
    /// </summary>
    private void CheckBuildingStateInputs()
    {
        if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            mmusGameManagerScript.GoToDefaultMenuState();
            muniCurrentKeyDown = mmusHotKeyManager.HotKeys["EscapeKeyCode"];
        }
        else if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["AltarKeyCode"]))
        {
            mmusGameManagerScript.BufferBuilding(Building.BUILDING_TYPE.ALTAR);
        }
        else if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["MineKeyCode"]))
        {
            mmusGameManagerScript.BufferBuilding(Building.BUILDING_TYPE.MATERIAL);
        }
        else if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["HouseKeyCode"]))
        {
            mmusGameManagerScript.BufferBuilding(Building.BUILDING_TYPE.HOUSING);
        }
        else if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["BlacksmithKeyCode"]))
        {
            mmusGameManagerScript.BufferBuilding(Building.BUILDING_TYPE.UPGRADE);
        }
    }

    /// <summary>
    /// Check inputs in the Moving building state
    /// Special state is a combination of building selected and building buffered
    /// Only keyboard input is to reset building to pre moved state.
    /// </summary>
    private void CheckMovingBuildingStateInputs()
    {
        if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            // If user wishes to cancel building move, reset building to its original position before move
            muniCurrentKeyDown = mmusHotKeyManager.HotKeys["EscapeKeyCode"];
            foreach (Building BuildingOnMap in mmusGameManagerScript.GameMap.GetBuildings())
            {
                BuildingOnMap.ToggleObjectOutlines(false);
            }
            if (mmusGameManagerScript.BufferedBuilding != null)
            {
                // Ignore building placements checks as user should never be unable to cancel a move
                mmusGameManagerScript.GameMap.PlaceBuilding(mmusGameManagerScript.BufferedBuilding, mmusGameManagerScript.OriginalBuildingPosition, true);
                mmusGameManagerScript.BufferedBuilding.ToggleObjectOutlines(true);
                mmusGameManagerScript.BufferedBuilding.MapGameObject.GetComponent<Collider>().enabled = true;
            }
            // Move back to the selected building state.
            mmusGameManagerScript.SetSelectedBuilding(mmusGameManagerScript.BufferedBuilding);
            mmusGameManagerScript.ClearBufferedBuilding();
        }
    }

    /// <summary>
    /// Check for inputs in the tier reward state.
    /// Only keyboard input is to exit to default game state.
    /// </summary>
    private void CheckTierRewardStateInputs()
    {
        if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            mmusGameManagerScript.GoToDefaultMenuState();
            mmusGameManagerScript.SetRewardsUIActive(false);
            mmusGameManagerScript.RewardUI.GetComponentInChildren<PopulateTierIcons>().RewardTooltipController.TooltipTextGameObject.SetActive(false);
            muniCurrentKeyDown = mmusHotKeyManager.HotKeys["EscapeKeyCode"];
        }
    }

    /// <summary>
    /// Check for inputs in the upgrade ui state
    /// Only keyboard input is to return to selecting the blacksmith building
    /// </summary>
    private void CheckUpgradeStateInput()
    {
        if (Input.GetKeyDown(mmusHotKeyManager.HotKeys["EscapeKeyCode"]) || Input.GetKeyDown(mmusHotKeyManager.HotKeys["BlackSmithUIKeyCode"]))
        {
            mmusGameManagerScript.EnterBuildingSelectedMenuState();
            mmusGameManagerScript.SetUpgradeUIActive(false);
            muniCurrentKeyDown
                = Input.GetKeyDown(mmusHotKeyManager.HotKeys["EscapeKeyCode"])
                ? mmusHotKeyManager.HotKeys["EscapeKeyCode"]
                : mmusHotKeyManager.HotKeys["BlackSmithUIKeyCode"];
        }
    }

    /// <summary>
    /// Method to detect when the game loses/gains focus
    /// Used to pause game when player tabs out
    /// </summary>
    /// <param name="pblnIsFocus"></param>
    private void OnApplicationFocus(bool pblnIsFocus)
    {
        if (!pblnIsFocus)
        {
            // Pause game on tab out so camera doesn't pan all over the place when tabbed out
            mmusGameManagerScript.PauseGame();
        }
    }

    /// <summary>
    /// Refresh the local copy of the hotkey profile in case hotkey settings are changed during gameplay
    /// </summary>
    public void RefreshHotkeyProfile()
    {
        mmusHotKeyManager.LoadHotkeyProfile();
    }
}
