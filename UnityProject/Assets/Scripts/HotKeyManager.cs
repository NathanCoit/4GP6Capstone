using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotKeyManager
{
    public Dictionary<string, KeyCode> HotKeys = new Dictionary<string, KeyCode>
    {
        { "AltarKeyCode", KeyCode.A },
        { "MineKeyCode", KeyCode.S },
        { "HouseKeyCode", KeyCode.D },
        { "BlacksmithKeyCode", KeyCode.F },
        { "BuildKeyCode", KeyCode.B },
        { "TierRewardKeyCode", KeyCode.V },
        { "EscapeKeyCode", KeyCode.Escape },
        { "BuildingUpgradeKeyCode", KeyCode.U},
        { "BuildingMoveKeyCode", KeyCode.M },
        { "BlackSmithUIKeyCode", KeyCode.X },
        { "BuyMinersKeyCode", KeyCode.K },
        { "StartBattleKeyCode", KeyCode.B }
    };

    public HotKeyManager()
    {
    }

    public void LoadHotkeyProfile()
    {
        List<string> keys = new List<string>(HotKeys.Keys);
        foreach(string hotkey in keys)
        {
            if(PlayerPrefs.HasKey(hotkey))
            {
                HotKeys[hotkey] = (KeyCode)PlayerPrefs.GetInt(hotkey);
            }
        }
    }

    public void SetHotKey(string pstrKey, string pstrHotKey)
    {
        KeyCode newHotKey = (KeyCode)Enum.Parse(typeof(KeyCode), pstrHotKey, true);
        if(HotKeys.ContainsKey(pstrKey))
        {
            PlayerPrefs.SetInt(pstrKey, (int)newHotKey);
            HotKeys[pstrKey] = newHotKey;
        }
    }

    public void SaveHotKeys()
    {
        foreach(KeyValuePair<string,KeyCode> hotKey in HotKeys)
        {
            PlayerPrefs.SetInt(hotKey.Key, (int)hotKey.Value);
        }
    }
}
