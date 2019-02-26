using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class containing methods used by on click UI buttons
/// Contains checks to prevent clicking while paused
/// Heavily connected with game manager.
/// </summary>
public class OnClickFunctions : MonoBehaviour
{
    public GameObject GameManagerObject;
    private GameManager GameManagerScript;

    public GameObject MenuPanelObject;
    private MenuPanelControls MenuPanelController;

    public GameObject PausedMenuPanel;
    public GameObject OptionsMenuPanel;
    public ConfirmationBoxController ConfirmationBoxScript;

    // Use this for initialization
    void Start()
    {
        GameManagerScript = GameManagerObject.GetComponent<GameManager>();
        MenuPanelController = MenuPanelObject.GetComponent<MenuPanelControls>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadLastSave()
    {
        SaveAndSettingsHelper.LoadLastSave(Application.persistentDataPath + "/SaveFiles", GameManagerScript.GameInfo);
    }

    public void SaveGameSettings()
    {
        SaveAndSettingsHelper.SaveSettingsFromOptionsMenu();
        GameManagerScript.HotKeyManager.LoadHotkeyProfile();
        MenuPanelController.SetButtonText();
        SaveAndSettingsHelper.ApplyGameSettings();
        GameManagerScript.ReturnToPauseMenu();
    }

    public void QuitToMenu()
    {
        // Destroy gameinfo object
        Destroy(GameManagerScript.GameInfo.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenOptionsMenu()
    {
        PausedMenuPanel.SetActive(false);
        OptionsMenuPanel.SetActive(true);
        GameManagerScript.EnterSettingsMenuState();
        SaveAndSettingsHelper.ApplySettingsToOptionsMenu();
    }

    public void SaveGame()
    {
        ConfirmationBoxScript.AttachCallbackToConfirmationBox(GameManagerScript.SaveGame, "Do you want to save?");
    }

    public void BuyMiners()
    {
        if(CheckIfPaused())
        {
            return;
        }
        GameManagerScript.BuyMinersForSelectedBuilding();
    }

    public void Upgrade()
    {
        if (CheckIfPaused())
        {
            return;
        }
        GameManagerScript.UpgradeSelectedBuilding();
    }

    public void Move()
    {
        if (CheckIfPaused())
        {
            return;
        }
        GameManagerScript.EnterMovingBuildingState();
    }

    public void ShowUpgradeShop()
    {
        if (CheckIfPaused())
        {
            return;
        }
        GameManagerScript.SetUpgradeUIActive();
    }

    public void BufferAltar()
    {
        if (CheckIfPaused())
        {
            return;
        }
        GameManagerScript.BufferBuilding(Building.BUILDING_TYPE.ALTAR);
    }

    public void BufferMine()
    {
        if (CheckIfPaused())
        {
            return;
        }
        GameManagerScript.BufferBuilding(Building.BUILDING_TYPE.MATERIAL);
    }

    public void BufferHousing()
    {
        if (CheckIfPaused())
        {
            return;
        }
        GameManagerScript.BufferBuilding(Building.BUILDING_TYPE.HOUSING);
    }

    public void BufferUpgrade()
    {
        if (CheckIfPaused())
        {
            return;
        }
        GameManagerScript.BufferBuilding(Building.BUILDING_TYPE.UPGRADE);
    }

    public void EnterBuildMenu()
    {
        if (CheckIfPaused())
        {
            return;
        }
        GameManagerScript.EnterBuildMenuState();
    }

    public void OpenTierRewards()
    {
        if (CheckIfPaused())
        {
            return;
        }
        GameManagerScript.EnterTierRewardsMenuState();
    }

    public void PauseGame()
    {
        if (CheckIfPaused())
        {
            return;
        }
        GameManagerScript.PauseGame();
    }

    public void OptionsMenuCancelButtonClicked()
    {
        if (SaveAndSettingsHelper.CheckForChangesInOptionsMenu())
        {
            ConfirmationBoxScript.AttachCallbackToConfirmationBox(
            GameManagerScript.ReturnToPauseMenu,
            "Unsaved changes will be lost. Are you sure you don't want to save?",
            "Don't Save",
            "Cancel");
        }
        else
        {
            GameManagerScript.ReturnToPauseMenu();
        }
    }

    private bool CheckIfPaused()
    {
        return GameManagerScript.CurrentMenuState == GameManager.MENUSTATE.Paused_State
            || GameManagerScript.CurrentMenuState == GameManager.MENUSTATE.Settings_Menu_State;
    }
}
