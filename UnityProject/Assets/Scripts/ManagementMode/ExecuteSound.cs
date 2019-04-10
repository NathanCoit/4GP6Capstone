using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Generic sound manager class to allow abstraction of audio clip execution.
/// </summary>
public class ExecuteSound : MonoBehaviour
{
    // Hack to allow adding a custom number of sounds in the unity editor
    // Used so that sounds can be imported only in the scene they are needed in
    // while same script is reused
    [System.Serializable]
    public struct NamedSound
    {
        public string Name;
        public AudioClip Sound;
    }
    public NamedSound[] Sounds;
    private Dictionary<string, AudioClip> mdictSounds = new Dictionary<string, AudioClip>();

    public AudioSource musicSource;

    // Use this for initialization
    void Awake()
    {
        foreach (NamedSound musSound in Sounds)
        {
            mdictSounds.Add(musSound.Name, musSound.Sound);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Public method called from other scripts to play a certain sound
    /// </summary>
    /// <param name="pstrSoundName"></param>
    public void PlaySound(string pstrSoundName)
    {
        if (mdictSounds.ContainsKey(pstrSoundName))
        {
            musicSource.clip = mdictSounds[pstrSoundName];
            musicSource.Play();
        }
    }

    /// <summary>
    /// Helper method for attaching an on hover sound to a game object.
    /// Assumes no event triggers exist on the object currently
    /// </summary>
    /// <param name="pstrSound"></param>
    /// <param name="puniGameObject"></param>
    public void AttachOnHoverSoundToObject(string pstrSound,  GameObject puniGameObject)
    {
        EventTrigger.Entry uniEventTriggerEntry = new EventTrigger.Entry()
        {
            eventID = EventTriggerType.PointerEnter
        };
        uniEventTriggerEntry.callback.AddListener((uniEventData) => PlaySound(pstrSound));
        EventTrigger uniEventTrigger = puniGameObject.AddComponent<EventTrigger>();
        uniEventTrigger.triggers.Add(uniEventTriggerEntry);
    }
}
