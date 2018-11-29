using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public GameObject GamemanagerObject;
    private GameManager GameManagerScript;
    private HotKeyManager hotKeyManager = new HotKeyManager();

	// Use this for initialization
	void Start () {
        GameManagerScript = GamemanagerObject.GetComponent<GameManager>();
        hotKeyManager.LoadHotkeyProfile();
	}
	
	// Update is called once per frame
	void Update () {
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
        }
    }

    private void CheckDefaultMenuStateInputs()
    {
        if (Input.GetKeyDown(hotKeyManager.HotKeys["BuildKeyCode"]))
        {
            GameManagerScript.EnterBuildMenuState();
        }
        else if (Input.GetKeyDown(hotKeyManager.HotKeys["TierRewardKeyCode"]))
        {
            GameManagerScript.EnterTierRewardsMenuState();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            // Cheat to move onto next tier
            GameManagerScript.UnlockNextTier();
        }
    }

    private void CheckSelectedBuildingStateInputs()
    {
        if (GameManagerScript.SelectedBuilding != null)
        {
            // Player's building
            if (GameManagerScript.SelectedBuilding.OwningFaction == GameManagerScript.PlayerFaction)
            {
                // Global hotkey for a selected building
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
        if (Input.GetKeyDown(hotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            GameManagerScript.SelectedBuilding.ToggleBuildingOutlines(false);
            GameManagerScript.SelectedBuilding = null;
            GameManagerScript.GoToDefaultMenuState();
            GameManagerScript.mblnPauseKeyDown = true;
        }
    }

    private void CheckBuildingStateInputs()
    {
        if (Input.GetKeyDown(hotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            GameManagerScript.GoToDefaultMenuState();
            GameManagerScript.mblnPauseKeyDown = true;
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

    private void CheckMovingBuildingStateInputs()
    {
        if (Input.GetKeyDown(hotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            GameManagerScript.mblnPauseKeyDown = true;
            foreach (Building BuildingOnMap in GameManagerScript.GameMap.GetBuildings())
            {
                BuildingOnMap.ToggleBuildingOutlines(false);
            }
            if (GameManagerScript.BufferedBuilding != null)
            {
                GameManagerScript.GameMap.PlaceBuilding(GameManagerScript.BufferedBuilding, GameManagerScript.OriginalBuildingPosition);
                GameManagerScript.BufferedBuilding.ToggleBuildingOutlines(true);
                GameManagerScript.BufferedBuilding.BuildingObject.GetComponent<Collider>().enabled = true;
            }
            GameManagerScript.SelectedBuilding = GameManagerScript.BufferedBuilding;
            GameManagerScript.BufferedBuilding = null;
            GameManagerScript.EnterBuildingSelectedMenuState();
        }
    }

    private void CheckTierRewardStateInputs()
    {
        if (Input.GetKeyDown(hotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            GameManagerScript.GoToDefaultMenuState();
            GameManagerScript.SetRewardsUIActive(false);
            GameManagerScript.mblnPauseKeyDown = true;
        }
    }

    private void CheckUpgradeStateInput()
    {
        if (Input.GetKeyDown(hotKeyManager.HotKeys["EscapeKeyCode"]))
        {
            GameManagerScript.CurrentMenuState = GameManager.MENUSTATE.Building_Selected_State;
            GameManagerScript.SetUpgradeUIActive(false);
            GameManagerScript.mblnPauseKeyDown = true;
        }
    }
}
