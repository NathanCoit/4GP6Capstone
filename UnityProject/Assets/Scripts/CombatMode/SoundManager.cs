using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip backgroundMusic;

    public List<AudioClip> unitSelectionSounds = new List<AudioClip>();

    private AudioSource musicSource;
    private AudioSource soundeffectScource;

    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponents<AudioSource>()[0];
        soundeffectScource = GetComponents<AudioSource>()[1];

        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playUnitSelect()
    {
        System.Random r = new System.Random();
        soundeffectScource.clip = unitSelectionSounds[r.Next(0, 2)];

        soundeffectScource.Play();
    }
}
