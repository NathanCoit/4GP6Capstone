using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for tracking keyboard inputs from the user in Management mode.
/// </summary>
public class InputManager : MonoBehaviour
{
    public GameObject GamemanagerObject;
    private GameManager GameManagerScript;
    private HotKeyManager hotKeyManager = new HotKeyManager();
    private KeyCode CurrentKeyDown = KeyCode.None;

    // Use this for initialization
    void Start()
    {
        GameManagerScript = GamemanagerObject.GetComponent<GameManager>();
        hotKeyManager.LoadHotkeyProfile();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for keybaord inputs depending on current gamestate
        switch (GameManagerScript.CurrentMenuState)
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
        }

        // Check to ensure a single keystroke does not count as two menu inputs.
        if (CurrentKeyDown != KeyCode.None && Input.GetKeyUp(CurrentKeyDown))
        {
            CurrentKeyDown = KeyCode.None;
        }
    }

    /// <summary>
    /// Check the paused menu inputs. Only keyboard input is to return to game.
    /// </summary>
    private void CheckPausedMenuStateInputs()
    {

        if (Input.GetKeyDown(hotKeyManager.HotKeys["EscapeKeyCode"])
        && CurrentKeyDown != hotKeyManager.HotKeys["EscapeKeyCode"])
        {
            CurrentKeyDown = hotKeyManager.HotKeys["EscapeKeyCode"];
            GameManagerScript.UnPauseGame();
        }
    }

    /// <summary>
    /// Check the inputs in the Settings menu state. Only keycode input is to return to pause menu.
    /// </summary>
    private void CheckSettingsMenuStateInputs()
    {
        if (Input.GetKeyDown(hotKeyManager.HotKeys["EscapeKeyCode"])
        && CurrentKeyDown != hotKeyManager.HotKeys["EscapeKeyCode"])
        {
            CurrentKeyDown = hotKeyManager.HotKeys["EscapeKeyCode"];
            GameManagerScript.ReturnToPauseMenu();
        }
    }

    /// <summary>
    /// Check the inputs in the default menu state.
    /// Build, open tier rewards, or pause.
    /// </summary>
    private void CheckDefaultMenuStateInputs()
    {
        if (Input.GetKeyDown(hotKeyManager.HotKeys["BuildKeyCode"]) && CurrentKeyDown != hotKeyManager.HotKeys["BuildKeyCode"])
        {
            GameManagerScript.EnterBuildMenuState();
        }
        else if (Input.GetKeyDown(hotKeyManager.HotKeys["TierRewardKeyCode"]))
        {
            GameManagerScript.EnterTierRewardsMenuState();
        }
        else if (Input.GetKeyDown(hotKeyManager.HotKeys["EscapeKeyCode"])
            && CurrentKeyDown != hotKeyManager.HotKeys["EscapeKeyCode"])
        {
            CurrentKeyDown = hotKeyManager.HotKeys["EscapeKeyCode"];
            GameManagerScript.PauseGame();
        }
#if DEBUG
        // Debug precompile directive to give option to unlock next tier.
        else if (Input.GetKeyDown(KeyCode.T))
        {
            // Cheat to move onto next tier
            GameManagerScript.UnlockNextTier();
        }
#endif
    }

    /// <summary>
    /// Check the inputs in the Building Selected State
    /// Upgrade, Move, Open upgrade UI, Buy Miners, Start Battle, and Exit to default state
    /// </summary>
    private void CheckSelectedBuildingStateInputs()
    {
        if (GameManagerScript.SelectedBuilding != null)
        {
            // If the selected building is owned by the player, allow interaction
            if (GameManagerScript.SelectedBuilding.OwningFaction == GameManagerScript.PlayerFaction)
            {
                // Hotkeys for all buildings except villages
                if (Input.GetKeyDown(hotKeyManager.HotKeys["BuildingUpgradeKeyCode"]) && GameManagerScript.SelectedBuilding.BuildingType != Building.BUILDING_TYPE.VILLAGE)
                {
                    // Attempt to upgrade selected building
                    // 3 is max upgrade level of a building
                    GameManagerScript.UpgradeSelectedBuilding();

                }
                else if (Input.GetKeyDown(hotKeyManager.HotKeys["BuildingMoveKeyCode"]))
                {
                    //Move player building if it isn't a village
                    if (GameManagerScript.SelectedBuilding.BuildingType != Building.BUILDING_TYPE.VILLAGE)
                    {
                        GameManagerScript.EnterMovingBuildingState();
                    }
                    else
                    {
                        // TODO add cannot move feedback
                    }

                }
                // Building specific hotkeys
                else if (Input.GetKeyDown(hotKeyManager.HotKeys["BlackSmithUIKeyCode"]) && GameManagerScript.SelectedBuilding.BuildingType == Building.BUILDING_TYPE.UPGRADE)
                {
                    GameManagerScript.SetUpgradeUIActive();
                }
                else if (Input.GetKeyDown(hotKeyManager.HotKeys["BuyMinersKeyCode"]) && GameManagerScript.SelectedBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL)
                {
                    GameManagerScript.BuyMinersForSelectedBuilding();
                }
            }
            else
            {
                // TODO add options when selecting an enemy building (start battle, view stats)
                // Owning faction is not player faction, enemy building
                if (GameManagerScript.SelectedBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE)
                {
                    // Enemy Village building, can start battle here
                    if (Input.GetKeyDown(hotKeyManager.HotKeys["StartBattleKeyCode"]))
                    {
                        // Initialize info file variables, save game state, move to combat mode scene
                        GameManagerScript.EnterCombatMode();
                    }
                }
            }
        }
        // Exit back to default state.
        if (Input.GetKeyDown(hotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            GameManagerScript.UnselectBuilding();
            GameManagerScript.GoToDefaultMenuState();
            CurrentKeyDown = hotKeyManager.HotKeys["EscapeKeyCode"];
        }
    }

    /// <summary>
    /// Check building state inputs
    /// Build an Altar, a Mine, a House, or an Upgrade Building
    /// </summary>
    private void CheckBuildingStateInputs()
    {
        if (Input.GetKeyDown(hotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            GameManagerScript.GoToDefaultMenuState();
            CurrentKeyDown = hotKeyManager.HotKeys["EscapeKeyCode"];
        }
        else if (Input.GetKeyDown(hotKeyManager.HotKeys["AltarKeyCode"]))
        {
            GameManagerScript.BufferBuilding(Building.BUILDING_TYPE.ALTAR);
        }
        else if (Input.GetKeyDown(hotKeyManager.HotKeys["MineKeyCode"]))
        {
            GameManagerScript.BufferBuilding(Building.BUILDING_TYPE.MATERIAL);
        }
        else if (Input.GetKeyDown(hotKeyManager.HotKeys["HouseKeyCode"]))
        {
            GameManagerScript.BufferBuilding(Building.BUILDING_TYPE.HOUSING);
        }
        else if (Input.GetKeyDown(hotKeyManager.HotKeys["BlacksmithKeyCode"]))
        {
            GameManagerScript.BufferBuilding(Building.BUILDING_TYPE.UPGRADE);
        }
    }

    /// <summary>
    /// Check inputs in the Moving building state
    /// Special state is a combination of building selected and building buffered
    /// Only keyboard input is to reset building to pre moved state.
    /// </summary>
    private void CheckMovingBuildingStateInputs()
    {
        if (Input.GetKeyDown(hotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            // If user wishes to cancel building move, reset building to its original position before move
            CurrentKeyDown = hotKeyManager.HotKeys["EscapeKeyCode"];
            foreach (Building BuildingOnMap in GameManagerScript.GameMap.GetBuildings())
            {
                BuildingOnMap.ToggleBuildingOutlines(false);
            }
            if (GameManagerScript.BufferedBuilding != null)
            {
                // Ignore building placements checks as user should never be unable to cancel a move
                GameManagerScript.GameMap.PlaceBuilding(GameManagerScript.BufferedBuilding, GameManagerScript.OriginalBuildingPosition, true);
                GameManagerScript.BufferedBuilding.ToggleBuildingOutlines(true);
                GameManagerScript.BufferedBuilding.BuildingObject.GetComponent<Collider>().enabled = true;
            }
            // Move back to the selected building state.
            GameManagerScript.SetSelectedBuilding(GameManagerScript.BufferedBuilding);
            GameManagerScript.ClearBufferedBuilding();
        }
    }

    /// <summary>
    /// Check for inputs in the tier reward state.
    /// Only keyboard input is to exit to default game state.
    /// </summary>
    private void CheckTierRewardStateInputs()
    {
        if (Input.GetKeyDown(hotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            GameManagerScript.GoToDefaultMenuState();
            GameManagerScript.SetRewardsUIActive(false);
            CurrentKeyDown = hotKeyManager.HotKeys["EscapeKeyCode"];
        }
    }

    /// <summary>
    /// Check for inputs in the upgrade ui state
    /// Only keyboard input is to return to selecting the blacksmith building
    /// </summary>
    private void CheckUpgradeStateInput()
    {
        if (Input.GetKeyDown(hotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            GameManagerScript.EnterBuildingSelectedMenuState();
            GameManagerScript.SetUpgradeUIActive(false);
            CurrentKeyDown = hotKeyManager.HotKeys["EscapeKeyCode"];
        }
    }
}
