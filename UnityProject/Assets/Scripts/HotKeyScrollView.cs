using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotKeyScrollView : MonoBehaviour {
    public HotKeyManager hotKeyManager = new HotKeyManager();
    private Dictionary<string, GameObject> HotKeySettors = null;
    public UnityEngine.Object HotKeySettorPrefab;
    private string SelectedInputField = string.Empty;
    private string NewHotKeyCode = string.Empty;
    private GameObject SelectedHotKeySettor = null;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(HotKeySettors != null)
        {
            if(!string.IsNullOrEmpty(SelectedInputField))
            {
                if(!string.IsNullOrEmpty(NewHotKeyCode))
                {
                    bool blnHotKeyAlreadyUsed = false;
                    foreach(KeyCode hotKey in hotKeyManager.HotKeys.Values)
                    {
                        if(hotKey.ToString().Equals(NewHotKeyCode))
                        {
                            blnHotKeyAlreadyUsed = true;
                            NewHotKeyCode = string.Empty;
                            // TODO figure out logic for enabling same keycode for different hotkey
                            // e.g. Build and start battle can both be B and not conflict, altar and mine cannot be the same though etc.
                        }
                    }
                    if(!blnHotKeyAlreadyUsed)
                    {
                        HotKeySettors[SelectedInputField].GetComponentInChildren<InputField>().text = NewHotKeyCode;
                        HotKeySettors[SelectedInputField].GetComponentInChildren<InputField>().colors = ColorBlock.defaultColorBlock;
                        hotKeyManager.HotKeys[SelectedInputField] = (KeyCode)Enum.Parse(typeof(KeyCode), NewHotKeyCode, true);
                        SelectedInputField = string.Empty;
                        NewHotKeyCode = string.Empty;
                    }
                }
            }
        }
	}

    private void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && !string.IsNullOrEmpty(SelectedInputField) && string.IsNullOrEmpty(NewHotKeyCode))
        {
            NewHotKeyCode = e.keyCode.ToString();
        }
    }

    public void CreateHotKeySettors()
    {
        DestroyHotKeySettors();
        HotKeySettors = new Dictionary<string, GameObject>();
        hotKeyManager = new HotKeyManager();
        hotKeyManager.LoadHotkeyProfile();
        GameObject hotKeySettor = null;
        string strHotKeyLabel = string.Empty;
        EventTrigger trigger = null;
        EventTrigger.Entry entry = null;
        foreach (KeyValuePair<string, KeyCode> hotKey in hotKeyManager.HotKeys)
        {
            hotKeySettor = (GameObject)Instantiate(HotKeySettorPrefab);
            // Remove KeyCode from label
            hotKeySettor.GetComponentInChildren<Text>().text = hotKey.Key.Substring(0, hotKey.Key.Length - 7);
            hotKeySettor.GetComponentInChildren<InputField>().text = hotKey.Value.ToString();
            trigger = hotKeySettor.GetComponentInChildren<InputField>().gameObject.GetComponent<EventTrigger>();
            entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entry.callback.AddListener((data) => { InputFieldClickDelegate((PointerEventData)data, hotKey.Key); });
            trigger.triggers.Add(entry);
            hotKeySettor.transform.parent = transform;
            hotKeySettor.transform.localScale = new Vector3(1, 1, 1);
            HotKeySettors.Add(hotKey.Key, hotKeySettor);
        }
    }

    private void DestroyHotKeySettors()
    {
        if(HotKeySettors != null)
        {
            foreach (GameObject hotKeySettor in HotKeySettors.Values)
            {
                Destroy(hotKeySettor);
            }
            HotKeySettors = null;
        }
    }

    public void InputFieldClickDelegate(PointerEventData data, string pstrKey)
    {
        SelectedInputField = pstrKey;
        if (SelectedHotKeySettor != null)
        {
            SelectedHotKeySettor.GetComponentInChildren<InputField>().colors = ColorBlock.defaultColorBlock;
            SelectedHotKeySettor = null;
        }
        if (HotKeySettors != null)
        {
            ColorBlock colorBlock = ColorBlock.defaultColorBlock;
            colorBlock.disabledColor = Color.blue;
            HotKeySettors[pstrKey].GetComponentInChildren<InputField>().colors = colorBlock;
            SelectedHotKeySettor = HotKeySettors[pstrKey];
        }
    }
}
