using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveAndSettingsHelper
{
    public static void ApplySettingsToOptionsMenu()
    {
        Slider audioSlider = GameObject.Find("AudioSliderObject").GetComponent<Slider>();
        audioSlider.minValue = 0;
        audioSlider.maxValue = 1;
        audioSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        GameObject.Find("GraphicsDropDownMenu").GetComponent<Dropdown>().value = PlayerPrefs.GetInt("GraphicsSetting");
        GameObject.Find("HotKeySettors").GetComponent<HotKeyScrollView>().CreateHotKeySettors();
    }

    public static void SaveSettingsFromOptionsMenu()
    {
        // Assumes options menu prefab is being used.
        float MasterVolume = GameObject.Find("AudioSliderObject").GetComponent<Slider>().value;
        int GraphicsSetting = GameObject.Find("GraphicsDropDownMenu").GetComponent<Dropdown>().value;

        // Save sound setting
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);

        // Save the graphics setting
        PlayerPrefs.SetInt("GraphicsSetting", GraphicsSetting);

        // Save hotkeys
        GameObject.Find("HotKeySettors").GetComponent<HotKeyScrollView>().hotKeyManager.SaveHotKeys();
        // TODO add more options to save
    }

    public static void ApplyGameSettings()
    {
        // set sound
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume");
        UnityEngine.Rendering.GraphicsTier graphicsSetting = UnityEngine.Rendering.GraphicsTier.Tier1;
        switch (PlayerPrefs.GetInt("GraphicsSetting"))
        {
            case 1:
                graphicsSetting = UnityEngine.Rendering.GraphicsTier.Tier2;
                break;
            case 2:
                graphicsSetting = UnityEngine.Rendering.GraphicsTier.Tier3;
                break;
        }
        Graphics.activeTier = graphicsSetting;
        // TODO, add more options
    }

    public static void LoadNewGameScene(string pstrSaveFilePath, GameInfo gameInfo)
    {
        SaveData loadedSaveData = LoadSaveData(pstrSaveFilePath);
        if (loadedSaveData != null)
        {
            gameInfo.PlayerFaction = loadedSaveData.PlayerFaction;
            gameInfo.SavedFactions = loadedSaveData.SavedFactions;
            gameInfo.MapRadius = loadedSaveData.MapRadius;
            gameInfo.CurrentTier = loadedSaveData.CurrentTier;
            gameInfo.PlayerRewards = loadedSaveData.PlayerRewards;
            gameInfo.PlayerMoraleCap = loadedSaveData.PlayerMoraleCap;
            gameInfo.EnemyChallengeTimer = loadedSaveData.EnemyChallengeTimer;
            gameInfo.WorshipperMultipliers = loadedSaveData.WorshipperMultipliers;
            gameInfo.MaterialMultipliers = loadedSaveData.MaterialMultipliers;
            gameInfo.FromSave = true;
            gameInfo.NewGame = false;
            SceneManager.LoadScene("UnderGodScene");
        }
    }

    public static void LoadLastSave(string pstrGameSaveFileDirectory, GameInfo gameInfo)
    {
        if (Directory.Exists(pstrGameSaveFileDirectory))
        {
            DirectoryInfo saveFileInfo = new DirectoryInfo(pstrGameSaveFileDirectory);
            FileInfo[] objSaveFiles = saveFileInfo.GetFiles().OrderByDescending(file => file.LastWriteTimeUtc).ToArray();
            if (objSaveFiles.Length > 0)
            {
                LoadNewGameScene(objSaveFiles[0].FullName, gameInfo);
            }
        }
    }

    public static bool DeleteSaveFile(string pstrFilePath)
    {
        bool blnFileDeleted = false;
        try
        {
            if (File.Exists(pstrFilePath))
            {
                if (pstrFilePath.EndsWith(".ugs"))
                {
                    File.Delete(pstrFilePath);
                    blnFileDeleted = true;
                }
            }
        }
        catch
        {
            blnFileDeleted = false;
        }
        return blnFileDeleted;
    }

    public static bool SaveGame(string pstrSaveFileDirectory, GameInfo pobjGameInfo)
    {
        bool blnSaved = false;
        try
        {
            string gameInfoAsJSON = JsonUtility.ToJson(pobjGameInfo);
            if (!Directory.Exists(pstrSaveFileDirectory))
            {
                Directory.CreateDirectory(pstrSaveFileDirectory);
            }

            string filePath = pstrSaveFileDirectory + "/" + DateTime.Now.ToFileTime() + ".ugs";
            File.WriteAllText(filePath, gameInfoAsJSON);
            blnSaved = true;
        }
        catch
        {
            blnSaved = false;
        }
        return blnSaved;
    }

    public static SaveData LoadSaveData(string pstrFilePath)
    {
        SaveData saveData = null;
        string gameInfoAsJSON = string.Empty;
        if (File.Exists(pstrFilePath))
        {
            gameInfoAsJSON = File.ReadAllText(pstrFilePath);
            saveData = JsonUtility.FromJson<SaveData>(gameInfoAsJSON);
        }
        return saveData;
    }
}
