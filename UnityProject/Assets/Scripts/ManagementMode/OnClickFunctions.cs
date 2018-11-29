using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickFunctions : MonoBehaviour {
    public GameObject GameManagerObject;
    private GameManager GameManagerScript;

    public GameObject MenuPanelObject;
    private MenuPanelControls MenuPanelController;

    public GameObject PausedMenuPanel;
    public GameObject OptionsMenuPanel;

    // Use this for initialization
    void Start () {
        GameManagerScript = GameManagerObject.GetComponent<GameManager>();
        MenuPanelController = MenuPanelObject.GetComponent<MenuPanelControls>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadLastSave()
    {
        SaveAndSettingsHelper.LoadLastSave(Application.persistentDataPath + "/SaveFiles", GameManagerScript.gameInfo);
    }

    public void SaveGameSettings()
    {
        SaveAndSettingsHelper.SaveSettingsFromOptionsMenu();
        GameManagerScript.hotKeyManager.LoadHotkeyProfile();
        MenuPanelController.SetButtonText();
        SaveAndSettingsHelper.ApplyGameSettings();
    }

    public void QuitToMenu()
    {
        // Destroy gameinfo object
        Destroy(GameManagerScript.gameInfo.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenOptionsMenu()
    {
        PausedMenuPanel.SetActive(false);
        OptionsMenuPanel.SetActive(true);
        GameManagerScript.CurrentMenuState = GameManager.MENUSTATE.Settings_Menu_State;
        SaveAndSettingsHelper.ApplySettingsToOptionsMenu();
    }

    public void SaveGame()
    {
        GameManagerScript.SaveGame();
    }

    public void BuyMiners()
    {
        GameManagerScript.BuyMinersForSelectedBuilding();
    }

    public void Upgrade()
    {
        GameManagerScript.UpgradeSelectedBuilding();
    }

    public void Move()
    {
        GameManagerScript.EnterMovingBuildingState();
    }

    public void ShowUpgradeShop()
    {
        GameManagerScript.SetUpgradeUIActive();
    }

    public void BufferAltar()
    {
        GameManagerScript.BufferBuilding(Building.BUILDING_TYPE.ALTAR);
    }

    public void BufferMine()
    {
        GameManagerScript.BufferBuilding(Building.BUILDING_TYPE.MATERIAL);
    }

    public void BufferHousing()
    {
        GameManagerScript.BufferBuilding(Building.BUILDING_TYPE.HOUSING);
    }

    public void BufferUpgrade()
    {
        GameManagerScript.BufferBuilding(Building.BUILDING_TYPE.UPGRADE);
    }
}
