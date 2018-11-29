using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class MainMenu : MonoBehaviour {
    private GameInfo gameInfo = null;
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

    public string GameSaveFileDirectory;
    private List<GameObject> marrButtonObjects;

    void Awake()
    {
        GameSaveFileDirectory = Application.persistentDataPath + "/SaveFiles";
    }

	// Use this for initialization
	void Start () {
        DisableAllPanels();
        MainUIPanel.SetActive(true);
        GameInfo.ApplyGameSettings();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            OpenMainUI();
        }
	}

    public void StartButtonClicked()
    {
        Dropdown godTypeDropDown = GodTypeDropDownObject.GetComponent<Dropdown>();
        InputField godNameInputFiled = GodNameInputFieldObject.GetComponent<InputField>();

        string strGodName = godNameInputFiled.text;
        if(!string.IsNullOrEmpty(strGodName))
        {
            Faction.GodType godType = (Faction.GodType)Enum.Parse(typeof(Faction.GodType), godTypeDropDown.options[godTypeDropDown.value].text, true);
            StartNewGame(strGodName, godType);
        }
        else
        {
            // Feedback, need a god name
        }
    }

    public void StartNewGame(string pstrGodName, Faction.GodType penumGodType)
    {
        GameObject NewGameInfoObject = (GameObject)Instantiate(GameInfoObjectPrefab);
        NewGameInfoObject.name = "GameInfo";
        gameInfo = NewGameInfoObject.GetComponent<GameInfo>();
        gameInfo.PlayerFaction.GodName = pstrGodName;
        gameInfo.PlayerFaction.Type = penumGodType;
        SceneManager.LoadScene("UnderGodScene");
    }

    public void OpenNewGameOptions()
    {
        DisableAllPanels();
        NewGameOptionsPanel.SetActive(true);
    }

    public void OpenMainUI()
    {
        DisableAllPanels();
        MainUIPanel.SetActive(true);
    }

    public void OpenOptionsMenu()
    {
        DisableAllPanels();
        OptionsPanel.SetActive(true);
        GameInfo.ApplySettingsToOptionsMenu();
    }

    public void DisableAllPanels()
    {
        MainUIPanel.SetActive(false);
        NewGameOptionsPanel.SetActive(false);
        LoadGameOptionsPanel.SetActive(false);
        OptionsPanel.SetActive(false);
        DestroySaveFileButtons();
    }

    public void EnableLoadSaveGamePanel()
    {
        DisableAllPanels();
        LoadGameOptionsPanel.SetActive(true);
        CreateSaveFileButtons();
    }

    public void CreateSaveFileButtons()
    {
        List<FileInfo> SaveFileInfos = new List<FileInfo>();
        Button btnComponent = null;
        GameObject gobjButtonObject = null;
        Text objButtonText = null;
        marrButtonObjects = new List<GameObject>();
        FileInfo[] objSaveFiles = null;
        SaveData saveFileData = null;
        if(Directory.Exists(GameSaveFileDirectory))
        {
            DirectoryInfo saveFileInfo = new DirectoryInfo(GameSaveFileDirectory);
            objSaveFiles = saveFileInfo.GetFiles().OrderByDescending(file => file.LastWriteTimeUtc).ToArray();
            foreach(FileInfo objFileInfo in objSaveFiles)
            {
                if(objFileInfo.Extension.Equals(".ugs"))
                {
                    SaveFileInfos.Add(objFileInfo);
                }
            }
        }
        foreach(FileInfo objFileInfo in SaveFileInfos)
        {
            string strSaveFileInfoText = string.Empty;
            gobjButtonObject = (GameObject)Instantiate(SaveButtonPrefab);
            gobjButtonObject.transform.SetParent(LoadMenuScrollPanel.transform);
            btnComponent = gobjButtonObject.GetComponent<Button>();
            objButtonText = gobjButtonObject.GetComponentInChildren<Text>();
            btnComponent.onClick.AddListener(() => LoadSaveGame(objFileInfo.FullName));
            gobjButtonObject.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(() => DeleteSaveFile(objFileInfo.FullName));

            saveFileData = GameInfo.LoadSaveData(objFileInfo.FullName);
            strSaveFileInfoText =
                string.Format("{0} God of {1}\nCurrentTier: {2}\n{3}",
                saveFileData.PlayerFaction.GodName,
                saveFileData.PlayerFaction.Type.ToString(),
                saveFileData.CurrentTier + 1,
                objFileInfo.LastWriteTimeUtc.ToLocalTime().ToShortDateString() + " " + objFileInfo.LastWriteTimeUtc.ToLocalTime().ToShortTimeString());
            objButtonText.text = strSaveFileInfoText;
            gobjButtonObject.transform.localScale = new Vector3(1, 1, 1);
            marrButtonObjects.Add(gobjButtonObject);
        }
    }

    public void DestroySaveFileButtons()
    {
        if(marrButtonObjects != null)
        {
            foreach (GameObject gobjButton in marrButtonObjects)
            {
                Destroy(gobjButton);
            }
            marrButtonObjects = null;
        }
    }

    public void DeleteSaveFile(string pstrFilePath)
    {
        if(GameInfo.DeleteSaveFile(pstrFilePath))
        {
            DestroySaveFileButtons();
            CreateSaveFileButtons();
        }
    }

    public void LoadSaveGame(string pstrFilePath)
    {
        string gameInfoAsJSON = string.Empty;
        GameObject NewGameInfoObject = (GameObject)Instantiate(GameInfoObjectPrefab);
        NewGameInfoObject.name = "GameInfo";
        GameInfo gameInfo = NewGameInfoObject.GetComponent<GameInfo>();

        GameInfo.LoadNewGameScene(pstrFilePath, gameInfo);
    }

    public void SaveSettings()
    {
        GameInfo.SaveSettingsFromOptionsMenu();
        GameInfo.ApplyGameSettings();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

