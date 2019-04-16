using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Script attached to the hotkey scroll view
/// Populates and gets values from objects within the scroll view
/// Used by the options menu common across different modes for viewing/updating current hotkeys
/// </summary>
public class HotKeyScrollView : MonoBehaviour {
    public UnityEngine.Object HotKeySettorPrefab;

    public HotKeyManager SettingsHotkeyManager { get; private set; }

    private Dictionary<string, GameObject> mdictHotKeySettorObjects = null;
    private string mstrSelectedInputField = string.Empty;
    private string mstrNewHotKeyCode = string.Empty;
    private GameObject muniSelectedHotKeySettorObject = null;
	// Use this for initialization
	void Start () {
        SettingsHotkeyManager = new HotKeyManager();
	}
	
	// Update is called once per frame
	void Update () {
		if(mdictHotKeySettorObjects != null)
        {
            if(!string.IsNullOrEmpty(mstrSelectedInputField))
            {
                if(!string.IsNullOrEmpty(mstrNewHotKeyCode))
                {
                    bool blnHotKeyAlreadyUsed = false;
                    foreach(KeyCode hotKey in SettingsHotkeyManager.HotKeys.Values)
                    {
                        if(hotKey.ToString().Equals(mstrNewHotKeyCode))
                        {
                            blnHotKeyAlreadyUsed = true;
                            mstrNewHotKeyCode = string.Empty;
                            // TODO figure out logic for enabling same keycode for different hotkey
                            // e.g. Build and start battle can both be B and not conflict, altar and mine cannot be the same though etc.
                        }
                    }
                    if(!blnHotKeyAlreadyUsed)
                    {
                        // Update stored hotkey with new hotkey
                        mdictHotKeySettorObjects[mstrSelectedInputField].GetComponentInChildren<InputField>().text = mstrNewHotKeyCode;
                        mdictHotKeySettorObjects[mstrSelectedInputField].GetComponentInChildren<InputField>().colors = ColorBlock.defaultColorBlock;
                        SettingsHotkeyManager.HotKeys[mstrSelectedInputField] = (KeyCode)Enum.Parse(typeof(KeyCode), mstrNewHotKeyCode, true);
                        mstrSelectedInputField = string.Empty;
                        mstrNewHotKeyCode = string.Empty;
                    }
                }
            }
        }
	}

    /// <summary>
    /// Method fired when a keyboard event fires
    /// Used to get the keycode of the keyboard input for settings hotkeys
    /// </summary>
    private void OnGUI()
    {
        Event uniEvent = Event.current;
        if (uniEvent.isKey && !string.IsNullOrEmpty(mstrSelectedInputField) && string.IsNullOrEmpty(mstrNewHotKeyCode))
        {
            mstrNewHotKeyCode = uniEvent.keyCode.ToString();
        }
    }

    /// <summary>
    /// Method for creating Hot key settor objects in scroll view
    /// </summary>
    public void CreateHotKeySettors()
    {
        // Destroy Existing objetcs
        DestroyHotKeySettors();
        mdictHotKeySettorObjects = new Dictionary<string, GameObject>();
        SettingsHotkeyManager = new HotKeyManager();
        SettingsHotkeyManager.LoadHotkeyProfile();
        GameObject uniHotKeySettorGameObject = null;
        string strHotKeyLabel = string.Empty;
        EventTrigger uniEventTrigger = null;
        EventTrigger.Entry unitEventTriggerEntry = null;
        foreach (KeyValuePair<string, KeyCode> kvalHotKeyCode in SettingsHotkeyManager.HotKeys)
        {
            uniHotKeySettorGameObject = (GameObject)Instantiate(HotKeySettorPrefab);
            // Remove KeyCode from label
            // TODO add spaces before capitol letters
            uniHotKeySettorGameObject.GetComponentInChildren<Text>().text = AddSpacesToSentence(kvalHotKeyCode.Key.Substring(0, kvalHotKeyCode.Key.Length - 7));
            uniHotKeySettorGameObject.GetComponentInChildren<InputField>().text = kvalHotKeyCode.Value.ToString();
            uniEventTrigger = uniHotKeySettorGameObject.GetComponentInChildren<InputField>().gameObject.GetComponent<EventTrigger>();
            unitEventTriggerEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            unitEventTriggerEntry.callback.AddListener((data) => { InputFieldClickDelegate((PointerEventData)data, kvalHotKeyCode.Key); });
            uniEventTrigger.triggers.Add(unitEventTriggerEntry);
            uniHotKeySettorGameObject.transform.parent = transform;
            uniHotKeySettorGameObject.transform.localScale = new Vector3(1, 1, 1);
            mdictHotKeySettorObjects.Add(kvalHotKeyCode.Key, uniHotKeySettorGameObject);
        }
    }

    /// <summary>
    /// Destroy current hotkey settor objects
    /// </summary>
    private void DestroyHotKeySettors()
    {
        if(mdictHotKeySettorObjects != null)
        {
            foreach (GameObject uniHotKeySettorGameObject in mdictHotKeySettorObjects.Values)
            {
                Destroy(uniHotKeySettorGameObject);
            }
            mdictHotKeySettorObjects = null;
        }
    }

    /// <summary>
    /// Event fired when a hotkey settor is clicked
    /// </summary>
    /// <param name="data"></param>
    /// <param name="pstrKey"></param>
    public void InputFieldClickDelegate(PointerEventData data, string pstrKey)
    {
        ColorBlock uniColorBlock;
        mstrSelectedInputField = pstrKey;
        if (muniSelectedHotKeySettorObject != null)
        {
            muniSelectedHotKeySettorObject.GetComponentInChildren<InputField>().colors = ColorBlock.defaultColorBlock;
            muniSelectedHotKeySettorObject = null;
        }
        if (mdictHotKeySettorObjects != null)
        {
            uniColorBlock = ColorBlock.defaultColorBlock;
            uniColorBlock.disabledColor = Color.blue;
            mdictHotKeySettorObjects[pstrKey].GetComponentInChildren<InputField>().colors = uniColorBlock;
            muniSelectedHotKeySettorObject = mdictHotKeySettorObjects[pstrKey];
        }
    }
    /// <summary>
    /// Add a space before capital letters for nicely printing labels
    /// See https://stackoverflow.com/questions/272633/add-spaces-before-capital-letters
    /// </summary>
    /// <param name="pstrBadLabel"></param>
    /// <returns></returns>
    private string AddSpacesToSentence(string pstrBadLabel)
    {
        // Regexs are weird
        return  Regex.Replace(pstrBadLabel, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
    }
}
