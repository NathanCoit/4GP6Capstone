using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Helper class for serializing and deserializing data for game states.
/// Abstracts the interactions between the game and the file system of the user
/// </summary>
public class SaveAndSettingsHelper
{
    /// <summary>
    /// Function called to apply currently stored player settings to the options menu
    /// </summary>
    public static void ApplySettingsToOptionsMenu()
    {
        Slider uniAudioSlider = GameObject.Find("AudioSliderObject").GetComponent<Slider>();
        uniAudioSlider.minValue = 0;
        uniAudioSlider.maxValue = 1;
        uniAudioSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        GameObject.Find("GraphicsDropDownMenu").GetComponent<Dropdown>().value = PlayerPrefs.GetInt("GraphicsSetting", 0);
        GameObject.Find("HotKeySettors").GetComponent<HotKeyScrollView>().CreateHotKeySettors();
    }

    /// <summary>
    /// Method for pulling values out of the options menu and saving them to the player prefs
    /// </summary>
    public static void SaveSettingsFromOptionsMenu()
    {
        // Assumes options menu prefab is being used.
        float fMasterVolume = GameObject.Find("AudioSliderObject").GetComponent<Slider>().value;
        int intGraphicsSetting = GameObject.Find("GraphicsDropDownMenu").GetComponent<Dropdown>().value;

        // Save sound setting
        PlayerPrefs.SetFloat("MasterVolume", fMasterVolume);

        // Save the graphics setting
        PlayerPrefs.SetInt("GraphicsSetting", intGraphicsSetting);

        // Save hotkeys
        GameObject.Find("HotKeySettors").GetComponent<HotKeyScrollView>().SettingsHotkeyManager.SaveHotKeys();
        // TODO add more options to save
    }

    /// <summary>
    /// Method for applying the currently stored setting values to the game
    /// </summary>
    public static void ApplyGameSettings()
    {
        // set sound
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        UnityEngine.Rendering.GraphicsTier uniGraphicsSetting = UnityEngine.Rendering.GraphicsTier.Tier1;
        switch (PlayerPrefs.GetInt("GraphicsSetting"))
        {
            case 1:
                uniGraphicsSetting = UnityEngine.Rendering.GraphicsTier.Tier2;
                break;
            case 2:
                uniGraphicsSetting = UnityEngine.Rendering.GraphicsTier.Tier3;
                break;
        }
        Graphics.activeTier = uniGraphicsSetting;
        // TODO, add more options
    }

    /// <summary>
    /// Method for loading a game info object with values from a save file
    /// Used by the main menu to load a save file
    /// </summary>
    /// <param name="pstrSaveFilePath"></param>
    /// <param name="pmusGameInfo"></param>
    public static void LoadSceneFromFile(string pstrSaveFilePath, GameInfo pmusGameInfo, bool pblnAsync = false)
    {
        SaveData musLoadedSaveData = LoadSaveData(pstrSaveFilePath);
        if (musLoadedSaveData != null)
        {
            pmusGameInfo.PlayerFaction = musLoadedSaveData.PlayerFaction;
            pmusGameInfo.SavedFactions = musLoadedSaveData.SavedFactions;
            pmusGameInfo.MapRadius = musLoadedSaveData.MapRadius;
            pmusGameInfo.CurrentTier = musLoadedSaveData.CurrentTier;
            pmusGameInfo.PlayerRewards = musLoadedSaveData.PlayerRewards;
            pmusGameInfo.PlayerMoraleCap = musLoadedSaveData.PlayerMoraleCap;
            pmusGameInfo.EnemyChallengeTimer = musLoadedSaveData.EnemyChallengeTimer;
            pmusGameInfo.WorshipperMultipliers = musLoadedSaveData.WorshipperMultipliers;
            pmusGameInfo.MaterialMultipliers = musLoadedSaveData.MaterialMultipliers;
            pmusGameInfo.TutorialFlag = musLoadedSaveData.TutorialFlag;
            pmusGameInfo.WorshipperAttackBuffs = musLoadedSaveData.WorshipperAttackBuffs;
            pmusGameInfo.WorshipperDefenseBuffs = musLoadedSaveData.WorshipperDefenseBuffs;
            pmusGameInfo.WorshipperMovementBuffs = musLoadedSaveData.WorshipperMovementBuffs;
            pmusGameInfo.GodAttackMultiplier = musLoadedSaveData.GodAttackMultiplier;
            pmusGameInfo.GodDefenseMultiplier = musLoadedSaveData.GodDefenseMultiplier;
            pmusGameInfo.GodHealthMultiplier = musLoadedSaveData.GodHealthMultiplier;
            pmusGameInfo.SavedTreasures = musLoadedSaveData.SavedTreasures;
            pmusGameInfo.FromSave = true;
            pmusGameInfo.NewGame = false;
            if (pblnAsync)
            {
                SceneManager.LoadSceneAsync("UnderGodScene");
            }
            else
            {
                SceneManager.LoadScene("UnderGodScene");
            }
        }
    }

