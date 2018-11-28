using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    private GameInfo gameInfo = null;
    public GameObject GameInfoObjectPrefab;
    public GameObject GodTypeDropDownObject;
    public GameObject GodNameInputFieldObject;
    public GameObject NewGameOptionsPanel;
    public GameObject MainUIPanel;
    public GameObject LoadGameOptionsPanel;

	// Use this for initialization
	void Start () {
        DisableAllPanels();
        MainUIPanel.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
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

    public void DisableAllPanels()
    {
        MainUIPanel.SetActive(false);
        NewGameOptionsPanel.SetActive(false);
        LoadGameOptionsPanel.SetActive(false);
    }

    public void EnableLoadSaveGamePanel()
    {
        DisableAllPanels();
        LoadGameOptionsPanel.SetActive(true);
    }

    public void LoadSaveGame()
    {
        string gameInfoAsJSON = string.Empty;
        string filePath = Application.dataPath + "/131878581333716560.ugs";
        if (File.Exists(filePath))
        {
            GameObject NewGameInfoObject = (GameObject)Instantiate(GameInfoObjectPrefab);
            NewGameInfoObject.name = "GameInfo";
            gameInfo = NewGameInfoObject.GetComponent<GameInfo>();

            gameInfoAsJSON = File.ReadAllText(filePath);
            SaveData loadedSaveData = JsonUtility.FromJson<SaveData>(gameInfoAsJSON);

            // Set gameinfo variables
            gameInfo.PlayerFaction = loadedSaveData.PlayerFaction;
            gameInfo.SavedFactions = loadedSaveData.SavedFactions;
            gameInfo.MapRadius = loadedSaveData.MapRadius;
            gameInfo.CurrentTier = loadedSaveData.CurrentTier;
            gameInfo.PlayerRewards = loadedSaveData.PlayerRewards;
            gameInfo.PlayerMoraleCap = loadedSaveData.PlayerMoraleCap;
            gameInfo.EnemyChallengeTimer = loadedSaveData.EnemyChallengeTimer;
            gameInfo.FromSave = true;
            gameInfo.NewGame = false;
            SceneManager.LoadScene("UnderGodScene");
        }
        else
        {
            // Save doesnt exist
        }
    }
}
