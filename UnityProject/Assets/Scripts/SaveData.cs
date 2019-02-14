using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SaveData object to be used as a serializable object for converting to JSON and saving to a file
/// A clone of the variables on GameInfo as to load a serialized object, it cannot be of type MonoBehaviour like GameInfo
/// Used to load save file states.
/// </summary>
public class SaveData
{
    public bool FinishedBattle = false;
    public GameInfo.SavedFaction EnemyFaction;
    public GameInfo.SavedFaction PlayerFaction;

    // Combat mode variables
    public GameInfo.BATTLESTATUS LastBattleStatus = GameInfo.BATTLESTATUS.Victory;

    // Management mode variables for loading scene
    public GameInfo.SavedFaction[] SavedFactions;
    public int CurrentTier = 0;
    public float MapRadius = 100f;
    public float PlayerMoraleCap = 1.0f;
    public bool NewGame = true;
    public bool FromSave = false;
    public string[] PlayerRewards;
    public int EnemyChallengeTimer;
    public float[] MaterialMultipliers;
    public float[] WorshipperMultipliers;
    public InformationBoxDisplay.TutorialFlag TutorialFlag;
}