    /// <summary>
    /// Method to load the save file with the newest modify date
    /// Used by management mode to load last save after player loses in combat mode
    /// </summary>
    /// <param name="pstrGameSaveFileDirectory"></param>
    /// <param name="pmusGameInfo"></param>
    public static void LoadLastSave(string pstrGameSaveFileDirectory, GameInfo pmusGameInfo)
    {
        if (Directory.Exists(pstrGameSaveFileDirectory))
        {
            DirectoryInfo sysSaveDirectoryInfo = new DirectoryInfo(pstrGameSaveFileDirectory);
            // Sort by last write time to get last modified save
            FileInfo[] arrSavedFileInfo = sysSaveDirectoryInfo.GetFiles().OrderByDescending(file => file.LastWriteTimeUtc).ToArray();
            if (arrSavedFileInfo.Length > 0)
            {
                LoadSceneFromFile(arrSavedFileInfo[0].FullName, pmusGameInfo);
            }
        }
    }

    /// <summary>
    /// Delete a save file from the given path
    /// Used by main menu for deleting save files to unclutter system
    /// </summary>
    /// <param name="pstrFilePath"></param>
    /// <returns></returns>
    public static bool DeleteSaveFile(string pstrFilePath)
    {
        bool blnFileDeleted = false;
        try
        {
            if (File.Exists(pstrFilePath))
            {
                // Only delete "undergods" ugs files
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

    /// <summary>
    /// Create a new save file from the given game info object
    /// Used by options menu in management mode to create new save files
    /// Serializes the game info object into a JSON object and saves to file
    /// </summary>
    /// <param name="pstrSaveFileDirectory"></param>
    /// <param name="pmusGameInfo"></param>
    /// <returns></returns>
    public static bool SaveGame(string pstrSaveFileDirectory, GameInfo pmusGameInfo)
    {
        bool blnSaved = false;
        string strGameInfoAsJSON = string.Empty;
        string strFilePath = string.Empty;
        try
        {
            strGameInfoAsJSON = JsonUtility.ToJson(pmusGameInfo);
            // Create a save directory if this is the first time creating a save file
            if (!Directory.Exists(pstrSaveFileDirectory))
            {
                Directory.CreateDirectory(pstrSaveFileDirectory);
            }
            // Give each file a unique name to save as
            strFilePath = pstrSaveFileDirectory + "/" + DateTime.Now.ToFileTime() + ".ugs";
            File.WriteAllText(strFilePath, strGameInfoAsJSON);
            blnSaved = true;
        }
        catch
        {
            blnSaved = false;
        }
        return blnSaved;
    }

    /// <summary>
    /// Load a save file
    /// Loads a serialized game info JSON object and creates a SaveData object
    /// Used by main menu for loading saves
    /// Save data must be used for loading as GameInfo in a MonBehaviour sub class
    /// </summary>
    /// <param name="pstrFilePath"></param>
    /// <returns></returns>
    public static SaveData LoadSaveData(string pstrFilePath)
    {
        SaveData musSaveData = null;
        string strGameInfoAsJSON = string.Empty;
        if (File.Exists(pstrFilePath))
        {
            strGameInfoAsJSON = File.ReadAllText(pstrFilePath);
            musSaveData = JsonUtility.FromJson<SaveData>(strGameInfoAsJSON);
        }
        return musSaveData;
    }

    /// <summary>
    /// Check for changes in the options menu to decide if confirmation dialog is needed when closing
    /// </summary>
    /// <returns></returns>
    public static bool CheckForChangesInOptionsMenu()
    {
        bool blnUnsavedChanges = false;
        // Get options menu settings
        float fChangedVolume = GameObject.Find("AudioSliderObject").GetComponent<Slider>().value;
        int intChangedGraphicsSetting = GameObject.Find("GraphicsDropDownMenu").GetComponent<Dropdown>().value;
        Dictionary<string, KeyCode> dictChangedHotkeys = new Dictionary<string, KeyCode>(GameObject.Find("HotKeySettors").GetComponent<HotKeyScrollView>().SettingsHotkeyManager.HotKeys);

        // Get saved settings
        float fSavedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        int intSavedGraphicsSetting = PlayerPrefs.GetInt("GraphicsSetting", 0);
        HotKeyManager musHotKeyManager = new HotKeyManager();
        musHotKeyManager.LoadHotkeyProfile();
        Dictionary<string, KeyCode> dictSavedHotkeys = musHotKeyManager.HotKeys;

        // Compare
        if (!fChangedVolume.Equals(fSavedVolume)
            || intChangedGraphicsSetting != intSavedGraphicsSetting)
        {
            blnUnsavedChanges = true;
        }
        foreach (KeyValuePair<string, KeyCode> kvalHotkey in dictChangedHotkeys)
        {
            if (kvalHotkey.Value != dictSavedHotkeys[kvalHotkey.Key])
            {
                blnUnsavedChanges = true;
            }
        }
        return blnUnsavedChanges;
    }
}
