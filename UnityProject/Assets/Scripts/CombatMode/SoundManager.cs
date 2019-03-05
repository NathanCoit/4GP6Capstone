using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip backgroundMusic;

    public List<AudioClip> unitSelectionSounds;
    public List<AudioClip> unitDeathSounds;
    public List<AudioClip> godSelectionSounds;
    public List<AudioClip> enemyGodTurnStartSounds;
    public List<AudioClip> godEnterBattleSounds;

    public AudioClip uiHover;
    public AudioClip uiSelect;

    public AudioClip unitAttack;
    public AudioClip unitMove;

    private int previousUnitSelectionRandom;
    private int previousGodRandom;
    private int previousEnemyGodRandom;
    private int previousUnitDeathRandom;
    private int previousGodEnterBattleRandom;

    private AudioSource musicSource;
    private AudioSource soundeffectScource;
    private AudioSource vocalSource;

    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponents<AudioSource>()[0];
        soundeffectScource = GetComponents<AudioSource>()[1];
        vocalSource = GetComponents<AudioSource>()[2];

        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playUiHover()
    {
        soundeffectScource.clip = uiHover;
        soundeffectScource.Play();
    }

    public void playUiSelect()
    {
        soundeffectScource.clip = uiSelect;
        soundeffectScource.Play();
    }

    public void playUnitAttack()
    {
        soundeffectScource.clip = unitAttack;
        soundeffectScource.Play();
    }

    public void playUnitMove()
    {
        soundeffectScource.clip = unitMove;
        soundeffectScource.Play();
    }

    //Play a radom unit select sound (but not the last one played)
    public void playUnitSelect()
    {
        List<int> acceptableSounds = new List<int>();


        for(int i = 0; i < unitSelectionSounds.Count; i++)
        {
            if (!(i == previousUnitSelectionRandom))
                acceptableSounds.Add(i);
        }

        System.Random r = new System.Random();

        int random = acceptableSounds[r.Next(0, acceptableSounds.Count)];
        vocalSource.clip = unitSelectionSounds[random];

        //Prevent playing the last played clip
        previousUnitSelectionRandom = random;

        vocalSource.Play();
    }

    //Play a radom unit select sound (but not the last one played)
    public void playUnitDeath()
    {
        List<int> acceptableSounds = new List<int>();


        for (int i = 0; i < unitSelectionSounds.Count; i++)
        {
            if (!(i == previousUnitDeathRandom))
                acceptableSounds.Add(i);
        }

        System.Random r = new System.Random();

        int random = acceptableSounds[r.Next(0, acceptableSounds.Count)];
        vocalSource.clip = unitDeathSounds[random];

        //Prevent playing the last played clip
        previousUnitDeathRandom = random;

        vocalSource.Play();
    }

    //Same as above, but with god sounds
    public void playGodSelect()
    {
        List<int> acceptableSounds = new List<int>();


        for (int i = 0; i < unitSelectionSounds.Count; i++)
        {
            if (!(i == previousGodRandom))
                acceptableSounds.Add(i);
        }

        System.Random r = new System.Random();

        int random = acceptableSounds[r.Next(0, acceptableSounds.Count)];
        vocalSource.clip = godSelectionSounds[random];

        //Prevent playing the last played clip
        previousGodRandom = random;

        vocalSource.Play();
    }

    public void playEnemyGodTurnStart()
    {
        List<int> acceptableSounds = new List<int>();


        for (int i = 0; i < unitSelectionSounds.Count; i++)
        {
            if (!(i == previousEnemyGodRandom))
                acceptableSounds.Add(i);
        }

        System.Random r = new System.Random();

        int random = acceptableSounds[r.Next(0, acceptableSounds.Count)];
        vocalSource.clip = enemyGodTurnStartSounds[random];

        //Prevent playing the last played clip
        previousEnemyGodRandom = random;

        vocalSource.Play();
    }

    public void playGodEnterBattle()
    {
        List<int> acceptableSounds = new List<int>();


        for (int i = 0; i < unitSelectionSounds.Count; i++)
        {
            if (!(i == previousGodEnterBattleRandom))
                acceptableSounds.Add(i);
        }

        System.Random r = new System.Random();

        int random = acceptableSounds[r.Next(0, acceptableSounds.Count)];
        vocalSource.clip = godEnterBattleSounds[random];

        //Prevent playing the last played clip
        previousGodEnterBattleRandom = random;

        vocalSource.Play();
    }
}
