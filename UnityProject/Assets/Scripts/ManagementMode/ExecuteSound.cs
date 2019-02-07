using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteSound : MonoBehaviour
{
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
    void Start()
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


}
