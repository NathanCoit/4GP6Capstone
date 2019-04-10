using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioClip backgroundMusic;

    //Unit selection sound lists (player)
    public List<AudioClip> mushroomUnitSelectionSounds;
    public List<AudioClip> forkUnitSelectionSounds;
    public List<AudioClip> shoeUnitSelectionSounds;
    public List<AudioClip> duckUnitSelectionSounds;

    private List<AudioClip> unitSelectionSounds;


    //Unit death sounds (both)
    public List<AudioClip> mushroomUnitDeathSounds;
    public List<AudioClip> forkUnitDeathSounds;
    public List<AudioClip> shoeUnitDeathSounds;
    public List<AudioClip> duckUnitDeathSounds;
    public List<AudioClip> houndsUnitDeathSounds;
    public List<AudioClip> jazzUnitDeathSounds;
    public List<AudioClip> loveUnitDeathSounds;
    public List<AudioClip> fireUnitDeathSounds;
    public List<AudioClip> waterUnitDeathSounds;
    public List<AudioClip> earthUnitDeathSounds;

    private List<AudioClip> playerUnitDeathSounds;
    private List<AudioClip> enemyUnitDeathSounds;

    //God selection sound lists (player)
    public List<AudioClip> mushroomGodSelectionSounds;
    public List<AudioClip> forkGodSelectionSounds;
    public List<AudioClip> shoeGodSelectionSounds;
    public List<AudioClip> duckGodSelectionSounds;

    private List<AudioClip> godSelectionSounds;


    //Enemy turn start sounds (enemy)
    public List<AudioClip> mushroomGodTurnStartSounds;
    public List<AudioClip> forkGodTurnStartSounds;
    public List<AudioClip> shoeGodTurnStartSounds;
    public List<AudioClip> duckGodTurnStartSounds;
    public List<AudioClip> houndsGodTurnStartSounds;
    public List<AudioClip> jazzGodTurnStartSounds;
    public List<AudioClip> loveGodTurnStartSounds;
    public List<AudioClip> fireGodTurnStartSounds;
    public List<AudioClip> waterGodTurnStartSounds;
    public List<AudioClip> earthGodTurnStartSounds;

    private List<AudioClip> enemyGodTurnStartSounds;

    //God enter battle sounds (both)
    public List<AudioClip> mushroomGodEnterBattleSounds;
    public List<AudioClip> forkGodEnterBattleSounds;
    public List<AudioClip> shoeGodEnterBattleSounds;
    public List<AudioClip> duckGodEnterBattleSounds;
    public List<AudioClip> houndsGodEnterBattleSounds;
    public List<AudioClip> jazzGodEnterBattleSounds;
    public List<AudioClip> loveGodEnterBattleSounds;
    public List<AudioClip> fireGodEnterBattleSounds;
    public List<AudioClip> waterGodEnterBattleSounds;
    public List<AudioClip> earthGodEnterBattleSounds;

    private List<AudioClip> playerGodEnterBattleSounds;
    private List<AudioClip> enemyGodEnterBattleSounds;


    //Background music (based on enemy god)
    public List<AudioClip> mushroomGodBGMs;
    public List<AudioClip> forkGodBGMs;
    public List<AudioClip> shoeGodBGMs;
    public List<AudioClip> duckGodBGMs;
    public List<AudioClip> houndGodBGMs;
    public List<AudioClip> jazzGodBGMs;
    public List<AudioClip> loveGodBGMs;
    public List<AudioClip> fireGodBGMs;
    public List<AudioClip> waterGodBGMs;
    public List<AudioClip> earthGodBGMs;

    private List<AudioClip> activeBGMList;


    //Ui sounds
    public AudioClip uiHover;
    public AudioClip uiSelect;

    //Unit move and attack sounds
    public AudioClip unitAttack;
    public AudioClip unitMove;

    private int BGMIndex;

    //Previous randoms (to make sure we don't play the same thing in a row)
    private int previousUnitSelectionRandom;
    private int previousGodRandom;
    private int previousEnemyGodRandom;
    private int previousUnitDeathRandom;
    private int previousGodEnterBattleRandom;

    //Audio Sources
    private AudioSource musicSource;
    private AudioSource soundeffectScource;
    private AudioSource playerVocalSource;
    private AudioSource enemyVocalSource;

    private bool startup;

    // Start is called before the first frame update
    void Start()
    {
        //Refernce all our audio sources (channels)
        musicSource = GetComponents<AudioSource>()[0];
        soundeffectScource = GetComponents<AudioSource>()[1];
        playerVocalSource = GetComponents<AudioSource>()[2];
        enemyVocalSource = GetComponents<AudioSource>()[3];

        //Startup flag
        startup = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startup && GameObject.Find("GameInfo") != null)
        {
            GameObject GameInfoObject = GameObject.Find("GameInfo");
            System.Random r = new System.Random();

            //Assign appropriate sounds based on enemy god
            switch (GameInfoObject.GetComponent<GameInfo>().EnemyFaction.Type)
            {
                //Tier 1
                case Faction.GodType.Mushrooms:
                    activeBGMList = mushroomGodBGMs;
                    enemyGodTurnStartSounds = mushroomGodTurnStartSounds;
                    enemyUnitDeathSounds = mushroomUnitDeathSounds;
                    enemyGodEnterBattleSounds = mushroomGodEnterBattleSounds;
                    break;
                case Faction.GodType.Forks:
                    activeBGMList = forkGodBGMs;
                    enemyGodTurnStartSounds = forkGodTurnStartSounds;
                    enemyUnitDeathSounds = forkUnitDeathSounds;
                    enemyGodEnterBattleSounds = forkGodEnterBattleSounds;
                    break;
                case Faction.GodType.Shoes:
                    activeBGMList = shoeGodBGMs;
                    enemyGodTurnStartSounds = shoeGodTurnStartSounds;
                    enemyUnitDeathSounds = shoeUnitDeathSounds;
                    enemyGodEnterBattleSounds = shoeGodEnterBattleSounds;
                    break;
                case Faction.GodType.Ducks:
                    activeBGMList = duckGodBGMs;
                    enemyGodTurnStartSounds = duckGodTurnStartSounds;
                    enemyUnitDeathSounds = duckUnitDeathSounds;
                    enemyGodEnterBattleSounds = duckGodEnterBattleSounds;
                    break;


                //Tier 2
                case Faction.GodType.Robots:
                    activeBGMList = houndGodBGMs;
                    enemyGodTurnStartSounds = houndsGodTurnStartSounds;
                    enemyUnitDeathSounds = houndsUnitDeathSounds;
                    enemyGodEnterBattleSounds = houndsGodEnterBattleSounds;
                    break;
                case Faction.GodType.Smiths:
                    activeBGMList = jazzGodBGMs;
                    enemyGodTurnStartSounds = jazzGodTurnStartSounds;
                    enemyUnitDeathSounds = jazzUnitDeathSounds;
                    enemyGodEnterBattleSounds = jazzGodEnterBattleSounds;
                    break;
                case Faction.GodType.Love:
                    activeBGMList = loveGodBGMs;
                    enemyGodTurnStartSounds = loveGodTurnStartSounds;
                    enemyUnitDeathSounds = loveUnitDeathSounds;
                    enemyGodEnterBattleSounds = loveGodEnterBattleSounds;
                    break;


                //Tier 3
                case Faction.GodType.Fire:
                    activeBGMList = fireGodBGMs;
                    enemyGodTurnStartSounds = fireGodTurnStartSounds;
                    enemyUnitDeathSounds = fireUnitDeathSounds;
                    enemyGodEnterBattleSounds = fireGodEnterBattleSounds;
                    break;
                case Faction.GodType.Water:
                    activeBGMList = waterGodBGMs;
                    enemyGodTurnStartSounds = waterGodTurnStartSounds;
                    enemyUnitDeathSounds = waterUnitDeathSounds;
                    enemyGodEnterBattleSounds = waterGodEnterBattleSounds;
                    break;
                case Faction.GodType.Lightning:
                    activeBGMList = earthGodBGMs;
                    enemyGodTurnStartSounds = earthGodTurnStartSounds;
                    enemyUnitDeathSounds = earthUnitDeathSounds;
                    enemyGodEnterBattleSounds = earthGodEnterBattleSounds;
                    break;
            }



            //Assign appropriate sounds based on player god
            switch (GameInfoObject.GetComponent<GameInfo>().PlayerFaction.Type)
            {
                //Player gods (only tier 1)
                case Faction.GodType.Mushrooms:
                    unitSelectionSounds = mushroomUnitSelectionSounds;
                    godSelectionSounds = mushroomGodSelectionSounds;
                    playerUnitDeathSounds = mushroomUnitDeathSounds;
                    playerGodEnterBattleSounds = mushroomGodEnterBattleSounds;
                    break;
                case Faction.GodType.Forks:
                    unitSelectionSounds = forkUnitSelectionSounds;
                    godSelectionSounds = forkGodSelectionSounds;
                    playerUnitDeathSounds = forkUnitDeathSounds;
                    playerGodEnterBattleSounds = forkGodEnterBattleSounds;
                    break;
                case Faction.GodType.Shoes:
                    unitSelectionSounds = shoeUnitSelectionSounds;
                    godSelectionSounds = shoeGodSelectionSounds;
                    playerUnitDeathSounds = shoeUnitDeathSounds;
                    playerGodEnterBattleSounds = shoeGodEnterBattleSounds;
                    break;
                case Faction.GodType.Ducks:
                    unitSelectionSounds = duckUnitSelectionSounds;
                    godSelectionSounds = duckGodSelectionSounds;
                    playerUnitDeathSounds = duckUnitDeathSounds;
                    playerGodEnterBattleSounds = duckGodEnterBattleSounds;
                    break;

            }

            //Start BGM at random index
            BGMIndex = r.Next(0, activeBGMList.Count);

            musicSource.clip = activeBGMList[BGMIndex];

            startup = false;

            musicSource.Play();
        }
        //If we're at the end of the current song
        else if(!startup && !musicSource.isPlaying)
        {
            //Wraparound
            BGMIndex++;
            if (BGMIndex > activeBGMList.Count - 1)
                BGMIndex = 0;

            //Start next song
            musicSource.clip = activeBGMList[BGMIndex];
            musicSource.Play();
        }
    }

    //UI sounds
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

    //Unit sounds
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

        if (unitSelectionSounds.Count > 1)
        {
            for (int i = 0; i < unitSelectionSounds.Count; i++)
            {
                if (!(i == previousUnitSelectionRandom))
                    acceptableSounds.Add(i);
            }

            System.Random r = new System.Random();

            int random = acceptableSounds[r.Next(0, acceptableSounds.Count)];
            playerVocalSource.clip = unitSelectionSounds[random];

            //Prevent playing the last played clip
            previousUnitSelectionRandom = random;
        }
        else
            playerVocalSource.clip = unitSelectionSounds[0];

        playerVocalSource.Play();
    }

    //Same as above, but with god sounds
    public void playGodSelect()
    {
        List<int> acceptableSounds = new List<int>();

        if (godSelectionSounds.Count > 1)
        {
            for (int i = 0; i < godSelectionSounds.Count; i++)
            {
                if (!(i == previousGodRandom))
                    acceptableSounds.Add(i);
            }

            System.Random r = new System.Random();

            int random = acceptableSounds[r.Next(0, acceptableSounds.Count)];

            playerVocalSource.clip = godSelectionSounds[random];


            previousGodRandom = random;
        }
        else
            playerVocalSource.clip = godSelectionSounds[0];

        playerVocalSource.Play();
    }

    //Play a random voice line at the start of enemy turn
    public void playEnemyGodTurnStart()
    {
        List<int> acceptableSounds = new List<int>();

        if (enemyGodTurnStartSounds.Count > 1)
        {
            for (int i = 0; i < enemyGodTurnStartSounds.Count; i++)
            {
                if (!(i == previousEnemyGodRandom))
                    acceptableSounds.Add(i);
            }

            System.Random r = new System.Random();

            int random = acceptableSounds[r.Next(0, acceptableSounds.Count)];
            enemyVocalSource.clip = enemyGodTurnStartSounds[random];

            //Prevent playing the last played clip
            previousEnemyGodRandom = random;
        }
        else
            enemyVocalSource.clip = enemyGodTurnStartSounds[0];

        enemyVocalSource.Play();
    }

    //Play a radom unit select sound (but not the last one played)
    public void playUnitDeath(Unit currentUnit)
    {
        List<int> acceptableSounds = new List<int>();

        if (currentUnit.isPlayer)
        {
            if (playerUnitDeathSounds.Count > 1)
            {
                for (int i = 0; i < playerUnitDeathSounds.Count; i++)
                {
                    if (!(i == previousUnitDeathRandom))
                        acceptableSounds.Add(i);
                }

                System.Random r = new System.Random();

                int random = acceptableSounds[r.Next(0, acceptableSounds.Count)];

                //Prevent playing the last played clip
                previousUnitDeathRandom = random;

                playerVocalSource.clip = playerUnitDeathSounds[random];
            }
            else
                playerVocalSource.clip = playerUnitDeathSounds[0];

            playerVocalSource.Play();
        }
        else
        {
            if (enemyUnitDeathSounds.Count > 1)
            {
                for (int i = 0; i < enemyUnitDeathSounds.Count; i++)
                {
                    if (!(i == previousUnitDeathRandom))
                        acceptableSounds.Add(i);
                }

                System.Random r = new System.Random();

                int random = acceptableSounds[r.Next(0, acceptableSounds.Count)];

                //Prevent playing the last played clip
                previousUnitDeathRandom = random;

                enemyVocalSource.clip = enemyUnitDeathSounds[random];
            }
            else
                enemyVocalSource.clip = enemyUnitDeathSounds[0];


            enemyVocalSource.Play();
        }  
    }

    //Play voice line when a god enters battle
    public void playGodEnterBattle(Unit currentUnit)
    {
        List<int> acceptableSounds = new List<int>();


        if (currentUnit.isPlayer)
        {
            if (playerGodEnterBattleSounds.Count > 1)
            {
                for (int i = 0; i < playerGodEnterBattleSounds.Count; i++)
                {
                    if (!(i == previousGodEnterBattleRandom))
                        acceptableSounds.Add(i);
                }

                System.Random r = new System.Random();

                int random = acceptableSounds[r.Next(0, acceptableSounds.Count)];

                //Prevent playing the last played clip
                previousGodEnterBattleRandom = random;

                playerVocalSource.clip = playerGodEnterBattleSounds[random];
            }
            else
                playerVocalSource.clip = playerGodEnterBattleSounds[0];

            playerVocalSource.Play();
        }
        else
        {
            if (enemyGodEnterBattleSounds.Count > 1)
            {
                for (int i = 0; i < enemyGodEnterBattleSounds.Count; i++)
                {
                    if (!(i == previousGodEnterBattleRandom))
                        acceptableSounds.Add(i);
                }

                System.Random r = new System.Random();

                int random = acceptableSounds[r.Next(0, acceptableSounds.Count)];

                //Prevent playing the last played clip
                previousGodEnterBattleRandom = random;

                enemyVocalSource.clip = enemyGodEnterBattleSounds[random];
            }
            else
                enemyVocalSource.clip = enemyGodEnterBattleSounds[0];


            enemyVocalSource.Play();
        }

    }
}
