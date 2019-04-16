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
    public InputManager InputScript;
    public GameObject MenuPanelObject;
    public GameObject PausedMenuPanel;
    public GameObject OptionsMenuPanel;
    public ConfirmationBoxController ConfirmationBoxScript;

    private MenuPanelControls mmusMenuPanelController;
    private GameManager mmusGameManagerScript;

    // Use this for initialization
    void Start()
    {
        mmusGameManagerScript = GameManagerObject.GetComponent<GameManager>();
        mmusMenuPanelController = MenuPanelObject.GetComponent<MenuPanelControls>();
    }

    /// <summary>
    /// Attached to game over load last save button
    /// </summary>
    public void LoadLastSave()
    {
        SaveAndSettingsHelper.LoadLastSave(Application.persistentDataPath + "/SaveFiles", mmusGameManagerScript.GameInfo);
    }

    /// <summary>
    /// Attached to Save settings button
    /// </summary>
    public void SaveGameSettings()
    {
        SaveAndSettingsHelper.SaveSettingsFromOptionsMenu();
        mmusGameManagerScript.HotKeyManager.LoadHotkeyProfile();
        InputScript.RefreshHotkeyProfile();
        mmusMenuPanelController.SetButtonText();
        SaveAndSettingsHelper.ApplyGameSettings();
        mmusGameManagerScript.ReturnToPauseMenu();
    }

    /// <summary>
    /// Attached to pause quit button
    /// </summary>
    public void QuitToMenu()
    {
        // Destroy gameinfo object
        Destroy(mmusGameManagerScript.GameInfo.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Attached to pause options button
    /// </summary>
    public void OpenOptionsMenu()
    {
        PausedMenuPanel.SetActive(false);
        OptionsMenuPanel.SetActive(true);
        mmusGameManagerScript.EnterSettingsMenuState();
        SaveAndSettingsHelper.ApplySettingsToOptionsMenu();
    }

    /// <summary>
    /// Attached to save game button
    /// </summary>
    public void SaveGame()
    {
        mmusGameManagerScript.SaveGame(true);
    }

    // Methods attached to buttons specific for buildings

    public void BuyMiners()
    {
        if(CheckIfPaused())
        {
            return;
        }
        mmusGameManagerScript.BuyMinersForSelectedBuilding();
    }

    public void Upgrade()
    {
        if (CheckIfPaused())
        {
            return;
        }
        mmusGameManagerScript.UpgradeSelectedBuilding();
    }

    public void Move()
    {
        if (CheckIfPaused())
        {
            return;
        }
        mmusGameManagerScript.EnterMovingBuildingState();
    }

    public void ShowUpgradeShop()
    {
        if (CheckIfPaused())
        {
            return;
        }
        mmusGameManagerScript.SetUpgradeUIActive();
    }

    public void BufferAltar()
    {
        if (CheckIfPaused())
        {
            return;
        }
        mmusGameManagerScript.BufferBuilding(Building.BUILDING_TYPE.ALTAR);
    }

    public void BufferMine()
    {
        if (CheckIfPaused())
        {
            return;
        }
        mmusGameManagerScript.BufferBuilding(Building.BUILDING_TYPE.MATERIAL);
    }

    public void BufferHousing()
    {
        if (CheckIfPaused())
        {
            return;
        }
        mmusGameManagerScript.BufferBuilding(Building.BUILDING_TYPE.HOUSING);
    }

    public void BufferUpgrade()
    {
        if (CheckIfPaused())
        {
            return;
        }
        mmusGameManagerScript.BufferBuilding(Building.BUILDING_TYPE.UPGRADE);
    }

    public void EnterBuildMenu()
    {
        if (CheckIfPaused())
        {
            return;
        }
        mmusGameManagerScript.EnterBuildMenuState();
    }

    public void OpenTierRewards()
    {
        if (CheckIfPaused())
        {
            return;
        }
        mmusGameManagerScript.EnterTierRewardsMenuState();
    }
    
    public void PauseGame()
    {
        if (CheckIfPaused())
        {
            return;
        }
        mmusGameManagerScript.PauseGame();
    }

    public void OptionsMenuCancelButtonClicked()
    {
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

    /// <summary>
    /// Check if the game is paused to disable some building buttons.
    /// Makes sure player can't interact with buildings while paused.
    /// </summary>
    /// <returns></returns>
    private bool CheckIfPaused()
    {
        return mmusGameManagerScript.CurrentMenuState == GameManager.MENUSTATE.Paused_State
            || mmusGameManagerScript.CurrentMenuState == GameManager.MENUSTATE.Settings_Menu_State;
    }
}
