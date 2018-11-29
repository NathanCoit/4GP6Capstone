using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
