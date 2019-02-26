using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExecuteSound : MonoBehaviour
{
    // Hack to allow adding a custom number of sounds in the unity editor
    // Used so that sounds can be imported only in the scene they are needed in
    [System.Serializable]
    public struct NamedSound
    {
        public string Name;
        public AudioClip Sound;
    }
    public NamedSound[] Sounds;
    private Dictionary<string, AudioClip> sounddict = new Dictionary<string, AudioClip>();

    public AudioSource musicSource;

    // Use this for initialization
    void Awake()
    {
        foreach (NamedSound sound in Sounds)
        {
            sounddict.Add(sound.Name, sound.Sound);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlaySound(string soundName)
    {
        if (sounddict.ContainsKey(soundName))
        {
            musicSource.clip = sounddict[soundName];
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
