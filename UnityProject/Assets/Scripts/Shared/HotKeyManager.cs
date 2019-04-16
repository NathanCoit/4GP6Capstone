using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for managing hotkeys saved.
/// Allows for custom hot key profiles for a user to be changed and read during runtime and between game sessions.
/// Create an instance of this class and call LoadHotKeyProfile to load the current Computers hotkey profile.
/// </summary>
public class HotKeyManager
{
    // Current hotkeys and default keycodes
    public Dictionary<string, KeyCode> HotKeys = new Dictionary<string, KeyCode>
    {
        { "AltarKeyCode", KeyCode.A },
        { "MineKeyCode", KeyCode.S },
        { "HouseKeyCode", KeyCode.D },
        { "BlacksmithKeyCode", KeyCode.F },
        { "BuildKeyCode", KeyCode.B },
        { "TierRewardKeyCode", KeyCode.V },
        { "EscapeKeyCode", KeyCode.Escape },
        { "BuildingUpgradeKeyCode", KeyCode.U },
        { "BuildingMoveKeyCode", KeyCode.M },
        { "BlackSmithUIKeyCode", KeyCode.X },
        { "BuyMinersKeyCode", KeyCode.K },
        { "StartBattleKeyCode", KeyCode.C },
        { "CentreOnGodKeyCode", KeyCode.Space},
        { "CentreOnVillageKeyCode", KeyCode.Z }
    };

    /// <summary>
    /// Load the current hotkeyprofile stored on this computer, if one exists.
    /// </summary>
    public void LoadHotkeyProfile()
    {
        List<string> arrKeys = new List<string>(HotKeys.Keys);
        foreach (string strHotkey in arrKeys)
        {
            if (PlayerPrefs.HasKey(strHotkey))
            {
                HotKeys[strHotkey] = (KeyCode)PlayerPrefs.GetInt(strHotkey);
            }
        }
    }

    /// <summary>
    /// Set the keycode of a current HotKey.
    /// </summary>
    /// <param name="pstrKey">The hotkey to set</param>
    /// <param name="pstrHotKey">The KeyCode to be set for that hotkey</param>
    public void SetHotKey(string pstrKey, string pstrHotKey)
    {
        KeyCode newHotKey = (KeyCode)Enum.Parse(typeof(KeyCode), pstrHotKey, true);
        if (HotKeys.ContainsKey(pstrKey))
        {
            PlayerPrefs.SetInt(pstrKey, (int)newHotKey);
            HotKeys[pstrKey] = newHotKey;
        }
    }

    /// <summary>
    /// Save the current instance of HotKeys for this computer.
    /// </summary>
    public void SaveHotKeys()
    {
        foreach (KeyValuePair<string, KeyCode> kvalHtoKey in HotKeys)
        {
            PlayerPrefs.SetInt(kvalHtoKey.Key, (int)kvalHtoKey.Value);
        }
    }
}
