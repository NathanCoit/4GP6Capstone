using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class MainMenu : MonoBehaviour {
    
    public GameObject GameInfoObjectPrefab;
    public GameObject GodTypeDropDownObject;
    public GameObject GodNameInputFieldObject;
    public GameObject NewGameOptionsPanel;
    public GameObject MainUIPanel;
    public GameObject LoadGameOptionsPanel;
    public GameObject LoadMenuScrollPanel;
    public GameObject OptionsPanel;
    public GameObject AudioSliderObject;
    public UnityEngine.Object SaveButtonPrefab;

    private string mstrGameSaveFileDirectory;
    private List<GameObject> marrButtonObjects;
    private GameInfo mmusGameInfo = null;

    void Awake()
    {
        // Give a consistent save file path for files
        // Unity provides a default persitent directory
        mstrGameSaveFileDirectory = Application.persistentDataPath + "/SaveFiles";
    }

	// Use this for initialization
	void Start () {
        // Disable all but main menu panel. Makes scene starting panel not reliant on scene settings
        DisableAllPanels();
        MainUIPanel.SetActive(true);
        SaveAndSettingsHelper.ApplyGameSettings();
	}
	
	// Update is called once per frame
	void Update () {
        // Maximum menu depth is 1, escape always returns to main menu
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            OpenMainUI();
        }
	}

    /// <summary>
    /// Method run when the start button game is clicked in the main menu
    /// </summary>
    public void StartButtonClicked()
    {
        Dropdown uniGodTypeDropDown = GodTypeDropDownObject.GetComponent<Dropdown>();
        InputField uniGodNameInputFiled = GodNameInputFieldObject.GetComponent<InputField>();
        string strGodName = uniGodNameInputFiled.text;
        Faction.GodType musGodType;
        // Can't use empty god name
        // Also limit size of god name to prevent text overflow in game
        if (!string.IsNullOrEmpty(strGodName) && strGodName.Length < 15)
        {
            musGodType = (Faction.GodType)Enum.Parse(typeof(Faction.GodType), uniGodTypeDropDown.options[uniGodTypeDropDown.value].text, true);
            StartNewGame(strGodName, musGodType);
        }
        else
        {
            // TODO Feedback, need a god name or name was too long
        }
    }

    /// <summary>
    /// Method for loading the management scene as a new game
    /// </summary>
    /// <param name="pstrGodName"></param>
    /// <param name="penumGodType"></param>
    public void StartNewGame(string pstrGodName, Faction.GodType penumGodType)
    {
        // Create an empty game info object, management scene will create starting scene
        GameObject uniNewGameInfoObject = (GameObject)Instantiate(GameInfoObjectPrefab);
        uniNewGameInfoObject.name = "GameInfo";
        mmusGameInfo = uniNewGameInfoObject.GetComponent<GameInfo>();
        mmusGameInfo.PlayerFaction.GodName = pstrGodName;
        mmusGameInfo.PlayerFaction.Type = penumGodType;
        SceneManager.LoadScene("UnderGodScene");
    }
    
    /// <summary>
    /// Method linked to open new game panel button
    /// </summary>
    public void OpenNewGameOptions()
    {
        DisableAllPanels();
        NewGameOptionsPanel.SetActive(true);
    }

    /// <summary>
    /// Method linked to escape key or back buttons in sub menus
    /// </summary>
    public void OpenMainUI()
    {
        DisableAllPanels();
        MainUIPanel.SetActive(true);
    }

    /// <summary>
    /// Method linked to open options menu button
    /// </summary>
    public void OpenOptionsMenu()
    {
        DisableAllPanels();
        OptionsPanel.SetActive(true);
        SaveAndSettingsHelper.ApplySettingsToOptionsMenu();
    }

    /// <summary>
    /// Method to close all panels and clear save file button objects
    /// </summary>
    public void DisableAllPanels()
    {
        MainUIPanel.SetActive(false);
        NewGameOptionsPanel.SetActive(false);
        LoadGameOptionsPanel.SetActive(false);
        OptionsPanel.SetActive(false);
        DestroySaveFileButtons();
    }

    /// <summary>
    /// Method linked to load saves button
    /// Also creates the save file buttons when loading
    /// </summary>
    public void EnableLoadSaveGamePanel()
    {
        DisableAllPanels();
        LoadGameOptionsPanel.SetActive(true);
        CreateSaveFileButtons();
    }

    /// <summary>
    /// Method for creating save file button objects that player can click on to load files
    /// Save file buttons are dynamically generated based on info from save and number of saves in save folder
    /// Allows an undefined number of saves to be loaded, no limit for player on number of saves
    /// </summary>
    public void CreateSaveFileButtons()
    {
        List<FileInfo> arrSaveFileInfos = new List<FileInfo>();
        Button untButtonComponent = null;
        GameObject uniButtonGameObject = null;
        Text uniButtonTextComponent = null;
        marrButtonObjects = new List<GameObject>();
        FileInfo[] arrSavedFileInfo = null;
        SaveData musLoadedSaveData = null;
        DirectoryInfo sysSaveDirectoryInfo = null;
        string strSaveFileInfoText = string.Empty;

        if (Directory.Exists(mstrGameSaveFileDirectory))
        {
            sysSaveDirectoryInfo = new DirectoryInfo(mstrGameSaveFileDirectory);
            arrSavedFileInfo = sysSaveDirectoryInfo.GetFiles().OrderByDescending(file => file.LastWriteTimeUtc).ToArray();
            foreach(FileInfo sysFileInfo in arrSavedFileInfo)
            {
                // Load all "undergods" ugs files from save directory
                if(sysFileInfo.Extension.Equals(".ugs"))
                {
                    arrSaveFileInfos.Add(sysFileInfo);
                }
            }
        }

        // Load information about each save and create a load button
        foreach(FileInfo sysFileInfo in arrSaveFileInfos)
        {
            uniButtonGameObject = (GameObject)Instantiate(SaveButtonPrefab);
            uniButtonGameObject.transform.SetParent(LoadMenuScrollPanel.transform);
            untButtonComponent = uniButtonGameObject.GetComponent<Button>();
            uniButtonTextComponent = uniButtonGameObject.GetComponentInChildren<Text>();
            untButtonComponent.onClick.AddListener(() => LoadSaveGame(sysFileInfo.FullName));
            uniButtonGameObject.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(() => DeleteSaveFile(sysFileInfo.FullName));

            musLoadedSaveData = SaveAndSettingsHelper.LoadSaveData(sysFileInfo.FullName);
            strSaveFileInfoText =
                string.Format("{0} God of {1}\nCurrentTier: {2}\n{3}",
                musLoadedSaveData.PlayerFaction.GodName,
                musLoadedSaveData.PlayerFaction.Type.ToString(),
                musLoadedSaveData.CurrentTier + 1,
                sysFileInfo.LastWriteTimeUtc.ToLocalTime().ToShortDateString() + " " + sysFileInfo.LastWriteTimeUtc.ToLocalTime().ToShortTimeString());
            uniButtonTextComponent.text = strSaveFileInfoText;
            uniButtonGameObject.transform.localScale = new Vector3(1, 1, 1);
            marrButtonObjects.Add(uniButtonGameObject);
        }
    }

    /// <summary>
    /// Clear save info buttons so they can be reloaded 
    /// </summary>
    public void DestroySaveFileButtons()
    {
        if(marrButtonObjects != null)
        {
            foreach (GameObject uniButtonGameObject in marrButtonObjects)
            {
                Destroy(uniButtonGameObject);
            }
            marrButtonObjects = null;
        }
    }

    /// <summary>
    /// Delet a given save file.
    /// Method attached to delete button on save file info buttons
    /// </summary>
    /// <param name="pstrFilePath"></param>
    public void DeleteSaveFile(string pstrFilePath)
    {
        if(SaveAndSettingsHelper.DeleteSaveFile(pstrFilePath))
        {
            // File successfully deleted, recreate save file buttons
            DestroySaveFileButtons();
            CreateSaveFileButtons();
        }
    }

    /// <summary>
    /// Load a save game
    /// Method attached to save file info buttons for loading on click
    /// </summary>
    /// <param name="pstrFilePath"></param>
    public void LoadSaveGame(string pstrFilePath)
    {
        // Create a Game Ifno object to be loaded with data about save
        GameObject NewGameInfoObject = (GameObject)Instantiate(GameInfoObjectPrefab);
        NewGameInfoObject.name = "GameInfo";
        GameInfo gameInfo = NewGameInfoObject.GetComponent<GameInfo>();

        SaveAndSettingsHelper.LoadSceneFromFile(pstrFilePath, gameInfo);
    }

    /// <summary>
    /// Method attached to save settins button
    /// </summary>
    public void SaveSettings()
    {
        SaveAndSettingsHelper.SaveSettingsFromOptionsMenu();
        SaveAndSettingsHelper.ApplyGameSettings();
    }

    /// <summary>
    /// Method attached to quit game button
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}

